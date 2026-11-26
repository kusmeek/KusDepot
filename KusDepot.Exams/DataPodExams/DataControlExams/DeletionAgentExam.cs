using DataPodServices.DataControl;
using DataPodServices.CatalogDB;
using DataPodServices.DataConfigs;
using NSubstitute;

namespace KusDepot.DataPodExams;

[TestFixture]
[NonParallelizable]
internal sealed class DeletionAgentExam
{
    private AgentPostgresFixture database = null!;

    [SetUp]
    public async Task SetUp()
    {
        this.database = new AgentPostgresFixture();
        await this.database.InitializeAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await this.database.DisposeAsync();
    }

    [Test]
    public async Task AgentCompletesDurableDeletionAndRemovesOnlyTheTargetInstanceToken()
    {
        Guid itemid = Guid.NewGuid();
        Guid oldSession = Guid.NewGuid();
        Guid newSession = Guid.NewGuid();
        Guid itemInstanceToken = Guid.NewGuid();
        Guid otherItemInstanceToken = Guid.NewGuid();
        DateTimeOffset now = DateTimeOffset.UtcNow;
        AgentTestDoubles doubles = new();

        DataControlDeletionJobState job = await this.database.DeletionJobs.EnsureDeletionJobAsync("agent-tests",itemid,itemInstanceToken);

        await this.database.TransferState.SaveSessionAsync(CreateSession(itemid,oldSession,itemInstanceToken,"agent-tests",now),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSession(itemid,newSession,otherItemInstanceToken,"agent-tests",now),"agent-tests");
        await this.database.TransferState.SaveJournalAsync(CreateJournal(itemid,oldSession,itemInstanceToken,"agent-tests",now),"agent-tests");
        await this.database.TransferState.SaveJournalAsync(CreateJournal(itemid,newSession,otherItemInstanceToken,"agent-tests",now),"agent-tests");

        ServiceProvider services = doubles.CreateDeletionServices(this.database);

        await using (services)
        {
            await RunAgentAsync(services,doubles,job,expectfailure:false,pollingInterval:TimeSpan.FromSeconds(3));
        }

        DataControlDeletionJobState? remainingJob = await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,itemInstanceToken);
        TransferSessionState? oldState = await this.database.TransferState.LoadSessionAsync(oldSession,"agent-tests");
        TransferSessionState? newState = await this.database.TransferState.LoadSessionAsync(newSession,"agent-tests");
        TransferPublicationJournalState? oldJournal = await this.database.TransferState.LoadJournalAsync(oldSession,"agent-tests");
        TransferPublicationJournalState? newJournal = await this.database.TransferState.LoadJournalAsync(newSession,"agent-tests");

