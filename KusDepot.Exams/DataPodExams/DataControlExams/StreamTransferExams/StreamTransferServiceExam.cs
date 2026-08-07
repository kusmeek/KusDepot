namespace KusDepot.DataServices;

[TestFixture] [NonParallelizable]
public class StreamTransferServiceExam
{
    private String? RootPath;
    private IDataStreamTransferService? Service;

    [OneTimeSetUp]
    public void Calibrate()
    {
        if(OperatingSystem.IsWindows() is false) { Assert.Ignore("Stream transfer currently relies on Windows-only staging storage."); }

        this.RootPath = Path.Combine(Path.GetTempPath(),"KusDepot","Exam","StreamTransferService",Guid.NewGuid().ToString("N",InvariantCulture));

        IStreamTransferStorage storage = StreamTransferExamHelpers.CreateStorage(this.RootPath);
        IDataItemTransferRegistry registry = new DataItemTransferRegistry(SegmentedTransferExamHelpers.CreateStorage(Path.Combine(this.RootPath,"registry","segmented")),storage);
        IDataItemTransferCoordinator coordinator = new DataItemTransferCoordinator();

        this.Service = new DataStreamTransferService(storage,registry,coordinator,Options.Create(new DataItemTransferServiceOptions(){ EnforceMinimumSegmentBytes = false , EnforceMaximumSegmentBytes = true }));
    }

    [OneTimeTearDown]
    public void Complete()
    {
        if(String.IsNullOrWhiteSpace(this.RootPath) is false && Directory.Exists(this.RootPath)) { Directory.Delete(this.RootPath,true); }
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

    [Test] [Order(1)]
    public async Task OpenStreamTransferCreatesInitialState()
    {
        OpenStreamTransferRequest request = StreamTransferExamHelpers.CreateOpenRequest();

        OpenStreamTransferResponse response = await this.Service!.OpenStreamTransfer(request);

        Check.That(response.State.SessionID).IsEqualTo(request.SessionID);
        Check.That(response.State.ItemID).IsEqualTo(request.ItemID);
        Check.That(response.State.Mode).IsEqualTo(DataItemTransferMode.StreamUpload);
        Check.That(response.State.Status).IsEqualTo(DataItemTransferStatus.Open);
        Check.That(response.State.AppendedLength).IsEqualTo(0);
        Check.That(response.State.StateVersion).IsEqualTo(0);
    }

    [Test] [Order(2)]
    public void OpenStreamTransferRejectsSecondActiveSourceForSameItem()
    {
        OpenStreamTransferRequest first = StreamTransferExamHelpers.CreateOpenRequest();
        OpenStreamTransferRequest second = StreamTransferExamHelpers.CreateOpenRequest(itemId:first.ItemID);

        _ = this.Service!.OpenStreamTransfer(first).Result;

        OperationFailedException? failure = null;

        try
        {
            _ = this.Service!.OpenStreamTransfer(second).Result;
        }
        catch ( AggregateException _ ) when(_.InnerException is OperationFailedException inner)
        {
            failure = inner;
        }

        Check.That(failure).IsNotNull();
        Check.That(failure!.FailureCode).IsEqualTo(StreamTransferFailureCode.ConflictOpenSession);
    }

    [Test] [Order(3)]
    public async Task PutStreamTransferSegmentUpdatesStateAndSupportsReadback()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(16,0x31);

        await this.Service!.OpenStreamTransfer(open);
        using StreamTransferExamHelpers.StreamTransferExamAppend append = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,payload);

