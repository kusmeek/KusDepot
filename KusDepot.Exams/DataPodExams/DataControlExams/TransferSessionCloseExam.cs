using DataPodServices.DataConfigs;
using DataPodServices.DataControl;
using KusDepot.Exceptions;
using NSubstitute;

namespace KusDepot.DataPodExams;

[TestFixture]
[NonParallelizable]
internal sealed class TransferSessionCloseExam
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
    public async Task StoreClosesReadSessionsIdempotentlyAndPreservesSessionsWithDependentsOrJournals()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        Guid itemid = Guid.NewGuid();
        Guid sourceSession = Guid.NewGuid();
        Guid followerSession = Guid.NewGuid();
        Guid stagedSession = Guid.NewGuid();
        Guid journaledSession = Guid.NewGuid();
        Guid journaledToken = Guid.NewGuid();

        await this.database.TransferState.SaveSessionAsync(CreateSession(itemid,sourceSession,Guid.NewGuid(),DataItemTransferMode.PublishedRead,null,now),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSession(itemid,followerSession,Guid.NewGuid(),DataItemTransferMode.StreamFollow,sourceSession,now),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSegmentedSession(itemid,stagedSession,Guid.NewGuid(),now,DataItemTransferStatus.Published,DataItemTransferMode.StagedRead,sourceSession),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSession(itemid,journaledSession,journaledToken,DataItemTransferMode.PublishedRead,null,now),"agent-tests");
        await this.database.TransferState.SaveJournalAsync(CreateJournal(itemid,journaledSession,journaledToken,now),"agent-tests");

        AgentTestDoubles doubles = new();
        await using ServiceProvider services = doubles.CreateTransferSessionServices(this.database);
        ITenantTransferStorageContext context = services.GetRequiredService<ITenantTransferStorageContext>();
        context.Set(new StorageSilo("agent-tests","UseDevelopmentStorage=true"));
        IDataControlTransferSessionStore sessions = services.GetRequiredService<IDataControlTransferSessionStore>();

        Check.That(await sessions.CloseReadSessionAsync(sourceSession,itemid)).IsFalse();
        Check.That(await sessions.CloseReadSessionAsync(journaledSession,itemid)).IsFalse();
        Check.That(await sessions.CloseReadSessionAsync(stagedSession,itemid)).IsTrue();
        Check.That(await sessions.CloseReadSessionAsync(followerSession,itemid)).IsTrue();
        Check.That(await sessions.CloseReadSessionAsync(followerSession,itemid)).IsTrue();
        Check.That(await this.database.TransferState.LoadSessionAsync(sourceSession,"agent-tests")).IsNotNull();
        Check.That(await this.database.TransferState.LoadSessionAsync(journaledSession,"agent-tests")).IsNotNull();
        Check.That(await this.database.TransferState.LoadSessionAsync(followerSession,"agent-tests")).IsNull();
        Check.That(await this.database.TransferState.LoadSessionAsync(stagedSession,"agent-tests")).IsNull();
    }

    [Test]
    public async Task StreamRemovePreservesSourceSessionWithDependentFollower()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        Guid itemid = Guid.NewGuid();
        Guid sourceSession = Guid.NewGuid();
        Guid followerSession = Guid.NewGuid();
        await this.database.TransferState.SaveSessionAsync(CreateSession(itemid,sourceSession,Guid.NewGuid(),DataItemTransferMode.StreamUpload,null,now,DataItemTransferStatus.Complete),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSession(itemid,followerSession,Guid.NewGuid(),DataItemTransferMode.StreamFollow,sourceSession,now),"agent-tests");

        AgentTestDoubles doubles = new();
        await using ServiceProvider services = doubles.CreateTransferSessionServices(this.database);
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ITenantTransferStorageContext>().Set(new StorageSilo("agent-tests","UseDevelopmentStorage=true"));
        IDataStreamTransferService stream = scope.ServiceProvider.GetRequiredService<IDataStreamTransferService>();

        OperationFailedException error = (await Assert.ThrowsAsync<OperationFailedException>(() => stream.RemoveTransfer(new RemoveTransferRequest { ItemID = itemid , SessionID = sourceSession })))!;

        Check.That(error.FailureCode).IsEqualTo(StreamTransferFailureCode.InvalidSessionState);
        Check.That(await this.database.TransferState.LoadSessionAsync(sourceSession,"agent-tests")).IsNotNull();
    }

    [Test]
    public async Task SegmentedRemovePreservesSourceSessionWithDependentFollower()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        Guid itemid = Guid.NewGuid();
        Guid sourceSession = Guid.NewGuid();
        Guid followerSession = Guid.NewGuid();
        Guid itemToken = Guid.NewGuid();
        await this.database.TransferState.SaveSessionAsync(CreateSegmentedSession(itemid,sourceSession,itemToken,now,DataItemTransferStatus.Complete),"agent-tests");
        await this.database.TransferState.SaveSessionAsync(CreateSession(itemid,followerSession,Guid.NewGuid(),DataItemTransferMode.StreamFollow,sourceSession,now),"agent-tests");

        AgentTestDoubles doubles = new();
        await using ServiceProvider services = doubles.CreateTransferSessionServices(this.database);
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ITenantTransferStorageContext>().Set(new StorageSilo("agent-tests","UseDevelopmentStorage=true"));
        IDataItemSegmentedTransferService segmented = scope.ServiceProvider.GetRequiredService<IDataItemSegmentedTransferService>();

        OperationFailedException error = (await Assert.ThrowsAsync<OperationFailedException>(() => segmented.RemoveTransfer(new RemoveTransferRequest { ItemID = itemid , SessionID = sourceSession })))!;

        Check.That(error.FailureCode).IsEqualTo(SegmentedTransferFailureCode.InvalidSessionState);
        Check.That(await this.database.TransferState.LoadSessionAsync(sourceSession,"agent-tests")).IsNotNull();
    }

    private static TransferSessionState CreateSession(Guid itemid , Guid sessionid , Guid token , DataItemTransferMode mode , Guid? sourcesessionid , DateTimeOffset timestamp , DataItemTransferStatus status = DataItemTransferStatus.Published)
    {
        DataStreamTransferManifest manifest = new()
        {
            CatalogName = "agent-tests",
            Created = timestamp,
            ItemInstanceToken = token,
            ItemID = itemid,
            Mode = mode,
            SessionID = sessionid,
            SourceSessionID = sourcesessionid,
            StateVersion = 0,
            Status = status,
            Updated = timestamp,
        };

        return new TransferSessionState
        {
            SessionID = sessionid,
            ItemID = itemid,
            ItemInstanceToken = token,
            Family = DataControlTransferFamily.Stream,
            Mode = mode,
            Status = status,
            StateVersion = 0,
            Created = timestamp,
            Updated = timestamp,
            ManifestJson = DataControlTransferSnapshot.SanitizeManifest(Encoding.UTF8.GetString(manifest.Serialize())),
        };
    }

    private static TransferSessionState CreateSegmentedSession(Guid itemid , Guid sessionid , Guid token , DateTimeOffset timestamp , DataItemTransferStatus status , DataItemTransferMode mode = DataItemTransferMode.PublishedRead , Guid? sourcesessionid = null)
    {
        DataItemTransferManifest manifest = new()
        {
            CatalogName = "agent-tests",
            Created = timestamp,
            ItemInstanceToken = token,
            ItemID = itemid,
            Mode = mode,
            SessionID = sessionid,
            SourceSessionID = sourcesessionid,
            StateVersion = 0,
            Status = status,
            Updated = timestamp,
        };

        return new TransferSessionState
        {
            SessionID = sessionid,
            ItemID = itemid,
            ItemInstanceToken = token,
            Family = DataControlTransferFamily.Segmented,
            Mode = mode,
            Status = status,
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
            Created = timestamp,
            ExpectedStateVersion = journal.ExpectedStateVersion,
            Family = journal.Family,
            ItemID = journal.ItemID,
            ItemInstanceToken = token,
            OperationID = journal.OperationID,
            Phase = journal.Phase,
            SessionID = sessionid,
            JournalJson = DataControlTransferSnapshot.SanitizeJournal(Encoding.UTF8.GetString(JsonUtility.Serialize(journal))),
            Updated = timestamp,
        };
    }
}
