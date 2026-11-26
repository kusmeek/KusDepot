using DataPodServices.DataControl;
using DataPodServices.CatalogDB;
using KusDepot.Exceptions;
using NSubstitute;

namespace KusDepot.DataPodExams;

[TestFixture]
[NonParallelizable]
internal sealed class TransferPublicationRecoveryAgentExam
{
    private AgentPostgresFixture database = null!;

    [SetUp]
    public async Task SetUp()
    {
        this.database = new AgentPostgresFixture();
        await this.database.InitializeAsync();
    }

    [Test]
    public async Task PostgresItemLockCanBeAttemptedWithoutWaiting()
    {
        Guid itemid = Guid.NewGuid();
        await using IAsyncDisposable held = await this.database.Coordinator.AcquireItemLockAsync(itemid);
        Task<IAsyncDisposable?> attempt = this.database.Coordinator.TryAcquireItemLockAsync(itemid);
        Task winner = await Task.WhenAny(attempt,Task.Delay(TimeSpan.FromSeconds(2)));
        Check.That(winner).IsEqualTo(attempt);
        Check.That(await attempt).IsNull();

        await held.DisposeAsync();
        IAsyncDisposable? available = await this.database.Coordinator.TryAcquireItemLockAsync(itemid);
        Check.That(available).IsNotNull();
        if(available is not null) { await available.DisposeAsync(); }
    }

    [TearDown]
    public async Task TearDown()
    {
        await this.database.DisposeAsync();
    }

    [Test]
    public async Task AgentDeletesFinalizedJournalsAndPreservesMismatchedTokenAndTerminalJournal()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        AgentTestDoubles doubles = new();
        Guid finalizedItem = Guid.NewGuid();
        Guid finalizedSession = Guid.NewGuid();
        Guid staleItem = Guid.NewGuid();
        Guid staleSession = Guid.NewGuid();
        Guid failedItem = Guid.NewGuid();
        Guid failedSession = Guid.NewGuid();
        Guid finalizedToken = Guid.NewGuid();
        Guid staleToken = Guid.NewGuid();
        Guid failedToken = Guid.NewGuid();

        doubles.Catalog.GetItemState(staleItem,Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>())
            .Returns(new CatalogItemState(staleItem,Guid.NewGuid(),false));

        await this.database.TransferState.SaveSessionAsync(CreateSession(finalizedItem,finalizedSession,finalizedToken,now),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSession(staleItem,staleSession,staleToken,now),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSession(failedItem,failedSession,failedToken,now),"agent-tests");
        await this.database.TransferState.SaveJournalAsync(CreateJournal(finalizedItem,finalizedSession,finalizedToken,TransferPublicationPhase.Finalized,now),"agent-tests");
        await this.database.TransferState.SaveJournalAsync(CreateJournal(staleItem,staleSession,staleToken,TransferPublicationPhase.Publishing,now),"agent-tests");
        await this.database.TransferState.SaveJournalAsync(CreateJournal(failedItem,failedSession,failedToken,TransferPublicationPhase.Failed,now),"agent-tests");
        IReadOnlyList<TransferPublicationRecoveryCandidate> candidates = await this.database.TransferState.LoadRecoverableJournalCandidatesAsync(DateTimeOffset.UtcNow,null,10);
        Assert.That(candidates.Any(item => item.SessionID == finalizedSession),Is.True);

        await using ServiceProvider services = doubles.CreateDeletionServices(this.database);
        TransferPublicationRecoveryAgentOptions options = new()
        {
            PollingInterval = TimeSpan.FromMilliseconds(50),
            LeaderRetryInterval = TimeSpan.FromMilliseconds(20),
            InitialJitterMaximum = TimeSpan.Zero,
            CycleJitterMaximum = TimeSpan.Zero,
            CandidatePageSize = 10,
            TerminalFailureThreshold = 3,
        };
        TransferPublicationRecoveryAgent agent = new(
            services.GetRequiredService<IServiceScopeFactory>(),
            this.database.Coordinator,
            doubles.GrainFactory,
            doubles.DataConfigs,
            doubles.TenantStorageFactory,
            this.database.LeaderLock,
            Options.Create(options));

        await agent.StartAsync(CancellationToken.None);
        try
        {
            await WaitForJournalDeletionAsync(finalizedSession,TimeSpan.FromSeconds(10));
        }
        finally
        {
            await agent.StopAsync(CancellationToken.None);
        }

        TransferPublicationJournalState? finalized = await this.database.TransferState.LoadJournalAsync(finalizedSession,"agent-tests");
        TransferPublicationJournalState? stale = await this.database.TransferState.LoadJournalAsync(staleSession,"agent-tests");
        TransferPublicationJournalState? failed = await this.database.TransferState.LoadJournalAsync(failedSession,"agent-tests");

