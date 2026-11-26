using DataPodServices.DataControl;
using KusDepot.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace KusDepot.DataPodExams;

[TestFixture]
[NonParallelizable]
internal sealed class SegmentedCoverageExam
{
    private const String Catalog = "agent-tests";
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
    public async Task OutOfOrderWritesMergeCoverageAndReplayDoesNotChargeAgain()
    {
        Guid sessionid = Guid.NewGuid();
        await CreateSession(sessionid,40);

        Check.That((await Commit(sessionid,10,10)).Accepted).IsTrue();
        Check.That((await Commit(sessionid,30,10)).Accepted).IsTrue();
        Check.That((await Commit(sessionid,20,10)).Accepted).IsTrue();

        IReadOnlyList<DataItemTransferRange> coverage = await this.database.TransferState.LoadSegmentedCoverageAsync(sessionid,Catalog);
        Check.That(coverage.Count).IsEqualTo(1);
        Check.That(coverage[0].Offset).IsEqualTo(10L);
        Check.That(coverage[0].Length).IsEqualTo(30L);

        TransferSessionState before = (await this.database.TransferState.LoadSessionAsync(sessionid,Catalog))!;
        SegmentedTransferWriteResult replay = await Commit(sessionid,20,10);
        Check.That(replay.Accepted).IsTrue();
        Check.That(replay.Replayed).IsTrue();
        Check.That(replay.StateVersion).IsEqualTo(before.StateVersion);

        TransferSessionState after = (await this.database.TransferState.LoadSessionAsync(sessionid,Catalog))!;
        Check.That(after.CoveredBytes).IsEqualTo(30L);
        Check.That(after.PlannedBlockCount).IsEqualTo(3L);
        Check.That(after.StateVersion).IsEqualTo(before.StateVersion);

        SegmentedTransferWriteResult last = await Commit(sessionid,0,10);
        Check.That(last.Accepted).IsTrue();
        Check.That(last.CompletedCoverage).IsTrue();
        after = (await this.database.TransferState.LoadSessionAsync(sessionid,Catalog))!;
        Check.That(after.CoveredBytes).IsEqualTo(40L);
        Check.That(after.PlannedBlockCount).IsEqualTo(4L);
        Check.That(after.Status).IsEqualTo(DataItemTransferStatus.Complete);
        coverage = await this.database.TransferState.LoadSegmentedCoverageAsync(sessionid,Catalog);
        Check.That(coverage.Count).IsEqualTo(1);
        Check.That(coverage[0].Offset).IsEqualTo(0L);
        Check.That(coverage[0].Length).IsEqualTo(40L);
    }

    [Test]
    public async Task CoverageExtendsPredecessorAndEmptyUploadRemainsOpen()
    {
        Guid sessionid = Guid.NewGuid();
        await CreateSession(sessionid,30);
        Check.That((await Commit(sessionid,0,10)).Accepted).IsTrue();
        Check.That((await Commit(sessionid,10,10)).Accepted).IsTrue();
        IReadOnlyList<DataItemTransferRange> coverage = await this.database.TransferState.LoadSegmentedCoverageAsync(sessionid,Catalog);
        Check.That(coverage.Count).IsEqualTo(1);
        Check.That(coverage[0].Offset).IsEqualTo(0L);
        Check.That(coverage[0].Length).IsEqualTo(20L);
        TransferSessionState partial = (await this.database.TransferState.LoadSessionAsync(sessionid,Catalog))!;
        Check.That(partial.CoveredBytes).IsEqualTo(20L);
        Check.That(partial.Status).IsEqualTo(DataItemTransferStatus.Open);
        Check.That(partial.StateVersion).IsEqualTo(2L);

        Guid emptySession = Guid.NewGuid();
        await CreateSession(emptySession,0);
        TransferSessionState empty = (await this.database.TransferState.LoadSessionAsync(emptySession,Catalog))!;
        Check.That(empty.Status).IsEqualTo(DataItemTransferStatus.Open);
        Check.That(empty.CoveredBytes).IsEqualTo(0L);
        Check.That(DataItemTransferManifest.Parse(empty.ManifestJson)!.ToState().MissingRanges.Length).IsEqualTo(0);
    }