        PutStreamTransferSegmentResponse written = await this.Service.PutStreamTransferSegment(append.Request,append.Payload);
        GetStreamTransferStateResponse state = await this.Service.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID });
        using StreamTransferReadResult read = await this.Service.GetStreamTransferSegment(StreamTransferExamHelpers.CreateReadRequest(open.SessionID,open.ItemID,0,payload.Length));
        using MemoryStream loaded = new();

        await read.Payload.CopyToAsync(loaded);
        StreamTransferSegmentFooter footer = read.FinalizeFooter();

        Check.That(written.Accepted).IsTrue();
        Check.That(written.AppendedLength).IsEqualTo(payload.Length);
        Check.That(state.State.AppendedLength).IsEqualTo(payload.Length);
        Check.That(state.State.StateVersion).IsEqualTo(written.StateVersion);
        Check.That(loaded.ToArray().SequenceEqual(payload)).IsTrue();
        Check.That(footer.ReturnedLength).IsEqualTo(payload.Length);
        Check.That(footer.AppendedLength).IsEqualTo(payload.Length);
        Check.That(footer.SegmentSHA512.SequenceEqual(SHA512.HashData(payload))).IsTrue();
    }

    [Test] [Order(4)]
    public async Task PutStreamTransferSegmentRejectsWrongExpectedStateVersion()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(8,0x41);

        await this.Service!.OpenStreamTransfer(open);
        using StreamTransferExamHelpers.StreamTransferExamAppend append = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,payload,expectedStateVersion:9);

        Check.ThatAsyncCode(async () => await this.Service.PutStreamTransferSegment(append.Request,append.Payload)).Throws<OperationFailedException>();
    }

    [Test] [Order(5)]
    public async Task PutStreamTransferSegmentRejectsOffsetMismatch()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();
        Byte[] firstPayload = StreamTransferExamHelpers.CreatePayload(8,0x51);
        Byte[] secondPayload = StreamTransferExamHelpers.CreatePayload(8,0x61);

        await this.Service!.OpenStreamTransfer(open);
        using StreamTransferExamHelpers.StreamTransferExamAppend first = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,firstPayload);
        _ = await this.Service.PutStreamTransferSegment(first.Request,first.Payload);
        using StreamTransferExamHelpers.StreamTransferExamAppend second = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,secondPayload,expectedStateVersion:1);

        OperationFailedException? failure = null;

        try
        {
            _ = await this.Service.PutStreamTransferSegment(second.Request,second.Payload);
        }
        catch ( OperationFailedException _ )
        {
            failure = _;
        }

        Check.That(failure).IsNotNull();
        Check.That(failure!.FailureCode).IsEqualTo(StreamTransferFailureCode.AppendOffsetMismatch);
    }

    [Test] [Order(6)]
    public async Task OpenFollowStreamTransferCreatesFollowerAndReadsCurrentPrefix()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(12,0x71);

        await this.Service!.OpenStreamTransfer(open);
        using StreamTransferExamHelpers.StreamTransferExamAppend append = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,payload);
        _ = await this.Service.PutStreamTransferSegment(append.Request,append.Payload);

        OpenFollowStreamTransferRequest follow = StreamTransferExamHelpers.CreateFollowRequest(open);
        OpenFollowStreamTransferResponse follower = await this.Service.OpenFollowStreamTransfer(follow);
        using StreamTransferReadResult read = await this.Service.GetStreamTransferSegment(StreamTransferExamHelpers.CreateReadRequest(follow.SessionID,follow.ItemID,0,payload.Length));
        using MemoryStream loaded = new();

        await read.Payload.CopyToAsync(loaded);

        Check.That(follower.State.Mode).IsEqualTo(DataItemTransferMode.StreamFollow);
        Check.That(follower.State.SourceSessionID).IsEqualTo(open.SessionID);
        Check.That(follower.State.AppendedLength).IsEqualTo(payload.Length);
        Check.That(loaded.ToArray().SequenceEqual(payload)).IsTrue();
    }

    [Test] [Order(7)]
    public async Task ReOpenStreamTransferReturnsCurrentSourceState()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(10,0x81);

        await this.Service!.OpenStreamTransfer(open);
        using StreamTransferExamHelpers.StreamTransferExamAppend append = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,payload);
        PutStreamTransferSegmentResponse written = await this.Service.PutStreamTransferSegment(append.Request,append.Payload);

        ReOpenStreamTransferResponse reopened = await this.Service.ReOpenStreamTransfer(StreamTransferExamHelpers.CreateReOpenRequest(open));

        Check.That(reopened.State.SessionID).IsEqualTo(open.SessionID);
        Check.That(reopened.State.ItemID).IsEqualTo(open.ItemID);
        Check.That(reopened.State.AppendedLength).IsEqualTo(payload.Length);
        Check.That(reopened.State.StateVersion).IsEqualTo(written.StateVersion);
    }

    [Test] [Order(8)]
    public async Task ReOpenFollowStreamTransferSynchronizesFollowerState()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();
        Byte[] firstPayload = StreamTransferExamHelpers.CreatePayload(8,0x91);
        Byte[] secondPayload = StreamTransferExamHelpers.CreatePayload(8,0xA1);

        await this.Service!.OpenStreamTransfer(open);
        using StreamTransferExamHelpers.StreamTransferExamAppend first = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,firstPayload);
        _ = await this.Service.PutStreamTransferSegment(first.Request,first.Payload);

        OpenFollowStreamTransferRequest follow = StreamTransferExamHelpers.CreateFollowRequest(open);
        _ = await this.Service.OpenFollowStreamTransfer(follow);

        using StreamTransferExamHelpers.StreamTransferExamAppend second = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,firstPayload.Length,secondPayload,expectedStateVersion:1);
        _ = await this.Service.PutStreamTransferSegment(second.Request,second.Payload);

        ReOpenFollowStreamTransferResponse reopened = await this.Service.ReOpenFollowStreamTransfer(StreamTransferExamHelpers.CreateReOpenFollowRequest(follow));

        Check.That(reopened.State.SessionID).IsEqualTo(follow.SessionID);
        Check.That(reopened.State.SourceSessionID).IsEqualTo(open.SessionID);
        Check.That(reopened.State.AppendedLength).IsEqualTo(firstPayload.Length + secondPayload.Length);
    }

    [Test] [Order(9)]
    public async Task CompleteStreamTransferMarksCommitted()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(14,0xB1);

        await this.Service!.OpenStreamTransfer(open);
        using StreamTransferExamHelpers.StreamTransferExamAppend append = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,payload);
        PutStreamTransferSegmentResponse written = await this.Service.PutStreamTransferSegment(append.Request,append.Payload);

        CompleteStreamTransferResponse begun = await this.Service.BeginCompleteStreamTransfer(
            StreamTransferExamHelpers.CreateCompleteRequest(open,written.StateVersion,payload.Length,SHA512.HashData(payload)));
        CompleteStreamTransferResponse completed = await this.Service.FinishCompleteStreamTransfer(new DataItemTransferIdentity()
        {
            SessionID = begun.State.SessionID,
            ItemID = begun.State.ItemID,
        });

        Check.That(completed.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(completed.State.AppendedLength).IsEqualTo(payload.Length);
        Check.That(completed.Published).IsFalse();
    }

    [Test] [Order(10)]
    public async Task BeginCompleteStreamTransferFreezesSessionAsCommitted()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(14,0xD1);

        await this.Service!.OpenStreamTransfer(open);
        using StreamTransferExamHelpers.StreamTransferExamAppend append = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,payload);
        PutStreamTransferSegmentResponse written = await this.Service.PutStreamTransferSegment(append.Request,append.Payload);

        CompleteStreamTransferResponse begun = await this.Service.BeginCompleteStreamTransfer(
            StreamTransferExamHelpers.CreateCompleteRequest(open,written.StateVersion,payload.Length,SHA512.HashData(payload)));

        OperationFailedException? reopenFailure = null;
        OperationFailedException? appendFailure = null;

        try
        {
            _ = await this.Service.ReOpenStreamTransfer(StreamTransferExamHelpers.CreateReOpenRequest(open,begun.State.StreamSHA512));
        }
        catch ( OperationFailedException _ )
        {
            reopenFailure = _;
        }

        using StreamTransferExamHelpers.StreamTransferExamAppend duplicate = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,payload.Length,StreamTransferExamHelpers.CreatePayload(4,0xD2),expectedStateVersion:written.StateVersion);

        try
        {
            _ = await this.Service.PutStreamTransferSegment(duplicate.Request,duplicate.Payload);
        }
        catch ( OperationFailedException _ )
        {
            appendFailure = _;
        }

        Check.That(begun.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(reopenFailure).IsNotNull();
        Check.That(reopenFailure!.FailureCode).IsEqualTo(StreamTransferFailureCode.InvalidSessionState);
        Check.That(appendFailure).IsNotNull();
        Check.That(appendFailure!.FailureCode).IsEqualTo(StreamTransferFailureCode.InvalidSessionState);
    }

    [Test] [Order(11)]
    public async Task CancelCompleteStreamTransferRestoresRetryableState()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(14,0xE1);

        await this.Service!.OpenStreamTransfer(open);
        using StreamTransferExamHelpers.StreamTransferExamAppend append = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,payload);
        PutStreamTransferSegmentResponse written = await this.Service.PutStreamTransferSegment(append.Request,append.Payload);

        CompleteStreamTransferResponse begun = await this.Service.BeginCompleteStreamTransfer(
            StreamTransferExamHelpers.CreateCompleteRequest(open,written.StateVersion,payload.Length,SHA512.HashData(payload)));

        GetStreamTransferStateResponse cancelled = await this.Service.CancelCompleteStreamTransfer(new DataItemTransferIdentity()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
        });

        CompleteStreamTransferResponse retried = await this.Service.BeginCompleteStreamTransfer(
            StreamTransferExamHelpers.CreateCompleteRequest(open,cancelled.State.StateVersion,payload.Length,SHA512.HashData(payload)));

        CompleteStreamTransferResponse finished = await this.Service.FinishCompleteStreamTransfer(new DataItemTransferIdentity()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
        });

        Check.That(begun.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(cancelled.State.Status).IsEqualTo(DataItemTransferStatus.Open);
        Check.That(retried.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(finished.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
    }

    [Test] [Order(12)]
    public async Task FinishCompleteStreamTransferReturnsCommittedState()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(14,0xF1);

        await this.Service!.OpenStreamTransfer(open);
        using StreamTransferExamHelpers.StreamTransferExamAppend append = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,payload);
        PutStreamTransferSegmentResponse written = await this.Service.PutStreamTransferSegment(append.Request,append.Payload);

        _ = await this.Service.BeginCompleteStreamTransfer(
            StreamTransferExamHelpers.CreateCompleteRequest(open,written.StateVersion,payload.Length,SHA512.HashData(payload)));

        CompleteStreamTransferResponse finished = await this.Service.FinishCompleteStreamTransfer(new DataItemTransferIdentity()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
        });

        Check.That(finished.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(finished.State.AppendedLength).IsEqualTo(payload.Length);
        Check.That(finished.Published).IsFalse();
    }

    [Test] [Order(13)]
    public async Task AbortTransferDeletesSession()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();

        await this.Service!.OpenStreamTransfer(open);

        AbortTransferResponse aborted = await this.Service.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID });

        Check.That(aborted.Aborted).IsTrue();

        OperationFailedException? failure = null;

        try
        {
            _ = await this.Service.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID });
        }
        catch ( OperationFailedException _ )
        {
            failure = _;
        }

        Check.That(failure).IsNotNull();
        Check.That(failure!.FailureCode).IsEqualTo(StreamTransferFailureCode.SessionNotFound);
    }

    [Test] [Order(14)]
    public async Task RemoveTransferDeletesCommittedSession()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest();
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(6,0xC1);

        await this.Service!.OpenStreamTransfer(open);
        using StreamTransferExamHelpers.StreamTransferExamAppend append = StreamTransferExamHelpers.CreateAppend(open.SessionID,open.ItemID,0,payload);
        PutStreamTransferSegmentResponse written = await this.Service.PutStreamTransferSegment(append.Request,append.Payload);
        CompleteStreamTransferResponse begun = await this.Service.BeginCompleteStreamTransfer(
            StreamTransferExamHelpers.CreateCompleteRequest(open,written.StateVersion,payload.Length,SHA512.HashData(payload)));
        _ = await this.Service.FinishCompleteStreamTransfer(new DataItemTransferIdentity()
        {
            SessionID = begun.State.SessionID,
            ItemID = begun.State.ItemID,
        });

        RemoveTransferResponse removed = await this.Service.RemoveTransfer(new RemoveTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID });

        Check.That(removed.Removed).IsTrue();

        OperationFailedException? failure = null;

        try
        {
            _ = await this.Service.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID });
        }
        catch ( OperationFailedException _ )
        {
            failure = _;
        }

        Check.That(failure).IsNotNull();
        Check.That(failure!.FailureCode).IsEqualTo(StreamTransferFailureCode.SessionNotFound);
    }

    [Test] [Order(15)]
    public async Task PutStreamTransferSegmentRejectsOversizeSegmentBeforeReadingPayload()
    {
        OpenStreamTransferRequest open = StreamTransferExamHelpers.CreateOpenRequest() with
        {
            RequestedSegmentSizePolicy = new DataItemTransferSegmentSizePolicy() { MinSegmentBytes = 1 , PreferredSegmentBytes = 8 , MaxSegmentBytes = 8 },
        };

        Byte[] payload = StreamTransferExamHelpers.CreatePayload(16,0x59);

        _ = await this.Service!.OpenStreamTransfer(open);

        PutStreamTransferSegmentRequest request = new()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            Offset = 0,
            Length = 16,
            ExpectedStateVersion = 0,
            SegmentSHA512 = new Byte[64],
            RequestID = Guid.NewGuid(),
        };

        using ThrowOnReadStream stream = new();

        OperationFailedException? failure = null;

        try
        {
            _ = await this.Service.PutStreamTransferSegment(request,stream);
        }
        catch ( OperationFailedException _ )
        {
            failure = _;
        }

        Check.That(failure).IsNotNull();
        Check.That(failure!.FailureCode).IsEqualTo(StreamTransferFailureCode.InvalidSegmentLength);
    }

    [Test] [Order(16)]
    public async Task OpenStreamTransferRejectsActiveSegmentedWriterForSameItem()
    {
        IDataItemSegmentedTransferService segmentedService = CreateShadowSegmentedService();
        OpenStreamTransferRequest streamOpen = StreamTransferExamHelpers.CreateOpenRequest(itemId:Guid.NewGuid());
        OpenUploadTransferRequest segmentedOpen = SegmentedTransferExamHelpers.CreateOpenUploadRequest(itemId:streamOpen.ItemID);

        _ = await segmentedService.OpenUploadTransfer(segmentedOpen);

        OperationFailedException? failure = null;

        try
        {
            _ = await this.Service!.OpenStreamTransfer(streamOpen);
        }
        catch ( OperationFailedException _ )
        {
            failure = _;
        }

        Check.That(failure).IsNotNull();
        Check.That(failure!.FailureCode).IsEqualTo(StreamTransferFailureCode.ConflictOpenSession);
    }

    [Test] [Order(17)]
    public async Task OpenStreamTransferRejectsCommittedSegmentedWriterForSameItem()
    {
        IDataItemSegmentedTransferService segmentedService = CreateShadowSegmentedService();
        OpenStreamTransferRequest streamOpen = StreamTransferExamHelpers.CreateOpenRequest(itemId:Guid.NewGuid());
        OpenUploadTransferRequest segmentedOpen = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:12,streamSeed:0xA2,itemId:streamOpen.ItemID);
        Byte[] payload = SegmentedTransferExamHelpers.CreatePayload(12,0xA2);

        _ = await segmentedService.OpenUploadTransfer(segmentedOpen);
        using SegmentedTransferExamHelpers.TransferSegmentExamPayload write = SegmentedTransferExamHelpers.CreatePutTransferSegment(segmentedOpen,0,payload);
        PutTransferSegmentResponse stored = await segmentedService.PutTransferSegment(write.Request,write.Payload);
        _ = await segmentedService.BeginCommitUploadTransfer(new CommitUploadTransferRequest(){ SessionID = segmentedOpen.SessionID , ItemID = segmentedOpen.ItemID , ExpectedStateVersion = stored.StateVersion });
        _ = await segmentedService.CompleteCommitUploadTransfer(new DataItemTransferIdentity(){ SessionID = segmentedOpen.SessionID , ItemID = segmentedOpen.ItemID });

        OperationFailedException? failure = null;

        try
        {
            _ = await this.Service!.OpenStreamTransfer(streamOpen);
        }
        catch ( OperationFailedException _ )
        {
            failure = _;
        }

        Check.That(failure).IsNotNull();
        Check.That(failure!.FailureCode).IsEqualTo(StreamTransferFailureCode.ConflictOpenSession);
    }

    [Test] [Order(18)]
    public async Task OpenStreamTransferAllowsRetryAfterCommittedSegmentedWriterRemoved()
    {
        IDataItemSegmentedTransferService segmentedService = CreateShadowSegmentedService();
        OpenStreamTransferRequest streamOpen = StreamTransferExamHelpers.CreateOpenRequest(itemId:Guid.NewGuid());
        OpenUploadTransferRequest segmentedOpen = SegmentedTransferExamHelpers.CreateOpenUploadRequest(streamLength:12,streamSeed:0xA3,itemId:streamOpen.ItemID);
        Byte[] payload = SegmentedTransferExamHelpers.CreatePayload(12,0xA3);

        _ = await segmentedService.OpenUploadTransfer(segmentedOpen);
        using SegmentedTransferExamHelpers.TransferSegmentExamPayload write = SegmentedTransferExamHelpers.CreatePutTransferSegment(segmentedOpen,0,payload);
        PutTransferSegmentResponse stored = await segmentedService.PutTransferSegment(write.Request,write.Payload);
        _ = await segmentedService.BeginCommitUploadTransfer(new CommitUploadTransferRequest(){ SessionID = segmentedOpen.SessionID , ItemID = segmentedOpen.ItemID , ExpectedStateVersion = stored.StateVersion });
        _ = await segmentedService.CompleteCommitUploadTransfer(new DataItemTransferIdentity(){ SessionID = segmentedOpen.SessionID , ItemID = segmentedOpen.ItemID });
        _ = await segmentedService.RemoveTransfer(new RemoveTransferRequest(){ SessionID = segmentedOpen.SessionID , ItemID = segmentedOpen.ItemID });

        OpenStreamTransferResponse opened = await this.Service!.OpenStreamTransfer(streamOpen);

        Check.That(opened.State.SessionID).IsEqualTo(streamOpen.SessionID);
        Check.That(opened.State.ItemID).IsEqualTo(streamOpen.ItemID);
    }

    private IDataItemSegmentedTransferService CreateShadowSegmentedService()
    {
        ISegmentedTransferStorage storage = SegmentedTransferExamHelpers.CreateStorage(Path.Combine(this.RootPath!,"registry","segmented"));
        IDataItemTransferRegistry registry = new DataItemTransferRegistry(storage,StreamTransferExamHelpers.CreateStorage(this.RootPath!));

        return new DataItemSegmentedTransferService(storage,registry,new DataItemTransferCoordinator(),new NullPublishedTransferSourceAccessor(),Options.Create(new DataItemTransferServiceOptions(){ EnforceMinimumSegmentBytes = false , EnforceMaximumSegmentBytes = true }));
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
