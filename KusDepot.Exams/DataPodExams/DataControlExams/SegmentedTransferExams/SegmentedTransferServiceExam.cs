namespace KusDepot.DataServices;

[TestFixture] [NonParallelizable]
public class SegmentedTransferServiceExam
{
    private String? RootPath;
    private IDataItemSegmentedTransferService? Service;

    [OneTimeSetUp]
    public void Calibrate()
    {
        if(OperatingSystem.IsWindows() is false) { Assert.Ignore("Segmented transfer currently relies on Windows-only staging storage."); }

        this.RootPath = Path.Combine(Path.GetTempPath(),"KusDepot","Exam","SegmentedTransferService",Guid.NewGuid().ToString("N",InvariantCulture));

        ISegmentedTransferStorage storage = SegmentedTransferExamHelpers.CreateStorage(this.RootPath);
        IDataItemTransferRegistry registry = new DataItemTransferRegistry(storage,StreamTransferExamHelpers.CreateStorage(Path.Combine(this.RootPath,"registry","stream")));
        IDataItemTransferCoordinator coordinator = new DataItemTransferCoordinator();

        this.Service = new DataItemSegmentedTransferService(storage,registry,coordinator,new NullPublishedTransferSourceAccessor(),Options.Create(new DataItemTransferServiceOptions() { EnforceMinimumSegmentBytes = false , EnforceMaximumSegmentBytes = true }));
    }

    internal sealed class NullPublishedTransferSourceAccessor : IPublishedTransferSource
    {
        public Task<PublishedTransferSnapshot?> TryLoadSnapshot(Guid itemId , CancellationToken cancel = default)
        {
            return Task.FromResult<PublishedTransferSnapshot?>(null);
        }

        public Task<Stream?> OpenReadRange(PublishedTransferSnapshot snapshot , DataItemTransferRange range , CancellationToken cancel = default)
        {
            return Task.FromResult<Stream?>(null);
        }
    }

    [OneTimeTearDown]
    public void Complete()
    {
        if(String.IsNullOrWhiteSpace(this.RootPath) is false && Directory.Exists(this.RootPath)) { Directory.Delete(this.RootPath,true); }
    }

    [Test] [Order(1)]
    public async Task OpenUploadTransferCreatesInitialState()
    {
        OpenUploadTransferRequest request = SegmentedTransferExamHelpers.CreateOpenUploadRequest();

        OpenUploadTransferResponse response = await this.Service!.OpenUploadTransfer(request);

        Check.That(response.State.SessionID).IsEqualTo(request.SessionID);
        Check.That(response.State.ItemID).IsEqualTo(request.ItemID);
        Check.That(response.State.Status).IsEqualTo(DataItemTransferStatus.Open);
        Check.That(response.State.StateVersion).IsEqualTo(0);
        Check.That(response.State.RealizedRanges).HasSize(0);
    }

    [Test] [Order(2)]
    public void OpenUploadTransferRejectsSecondActiveSessionForSameItem()
    {
        OpenUploadTransferRequest first = SegmentedTransferExamHelpers.CreateOpenUploadRequest();
        OpenUploadTransferRequest second = SegmentedTransferExamHelpers.CreateOpenUploadRequest(itemId:first.ItemID);

        _ = this.Service!.OpenUploadTransfer(first).Result;

        Check.ThatAsyncCode(async () => await this.Service!.OpenUploadTransfer(second)).Throws<OperationFailedException>();
    }