        Check.That(finalized).IsNull();
        Check.That(stale?.Phase).IsEqualTo(TransferPublicationPhase.Publishing);
        Check.That(failed?.Phase).IsEqualTo(TransferPublicationPhase.Failed);
        await doubles.Catalog.DidNotReceive().AddUpdate(Arg.Any<Descriptor?>(),Arg.Any<Guid>(),Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SegmentedFinalizationAndRecoveryReplayPublishedUpload()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        BinaryItem item = new(new Byte[] { 1,2,3 });
        Guid itemid = item.GetID()!.Value,sessionid = Guid.NewGuid(),token = Guid.NewGuid();
        Byte[] objectPayload = item.Serialize();
        DataItemTransferManifest manifest = new()
        {
            CatalogName = "agent-tests", Created = now, ItemID = itemid, ItemInstanceToken = token,
            Mode = DataItemTransferMode.Upload, ObjectSHA512 = SHA512.HashData(objectPayload), SessionID = sessionid,
            StateVersion = 7, Status = DataItemTransferStatus.Publishing, StreamLength = 0, Updated = now,
        };

        AgentTestDoubles doubles = new();
        doubles.TenantStorage.ObjectBytes = objectPayload;
        await using ServiceProvider services = doubles.CreateTransferSessionServices(this.database);
        await using AsyncServiceScope serviceScope = services.CreateAsyncScope();
        IDataControlTransferSessionStore sessions = serviceScope.ServiceProvider.GetRequiredService<IDataControlTransferSessionStore>();
        IDataItemSegmentedTransferService segmented = serviceScope.ServiceProvider.GetRequiredService<IDataItemSegmentedTransferService>();
        await sessions.SaveSegmentedManifestAsync(manifest);
        DataItemTransferIdentity identity = new() { ItemID = itemid, SessionID = sessionid };

        await using(ITransferMutationScope mutation = await this.database.Coordinator.AcquireMutationScopeAsync(itemid,sessionid))
        {
            PublishSegmentedTransferResponse first = await segmented.FinishPublishSegmentedTransfer(identity,mutation);
            Assert.That(first.Published,Is.True);
            Assert.That(first.State.Status,Is.EqualTo(DataItemTransferStatus.Published));
            Assert.That(first.State.StateVersion,Is.EqualTo(8));

            PublishSegmentedTransferResponse replay = await segmented.FinishPublishSegmentedTransfer(identity,mutation);
            Assert.That(replay.Published,Is.True);
            Assert.That(replay.State.StateVersion,Is.EqualTo(first.State.StateVersion));
        }

        Guid readSession = Guid.NewGuid(),openSession = Guid.NewGuid();
        await sessions.SaveSegmentedManifestAsync(manifest with { SessionID = readSession, Mode = DataItemTransferMode.PublishedRead, Status = DataItemTransferStatus.Published });
        await sessions.SaveSegmentedManifestAsync(manifest with { SessionID = openSession, Status = DataItemTransferStatus.Open });
        await using(ITransferMutationScope mutation = await this.database.Coordinator.AcquireMutationScopeAsync(itemid,readSession))
        {
            OperationFailedException invalidMode = (await Assert.ThrowsAsync<OperationFailedException>(() => segmented.FinishPublishSegmentedTransfer(new() { ItemID = itemid, SessionID = readSession },mutation)))!;
            Assert.That(invalidMode.FailureCode,Is.EqualTo(SegmentedTransferFailureCode.InvalidSessionState));
        }
        await using(ITransferMutationScope mutation = await this.database.Coordinator.AcquireMutationScopeAsync(itemid,openSession))
        {
            OperationFailedException invalidState = (await Assert.ThrowsAsync<OperationFailedException>(() => segmented.FinishPublishSegmentedTransfer(new() { ItemID = itemid, SessionID = openSession },mutation)))!;
            Assert.That(invalidState.FailureCode,Is.EqualTo(SegmentedTransferFailureCode.InvalidSessionState));
        }

        await this.database.TransferState.SaveJournalAsync(CreateJournal(itemid,sessionid,token,TransferPublicationPhase.CatalogUpdated,now),"agent-tests");
        TransferPublicationRecoveryAgentOptions options = new()
        {
            PollingInterval = TimeSpan.FromMilliseconds(50), LeaderRetryInterval = TimeSpan.FromMilliseconds(20),
            InitialJitterMaximum = TimeSpan.Zero, CycleJitterMaximum = TimeSpan.Zero, CandidatePageSize = 10, TerminalFailureThreshold = 3,
        };
        TransferPublicationRecoveryAgent agent = new(services.GetRequiredService<IServiceScopeFactory>(),this.database.Coordinator,doubles.GrainFactory,
            doubles.DataConfigs,doubles.TenantStorageFactory,this.database.LeaderLock,Options.Create(options));
        await agent.StartAsync(CancellationToken.None);
        try { await WaitForJournalDeletionAsync(sessionid,TimeSpan.FromSeconds(10)); }
        finally { await agent.StopAsync(CancellationToken.None); }

        TransferSessionState? persisted = await this.database.TransferState.LoadSessionAsync(sessionid,"agent-tests");
        Assert.That(persisted!.Status,Is.EqualTo(DataItemTransferStatus.Published));
        Assert.That(persisted.StateVersion,Is.EqualTo(8));
        Assert.That(await this.database.TransferState.LoadJournalAsync(sessionid,"agent-tests"),Is.Null);
        await doubles.Catalog.DidNotReceive().AddUpdate(Arg.Any<Descriptor?>(),Arg.Any<Guid>(),Arg.Any<TraceContext?>(),Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task StreamJournalReloadPreservesAndVerifiesFinalObject()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        Guid itemid = Guid.NewGuid(),sessionid = Guid.NewGuid(),token = Guid.NewGuid(),operationid = Guid.NewGuid();
        Byte[] initialObject = [1,2,3],finalObject = [4,5,6];
        DataStreamTransferManifest manifest = new()
        {
            CatalogName = "agent-tests", Created = now, ItemID = itemid, ItemInstanceToken = token,
            Mode = DataItemTransferMode.StreamUpload, ObjectSHA512 = SHA512.HashData(initialObject),
            SessionID = sessionid, Status = DataItemTransferStatus.Open, Updated = now,
        };
        await this.database.TransferState.SaveSessionAsync(new TransferSessionState()
        {
            SessionID = sessionid, ItemID = itemid, ItemInstanceToken = token, Family = DataControlTransferFamily.Stream,
            Mode = manifest.Mode, Status = manifest.Status, Created = now, Updated = now,
            ManifestJson = DataControlTransferSnapshot.SanitizeManifest(Encoding.UTF8.GetString(manifest.Serialize())),
        },"agent-tests");

        AgentTestDoubles doubles = new();
        String blobName = String.Concat(TransferStoragePath.Create(token,sessionid).Prefix,"/publications/",operationid.ToString("N"),"/object");
        TransferStoragePath path = TransferStoragePath.Create(token,sessionid);
        Assert.That(path.CreatePublicationObjectName(operationid),Is.EqualTo(blobName));
        Assert.Throws<ArgumentException>(() => path.CreatePublicationObjectName(Guid.Empty));
        doubles.TenantStorage.PublicationObjectReference = new TenantTransferBlobReference()
        {
            BlobName = blobName, Length = finalObject.LongLength, SHA512 = SHA512.HashData(finalObject),
        };
        doubles.TenantStorage.PublicationObjectBytes = finalObject;

        PostgresPublicationJournalStore store = new(this.database.TransferState,doubles.DataConfigs,doubles.TenantStorageFactory);
        TransferPublicationJournal journal = new()
        {
            CatalogName = "agent-tests", Created = now, ItemID = itemid, ItemInstanceToken = token,
            SessionID = sessionid, OperationID = operationid, Family = TransferPublicationFamily.Stream,
            Phase = TransferPublicationPhase.Prepared, FinalObjectPayload = finalObject, Updated = now,
        };

        await store.GetOrCreateAsync(journal,"agent-tests");
        TransferPublicationJournalState? persisted = await this.database.TransferState.LoadJournalAsync(sessionid,"agent-tests");
        Assert.That(persisted!.FinalObject!.SHA512,Is.EqualTo(SHA512.HashData(finalObject)));
        TransferPublicationJournal? loaded = await store.LoadAsync(sessionid,"agent-tests");
        Assert.That(loaded!.FinalObjectPayload,Is.EqualTo(finalObject));
        Assert.That(loaded.FinalObjectPayload,Is.Not.EqualTo(initialObject));

        doubles.TenantStorage.PublicationObjectBytes = null;
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await store.LoadAsync(sessionid,"agent-tests"));
        doubles.TenantStorage.PublicationObjectBytes = [4,5,7];
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await store.LoadAsync(sessionid,"agent-tests"));