    [Test]
    public async Task NeighborLookupsGenerateOrderedLimitedSql()
    {
        Guid sessionid = Guid.NewGuid();
        await CreateSession(sessionid,40);
        await Commit(sessionid,0,10);
        await Commit(sessionid,20,10);
        IReadOnlyList<String> commands = await this.database.CaptureCommandsAsync(async () =>
        {
            await this.database.TransferState.HasSegmentedRangeOverlapAsync(sessionid,new DataItemTransferRange { Offset = 10 , Length = 5 },Catalog);
            await Commit(sessionid,10,5);
        });

        String[] neighbors = commands.Where(command => command.Contains("ORDER BY",StringComparison.Ordinal) &&
            (command.Contains("SegmentedRanges",StringComparison.Ordinal) || command.Contains("SegmentedCoverage",StringComparison.Ordinal))).ToArray();
        Check.That(neighbors.Length >= 6).IsTrue();
        Check.That(neighbors.All(command => command.Contains("LIMIT 1",StringComparison.Ordinal))).IsTrue();
        Check.That(neighbors.Any(command => command.Contains("DESC",StringComparison.Ordinal))).IsTrue();
    }

    [Test]
    public async Task OverlapStaleVersionAndConflictingReplayLeaveAccountingUnchanged()
    {
        Guid sessionid = Guid.NewGuid();
        await CreateSession(sessionid,40);
        Check.That((await Commit(sessionid,10,10)).Accepted).IsTrue();
        TransferSessionState before = (await this.database.TransferState.LoadSessionAsync(sessionid,Catalog))!;

        Check.That((await Commit(sessionid,15,10)).Accepted).IsFalse();
        Check.That((await Commit(sessionid,10,10,differentHash:true)).Accepted).IsFalse();
        Check.That((await Commit(sessionid,30,10,expectedVersion:0)).Accepted).IsFalse();
        Check.That(await this.database.TransferState.IsSegmentedRangeCoveredAsync(sessionid,new DataItemTransferRange { Offset = 12 , Length = 4 },Catalog)).IsTrue();
        Check.That(await this.database.TransferState.HasSegmentedRangeOverlapAsync(sessionid,new DataItemTransferRange { Offset = 19 , Length = 2 },Catalog)).IsTrue();
        Check.That(await this.database.TransferState.HasSegmentedRangeOverlapAsync(sessionid,new DataItemTransferRange { Offset = 20 , Length = 2 },Catalog)).IsFalse();

        TransferSessionState after = (await this.database.TransferState.LoadSessionAsync(sessionid,Catalog))!;
        Check.That(after.CoveredBytes).IsEqualTo(before.CoveredBytes);
        Check.That(after.PlannedBlockCount).IsEqualTo(before.PlannedBlockCount);
        Check.That(after.StateVersion).IsEqualTo(before.StateVersion);
        Check.That((await this.database.TransferState.LoadSegmentedRangesAsync(sessionid,Catalog)).Count).IsEqualTo(1);
    }

    [Test]
    public async Task ConcurrentWritesSerializeAndAcceptedWritesUpdateActivity()
    {
        Guid sessionid = Guid.NewGuid();
        await CreateSession(sessionid,40);
        DateTimeOffset oldActivity = DateTimeOffset.UtcNow.AddDays(-1);
        await this.database.SetSessionLastActivityAsync(sessionid,oldActivity);

        SegmentedTransferWriteResult[] results = await Task.WhenAll(
            Commit(sessionid,0,10,expectedVersion:0),
            Commit(sessionid,20,10,expectedVersion:0));

        Check.That(results.Count(result => result.Accepted)).IsEqualTo(1);
        TransferSessionState state = (await this.database.TransferState.LoadSessionAsync(sessionid,Catalog))!;
        Check.That(state.CoveredBytes).IsEqualTo(10L);
        Check.That(state.PlannedBlockCount).IsEqualTo(1L);
        Check.That(state.StateVersion).IsEqualTo(1L);
        Check.That(state.LastActivity > oldActivity).IsTrue();
        Check.That((await this.database.TransferState.LoadSegmentedRangesAsync(sessionid,Catalog)).Count).IsEqualTo(1);
        Check.That((await this.database.TransferState.LoadSegmentedCoverageAsync(sessionid,Catalog)).Count).IsEqualTo(1);
    }