    [Test] [Order(3)]
    public async Task PutTransferSegmentUpdatesStateAndSupportsReadback()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:64);
        Byte[] payload = SegmentedTransferExamHelpers.CreatePayload(16,0x22);
        using SegmentedTransferExamHelpers.TransferSegmentExamPayload write = SegmentedTransferExamHelpers.CreatePutTransferSegment(open,8,payload);

        await this.Service!.OpenUploadTransfer(open);

        PutTransferSegmentResponse response = await this.Service.PutTransferSegment(write.Request,write.Payload);
        GetTransferStateResponse state = await this.Service.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID });
        using SegmentedTransferReadResult read = await this.Service.GetTransferSegment(new GetTransferSegmentRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , Offset = 8 , Length = payload.Length });
        using MemoryStream loaded = new();

        await read.Payload.CopyToAsync(loaded);
        TransferSegmentFooter footer = read.FinalizeFooter();

        Check.That(response.Accepted).IsTrue();
        Check.That(response.CompletedCoverage).IsFalse();
        Check.That(state.State.RealizedRanges).HasSize(1);
        Check.That(state.State.RealizedRanges[0].Offset).IsEqualTo(8);
        Check.That(state.State.RealizedRanges[0].Length).IsEqualTo(payload.Length);
        Check.That(loaded.ToArray().SequenceEqual(payload)).IsTrue();
        Check.That(footer.StateVersion).IsEqualTo(state.State.StateVersion);
        Check.That(footer.SegmentSHA512.SequenceEqual(SHA512.HashData(payload))).IsTrue();
    }

    [Test] [Order(4)]
    public async Task PutTransferSegmentRejectsOverlap()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:64);
        Byte[] firstPayload = SegmentedTransferExamHelpers.CreatePayload(16,0x31);
        Byte[] overlapPayload = firstPayload.AsSpan(4,8).ToArray();

        await this.Service!.OpenUploadTransfer(open);
        using SegmentedTransferExamHelpers.TransferSegmentExamPayload first = SegmentedTransferExamHelpers.CreatePutTransferSegment(open,8,firstPayload);
        PutTransferSegmentResponse firstResponse = await this.Service.PutTransferSegment(first.Request,first.Payload);

        using SegmentedTransferExamHelpers.TransferSegmentExamPayload overlap = SegmentedTransferExamHelpers.CreatePutTransferSegment(open,12,overlapPayload,firstResponse.StateVersion);

        PutTransferSegmentResponse overlapResponse = await this.Service.PutTransferSegment(overlap.Request,overlap.Payload);

        Check.That(overlapResponse.Accepted).IsFalse();
    }

    [Test] [Order(5)]
    public async Task PutTransferSegmentRejectsStaleExpectedStateVersion()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:64);
        Byte[] firstPayload = SegmentedTransferExamHelpers.CreatePayload(16,0x41);
        Byte[] secondPayload = SegmentedTransferExamHelpers.CreatePayload(8,0x51);

        await this.Service!.OpenUploadTransfer(open);

        using SegmentedTransferExamHelpers.TransferSegmentExamPayload first = SegmentedTransferExamHelpers.CreatePutTransferSegment(open,0,firstPayload);
        PutTransferSegmentResponse firstResponse = await this.Service.PutTransferSegment(first.Request,first.Payload);

        using SegmentedTransferExamHelpers.TransferSegmentExamPayload stale = SegmentedTransferExamHelpers.CreatePutTransferSegment(open,16,secondPayload);

        Check.That(firstResponse.Accepted).IsTrue();
        Check.ThatAsyncCode(async () => await this.Service.PutTransferSegment(stale.Request,stale.Payload)).Throws<OperationFailedException>();
    }

    [Test] [Order(6)]
    public async Task CommitUploadTransferValidatesAndMarksCommitted()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:32,streamSeed:0x40);
        Byte[] fullPayload = SegmentedTransferExamHelpers.CreatePayload(32,0x40);
        using SegmentedTransferExamHelpers.TransferSegmentExamPayload write = SegmentedTransferExamHelpers.CreatePutTransferSegment(open,0,fullPayload);

        await this.Service!.OpenUploadTransfer(open);
        PutTransferSegmentResponse stored = await this.Service.PutTransferSegment(write.Request,write.Payload);

        _ = await this.Service.BeginCommitUploadTransfer(new CommitUploadTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored.StateVersion });

        CommitUploadTransferResponse commit = await this.Service.CompleteCommitUploadTransfer(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID });

        Check.That(commit.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(commit.Published).IsFalse();
    }

    [Test] [Order(7)]
    public async Task CommitUploadTransferAllowsSerializedOnlyFiniteItem()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:0,streamSeed:0x40) with
        {
            StreamSHA512 = Array.Empty<Byte>(),
            ObjectInfo = new DataItemTransferObjectInfo(){ ObjectType = typeof(BinaryItem).FullName , ContentStreamed = false },
        };

        await this.Service!.OpenUploadTransfer(open);

        _ = await this.Service.BeginCommitUploadTransfer(new CommitUploadTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = 0 });

        CommitUploadTransferResponse commit = await this.Service.CompleteCommitUploadTransfer(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID });

        Check.That(commit.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(commit.State.StreamLength).IsEqualTo(0);
        Check.That(commit.Published).IsFalse();
    }

    [Test] [Order(8)]
    public async Task AbortTransferDeletesSession()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest();

        await this.Service!.OpenUploadTransfer(open);

        AbortTransferResponse aborted = await this.Service.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID });

        Check.That(aborted.Aborted).IsTrue();

        OperationFailedException? failure = null;

        try
        {
            _ = await this.Service.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID });
        }
        catch ( OperationFailedException _ )
        {
            failure = _;
        }

        Check.That(failure).IsNotNull();
        Check.That(failure!.FailureCode).IsEqualTo(SegmentedTransferFailureCode.SessionNotFound);
    }

    [Test] [Order(9)]
    public async Task OpenGetTransferCanFollowRealizedStagedRanges()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:64);
        Byte[] payload = SegmentedTransferExamHelpers.CreatePayload(16,0x61);
        using SegmentedTransferExamHelpers.TransferSegmentExamPayload write = SegmentedTransferExamHelpers.CreatePutTransferSegment(open,8,payload);

        await this.Service!.OpenUploadTransfer(open);
        _ = await this.Service.PutTransferSegment(write.Request,write.Payload);

        OpenGetTransferResponse follower = await this.Service.OpenGetTransfer(new OpenGetTransferRequest(){ SessionID = Guid.NewGuid() , ItemID = open.ItemID , SourceSessionID = open.SessionID });

        using SegmentedTransferReadResult read = await this.Service.GetTransferSegment(new GetTransferSegmentRequest(){ SessionID = follower.State.SessionID , ItemID = open.ItemID , Offset = 8 , Length = payload.Length });
        using MemoryStream loaded = new();

        await read.Payload.CopyToAsync(loaded);

        Check.That(follower.State.Mode).IsEqualTo(DataItemTransferMode.ReadStaged);
        Check.That(follower.State.SourceSessionID).IsEqualTo(open.SessionID);
        Check.That(loaded.ToArray().SequenceEqual(payload)).IsTrue();
    }

    [Test] [Order(10)]
    public async Task OpenGetTransferCommittedReadFillsLocalCacheFromPublishedSource()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:64,streamSeed:0x71);
        Byte[] streamPayload = SegmentedTransferExamHelpers.CreatePayload(64,0x71);
        Byte[] objectPayload = open.ObjectPayload;
        ISegmentedTransferStorage storage = SegmentedTransferExamHelpers.CreateStorage(this.RootPath!);
        IDataItemTransferRegistry registry = new DataItemTransferRegistry(storage,StreamTransferExamHelpers.CreateStorage(Path.Combine(this.RootPath!,"registry","stream-read")));
        IDataItemTransferCoordinator coordinator = new DataItemTransferCoordinator();
        IDataItemSegmentedTransferService service = new DataItemSegmentedTransferService(storage,registry,coordinator,new InMemoryPublishedTransferSource(open.ItemID,objectPayload,streamPayload,open.ObjectInfo),Options.Create(new DataItemTransferServiceOptions()));

        OpenGetTransferResponse response = await service.OpenGetTransfer(new OpenGetTransferRequest(){ SessionID = Guid.NewGuid() , ItemID = open.ItemID });

        Check.That(response.State.Mode).IsEqualTo(DataItemTransferMode.ReadCommitted);
        Check.That(response.State.RealizedRanges).HasSize(0);
        Check.That(response.State.MissingRanges).HasSize(1);

        using SegmentedTransferReadResult read = await service.GetTransferSegment(new GetTransferSegmentRequest(){ SessionID = response.State.SessionID , ItemID = open.ItemID , Offset = 0 , Length = streamPayload.Length });
        using MemoryStream loaded = new();

        await read.Payload.CopyToAsync(loaded);

        GetTransferStateResponse state = await service.GetTransferState(new DataItemTransferIdentity(){ SessionID = response.State.SessionID , ItemID = open.ItemID });

        Check.That(loaded.ToArray().SequenceEqual(streamPayload)).IsTrue();
        Check.That(state.State.RealizedRanges).HasSize(1);
        Check.That(state.State.RealizedRanges[0].Offset).IsEqualTo(0);
        Check.That(state.State.RealizedRanges[0].Length).IsEqualTo(streamPayload.Length);
    }

    [Test] [Order(11)]
    public async Task PutTransferSegmentRejectsOversizeSegmentBeforeReadingPayload()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:64) with
        {
            RequestedSegmentSizePolicy = new DataItemTransferSegmentSizePolicy() { MinSegmentBytes = 1 , PreferredSegmentBytes = 8 , MaxSegmentBytes = 8 },
        };

        _ = await this.Service!.OpenUploadTransfer(open);

        PutTransferSegmentRequest request = new()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            Offset = 0,
            Length = 16,
            ExpectedStateVersion = 0,
            SegmentSHA512 = new Byte[64],
            StreamSHA512 = open.StreamSHA512,
            RequestID = Guid.NewGuid(),
        };

        using ThrowOnReadStream stream = new();

        OperationFailedException? failure = null;

        try
        {
            _ = await this.Service.PutTransferSegment(request,stream);
        }
        catch ( OperationFailedException _ )
        {
            failure = _;
        }

        Check.That(failure).IsNotNull();
        Check.That(failure!.FailureCode).IsEqualTo(SegmentedTransferFailureCode.InvalidSegmentLength);
    }

    [Test] [Order(12)]
    public async Task BeginCommitUploadTransferFreezesSessionAsCommitting()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:32,streamSeed:0x81);
        Byte[] fullPayload = SegmentedTransferExamHelpers.CreatePayload(32,0x81);
        using SegmentedTransferExamHelpers.TransferSegmentExamPayload write = SegmentedTransferExamHelpers.CreatePutTransferSegment(open,0,fullPayload);

        await this.Service!.OpenUploadTransfer(open);
        PutTransferSegmentResponse stored = await this.Service.PutTransferSegment(write.Request,write.Payload);

        CommitUploadTransferResponse begun = await this.Service.BeginCommitUploadTransfer(new CommitUploadTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = stored.StateVersion,
        });

        OperationFailedException? reopenFailure = null;
        OperationFailedException? putFailure = null;

        try
        {
            _ = await this.Service.ReOpenUploadTransfer(new ReOpenUploadTransferRequest()
            {
                SessionID = open.SessionID,
                ItemID = open.ItemID,
                ObjectSHA512 = open.ObjectSHA512,
                StreamSHA512 = open.StreamSHA512,
                StreamLength = open.StreamLength,
            });
        }
        catch ( OperationFailedException _ )
        {
            reopenFailure = _;
        }

        using SegmentedTransferExamHelpers.TransferSegmentExamPayload duplicate = SegmentedTransferExamHelpers.CreatePutTransferSegment(open,0,fullPayload,stored.StateVersion);

        try
        {
            _ = await this.Service.PutTransferSegment(duplicate.Request,duplicate.Payload);
        }
        catch ( OperationFailedException _ )
        {
            putFailure = _;
        }

        Check.That(begun.State.Status).IsEqualTo(DataItemTransferStatus.Committing);
        Check.That(reopenFailure).IsNotNull();
        Check.That(reopenFailure!.FailureCode).IsEqualTo(SegmentedTransferFailureCode.InvalidSessionState);
        Check.That(putFailure).IsNotNull();
        Check.That(putFailure!.FailureCode).IsEqualTo(SegmentedTransferFailureCode.InvalidSessionState);
    }

    [Test] [Order(13)]
    public async Task CancelCommitUploadTransferRestoresRetryableState()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:32,streamSeed:0x82);
        Byte[] fullPayload = SegmentedTransferExamHelpers.CreatePayload(32,0x82);
        using SegmentedTransferExamHelpers.TransferSegmentExamPayload write = SegmentedTransferExamHelpers.CreatePutTransferSegment(open,0,fullPayload);

        await this.Service!.OpenUploadTransfer(open);
        PutTransferSegmentResponse stored = await this.Service.PutTransferSegment(write.Request,write.Payload);

        CommitUploadTransferResponse begun = await this.Service.BeginCommitUploadTransfer(new CommitUploadTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = stored.StateVersion,
        });

        GetTransferStateResponse cancelled = await this.Service.CancelCommitUploadTransfer(new DataItemTransferIdentity()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
        });

        CommitUploadTransferResponse retried = await this.Service.BeginCommitUploadTransfer(new CommitUploadTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = cancelled.State.StateVersion,
        });

        CommitUploadTransferResponse completed = await this.Service.CompleteCommitUploadTransfer(new DataItemTransferIdentity()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
        });

        Check.That(begun.State.Status).IsEqualTo(DataItemTransferStatus.Committing);
        Check.That(cancelled.State.Status).IsEqualTo(DataItemTransferStatus.Complete);
        Check.That(retried.State.Status).IsEqualTo(DataItemTransferStatus.Committing);
        Check.That(completed.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
    }

    [Test] [Order(14)]
    public async Task CompleteCommitUploadTransferFinalizesCommittedState()
    {
        OpenUploadTransferRequest open = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:32,streamSeed:0x83);
        Byte[] fullPayload = SegmentedTransferExamHelpers.CreatePayload(32,0x83);
        using SegmentedTransferExamHelpers.TransferSegmentExamPayload write = SegmentedTransferExamHelpers.CreatePutTransferSegment(open,0,fullPayload);

        await this.Service!.OpenUploadTransfer(open);
        PutTransferSegmentResponse stored = await this.Service.PutTransferSegment(write.Request,write.Payload);

        _ = await this.Service.BeginCommitUploadTransfer(new CommitUploadTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = stored.StateVersion,
        });

        CommitUploadTransferResponse completed = await this.Service.CompleteCommitUploadTransfer(new DataItemTransferIdentity()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
        });

        Check.That(completed.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(completed.Published).IsFalse();
    }

    [Test] [Order(15)]
    public async Task OpenUploadTransferRejectsActiveStreamWriterForSameItem()
    {
        IDataStreamTransferService streamService = CreateShadowStreamService();
        OpenUploadTransferRequest segmentedOpen = SegmentedTransferExamHelpers.CreateOpenUploadRequest(itemId:Guid.NewGuid());
        OpenStreamTransferRequest streamOpen = StreamTransferExamHelpers.CreateOpenRequest(itemId:segmentedOpen.ItemID);

        _ = await streamService.OpenStreamTransfer(streamOpen);

        OperationFailedException? failure = null;

        try
        {
            _ = await this.Service!.OpenUploadTransfer(segmentedOpen);
        }
        catch ( OperationFailedException _ )
        {
            failure = _;
        }

        Check.That(failure).IsNotNull();
        Check.That(failure!.FailureCode).IsEqualTo(SegmentedTransferFailureCode.ConflictOpenSession);
    }

    [Test] [Order(16)]
    public async Task OpenUploadTransferRejectsCommittedStreamWriterForSameItem()
    {
        IDataStreamTransferService streamService = CreateShadowStreamService();
        OpenUploadTransferRequest segmentedOpen = SegmentedTransferExamHelpers.CreateOpenUploadRequest(itemId:Guid.NewGuid());
        OpenStreamTransferRequest streamOpen = StreamTransferExamHelpers.CreateOpenRequest(itemId:segmentedOpen.ItemID);
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(12,0x92);

        _ = await streamService.OpenStreamTransfer(streamOpen);
        using StreamTransferExamHelpers.StreamTransferExamAppend append = StreamTransferExamHelpers.CreateAppend(streamOpen.SessionID,streamOpen.ItemID,0,payload);
        PutStreamTransferSegmentResponse written = await streamService.PutStreamTransferSegment(append.Request,append.Payload);
        CompleteStreamTransferResponse begun = await streamService.BeginCompleteStreamTransfer(
            StreamTransferExamHelpers.CreateCompleteRequest(streamOpen,written.StateVersion,payload.Length,SHA512.HashData(payload)));
        _ = await streamService.FinishCompleteStreamTransfer(new DataItemTransferIdentity(){ SessionID = begun.State.SessionID , ItemID = begun.State.ItemID });

        OperationFailedException? failure = null;

        try
        {
            _ = await this.Service!.OpenUploadTransfer(segmentedOpen);
        }
        catch ( OperationFailedException _ )
        {
            failure = _;
        }

        Check.That(failure).IsNotNull();
        Check.That(failure!.FailureCode).IsEqualTo(SegmentedTransferFailureCode.ConflictOpenSession);
    }

    [Test] [Order(17)]
    public async Task OpenUploadTransferAllowsRetryAfterCommittedStreamWriterRemoved()
    {
        IDataStreamTransferService streamService = CreateShadowStreamService();
        OpenUploadTransferRequest segmentedOpen = SegmentedTransferExamHelpers.CreateOpenUploadRequest(itemId:Guid.NewGuid());
        OpenStreamTransferRequest streamOpen = StreamTransferExamHelpers.CreateOpenRequest(itemId:segmentedOpen.ItemID);
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(12,0x93);

        _ = await streamService.OpenStreamTransfer(streamOpen);
        using StreamTransferExamHelpers.StreamTransferExamAppend append = StreamTransferExamHelpers.CreateAppend(streamOpen.SessionID,streamOpen.ItemID,0,payload);
        PutStreamTransferSegmentResponse written = await streamService.PutStreamTransferSegment(append.Request,append.Payload);
        CompleteStreamTransferResponse begun = await streamService.BeginCompleteStreamTransfer(
            StreamTransferExamHelpers.CreateCompleteRequest(streamOpen,written.StateVersion,payload.Length,SHA512.HashData(payload)));
        _ = await streamService.FinishCompleteStreamTransfer(new DataItemTransferIdentity(){ SessionID = begun.State.SessionID , ItemID = begun.State.ItemID });
        _ = await streamService.RemoveTransfer(new RemoveTransferRequest(){ SessionID = streamOpen.SessionID , ItemID = streamOpen.ItemID });

        OpenUploadTransferResponse opened = await this.Service!.OpenUploadTransfer(segmentedOpen);

        Check.That(opened.State.SessionID).IsEqualTo(segmentedOpen.SessionID);
        Check.That(opened.State.ItemID).IsEqualTo(segmentedOpen.ItemID);
    }

    private IDataStreamTransferService CreateShadowStreamService()
    {
        IStreamTransferStorage storage = StreamTransferExamHelpers.CreateStorage(Path.Combine(this.RootPath!,"registry","stream"));
        IDataItemTransferRegistry registry = new DataItemTransferRegistry(SegmentedTransferExamHelpers.CreateStorage(this.RootPath!),storage);

        return new DataStreamTransferService(storage,registry,new DataItemTransferCoordinator(),Options.Create(new DataItemTransferServiceOptions(){ EnforceMinimumSegmentBytes = false , EnforceMaximumSegmentBytes = true }));
    }

    internal sealed class InMemoryPublishedTransferSource : IPublishedTransferSource
    {
        private readonly Guid itemId;
        private readonly Byte[] objectPayload;
        private readonly Byte[] streamPayload;
        private readonly DataItemTransferObjectInfo? objectInfo;

        public InMemoryPublishedTransferSource(Guid itemId , Byte[] objectPayload , Byte[] streamPayload , DataItemTransferObjectInfo? objectInfo)
        {
            this.itemId = itemId;
            this.objectPayload = objectPayload;
            this.streamPayload = streamPayload;
            this.objectInfo = objectInfo;
        }

        public Task<Stream?> OpenReadRange(PublishedTransferSnapshot snapshot , DataItemTransferRange range , CancellationToken cancel = default)
        {
            if(snapshot.ItemId != this.itemId || range.Validate() is false || range.EndOffsetExclusive > this.streamPayload.LongLength) { return Task.FromResult<Stream?>(null); }

            Stream stream = new MemoryStream(this.streamPayload,checked((Int32)range.Offset),checked((Int32)range.Length),writable:false,publiclyVisible:false);
            return Task.FromResult<Stream?>(stream);
        }

        public Task<PublishedTransferSnapshot?> TryLoadSnapshot(Guid itemId , CancellationToken cancel = default)
        {
            if(itemId != this.itemId) { return Task.FromResult<PublishedTransferSnapshot?>(null); }

            return Task.FromResult<PublishedTransferSnapshot?>(new PublishedTransferSnapshot()
            {
                ItemId = itemId,
                ObjectPayload = this.objectPayload,
                ObjectSHA512 = SHA512.HashData(this.objectPayload),
                StreamSHA512 = SHA512.HashData(this.streamPayload),
                StreamLength = this.streamPayload.LongLength,
                RealizedRanges = this.streamPayload.Length == 0 ? Array.Empty<DataItemTransferRange>() : new[]{ new DataItemTransferRange(){ Offset = 0 , Length = this.streamPayload.LongLength } },
                ObjectInfo = this.objectInfo,
            });
        }
    }

    private sealed class ThrowOnReadStream : Stream
    {
        public override Boolean CanRead => true;
        public override Boolean CanSeek => false;
        public override Boolean CanWrite => false;
        public override Int64 Length => throw new NotSupportedException();
        public override Int64 Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush() { }

        public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count)
        {
            throw new InvalidOperationException("Payload should not be read.");
        }

        public override Int64 Seek(Int64 offset , SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(Int64 value)
        {
            throw new NotSupportedException();
        }

        public override void Write(Byte[] buffer , Int32 offset , Int32 count)
        {
            throw new NotSupportedException();
        }
    }
}