        Check.That(remainingJob).IsNull();
        Check.That(oldState).IsNull();
        Check.That(newState).IsNotNull();
        Check.That(oldJournal).IsNull();
        Check.That(newJournal).IsNotNull();
        await doubles.Catalog.Received(1).FinalizeDelete(itemid,itemInstanceToken,Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>());
        Check.That(doubles.TenantStorageFactory.DeleteInstanceTransfersCallCount).IsEqualTo(1);
        Check.That(doubles.TenantStorageFactory.LastDeletedItemID).IsEqualTo(itemid);
        Check.That(doubles.TenantStorageFactory.LastDeletedItemInstanceToken).IsEqualTo(itemInstanceToken);
    }

    [Test]
    public async Task AgentPersistsFailureAndFreshAgentResumesAfterRetryIsDue()
    {
        Guid itemid = Guid.NewGuid();
        Guid itemInstanceToken = Guid.NewGuid();
        AgentTestDoubles doubles = new();

        DataControlDeletionJobState job = await this.database.DeletionJobs.EnsureDeletionJobAsync("agent-tests",itemid,itemInstanceToken);

        doubles.Catalog.FinalizeDelete(itemid,itemInstanceToken,Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<CatalogDeleteFinalizationResult>(new InvalidOperationException("Injected catalog outage.")));

        ServiceProvider firstServices = doubles.CreateDeletionServices(this.database);

        await using (firstServices)
        {
            await RunAgentAsync(firstServices,doubles,job,expectfailure:true);
        }

        DataControlDeletionJobState? failed = await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,itemInstanceToken);
        Check.That(failed?.Progress).IsEqualTo(DataControlDeletionProgress.TransferStateRemoved);
        Check.That(failed?.AttemptCount).IsEqualTo(1);
        Check.That(failed?.LastError).Contains("Injected catalog outage.");

        DateTimeOffset nextAttempt = DateTimeOffset.UtcNow.AddMilliseconds(200);

        await this.database.DeletionJobs.RecordDeletionFailureAsync("agent-tests",itemid,itemInstanceToken,"Make retry due.",nextAttempt);

        DataControlDeletionJobState? scheduled = await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,itemInstanceToken);

        Check.That(scheduled?.NextAttempt is not null).IsTrue();
        TimeSpan retryWait = scheduled!.NextAttempt!.Value - DateTimeOffset.UtcNow;
        if(retryWait > TimeSpan.Zero) { await Task.Delay(retryWait); }
        doubles.Catalog.FinalizeDelete(itemid,itemInstanceToken,Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult(CatalogDeleteFinalizationResult.Removed));

        ServiceProvider restartedServices = doubles.CreateDeletionServices(this.database);
        await using (restartedServices)
        {
            await RunAgentAsync(restartedServices,doubles,job,expectfailure:false);
        }

        DataControlDeletionJobState? completed = await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,itemInstanceToken);

        Check.That(completed).IsNull();

        await doubles.Catalog.Received(2).FinalizeDelete(itemid,itemInstanceToken,Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task RepeatsArtifactCleanupWhenInterruptedBeforeProgressAdvance()
    {
        Guid itemid = Guid.NewGuid();
        Guid itemInstanceToken = Guid.NewGuid();
        AgentTestDoubles doubles = new();

        DataControlDeletionJobState job = await this.database.DeletionJobs.EnsureDeletionJobAsync("agent-tests",itemid,itemInstanceToken);

        await using ServiceProvider services = doubles.CreateDeletionServices(this.database);

        using CancellationTokenSource interruption = new();
        IDataControlDeletionJobStore interruptedJobs = new InterruptedDeletionJobStore(
            this.database.DeletionJobs,interruption,DeletionJobInterruption.AdvanceProgress);

        DataControlDeletionProcessor interruptedProcessor = new(
            new TestScopeFactory(services),this.database.Coordinator,interruptedJobs,doubles.DataConfigs,
            Options.Create(new DeletionAgentOptions()));

        await Assert.ThrowsAsync<OperationCanceledException>(async () => await interruptedProcessor.ProcessWithLockAsync(job,interruption.Token));

        Check.That(doubles.TenantStorageFactory.DeleteInstanceTransfersCallCount).IsEqualTo(1);

        DataControlDeletionJobState? persisted = await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,itemInstanceToken);

        Check.That(persisted?.Progress).IsEqualTo(DataControlDeletionProgress.CatalogPending);

        DataControlDeletionProcessor resumedProcessor = new(
            new TestScopeFactory(services),this.database.Coordinator,this.database.DeletionJobs,doubles.DataConfigs,
            Options.Create(new DeletionAgentOptions()));

        Check.That(await resumedProcessor.ProcessWithLockAsync(persisted!)).IsNull();

        Check.That(doubles.TenantStorageFactory.DeleteInstanceTransfersCallCount).IsEqualTo(2);

        Check.That(await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,itemInstanceToken)).IsNull();
    }

    [Test]
    public async Task RemovesJobOnRetryWhenInterruptedAfterCatalogFinalization()
    {
        Guid itemid = Guid.NewGuid();
        Guid itemInstanceToken = Guid.NewGuid();
        AgentTestDoubles doubles = new();

        DataControlDeletionJobState job = await this.database.DeletionJobs.EnsureDeletionJobAsync("agent-tests",itemid,itemInstanceToken);

        Check.That(await this.database.DeletionJobs.AdvanceDeletionAsync("agent-tests",itemid,itemInstanceToken,DataControlDeletionProgress.CatalogPending,DataControlDeletionProgress.PublishedArtifactsRemoved)).IsTrue();
        Check.That(await this.database.DeletionJobs.AdvanceDeletionAsync("agent-tests",itemid,itemInstanceToken,DataControlDeletionProgress.PublishedArtifactsRemoved,DataControlDeletionProgress.TransferStateRemoved)).IsTrue();

        Int32 finalizeCount = 0;
        doubles.Catalog.FinalizeDelete(itemid,itemInstanceToken,Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(Interlocked.Increment(ref finalizeCount) == 1
                ? CatalogDeleteFinalizationResult.Removed
                : CatalogDeleteFinalizationResult.AlreadyAbsent));

        await using ServiceProvider services = doubles.CreateDeletionServices(this.database);

        using CancellationTokenSource interruption = new();

        IDataControlDeletionJobStore interruptedJobs = new InterruptedDeletionJobStore(
            this.database.DeletionJobs,interruption,DeletionJobInterruption.DeleteJob);

        DataControlDeletionProcessor interruptedProcessor = new(
            new TestScopeFactory(services),this.database.Coordinator,interruptedJobs,doubles.DataConfigs,
            Options.Create(new DeletionAgentOptions()));

        await Assert.ThrowsAsync<OperationCanceledException>(async () => await interruptedProcessor.ProcessWithLockAsync(job,interruption.Token));
        Check.That(finalizeCount).IsEqualTo(1);

        Check.That(await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,itemInstanceToken)).IsNotNull();

        DataControlDeletionProcessor resumedProcessor = new(
            new TestScopeFactory(services),this.database.Coordinator,this.database.DeletionJobs,doubles.DataConfigs,
            Options.Create(new DeletionAgentOptions()));

        Check.That(await resumedProcessor.ProcessWithLockAsync(job)).IsNull();

        Check.That(finalizeCount).IsEqualTo(2);
        Check.That(await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,itemInstanceToken)).IsNull();
    }

    private async Task RunAgentAsync(ServiceProvider services , AgentTestDoubles doubles , DataControlDeletionJobState job , Boolean expectfailure , TimeSpan? pollingInterval = null)
    {
        DeletionAgentOptions options = new()
        {
            PollingInterval = pollingInterval ?? TimeSpan.FromMilliseconds(20),
            LeaderRetryInterval = TimeSpan.FromMilliseconds(20),
            InitialJitterMaximum = TimeSpan.Zero,
            CycleJitterMaximum = TimeSpan.Zero,
            RetryBaseDelay = TimeSpan.FromMilliseconds(10),
            MaximumRetryDelay = TimeSpan.FromMilliseconds(100),
            CandidatePageSize = 10,
        };
        DataControlDeletionProcessor processor = new(
            new TestScopeFactory(services),
            this.database.Coordinator,
            this.database.DeletionJobs,
            doubles.DataConfigs,
            Options.Create(options));

        DeletionAgent agent = new(this.database.DeletionJobs,doubles.DataConfigs,doubles.GrainFactory,processor,this.database.LeaderLock,Options.Create(options));

        await agent.StartAsync(CancellationToken.None);

        await WaitForJobAsync(agent,job,TimeSpan.FromSeconds(10),expectfailure);
    }

    [Test]
    public async Task AgentReconcilesPendingCatalogDeleteWithoutExistingJob()
    {
        Guid itemid = Guid.NewGuid();
        Guid itemInstanceToken = Guid.NewGuid();
        AgentTestDoubles doubles = new();

        CatalogPendingDeletePage pendingPage = new(new[] { new CatalogItemIdentity(itemid,itemInstanceToken) },null);
        Int32 pendingPageRead = 0;
        doubles.Catalog.GetPendingDeletes(Arg.Any<Guid?>(),Arg.Any<Int32>(),Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(Interlocked.Exchange(ref pendingPageRead,1) is 0 ? pendingPage : new CatalogPendingDeletePage(Array.Empty<CatalogItemIdentity>(),null)));

        TaskCompletionSource finalized = new(TaskCreationOptions.RunContinuationsAsynchronously);

        doubles.Catalog.FinalizeDelete(itemid,itemInstanceToken,Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                finalized.TrySetResult();
                return Task.FromResult(CatalogDeleteFinalizationResult.Removed);
            });

        await using ServiceProvider services = doubles.CreateDeletionServices(this.database);

        DeletionAgentOptions options = new()
        {
            PollingInterval = TimeSpan.FromMilliseconds(20),
            LeaderRetryInterval = TimeSpan.FromMilliseconds(20),
            InitialJitterMaximum = TimeSpan.Zero,
            CycleJitterMaximum = TimeSpan.Zero,
            RetryBaseDelay = TimeSpan.FromMilliseconds(10),
            MaximumRetryDelay = TimeSpan.FromMilliseconds(100),
            CandidatePageSize = 10,
        };
        DataControlDeletionProcessor processor = new(
            new TestScopeFactory(services),
            this.database.Coordinator,
            this.database.DeletionJobs,
            doubles.DataConfigs,
            Options.Create(options));

        DeletionAgent agent = new(this.database.DeletionJobs,doubles.DataConfigs,doubles.GrainFactory,processor,this.database.LeaderLock,Options.Create(options));

        await agent.StartAsync(CancellationToken.None);

        await finalized.Task.WaitAsync(TimeSpan.FromSeconds(10));

        DateTimeOffset stopAt = DateTimeOffset.UtcNow.AddSeconds(10);
        while(DateTimeOffset.UtcNow < stopAt && await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,itemInstanceToken) is not null) { await Task.Delay(20); }

        await agent.StopAsync(CancellationToken.None);

        Check.That(await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,itemInstanceToken)).IsNull();

        await doubles.Catalog.Received(1).FinalizeDelete(itemid,itemInstanceToken,Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task CompetingAgentsUseOnlyOneDeletionLeader()
    {
        AgentTestDoubles doubles = new();
        TaskCompletionSource firstScan = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource followerScan = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource releaseScan = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Int32 catalogListReads = 0;
        doubles.DataConfigs.GetConfiguredCatalogNamesAsync(Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                if(Interlocked.Increment(ref catalogListReads) == 1)
                {
                    firstScan.TrySetResult();
                    await releaseScan.Task;
                }
                else { followerScan.TrySetResult(); }

                return new ConfiguredCatalogNames(new[] { "agent-tests" },default);
            });

        await using ServiceProvider services = doubles.CreateDeletionServices(this.database);
        DeletionAgent leader = CreateAgent(doubles,services,TimeSpan.FromMilliseconds(20));
        DeletionAgent follower = CreateAgent(doubles,services,TimeSpan.FromMilliseconds(20));

        await leader.StartAsync(CancellationToken.None);
        await firstScan.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await follower.StartAsync(CancellationToken.None);

        try
        {
            await Task.Delay(150);
            Check.That(Volatile.Read(ref catalogListReads)).IsEqualTo(1);
        }
        finally
        {
            releaseScan.TrySetResult();
            await leader.StopAsync(CancellationToken.None);
            await followerScan.Task.WaitAsync(TimeSpan.FromSeconds(5));
            await follower.StopAsync(CancellationToken.None);
        }

        Check.That(Volatile.Read(ref catalogListReads)).IsEqualTo(2);
    }

    private DeletionAgent CreateAgent(AgentTestDoubles doubles , ServiceProvider services , TimeSpan retryInterval)
    {
        DeletionAgentOptions options = new()
        {
            PollingInterval = TimeSpan.FromHours(1),
            LeaderRetryInterval = retryInterval,
            InitialJitterMaximum = TimeSpan.Zero,
            CycleJitterMaximum = TimeSpan.Zero,
            CandidatePageSize = 10,
        };
        DataControlDeletionProcessor processor = new(new TestScopeFactory(services),this.database.Coordinator,this.database.DeletionJobs,doubles.DataConfigs,Options.Create(options));
        return new DeletionAgent(this.database.DeletionJobs,doubles.DataConfigs,doubles.GrainFactory,processor,this.database.LeaderLock,Options.Create(options));
    }

    [Test]
    public async Task AgentContinuesReconcilingLaterCatalogAfterEarlierCatalogFailure()
    {
        const String failedCatalogName = "agent-failed-catalog";
        const String laterCatalogName = "agent-later-catalog";
        Guid itemid = Guid.NewGuid();
        Guid itemInstanceToken = Guid.NewGuid();
        AgentTestDoubles doubles = new();
        ICatalogDB laterCatalog = Substitute.For<ICatalogDB>();
        Int32 pendingPageRead = 0;
        CatalogPendingDeletePage pendingPage = new(new[] { new CatalogItemIdentity(itemid,itemInstanceToken) },null);
        TaskCompletionSource finalized = new(TaskCreationOptions.RunContinuationsAsynchronously);

        doubles.DataConfigs.GetConfiguredCatalogNamesAsync(Arg.Any<CancellationToken>())
            .Returns(new ConfiguredCatalogNames(new[] { failedCatalogName,laterCatalogName },default));
        doubles.DataConfigs.GetConfiguredStorageForRecoveryAsync(Arg.Any<String>(),Arg.Any<CancellationToken>())
            .Returns(call => call.ArgAt<String>(0) == failedCatalogName
                ? Task.FromResult(new TenantStorageResolution(CatalogResolutionStatus.Authorized,failedCatalogName,"UseDevelopmentStorage=true",default))
                : Task.FromResult(new TenantStorageResolution(CatalogResolutionStatus.Authorized,laterCatalogName,"UseDevelopmentStorage=true",default)));
        ICatalogDB failedCatalog = Substitute.For<ICatalogDB>();
        failedCatalog.GetPendingDeletes(Arg.Any<Guid?>(),Arg.Any<Int32>(),Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<CatalogPendingDeletePage>(new InvalidOperationException("Injected catalog outage.")));
        doubles.GrainFactory.GetGrain<ICatalogDB>(failedCatalogName).Returns(failedCatalog);
        doubles.GrainFactory.GetGrain<ICatalogDB>(laterCatalogName).Returns(laterCatalog);
        laterCatalog.GetPendingDeletes(Arg.Any<Guid?>(),Arg.Any<Int32>(),Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(Interlocked.Exchange(ref pendingPageRead,1) is 0 ? pendingPage : new CatalogPendingDeletePage(Array.Empty<CatalogItemIdentity>(),null)));
        laterCatalog.FinalizeDelete(itemid,itemInstanceToken,Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                finalized.TrySetResult();
                return Task.FromResult(CatalogDeleteFinalizationResult.Removed);
            });

        await using ServiceProvider services = doubles.CreateDeletionServices(this.database);
        DeletionAgentOptions options = new()
        {
            PollingInterval = TimeSpan.FromMilliseconds(20),
            LeaderRetryInterval = TimeSpan.FromMilliseconds(20),
            InitialJitterMaximum = TimeSpan.Zero,
            CycleJitterMaximum = TimeSpan.Zero,
            RetryBaseDelay = TimeSpan.FromMilliseconds(10),
            MaximumRetryDelay = TimeSpan.FromMilliseconds(100),
            CandidatePageSize = 10,
        };
        DataControlDeletionProcessor processor = new(
            new TestScopeFactory(services),
            this.database.Coordinator,
            this.database.DeletionJobs,
            doubles.DataConfigs,
            Options.Create(options));
        DeletionAgent agent = new(this.database.DeletionJobs,doubles.DataConfigs,doubles.GrainFactory,processor,this.database.LeaderLock,Options.Create(options));

        await agent.StartAsync(CancellationToken.None);
        await finalized.Task.WaitAsync(TimeSpan.FromSeconds(10));

        DateTimeOffset stopAt = DateTimeOffset.UtcNow.AddSeconds(10);
        while(DateTimeOffset.UtcNow < stopAt && await this.database.DeletionJobs.LoadDeletionJobAsync(laterCatalogName,itemid,itemInstanceToken) is not null) { await Task.Delay(20); }

        await agent.StopAsync(CancellationToken.None);

        Check.That(await this.database.DeletionJobs.LoadDeletionJobAsync(laterCatalogName,itemid,itemInstanceToken)).IsNull();
        await doubles.DataConfigs.Received().GetConfiguredStorageForRecoveryAsync(failedCatalogName,Arg.Any<CancellationToken>());
        await laterCatalog.Received(1).FinalizeDelete(itemid,itemInstanceToken,Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task DeletionJobsForDifferentItemInstanceTokensDoNotCollide()
    {
        Guid itemid = Guid.NewGuid();
        Guid oldToken = Guid.NewGuid();
        Guid newToken = Guid.NewGuid();

        DataControlDeletionJobState oldJob = await this.database.DeletionJobs.EnsureDeletionJobAsync("agent-tests",itemid,oldToken);
        DataControlDeletionJobState newJob = await this.database.DeletionJobs.EnsureDeletionJobAsync("agent-tests",itemid,newToken);

        Check.That(oldJob.ItemInstanceToken).IsEqualTo(oldToken);
        Check.That(newJob.ItemInstanceToken).IsEqualTo(newToken);
        Check.That(await this.database.DeletionJobs.DeleteDeletionJobAsync("agent-tests",itemid,oldToken)).IsTrue();
        Check.That(await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,oldToken)).IsNull();
        Check.That(await this.database.DeletionJobs.LoadDeletionJobAsync("agent-tests",itemid,newToken)).IsNotNull();
    }

    private async Task WaitForJobAsync(DeletionAgent agent , DataControlDeletionJobState candidate , TimeSpan timeout , Boolean expectfailure)
    {
        DateTimeOffset stopAt = DateTimeOffset.UtcNow + timeout;

        while(DateTimeOffset.UtcNow < stopAt)
        {
            DataControlDeletionJobState? job = await this.database.DeletionJobs.LoadDeletionJobAsync(candidate.CatalogName,candidate.ItemID,candidate.ItemInstanceToken);
            if(expectfailure && job is { AttemptCount: > 0 }) { await agent.StopAsync(CancellationToken.None); return; }
            if(!expectfailure && job is null) { await agent.StopAsync(CancellationToken.None); return; }
            await Task.Delay(20);
        }

        await agent.StopAsync(CancellationToken.None);

        Assert.Fail("DeletionAgent did not complete the durable job before timeout.");
    }

    private sealed class TestScopeFactory(IServiceProvider services) : IServiceScopeFactory
    {
        public IServiceScope CreateScope() => new TestScope(services.CreateScope());

        private sealed class TestScope(IServiceScope scope) : IServiceScope
        {
            public IServiceProvider ServiceProvider => scope.ServiceProvider;

            public void Dispose() => scope.Dispose();
        }
    }

    private enum DeletionJobInterruption
    {
        AdvanceProgress,
        DeleteJob,
    }

    private sealed class InterruptedDeletionJobStore(
        IDataControlDeletionJobStore inner,
        CancellationTokenSource interruption,
        DeletionJobInterruption boundary) : IDataControlDeletionJobStore
    {
        public Task<Boolean> AdvanceDeletionAsync(String catalogname , Guid itemid , Guid itemInstanceToken , DataControlDeletionProgress expectedprogress , DataControlDeletionProgress progress , CancellationToken cancel = default)
        {
            if(boundary is DeletionJobInterruption.AdvanceProgress) { Interrupt(); }
            return inner.AdvanceDeletionAsync(catalogname,itemid,itemInstanceToken,expectedprogress,progress,cancel);
        }

        public Task<DataControlDeletionJobState> EnsureDeletionJobAsync(String catalogname , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default) =>
            inner.EnsureDeletionJobAsync(catalogname,itemid,itemInstanceToken,cancel);

        public Task<DataControlDeletionJobState?> LoadDeletionJobAsync(String catalogname , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default) =>
            inner.LoadDeletionJobAsync(catalogname,itemid,itemInstanceToken,cancel);

        public Task<IReadOnlyList<DataControlDeletionJobState>> LoadDueDeletionJobsAsync(DateTimeOffset now , Int32 pageSize , CancellationToken cancel = default) =>
            inner.LoadDueDeletionJobsAsync(now,pageSize,cancel);

        public Task<Boolean> RecordDeletionFailureAsync(String catalogname , Guid itemid , Guid itemInstanceToken , String error , DateTimeOffset nextattempt , CancellationToken cancel = default) =>
            inner.RecordDeletionFailureAsync(catalogname,itemid,itemInstanceToken,error,nextattempt,cancel);

        public Task<Boolean> DeleteDeletionJobAsync(String catalogname , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default)
        {
            if(boundary is DeletionJobInterruption.DeleteJob) { Interrupt(); }
            return inner.DeleteDeletionJobAsync(catalogname,itemid,itemInstanceToken,cancel);
        }

        private void Interrupt()
        {
            interruption.Cancel();
            throw new OperationCanceledException(interruption.Token);
        }
    }

    private static TransferSessionState CreateSession(Guid itemid , Guid sessionid , Guid itemInstanceToken , String catalogname , DateTimeOffset timestamp)
    {
        DataItemTransferManifest manifest = new()
        {
            CatalogName = catalogname,
            Created = timestamp,
            ItemInstanceToken = itemInstanceToken,
            ItemID = itemid,
            Mode = DataItemTransferMode.PublishedRead,
            ObjectSHA512 = SHA512.HashData(Array.Empty<Byte>()),
            SessionID = sessionid,
            StateVersion = 0,
            Status = DataItemTransferStatus.Published,
            StreamLength = 0,
            Updated = timestamp,
        };

        return new TransferSessionState
        {
            SessionID = sessionid,
            ItemID = itemid,
            ItemInstanceToken = itemInstanceToken,
            Family = DataControlTransferFamily.Segmented,
            Mode = DataItemTransferMode.PublishedRead,
            Status = DataItemTransferStatus.Published,
            StateVersion = 0,
            Created = timestamp,
            Updated = timestamp,
            ManifestJson = DataControlTransferSnapshot.SanitizeManifest(Encoding.UTF8.GetString(manifest.Serialize())),
        };
    }

    private static TransferPublicationJournalState CreateJournal(Guid itemid , Guid sessionid , Guid itemInstanceToken , String catalogname , DateTimeOffset timestamp)
    {
        TransferPublicationJournal journal = new()
        {
            CatalogName = catalogname,
            Created = timestamp,
            ExpectedStateVersion = 0,
            Family = TransferPublicationFamily.Segmented,
            ItemID = itemid,
            ItemInstanceToken = itemInstanceToken,
            OperationID = Guid.NewGuid(),
            Phase = TransferPublicationPhase.Finalized,
            SessionID = sessionid,
            Updated = timestamp,
        };
        return new TransferPublicationJournalState
        {
            CatalogName = catalogname,
            Created = timestamp,
            ExpectedStateVersion = 0,
            Family = TransferPublicationFamily.Segmented,
            ItemID = itemid,
            ItemInstanceToken = itemInstanceToken,
            OperationID = journal.OperationID,
            Phase = TransferPublicationPhase.Finalized,
            SessionID = sessionid,
            JournalJson = DataControlTransferSnapshot.SanitizeJournal(Encoding.UTF8.GetString(JsonUtility.Serialize(journal))),
            Updated = timestamp,
        };
    }

}