    [Test]
    public async Task DatabaseFailureRollsBackCoverageRangeAndSessionAccounting()
    {
        Guid sessionid = Guid.NewGuid();
        await CreateSession(sessionid,40);
        await this.database.ExecuteSqlAsync("ALTER TABLE \"SegmentedCoverage\" ADD CONSTRAINT \"ExamRejectLength\" CHECK (\"Length\" <> 10)");

        await Assert.ThrowsAsync<DbUpdateException>(() => Commit(sessionid,0,10));

        TransferSessionState state = (await this.database.TransferState.LoadSessionAsync(sessionid,Catalog))!;
        Check.That(state.CoveredBytes).IsEqualTo(0L);
        Check.That(state.PlannedBlockCount).IsEqualTo(0L);
        Check.That(state.StateVersion).IsEqualTo(0L);
        Check.That(state.Status).IsEqualTo(DataItemTransferStatus.Open);
        Check.That((await this.database.TransferState.LoadSegmentedRangesAsync(sessionid,Catalog)).Count).IsEqualTo(0);
        Check.That((await this.database.TransferState.LoadSegmentedCoverageAsync(sessionid,Catalog)).Count).IsEqualTo(0);
    }

    [Test]
    public async Task ServiceVerifiesReplayPayloadAndReturnsCommittedTimestamp()
    {
        Guid sessionid = Guid.NewGuid();
        await CreateSession(sessionid,40);
        TransferSessionState original = (await this.database.TransferState.LoadSessionAsync(sessionid,Catalog))!;
        AgentTestDoubles doubles = new();
        doubles.TenantStorage.AcceptVerifiedRanges = true;
        ITenantTransferStorageContext context = doubles.StorageContext;
        await using ServiceProvider services = doubles.CreateTransferSessionServices(this.database);
        DataControlTransferSessionStore sessions = new(this.database.TransferState,context);
        DataControlSegmentedTransferService service = new(sessions,services.GetRequiredService<IDataItemTransferRegistry>(),this.database.Coordinator,
            services.GetRequiredService<IDataControlPublicationTransferStore>(),services.GetRequiredService<IPublishedTransferSource>(),context,doubles.GrainFactory,
            Options.Create(new DataItemTransferServiceOptions()));
        Byte[] payload = Enumerable.Range(1,10).Select(index => (Byte)index).ToArray();
        PutTransferSegmentRequest request = new()
        {
            SessionID = sessionid, ItemID = original.ItemID, Offset = 0, Length = payload.Length,
            SegmentSHA512 = SHA512.HashData(payload), ExpectedStateVersion = 0,
        };

        PutTransferSegmentResponse first = await service.PutTransferSegment(request,new MemoryStream(payload,false));
        Check.That(first.Accepted).IsTrue();
        Check.That(first.Replayed).IsFalse();
        GetTransferStateResponse state = await service.GetTransferState(new DataItemTransferIdentity { SessionID = sessionid , ItemID = original.ItemID });
        Check.That(first.Updated).IsEqualTo(state.State.Updated);
        Check.That(state.State.RealizedBytes).IsEqualTo(10L);
        DataControlServerSessionInfo[] inventory = await service.GetSessions(new GetTransferSessionsRequest { SessionID = sessionid });
        Check.That(inventory.Single().SegmentedState!.RealizedBytes).IsEqualTo(10L);
        SegmentedTransferReadResult read = await service.GetTransferSegment(new GetTransferSegmentRequest { SessionID = sessionid , ItemID = original.ItemID , Offset = 0 , Length = payload.Length });
        using MemoryStream copied = new();
        await read.Payload.CopyToAsync(copied);
        await read.Payload.DisposeAsync();
        Check.That(copied.ToArray().SequenceEqual(payload)).IsTrue();

        PutTransferSegmentResponse replay = await service.PutTransferSegment(request,new MemoryStream(payload,false));
        Check.That(replay.Accepted).IsTrue();
        Check.That(replay.Replayed).IsTrue();
        Check.That(replay.StateVersion).IsEqualTo(first.StateVersion);
        Byte[] different = payload.ToArray(); different[0]++;
        OperationFailedException mismatch = (await Assert.ThrowsAsync<OperationFailedException>(() => service.PutTransferSegment(request,new MemoryStream(different,false))))!;
        Check.That(mismatch.FailureCode).IsEqualTo(SegmentedTransferFailureCode.InvalidSegmentHash);
        PutTransferSegmentResponse conflict = await service.PutTransferSegment(request with { SegmentSHA512 = SHA512.HashData(different) },new MemoryStream(different,false));
        Check.That(conflict.Accepted).IsFalse();
        PutTransferSegmentResponse contained = await service.PutTransferSegment(request with { Offset = 1 , Length = 2 , SegmentSHA512 = SHA512.HashData(payload.AsSpan(1,2)) },new MemoryStream(payload,1,2,false));
        Check.That(contained.Accepted).IsFalse();
        Check.That(await this.database.TransferState.LoadSegmentedCoverageAsync(sessionid,"another-catalog")).IsEmpty();
        Check.That(await this.database.TransferState.LoadSessionAsync(sessionid,"another-catalog")).IsNull();
        TransferSessionState after = (await this.database.TransferState.LoadSessionAsync(sessionid,Catalog))!;
        Check.That(after.CoveredBytes).IsEqualTo(10L);
        Check.That(after.PlannedBlockCount).IsEqualTo(1L);
        Check.That(after.StateVersion).IsEqualTo(1L);
        Check.That(doubles.TenantStorage.VerifiedRangeWrites).IsEqualTo(1);
        Check.That(await this.database.TransferState.DeleteSessionAsync(sessionid,Catalog)).IsTrue();
        Check.That((await this.database.TransferState.LoadSegmentedCoverageAsync(sessionid,Catalog)).Count).IsEqualTo(0);
        Check.That((await this.database.TransferState.LoadSegmentedRangesAsync(sessionid,Catalog)).Count).IsEqualTo(0);
    }

