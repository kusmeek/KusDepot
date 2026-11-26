using DataPodServices.DataControl;
using DataPodServices.DataConfigs;
using NSubstitute;

namespace KusDepot.DataPodExams;

[TestFixture]
[NonParallelizable]
internal sealed class TransferSessionCleanupAgentExam
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
    public async Task AgentDeletesStaleSessionsAndPreservesFreshJournaledAndDependentSourceSessions()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        Guid staleItem = Guid.NewGuid();
        Guid staleSession = Guid.NewGuid();
        Guid freshItem = Guid.NewGuid();
        Guid freshSession = Guid.NewGuid();
        Guid journaledItem = Guid.NewGuid();
        Guid journaledSession = Guid.NewGuid();
        Guid journaledToken = Guid.NewGuid();
        Guid sourceItem = Guid.NewGuid();
        Guid sourceSession = Guid.NewGuid();
        Guid dependentSession = Guid.NewGuid();

        await this.database.TransferState.SaveSessionAsync(CreateSession(staleItem,staleSession,Guid.NewGuid(),DataItemTransferMode.StreamUpload,null,now,"agent-tests"),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSession(freshItem,freshSession,Guid.NewGuid(),DataItemTransferMode.StreamUpload,null,now,"agent-tests"),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSession(journaledItem,journaledSession,journaledToken,DataItemTransferMode.StreamUpload,null,now,"agent-tests"),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSession(sourceItem,sourceSession,Guid.NewGuid(),DataItemTransferMode.StreamUpload,null,now,"agent-tests"),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSession(sourceItem,dependentSession,Guid.NewGuid(),DataItemTransferMode.StreamFollow,sourceSession,now,"agent-tests"),"agent-tests");

        DateTimeOffset sourceActivity = now.AddHours(-30);
        DateTimeOffset dependentActivity = now.AddHours(-25);
        foreach(Guid sessionid in new[] { staleSession , journaledSession }) { await this.database.SetSessionLastActivityAsync(sessionid,now.AddHours(-30)); }
        await this.database.SetSessionLastActivityAsync(sourceSession,sourceActivity);
        await this.database.SetSessionLastActivityAsync(dependentSession,dependentActivity);

        TransferPublicationJournalState journal = CreateJournal(journaledItem,journaledSession,journaledToken,now);
        await this.database.TransferState.SaveJournalAsync(journal,"agent-tests");

        AgentTestDoubles doubles = new();
        await using ServiceProvider services = doubles.CreateDeletionServices(this.database);
        TransferSessionCleanupAgent agent = CreateAgent(services,doubles,TimeSpan.FromHours(1));

        await agent.StartAsync(CancellationToken.None);
        try
        {
            await WaitForSessionStateAsync(staleSession,expectedExists:false,TimeSpan.FromSeconds(10));
            await WaitForSessionStateAsync(dependentSession,expectedExists:false,TimeSpan.FromSeconds(10));

            Check.That(await LoadSessionAsync(freshSession)).IsNotNull();
            Check.That(await LoadSessionAsync(journaledSession)).IsNotNull();
            Check.That(await LoadSessionAsync(sourceSession)).IsNotNull();
        }
        finally { await agent.StopAsync(CancellationToken.None); }

        TransferSessionCleanupAgent nextScan = CreateAgent(services,doubles,TimeSpan.FromHours(1));
        await nextScan.StartAsync(CancellationToken.None);
        try { await WaitForSessionStateAsync(sourceSession,expectedExists:false,TimeSpan.FromSeconds(10)); }
        finally { await nextScan.StopAsync(CancellationToken.None); }
    }

    [Test]
    public async Task AgentRechecksActivityAfterWaitingForMutationLock()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        Guid itemid = Guid.NewGuid();
        Guid sessionid = Guid.NewGuid();
        Guid token = Guid.NewGuid();
        await this.database.TransferState.SaveSessionAsync(CreateSession(itemid,sessionid,token,DataItemTransferMode.StreamUpload,null,now,"agent-tests"),"agent-tests");
        await this.database.SetSessionLastActivityAsync(sessionid,now.AddHours(-30));

        AgentTestDoubles doubles = new();
        await using ServiceProvider services = doubles.CreateDeletionServices(this.database);
        TransferSessionCleanupAgent agent = CreateAgent(services,doubles,TimeSpan.FromHours(1));
        await using ITransferMutationScope mutation = await this.database.Coordinator.AcquireMutationScopeAsync(itemid,sessionid);

        await agent.StartAsync(CancellationToken.None);
        try
        {
            await Task.Delay(200);
            Boolean touched = await this.database.TransferState.TouchTransferSessionAsync(sessionid,itemid,"agent-tests",DateTimeOffset.UtcNow);
            Check.That(touched).IsTrue();
        }
        finally
        {
            await mutation.DisposeAsync();
            await agent.StopAsync(CancellationToken.None);
        }

        Check.That(await LoadSessionAsync(sessionid)).IsNotNull();
    }

    [Test]
    public async Task CompetingAgentsUseOnlyOneCleanupLeader()
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
        TransferSessionCleanupAgent leader = CreateAgent(services,doubles,TimeSpan.FromHours(1));
        TransferSessionCleanupAgent follower = CreateAgent(services,doubles,TimeSpan.FromHours(1));

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

    private TransferSessionCleanupAgent CreateAgent(ServiceProvider services , AgentTestDoubles doubles , TimeSpan pollingInterval)
    {
        return new TransferSessionCleanupAgent(
            services.GetRequiredService<IServiceScopeFactory>(),
            this.database.Coordinator,
            doubles.DataConfigs,
            this.database.TransferState,
            this.database.LeaderLock,
            Options.Create(new TransferSessionCleanupAgentOptions
            {
                InactivityTimeout = TimeSpan.FromHours(24),
                PollingInterval = pollingInterval,
                LeaderRetryInterval = TimeSpan.FromMilliseconds(20),
                InitialJitterMaximum = TimeSpan.Zero,
                CandidatePageSize = 10,
            }));
    }

    private Task<TransferSessionState?> LoadSessionAsync(Guid sessionid) => this.database.TransferState.LoadSessionAsync(sessionid,"agent-tests");

    private async Task WaitForSessionStateAsync(Guid sessionid , Boolean expectedExists , TimeSpan timeout)
    {
        DateTimeOffset stopAt = DateTimeOffset.UtcNow + timeout;
        while(DateTimeOffset.UtcNow < stopAt)
        {
            if((await LoadSessionAsync(sessionid) is not null) == expectedExists) { return; }
            await Task.Delay(25);
        }
        Assert.Fail($"Transfer session {sessionid} did not reach expected existence state {expectedExists} before timeout.");
    }

    private static TransferSessionState CreateSession(Guid itemid , Guid sessionid , Guid token , DataItemTransferMode mode , Guid? sourcesessionid , DateTimeOffset timestamp , String catalogname)
    {
        DataStreamTransferManifest manifest = new()
        {
            AppendedLength = 0,
            CatalogName = catalogname,
            Created = timestamp,
            ItemInstanceToken = token,
            ItemID = itemid,
            Mode = mode,
            SessionID = sessionid,
            SourceSessionID = sourcesessionid,
            StateVersion = 0,
            Status = DataItemTransferStatus.Open,
            Updated = timestamp,
        };

        return new TransferSessionState
        {
            SessionID = sessionid,
            ItemID = itemid,
            ItemInstanceToken = token,
            Family = DataControlTransferFamily.Stream,
            Mode = mode,
            Status = DataItemTransferStatus.Open,
            StateVersion = 0,
            Created = timestamp,
            Updated = timestamp,
            ManifestJson = DataControlTransferSnapshot.SanitizeManifest(Encoding.UTF8.GetString(manifest.Serialize())),
        };
    }

    private static TransferPublicationJournalState CreateJournal(Guid itemid , Guid sessionid , Guid token , DateTimeOffset timestamp)
    {
        TransferPublicationJournal journal = new()
        {
            CatalogName = "agent-tests",
            Created = timestamp,
            ExpectedStateVersion = 0,
            Family = TransferPublicationFamily.Stream,
            ItemID = itemid,
            ItemInstanceToken = token,
            OperationID = Guid.NewGuid(),
            Phase = TransferPublicationPhase.Publishing,
            SessionID = sessionid,
            Updated = timestamp,
        };

        return new TransferPublicationJournalState
        {
            CatalogName = journal.CatalogName,
            Created = journal.Created,
            ExpectedStateVersion = journal.ExpectedStateVersion,
            Family = journal.Family,
            ItemID = journal.ItemID,
            ItemInstanceToken = journal.ItemInstanceToken,
            OperationID = journal.OperationID,
            Phase = journal.Phase,
            SessionID = journal.SessionID,
            JournalJson = DataControlTransferSnapshot.SanitizeJournal(Encoding.UTF8.GetString(JsonUtility.Serialize(journal))),
            Updated = journal.Updated,
        };
    }
}