        await store.DeleteAsync(sessionid,"agent-tests");
        await store.GetOrCreateAsync(journal with { OperationID = Guid.NewGuid(), FinalObjectPayload = Array.Empty<Byte>() },"agent-tests");
        loaded = await store.LoadAsync(sessionid,"agent-tests");
        Assert.That(loaded!.FinalObjectPayload,Is.Empty);
    }

    [Test]
    public async Task StreamJournalRejectsUnstagedFinalObject()
    {
        AgentTestDoubles doubles = new();
        ITransferStateStore state = Substitute.For<ITransferStateStore>();
        PostgresPublicationJournalStore store = new(state,doubles.DataConfigs,doubles.TenantStorageFactory);
        TransferPublicationJournal journal = new()
        {
            CatalogName = "agent-tests", ItemID = Guid.NewGuid(), ItemInstanceToken = Guid.NewGuid(),
            SessionID = Guid.NewGuid(), OperationID = Guid.NewGuid(), Family = TransferPublicationFamily.Stream,
            Phase = TransferPublicationPhase.Prepared, FinalObjectPayload = [1,2,3],
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await store.GetOrCreateAsync(journal,"agent-tests"));
        await state.DidNotReceive().SaveJournalAsync(Arg.Any<TransferPublicationJournalState>(),Arg.Any<String>(),Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task PostgresLeaderLockAllowsOnlyOneConcurrentLease()
    {
        IPostgresAdvisoryLeaderLease? first = await this.database.LeaderLock.TryAcquireAsync(TransferPublicationRecoveryAgentOptions.LeaderLockName);
        Check.That(first).IsNotNull();
        try
        {
            IPostgresAdvisoryLeaderLease? second = await this.database.LeaderLock.TryAcquireAsync(TransferPublicationRecoveryAgentOptions.LeaderLockName);
            Check.That(second).IsNull();

            IPostgresAdvisoryLeaderLease? cleanup = await this.database.LeaderLock.TryAcquireAsync(TransferSessionCleanupAgentOptions.LeaderLockName);
            Check.That(cleanup).IsNotNull();
            if(cleanup is not null) { await cleanup.DisposeAsync(); }

            IPostgresAdvisoryLeaderLease? deletion = await this.database.LeaderLock.TryAcquireAsync(DeletionAgentOptions.LeaderLockName);
            Check.That(deletion).IsNotNull();
            if(deletion is not null) { await deletion.DisposeAsync(); }

            await first!.VerifyAsync();
        }
        finally
        {
            if(first is not null) { await first.DisposeAsync(); }
        }

        IPostgresAdvisoryLeaderLease? afterRelease = await this.database.LeaderLock.TryAcquireAsync(TransferPublicationRecoveryAgentOptions.LeaderLockName);
        Check.That(afterRelease).IsNotNull();
        if(afterRelease is not null) { await afterRelease.DisposeAsync(); }
    }

    private async Task WaitForJournalDeletionAsync(Guid sessionid , TimeSpan timeout)
    {
        DateTimeOffset stopAt = DateTimeOffset.UtcNow + timeout;
        while(DateTimeOffset.UtcNow < stopAt)
        {
            if(await this.database.TransferState.LoadJournalAsync(sessionid,"agent-tests") is null) { return; }
            await Task.Delay(20);
        }
        Assert.Fail("The recovery Agent did not remove the finalized journal before timeout.");
    }

    private static TransferSessionState CreateSession(Guid itemid , Guid sessionid , Guid itemInstanceToken , DateTimeOffset timestamp)
    {
        DataItemTransferManifest manifest = new()
        {
            CatalogName = "agent-tests",
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

    private static TransferPublicationJournalState CreateJournal(Guid itemid , Guid sessionid , Guid itemInstanceToken , TransferPublicationPhase phase , DateTimeOffset timestamp)
    {
        TransferPublicationJournal journal = new()
        {
            CatalogName = "agent-tests",
            Created = timestamp,
            ExpectedStateVersion = 0,
            Family = TransferPublicationFamily.Segmented,
            ItemID = itemid,
            ItemInstanceToken = itemInstanceToken,
            OperationID = Guid.NewGuid(),
            Phase = phase,
            SessionID = sessionid,
            Updated = timestamp,
        };

        return new TransferPublicationJournalState
        {
            CatalogName = "agent-tests",
            Created = timestamp,
            ExpectedStateVersion = 0,
            Family = TransferPublicationFamily.Segmented,
            ItemID = itemid,
            ItemInstanceToken = itemInstanceToken,
            OperationID = journal.OperationID,
            Phase = phase,
            SessionID = sessionid,
            JournalJson = DataControlTransferSnapshot.SanitizeJournal(Encoding.UTF8.GetString(JsonUtility.Serialize(journal))),
            Updated = timestamp,
        };
    }
}