    private async Task CreateSession(Guid sessionid , Int64 streamLength)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        DataItemTransferManifest manifest = new()
        {
            CatalogName = Catalog,
            SessionID = sessionid,
            ItemID = Guid.NewGuid(),
            ItemInstanceToken = Guid.NewGuid(),
            Mode = DataItemTransferMode.Upload,
            Status = DataItemTransferStatus.Open,
            StreamLength = streamLength,
            Created = now,
            Updated = now,
        };
        await this.database.TransferState.SaveSessionAsync(ToSession(manifest),Catalog);
    }

    private async Task<SegmentedTransferWriteResult> Commit(Guid sessionid , Int64 offset , Int64 length , Boolean differentHash = false , Int64? expectedVersion = null)
    {
        TransferSessionState current = (await this.database.TransferState.LoadSessionAsync(sessionid,Catalog))!;
        DataItemTransferManifest manifest = DataItemTransferManifest.Parse(current.ManifestJson)!;
        Int64 expected = expectedVersion ?? current.StateVersion;
        DateTimeOffset updated = DateTimeOffset.UtcNow;
        DataItemTransferManifest proposed = manifest with { StateVersion = expected + 1 , Updated = updated };
        Byte[] hash = SHA512.HashData(Encoding.UTF8.GetBytes(differentHash ? "different" : $"{offset}:{length}"));
        SegmentedTransferRangeState range = new()
        {
            SessionID = sessionid,
            Range = new DataItemTransferRange { Offset = offset , Length = length },
            Payload = new TransferBlobReference { BlobName = $"range-{offset}", Length = length , SHA512 = hash },
            Created = updated,
        };
        return await this.database.TransferState.CommitSegmentedRangeAsync(range,ToSession(proposed),expected,Catalog);
    }

    private static TransferSessionState ToSession(DataItemTransferManifest manifest)
    {
        return new TransferSessionState
        {
            SessionID = manifest.SessionID,
            ItemID = manifest.ItemID,
            ItemInstanceToken = manifest.ItemInstanceToken,
            Family = DataControlTransferFamily.Segmented,
            Mode = manifest.Mode,
            Status = manifest.Status,
            StateVersion = manifest.StateVersion,
            Created = manifest.Created,
            Updated = manifest.Updated,
            ManifestJson = DataControlTransferSnapshot.SanitizeManifest(Encoding.UTF8.GetString(manifest.Serialize())),
        };
    }
}
