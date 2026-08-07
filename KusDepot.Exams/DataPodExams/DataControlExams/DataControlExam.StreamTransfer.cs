namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test] [Order(25)]
    public async Task StreamTransferRoundTrip()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        OpenStreamTransferRequest open = CreateOpenStreamTransferRequest();

        RestResponse<OpenStreamTransferResponse> opened = await _dc.OpenStreamTransfer(open);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        opened = await _dc.OpenStreamTransfer(open,this.ReadToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        opened = await _dc.OpenStreamTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(opened.Data).IsNotNull();
        Check.That(opened.Data!.State.SessionID).IsEqualTo(open.SessionID);
        Check.That(opened.Data.State.ItemID).IsEqualTo(open.ItemID);
        Check.That(opened.Data.State.Mode).IsEqualTo(DataItemTransferMode.StreamUpload);
        Check.That(opened.Data.State.StateVersion).IsEqualTo(0);
        Check.That(opened.Data.State.Status).IsEqualTo(DataItemTransferStatus.Open);

        Byte[] payload = CreateSegmentedTransferPayload(16,0x72);
        PutStreamTransferSegmentRequest putRequest = CreatePutStreamTransferSegmentRequest(open,0,payload);

        using MemoryStream upload = new(payload,false);
        PutStreamTransferSegmentClientResponse? stored = await _dc.PutStreamTransferSegment(putRequest,upload,this.WriteToken);

        Check.That(stored).IsNotNull();
        Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(stored.Response).IsNotNull();
        Check.That(stored.Response!.Accepted).IsTrue();
        Check.That(stored.Response.StateVersion).IsEqualTo(1);
        Check.That(stored.Response.AppendedLength).IsEqualTo(payload.Length);

        RestResponse<GetStreamTransferStateResponse> state = await _dc.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.StateVersion).IsEqualTo(1);
        Check.That(state.Data.State.AppendedLength).IsEqualTo(payload.Length);

        StreamSegmentDownloadInfo? downloaded = await _dc.GetStreamTransferSegment(new GetStreamTransferSegmentRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , Offset = 0 , Length = payload.Length },this.WriteToken);

        Check.That(downloaded).IsNotNull();
        Check.That((Int32)downloaded!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(downloaded.Payload.SequenceEqual(payload)).IsTrue();
        Check.That(downloaded.Footer.SessionID).IsEqualTo(open.SessionID);
        Check.That(downloaded.Footer.ItemID).IsEqualTo(open.ItemID);
        Check.That(downloaded.Footer.RequestedOffset).IsEqualTo(0);
        Check.That(downloaded.Footer.ReturnedLength).IsEqualTo(payload.Length);
        Check.That(downloaded.Footer.AppendedLength).IsEqualTo(payload.Length);
        Check.That(downloaded.Footer.StateVersion).IsEqualTo(1);
        Check.That(downloaded.Footer.SegmentSHA512.SequenceEqual(SHA512.HashData(payload))).IsTrue();

        RestResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(aborted.Data).IsNotNull();
        Check.That(aborted.Data!.Aborted).IsTrue();
    }

    [Test] [Order(26)]
    public async Task StreamTransferFollowAndReOpenSynchronizesFollowerState()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        OpenStreamTransferRequest open = CreateOpenStreamTransferRequest();
        RestResponse<OpenStreamTransferResponse> opened = await _dc.OpenStreamTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] firstPayload = CreateSegmentedTransferPayload(8,0x73);
        using MemoryStream firstUpload = new(firstPayload,false);
        PutStreamTransferSegmentClientResponse? first = await _dc.PutStreamTransferSegment(CreatePutStreamTransferSegmentRequest(open,0,firstPayload),firstUpload,this.WriteToken);

        Check.That(first).IsNotNull();
        Check.That((Int32)first!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(first.Response).IsNotNull();

        OpenFollowStreamTransferRequest follow = new()
        {
            SessionID = Guid.NewGuid(),
            ItemID = open.ItemID,
            SourceSessionID = open.SessionID,
        };

        RestResponse<OpenFollowStreamTransferResponse> followed = await _dc.OpenFollowStreamTransfer(follow,this.ReadToken);

        Check.That((Int32)followed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(followed.Data).IsNotNull();
        Check.That(followed.Data!.ObjectPayload.SequenceEqual(open.ObjectPayload)).IsTrue();
        Check.That(followed.Data!.State.Mode).IsEqualTo(DataItemTransferMode.StreamFollow);
        Check.That(followed.Data.State.SourceSessionID).IsEqualTo(open.SessionID);
        Check.That(followed.Data.State.AppendedLength).IsEqualTo(firstPayload.Length);

        Byte[] secondPayload = CreateSegmentedTransferPayload(8,0x74);
        using MemoryStream secondUpload = new(secondPayload,false);
        PutStreamTransferSegmentClientResponse? second = await _dc.PutStreamTransferSegment(CreatePutStreamTransferSegmentRequest(open,firstPayload.Length,secondPayload,first.Response!.StateVersion),secondUpload,this.WriteToken);

        Check.That(second).IsNotNull();
        Check.That((Int32)second!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(second.Response).IsNotNull();

        RestResponse<ReOpenFollowStreamTransferResponse> reopened = await _dc.ReOpenFollowStreamTransfer(new ReOpenFollowStreamTransferRequest()
        {
            SessionID = follow.SessionID,
            ItemID = follow.ItemID,
            SourceSessionID = follow.SourceSessionID,
        },this.ReadToken);

        Check.That((Int32)reopened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(reopened.Data).IsNotNull();
        Check.That(reopened.Data!.ObjectPayload.SequenceEqual(open.ObjectPayload)).IsTrue();
        Check.That(reopened.Data!.State.SessionID).IsEqualTo(follow.SessionID);
        Check.That(reopened.Data.State.SourceSessionID).IsEqualTo(open.SessionID);
        Check.That(reopened.Data.State.AppendedLength).IsEqualTo(firstPayload.Length + secondPayload.Length);

        StreamSegmentDownloadInfo? downloaded = await _dc.GetStreamTransferSegment(new GetStreamTransferSegmentRequest(){ SessionID = follow.SessionID , ItemID = follow.ItemID , Offset = 0 , Length = firstPayload.Length + secondPayload.Length },this.ReadToken);

        Check.That(downloaded).IsNotNull();
        Check.That((Int32)downloaded!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(downloaded.Payload.SequenceEqual(firstPayload.Concat(secondPayload).ToArray())).IsTrue();

        RestResponse<AbortTransferResponse> abortedFollower = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = follow.SessionID , ItemID = follow.ItemID },this.ReadToken);
        Check.That((Int32)abortedFollower.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        RestResponse<AbortTransferResponse> abortedSource = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)abortedSource.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test] [Order(27)]
    public async Task StreamTransferCompleteRetainsSessionAndRejectsReOpenInCommittedState()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        StreamTransferCommitScenario scenario = await CreateStreamTransferCommitScenario(1024 * 1024,0x75);
        OpenStreamTransferRequest open = scenario.Open;
        RestResponse<OpenStreamTransferResponse> opened = await _dc.OpenStreamTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(opened.Data).IsNotNull();

        using MemoryStream upload = new(scenario.StreamPayload,false);
        PutStreamTransferSegmentClientResponse? stored = await _dc.PutStreamTransferSegment(CreatePutStreamTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);

        Check.That(stored).IsNotNull();
        Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(stored.Response).IsNotNull();
        Check.That(stored.Response!.Accepted).IsTrue();

        RestResponse<CompleteStreamTransferResponse> committed = await _dc.CompleteStreamTransfer(new CompleteStreamTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = stored.Response.StateVersion,
            FinalObjectInfo = open.ObjectInfo,
            FinalObjectPayload = open.ObjectPayload,
            FinalObjectSHA512 = open.ObjectSHA512,
            FinalStreamLength = scenario.StreamPayload.LongLength,
            FinalStreamSHA512 = SHA512.HashData(scenario.StreamPayload),
        },this.WriteToken);

        Check.That((Int32)committed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(committed.Data).IsNotNull();
        Check.That(committed.Data!.Published).IsTrue();
        Check.That(committed.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(committed.Data.State.AppendedLength).IsEqualTo(scenario.StreamPayload.LongLength);

        RestResponse<GetStreamTransferStateResponse> state = await _dc.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(state.Data.State.AppendedLength).IsEqualTo(scenario.StreamPayload.LongLength);

        String? downloadedStreamPath = null;

        try
        {
            DataControlDownloadCompletionResult? downloaded = await _dc.Download(open.ItemID,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsNotNull();
            Check.That(Guid.Equals(open.ItemID,downloaded.Item!.GetID())).IsTrue();
            Check.That(downloaded.Item).IsInstanceOf<DataStreamItem>();
            Check.That(File.Exists(downloaded.StreamPath)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(downloaded.StreamPath)).SequenceEqual(scenario.StreamPayload)).IsTrue();

            downloadedStreamPath = downloaded.StreamPath;

            DataItem? streamedItem = downloaded.Item;
            Check.That(streamedItem).IsNotNull();
            Check.That(Guid.Equals(open.ItemID,streamedItem!.GetID())).IsTrue();
            Check.That(streamedItem).IsInstanceOf<DataStreamItem>();
        }
        finally
        {
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }

        RestResponse<ReOpenStreamTransferResponse> reopened = await _dc.ReOpenStreamTransfer(new ReOpenStreamTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ObjectSHA512 = open.ObjectSHA512,
            StreamSHA512 = SHA512.HashData(scenario.StreamPayload),
        },this.WriteToken);

        Check.That((Int32)reopened.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        RestResponse<RemoveTransferResponse> removed = await _dc.RemoveTransfer(new RemoveTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)removed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(removed.Data).IsNotNull();
        Check.That(removed.Data!.Removed).IsTrue();

        RestResponse<GetStreamTransferStateResponse> removedState = await _dc.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)removedState.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);

        RestResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test] [Order(28)]
    public async Task ClientStreamUploadAndDownloadRoundTripCommittedContent()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.ClientStreamRoundTrip.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(1024 * 1024,0x76);
        String? uploadedWorkingDirectory = null;
        String? downloadedStreamPath = null;

        try
        {
            await using FileStream source = new(filePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await source.WriteAsync(payload);
            source.Position = 0;

            DataStreamItem item = new(source);

            DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken);

            Check.That(uploaded).IsNotNull();
            Check.That(uploaded!.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(uploaded.Stream).IsNotNull();
            Check.That(uploaded.Stream!.Published).IsTrue();
            Check.That(uploaded.Session.StreamState).IsNotNull();
            Check.That(uploaded.Session.StreamState!.Status).IsEqualTo(DataItemTransferStatus.Committed);

            uploadedWorkingDirectory = uploaded.Session.WorkingDirectory;

            DataControlDownloadCompletionResult? downloaded = await _dc.Download(item.GetID()!.Value,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsNotNull();
            Check.That(Guid.Equals(item.GetID(),downloaded.Item!.GetID())).IsTrue();
            Check.That(downloaded.Item).IsInstanceOf<DataStreamItem>();
            Check.That(File.Exists(downloaded.StreamPath)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(downloaded.StreamPath)).SequenceEqual(payload)).IsTrue();

            downloadedStreamPath = downloaded.StreamPath;

            RestResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
            DeleteWorkingDirectory(uploadedWorkingDirectory);
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }
    }

    [Test] [Order(29)]
    public async Task StreamTransferCompleteRetainedSessionBlocksNewWriterOpen()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        StreamTransferCommitScenario initialScenario = await CreateStreamTransferCommitScenario(256 * 1024,0x81);
        OpenStreamTransferRequest initialOpen = initialScenario.Open;

        RestResponse<OpenStreamTransferResponse> initialOpened = await _dc.OpenStreamTransfer(initialOpen,this.WriteToken);
        Check.That((Int32)initialOpened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        using MemoryStream initialUpload = new(initialScenario.StreamPayload,false);
        PutStreamTransferSegmentClientResponse? initialStored = await _dc.PutStreamTransferSegment(CreatePutStreamTransferSegmentRequest(initialOpen,0,initialScenario.StreamPayload),initialUpload,this.WriteToken);

        Check.That(initialStored).IsNotNull();
        Check.That((Int32)initialStored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(initialStored.Response).IsNotNull();

        RestResponse<CompleteStreamTransferResponse> initialCompleted = await _dc.CompleteStreamTransfer(new CompleteStreamTransferRequest()
        {
            SessionID = initialOpen.SessionID,
            ItemID = initialOpen.ItemID,
            ExpectedStateVersion = initialStored.Response!.StateVersion,
            FinalObjectInfo = initialOpen.ObjectInfo,
            FinalObjectPayload = initialOpen.ObjectPayload,
            FinalObjectSHA512 = initialOpen.ObjectSHA512,
            FinalStreamLength = initialScenario.StreamPayload.LongLength,
            FinalStreamSHA512 = SHA512.HashData(initialScenario.StreamPayload),
        },this.WriteToken);

        Check.That((Int32)initialCompleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(initialCompleted.Data).IsNotNull();
        Check.That(initialCompleted.Data!.Published).IsTrue();

        OpenStreamTransferRequest conflictingOpen = new()
        {
            SessionID = Guid.NewGuid(),
            ItemID = initialOpen.ItemID,
            ObjectPayload = initialOpen.ObjectPayload,
            ObjectSHA512 = initialOpen.ObjectSHA512,
            RequestedSegmentSizePolicy = initialOpen.RequestedSegmentSizePolicy,
            ObjectInfo = initialOpen.ObjectInfo,
        };

        RestResponse<OpenStreamTransferResponse> conflictingOpened = await _dc.OpenStreamTransfer(conflictingOpen,this.WriteToken);
        Check.That((Int32)conflictingOpened.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        RestResponse<RemoveTransferResponse> removedInitial = await _dc.RemoveTransfer(new RemoveTransferRequest(){ SessionID = initialOpen.SessionID , ItemID = initialOpen.ItemID },this.WriteToken);
        Check.That((Int32)removedInitial.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        RestResponse<OpenStreamTransferResponse> reopenedAfterCleanup = await _dc.OpenStreamTransfer(conflictingOpen,this.WriteToken);
        Check.That((Int32)reopenedAfterCleanup.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        RestResponse<AbortTransferResponse> abortedRetry = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = conflictingOpen.SessionID , ItemID = conflictingOpen.ItemID },this.WriteToken);
        Check.That((Int32)abortedRetry.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(abortedRetry.Data).IsNotNull();
        Check.That(abortedRetry.Data!.Aborted).IsTrue();

        RestResponse<Guid> deleted = await _dc.Delete(conflictingOpen.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    private static OpenStreamTransferRequest CreateOpenStreamTransferRequest(Byte objectSeed = 0x71,Guid? itemId = null)
    {
        Byte[] objectPayload = CreateSegmentedTransferPayload(32,objectSeed);

        return new()
        {
            SessionID = Guid.NewGuid(),
            ItemID = itemId ?? Guid.NewGuid(),
            ObjectPayload = objectPayload,
            ObjectSHA512 = SHA512.HashData(objectPayload),
            RequestedSegmentSizePolicy = new DataItemTransferSegmentSizePolicy() { MinSegmentBytes = 1,PreferredSegmentBytes = 16,MaxSegmentBytes = 1024 * 1024 },
            ObjectInfo = new DataItemTransferObjectInfo() { ObjectType = typeof(DataStreamItem).FullName,ContentStreamed = true },
        };
    }

    private static PutStreamTransferSegmentRequest CreatePutStreamTransferSegmentRequest(OpenStreamTransferRequest request,Int64 offset,Byte[] payload,Int64 expectedStateVersion = 0)
    {
        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            Offset = offset,
            Length = payload.LongLength,
            ExpectedStateVersion = expectedStateVersion,
            SegmentSHA512 = SHA512.HashData(payload),
            StreamSHA512 = offset == 0 ? SHA512.HashData(payload) : Array.Empty<Byte>(),
            RequestID = Guid.NewGuid(),
        };
    }

    private static async Task<StreamTransferCommitScenario> CreateStreamTransferCommitScenario(Int32 streamLength,Byte streamSeed)
    {
        Byte[] streamPayload = CreateSegmentedTransferPayload(streamLength,streamSeed);

        await using MemoryStream stream = new(streamPayload,false);

        DataStreamItem item = new(stream);
        Guid itemId = item.GetID()!.Value;
        Byte[] objectPayload = item.Serialize();

        return new()
        {
            Open = new()
            {
                SessionID = Guid.NewGuid(),
                ItemID = itemId,
                ObjectPayload = objectPayload,
                ObjectSHA512 = SHA512.HashData(objectPayload),
                RequestedSegmentSizePolicy = new DataItemTransferSegmentSizePolicy() { MinSegmentBytes = 1,PreferredSegmentBytes = Math.Min(16,streamLength),MaxSegmentBytes = streamLength },
                ObjectInfo = new DataItemTransferObjectInfo() { ObjectType = typeof(DataStreamItem).FullName,ContentStreamed = true },
            },
            StreamPayload = streamPayload,
        };
    }

    private sealed record StreamTransferCommitScenario
    {
        public OpenStreamTransferRequest Open { get; init; } = new();

        public Byte[] StreamPayload { get; init; } = Array.Empty<Byte>();
    }
}
