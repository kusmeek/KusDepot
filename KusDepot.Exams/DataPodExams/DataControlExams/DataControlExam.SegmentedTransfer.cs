namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test] [Order(7)]
    public async Task SegmentedTransferRoundTrip()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);

        RestResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        opened = await _dc.OpenUploadTransfer(open,this.ReadToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(opened.Data).IsNotNull();
        Check.That(opened.Data!.State.SessionID).IsEqualTo(open.SessionID);
        Check.That(opened.Data.State.ItemID).IsEqualTo(open.ItemID);
        Check.That(opened.Data.State.StateVersion).IsEqualTo(0);
        Check.That(opened.Data.State.Status).IsEqualTo(DataItemTransferStatus.Open);

        Byte[] payload = CreateSegmentedTransferPayload(16,0x44);
        PutTransferSegmentRequest putRequest = CreatePutTransferSegmentRequest(open,8,payload);

        using MemoryStream upload = new(payload,false);
        PutTransferSegmentClientResponse? stored = await _dc.PutTransferSegment(putRequest,upload,this.WriteToken);

        Check.That(stored).IsNotNull();
        Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(stored.Response).IsNotNull();
        Check.That(stored.Response!.Accepted).IsTrue();
        Check.That(stored.Response.StateVersion).IsEqualTo(1);
        Check.That(stored.Response.Range.Offset).IsEqualTo(8);
        Check.That(stored.Response.Range.Length).IsEqualTo(payload.Length);

        RestResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.StateVersion).IsEqualTo(1);
        Check.That(state.Data.State.RealizedBytes).IsEqualTo(payload.Length);
        Check.That(state.Data.State.RealizedRanges.Length).IsEqualTo(1);
        Check.That(state.Data.State.RealizedRanges[0].Offset).IsEqualTo(8);
        Check.That(state.Data.State.RealizedRanges[0].Length).IsEqualTo(payload.Length);

        SegmentDownloadInfo? downloaded = await _dc.GetTransferSegment(new GetTransferSegmentRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , Offset = 8 , Length = payload.Length },this.WriteToken);

        Check.That(downloaded).IsNotNull();
        Check.That((Int32)downloaded!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(downloaded.Payload.SequenceEqual(payload)).IsTrue();
        Check.That(downloaded.Footer.SessionID).IsEqualTo(open.SessionID);
        Check.That(downloaded.Footer.ItemID).IsEqualTo(open.ItemID);
        Check.That(downloaded.Footer.RequestedOffset).IsEqualTo(8);
        Check.That(downloaded.Footer.ReturnedLength).IsEqualTo(payload.Length);
        Check.That(downloaded.Footer.StateVersion).IsEqualTo(1);
        Check.That(downloaded.Footer.SegmentSHA512.SequenceEqual(SHA512.HashData(payload))).IsTrue();

        RestResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(aborted.Data).IsNotNull();
        Check.That(aborted.Data!.Aborted).IsTrue();
    }

    [Test] [Order(8)]
    public async Task SegmentedTransferRejectsOverlap()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);
        RestResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] firstPayload = CreateSegmentedTransferPayload(16,0x52);
        PutTransferSegmentRequest firstRequest = CreatePutTransferSegmentRequest(open,8,firstPayload);

        using MemoryStream firstUpload = new(firstPayload,false);
        PutTransferSegmentClientResponse? first = await _dc.PutTransferSegment(firstRequest,firstUpload,this.WriteToken);

        Check.That(first).IsNotNull();
        Check.That((Int32)first!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(first.Response).IsNotNull();
        Check.That(first.Response!.Accepted).IsTrue();

        Byte[] overlapPayload = firstPayload.AsSpan(4,8).ToArray();
        PutTransferSegmentRequest overlapRequest = CreatePutTransferSegmentRequest(open,12,overlapPayload,first.Response.StateVersion);

        using MemoryStream overlapUpload = new(overlapPayload,false);
        PutTransferSegmentClientResponse? overlap = await _dc.PutTransferSegment(overlapRequest,overlapUpload,this.WriteToken);

        Check.That(overlap).IsNotNull();
        Check.That((Int32)overlap!.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        RestResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test] [Order(9)]
    public async Task SegmentedTransferRejectsStaleExpectedStateVersion()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);
        RestResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] firstPayload = CreateSegmentedTransferPayload(16,0x63);
        PutTransferSegmentRequest firstRequest = CreatePutTransferSegmentRequest(open,0,firstPayload);

        using MemoryStream firstUpload = new(firstPayload,false);
        PutTransferSegmentClientResponse? first = await _dc.PutTransferSegment(firstRequest,firstUpload,this.WriteToken);

        Check.That(first).IsNotNull();
        Check.That((Int32)first!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(first.Response).IsNotNull();
        Check.That(first.Response!.Accepted).IsTrue();

        Byte[] stalePayload = CreateSegmentedTransferPayload(8,0x74);
        PutTransferSegmentRequest staleRequest = CreatePutTransferSegmentRequest(open,24,stalePayload,expectedStateVersion:0);

        using MemoryStream staleUpload = new(stalePayload,false);
        PutTransferSegmentClientResponse? stale = await _dc.PutTransferSegment(staleRequest,staleUpload,this.WriteToken);

        Check.That(stale).IsNotNull();
        Check.That((Int32)stale!.StatusCode).IsEqualTo(StatusCodes.Status412PreconditionFailed);

        RestResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test] [Order(10)]
    public async Task SegmentedTransferCommitPublishesAndRetainsSessionUntilRemove()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        SegmentedTransferCommitScenario scenario = await CreateCommitScenario(streamLength:5*1024*1024,streamSeed:0x40);
        OpenUploadTransferRequest open = scenario.Open;
        RestResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(opened.Data).IsNotNull();

        Byte[] fullPayload = scenario.StreamPayload;
        PutTransferSegmentRequest storeRequest = CreatePutTransferSegmentRequest(open,0,fullPayload);

        using MemoryStream upload = new(fullPayload,false);
        PutTransferSegmentClientResponse? stored = await _dc.PutTransferSegment(storeRequest,upload,this.WriteToken);

        Check.That(stored).IsNotNull();
        Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(stored.Response).IsNotNull();
        Check.That(stored.Response!.Accepted).IsTrue();
        Check.That(stored.Response.CompletedCoverage).IsTrue();

        RestResponse<CommitUploadTransferResponse> committed = await _dc.CommitUploadTransfer(new CommitUploadTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored.Response.StateVersion },this.WriteToken);

        Check.That((Int32)committed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(committed.Data).IsNotNull();
        Check.That(committed.Data!.Published).IsTrue();
        Check.That(committed.Data.State.Status).IsEqualTo(DataItemTransferStatus.Committed);

        RestResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(state.Data.State.StateVersion).IsEqualTo(committed.Data.State.StateVersion);
        Check.That(state.Data.State.RealizedBytes).IsEqualTo(fullPayload.LongLength);

        String? downloadedStreamPath = null;

        try
        {
            DataControlDownloadCompletionResult? downloaded = await _dc.Download(open.ItemID,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsNotNull();
            Check.That(Guid.Equals(open.ItemID,downloaded.Item!.GetID())).IsTrue();
            Check.That(File.Exists(downloaded.StreamPath)).IsTrue();

            Byte[] streamBytes = await File.ReadAllBytesAsync(downloaded.StreamPath);
            Check.That(streamBytes.SequenceEqual(fullPayload)).IsTrue();

            downloadedStreamPath = downloaded.StreamPath;

            DataItem? streamedItem = downloaded.Item;
            Check.That(streamedItem).IsNotNull();
            Check.That(Guid.Equals(open.ItemID,streamedItem!.GetID())).IsTrue();

            Descriptor? streamedDescriptor = streamedItem.GetDescriptor();
            Check.That(streamedDescriptor).IsNotNull();
            Check.That(streamedDescriptor!.ContentStreamed).IsEqualTo(true);
        }
        finally
        {
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }

        RestResponse<RemoveTransferResponse> removed = await _dc.RemoveTransfer(new RemoveTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)removed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(removed.Data).IsNotNull();
        Check.That(removed.Data!.Removed).IsTrue();

        RestResponse<GetTransferStateResponse> removedState = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)removedState.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);

        RestResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test] [Order(11)]
    public async Task SegmentedTransferReOpenReturnsCurrentPartialState()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:96);
        RestResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] payload = CreateSegmentedTransferPayload(24,0x55);
        PutTransferSegmentRequest putRequest = CreatePutTransferSegmentRequest(open,16,payload);

        using MemoryStream upload = new(payload,false);
        PutTransferSegmentClientResponse? stored = await _dc.PutTransferSegment(putRequest,upload,this.WriteToken);

        Check.That(stored).IsNotNull();
        Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(stored.Response).IsNotNull();
        Check.That(stored.Response!.Accepted).IsTrue();

        ReOpenUploadTransferRequest reopenRequest = new()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ObjectSHA512 = open.ObjectSHA512,
            StreamSHA512 = open.StreamSHA512,
            StreamLength = open.StreamLength,
        };

        RestResponse<ReOpenUploadTransferResponse> reopened = await _dc.ReOpenUploadTransfer(reopenRequest,this.WriteToken);

        Check.That((Int32)reopened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(reopened.Data).IsNotNull();
        Check.That(reopened.Data!.State.SessionID).IsEqualTo(open.SessionID);
        Check.That(reopened.Data.State.ItemID).IsEqualTo(open.ItemID);
        Check.That(reopened.Data.State.StateVersion).IsEqualTo(stored.Response.StateVersion);
        Check.That(reopened.Data.State.Status).IsEqualTo(DataItemTransferStatus.Open);
        Check.That(reopened.Data.State.RealizedBytes).IsEqualTo(payload.Length);
        Check.That(reopened.Data.State.RealizedRanges.Length).IsEqualTo(1);
        Check.That(reopened.Data.State.RealizedRanges[0].Offset).IsEqualTo(16);
        Check.That(reopened.Data.State.RealizedRanges[0].Length).IsEqualTo(payload.Length);

        RestResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test] [Order(12)]
    public async Task SegmentedTransferAbortRemovesStateAndRejectsReOpen()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);
        RestResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        RestResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(aborted.Data).IsNotNull();
        Check.That(aborted.Data!.Aborted).IsTrue();

        RestResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);

        RestResponse<ReOpenUploadTransferResponse> reopened = await _dc.ReOpenUploadTransfer(new ReOpenUploadTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ObjectSHA512 = open.ObjectSHA512,
            StreamSHA512 = open.StreamSHA512,
            StreamLength = open.StreamLength,
        },this.WriteToken);

        Check.That((Int32)reopened.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);
    }

    [Test] [Order(13)]
    public async Task SegmentedTransferCommitRetainsSessionAndRejectsReOpenInCommittedState()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        SegmentedTransferCommitScenario scenario = await CreateCommitScenario(streamLength:1024*1024,streamSeed:0x48);
        OpenUploadTransferRequest open = scenario.Open;
        RestResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] fullPayload = scenario.StreamPayload;
        PutTransferSegmentRequest storeRequest = CreatePutTransferSegmentRequest(open,0,fullPayload);

        using MemoryStream upload = new(fullPayload,false);
        PutTransferSegmentClientResponse? stored = await _dc.PutTransferSegment(storeRequest,upload,this.WriteToken);

        Check.That(stored).IsNotNull();
        Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(stored.Response).IsNotNull();

        RestResponse<CommitUploadTransferResponse> committed = await _dc.CommitUploadTransfer(new CommitUploadTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored.Response!.StateVersion },this.WriteToken);

        Check.That((Int32)committed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(committed.Data).IsNotNull();
        Check.That(committed.Data!.Published).IsTrue();

        RestResponse<ReOpenUploadTransferResponse> reopened = await _dc.ReOpenUploadTransfer(new ReOpenUploadTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ObjectSHA512 = open.ObjectSHA512,
            StreamSHA512 = open.StreamSHA512,
            StreamLength = open.StreamLength,
        },this.WriteToken);

        Check.That((Int32)reopened.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        RestResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Committed);

        RestResponse<RemoveTransferResponse> removed = await _dc.RemoveTransfer(new RemoveTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)removed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(removed.Data).IsNotNull();
        Check.That(removed.Data!.Removed).IsTrue();

        RestResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test] [Order(14)]
    public async Task ClientUploadCanCommitThroughOpenUpload()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.ClientUpload.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(1024 * 1024,0x57);

        try
        {
            await File.WriteAllBytesAsync(filePath,payload);

            BinaryItem item = new();
            item.SetFILE(filePath);
            Check.That(await item.SetContentStreamed(true)).IsTrue();

            DataControlUploadSession? session = await _dc.OpenUpload(item,this.WriteToken);

            Check.That(session).IsNotNull();
            Check.That(session!.Session.State.Mode).IsEqualTo(DataItemTransferMode.Upload);

            Int32 uploaded = await session.UploadToCompletion();
            Check.That(uploaded > 0).IsTrue();

            DataControlUploadCompletionResult? committed = await session.Commit();

            Check.That(committed).IsNotNull();
            Check.That(committed!.Segmented!.Published).IsTrue();
            Check.That(committed.Segmented.State.Status).IsEqualTo(DataItemTransferStatus.Committed);

            RestResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
        }
    }

    [Test] [Order(15)]
    public async Task ClientDownloadCanMaterializeCommittedItem()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        SegmentedTransferCommitScenario scenario = await CreateCommitScenario(streamLength:1024 * 1024,streamSeed:0x66);
        OpenUploadTransferRequest open = scenario.Open;

        RestResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);
        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        using MemoryStream upload = new(scenario.StreamPayload,false);
        PutTransferSegmentClientResponse? stored = await _dc.PutTransferSegment(CreatePutTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);

        Check.That(stored).IsNotNull();
        Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(stored.Response).IsNotNull();

        RestResponse<CommitUploadTransferResponse> committed = await _dc.CommitUploadTransfer(new CommitUploadTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored.Response!.StateVersion },this.WriteToken);

        Check.That((Int32)committed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(committed.Data).IsNotNull();
        Check.That(committed.Data!.Published).IsTrue();

        DataControlDownloadSession? session = await _dc.OpenDownload(open.ItemID,this.ReadToken);

        Check.That(session).IsNotNull();
        Check.That(session!.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadCommitted);

        Int32 downloaded = await session.DownloadToCompletion();
        Check.That(downloaded > 0).IsTrue();

        DataControlDownloadCompletionResult? materialized = await session.Materialize();

        Check.That(materialized).IsNotNull();
        Check.That(materialized!.Item).IsNotNull();
        Check.That(Guid.Equals(open.ItemID,materialized.Item!.GetID())).IsTrue();
        Check.That(File.Exists(materialized.StreamPath)).IsTrue();
        Check.That((await File.ReadAllBytesAsync(materialized.StreamPath)).SequenceEqual(scenario.StreamPayload)).IsTrue();

        RestResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test] [Order(16)]
    public async Task ClientUploadAndDownloadHelpersRoundTripCommittedContent()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.ClientRoundTrip.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(1024 * 1024,0x6A);
        String? downloadedStreamPath = null;

        try
        {
            BinaryItem item = await CreateClientBinaryItem(filePath,payload);

            DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken);

            Check.That(uploaded).IsNotNull();
            Check.That(uploaded!.Segmented!.Published).IsTrue();
            Check.That(uploaded.Session.State.Status).IsEqualTo(DataItemTransferStatus.Committed);

            DataControlDownloadCompletionResult? downloaded = await _dc.Download(item.GetID()!.Value,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsNotNull();
            Check.That(Guid.Equals(item.GetID(),downloaded.Item!.GetID())).IsTrue();
            Check.That(File.Exists(downloaded.StreamPath)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(downloaded.StreamPath)).SequenceEqual(payload)).IsTrue();

            downloadedStreamPath = downloaded.StreamPath;

            RestResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }
    }

    [Test] [Order(17)]
    public async Task ClientUploadAndDownloadHelpersRoundTripSerializedOnlyFiniteContent()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        Byte[] payload = CreateSegmentedTransferPayload(1024,0x6C);

        BinaryItem item = new(payload);

        DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken);

        Check.That(uploaded).IsNotNull();
        Check.That(uploaded!.Segmented!.Published).IsTrue();
        Check.That(uploaded.Session.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
        Check.That(uploaded.Session.State.StreamLength).IsEqualTo(0);

        DataControlDownloadCompletionResult? downloaded = await _dc.Download(item.GetID()!.Value,this.ReadToken);

        Check.That(downloaded).IsNotNull();
        Check.That(downloaded!.Item).IsNotNull();
        Check.That(Guid.Equals(item.GetID(),downloaded.Item!.GetID())).IsTrue();
        Check.That(downloaded.Item.GetContentStreamed()).IsFalse();
        Check.That(File.Exists(downloaded.StreamPath)).IsTrue();
        Check.That(new FileInfo(downloaded.StreamPath).Length).IsEqualTo(0);
        Check.That(downloaded.Item is BinaryItem).IsTrue();
        Check.That(((BinaryItem)downloaded.Item).GetContent()!.SequenceEqual(payload)).IsTrue();

        DeleteWorkingDirectoryForPath(downloaded.StreamPath);

        RestResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test] [Order(18)]
    public async Task ClientDownloadMaterializeRequiresCompletedSession()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        SegmentedTransferCommitScenario scenario = await CreateCommitScenario(streamLength:1024 * 1024,streamSeed:0x6B);
        OpenUploadTransferRequest open = scenario.Open;
        String? workingDirectory = null;

        try
        {
            RestResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);
            Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            using MemoryStream upload = new(scenario.StreamPayload,false);
            PutTransferSegmentClientResponse? stored = await _dc.PutTransferSegment(CreatePutTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);

            Check.That(stored).IsNotNull();
            Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(stored.Response).IsNotNull();

            RestResponse<CommitUploadTransferResponse> committed = await _dc.CommitUploadTransfer(new CommitUploadTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored.Response!.StateVersion },this.WriteToken);

            Check.That((Int32)committed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(committed.Data).IsNotNull();
            Check.That(committed.Data!.Published).IsTrue();

            DataControlDownloadSession? session = await _dc.OpenDownload(open.ItemID,this.ReadToken);

            Check.That(session).IsNotNull();
            workingDirectory = session!.Session.WorkingDirectory;
            Check.That(Directory.Exists(workingDirectory)).IsTrue();
            Check.That(await session.Materialize()).IsNull();

            Int32 downloaded = await session.DownloadToCompletion();
            Check.That(downloaded > 0).IsTrue();
            Check.That(await session.Materialize()).IsNotNull();

            RestResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            DeleteWorkingDirectory(workingDirectory);
        }
    }

    [Test] [Order(19)]
    public async Task ClientUploadAbortRemovesLocalWorkingDirectory()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.ClientAbort.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(256 * 1024,0x6C);

        try
        {
            BinaryItem item = await CreateClientBinaryItem(filePath,payload);

            DataControlUploadSession? session = await _dc.OpenUpload(item,this.WriteToken);

            Check.That(session).IsNotNull();
            Check.That(Directory.Exists(session!.Session.WorkingDirectory)).IsTrue();

            Boolean aborted = await session.Abort();

            Check.That(aborted).IsTrue();
            Check.That(Directory.Exists(session.Session.WorkingDirectory)).IsFalse();

            RestResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = session.Session.SessionId , ItemID = session.Session.ItemId },this.WriteToken);
            Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
        }
    }

    [Test] [Order(20)]
    public async Task ClientUploadDeleteLocalStateRemovesWorkingDirectoryWithoutAbortingRemoteSession()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.ClientDeleteUpload.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(256 * 1024,0x6D);

        try
        {
            BinaryItem item = await CreateClientBinaryItem(filePath,payload);

            DataControlUploadSession? session = await _dc.OpenUpload(item,this.WriteToken);

            Check.That(session).IsNotNull();
            Check.That(Directory.Exists(session!.Session.WorkingDirectory)).IsTrue();

            Boolean deletedLocal = await session.DeleteLocalState();

            Check.That(deletedLocal).IsTrue();
            Check.That(Directory.Exists(session.Session.WorkingDirectory)).IsFalse();

            RestResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = session.Session.SessionId , ItemID = session.Session.ItemId },this.WriteToken);
            Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(state.Data).IsNotNull();
            Check.That(state.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Open);

            Boolean aborted = await session.Abort();

            Check.That(aborted).IsTrue();

            RestResponse<GetTransferStateResponse> abortedState = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = session.Session.SessionId , ItemID = session.Session.ItemId },this.WriteToken);
            Check.That((Int32)abortedState.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
        }
    }

    [Test] [Order(21)]
    public async Task ClientDownloadDeleteLocalStateRemovesWorkingDirectoryWithoutDeletingCommittedItem()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        SegmentedTransferCommitScenario scenario = await CreateCommitScenario(streamLength:1024 * 1024,streamSeed:0x6E);
        OpenUploadTransferRequest open = scenario.Open;
        String? reopenedWorkingDirectory = null;

        try
        {
            RestResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);
            Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            using MemoryStream upload = new(scenario.StreamPayload,false);
            PutTransferSegmentClientResponse? stored = await _dc.PutTransferSegment(CreatePutTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);

            Check.That(stored).IsNotNull();
            Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(stored.Response).IsNotNull();

            RestResponse<CommitUploadTransferResponse> committed = await _dc.CommitUploadTransfer(new CommitUploadTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored.Response!.StateVersion },this.WriteToken);

            Check.That((Int32)committed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(committed.Data).IsNotNull();
            Check.That(committed.Data!.Published).IsTrue();

            DataControlDownloadSession? session = await _dc.OpenDownload(open.ItemID,this.ReadToken);

            Check.That(session).IsNotNull();
            Check.That(Directory.Exists(session!.Session.WorkingDirectory)).IsTrue();

            Boolean deletedLocal = await session.DeleteLocalState();

            Check.That(deletedLocal).IsTrue();
            Check.That(Directory.Exists(session.Session.WorkingDirectory)).IsFalse();

            DataControlDownloadSession? reopened = await _dc.OpenDownload(open.ItemID,this.ReadToken);

            Check.That(reopened).IsNotNull();
            reopenedWorkingDirectory = reopened!.Session.WorkingDirectory;
            Check.That(Directory.Exists(reopenedWorkingDirectory)).IsTrue();

            Int32 downloaded = await reopened.DownloadToCompletion();

            Check.That(downloaded > 0).IsTrue();
            Check.That(await reopened.Materialize()).IsNotNull();

            RestResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            DeleteWorkingDirectory(reopenedWorkingDirectory);
        }
    }

    [Test] [Order(22)]
    public async Task ClientUploadHelperReportsProgress()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.ClientUploadProgress.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(1024 * 1024,0x6F);
        List<DataItemTransferProgress> progress = new();
        DataItemTransferProgressReporter reporter = DataItemTransferProgressReporter.Create(progress.Add);
        String? workingDirectory = null;

        try
        {
            BinaryItem item = await CreateClientBinaryItem(filePath,payload);

            DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken,null,reporter);

            Check.That(uploaded).IsNotNull();
            Check.That(uploaded!.Segmented!.Published).IsTrue();
            Check.That(uploaded.Session.State.Status).IsEqualTo(DataItemTransferStatus.Committed);

            workingDirectory = uploaded.Session.WorkingDirectory;

            Check.That(reporter.Latest).IsNotNull();
            AssertTransferProgressSequence(progress,item.GetID()!.Value,uploaded.Session.SessionId,DataItemTransferMode.Upload,payload.LongLength);

            RestResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
            DeleteWorkingDirectory(workingDirectory);
        }
    }

    [Test] [Order(23)]
    public async Task ClientDownloadHelperReportsProgress()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.RichDownloadProgress.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(1024 * 1024,0x70);
        List<DataItemTransferProgress> progress = new();
        DataItemTransferProgressReporter reporter = DataItemTransferProgressReporter.Create(progress.Add);
        String? uploadedWorkingDirectory = null;
        String? downloadedStreamPath = null;

        try
        {
            BinaryItem item = await CreateClientBinaryItem(filePath,payload);

            DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken);

            Check.That(uploaded).IsNotNull();
            Check.That(uploaded!.Segmented!.Published).IsTrue();

            uploadedWorkingDirectory = uploaded.Session.WorkingDirectory;

            DataControlDownloadCompletionResult? downloaded = await _dc.Download(item.GetID()!.Value,this.ReadToken,null,reporter);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsNotNull();
            Check.That(Guid.Equals(item.GetID(),downloaded.Item!.GetID())).IsTrue();
            Check.That(File.Exists(downloaded.StreamPath)).IsTrue();
            Check.That(reporter.Latest).IsNotNull();
            Check.That((await File.ReadAllBytesAsync(downloaded.StreamPath)).SequenceEqual(payload)).IsTrue();

            downloadedStreamPath = downloaded.StreamPath;

            AssertTransferProgressSequence(progress,item.GetID()!.Value,downloaded.Session.SessionId,DataItemTransferMode.ReadCommitted,payload.LongLength);

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

    [Test] [Order(24)]
    public async Task SegmentedTransferCommitRetainedSessionBlocksNewWriterOpen()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        SegmentedTransferCommitScenario initialScenario = await CreateCommitScenario(streamLength:256 * 1024,streamSeed:0x91);
        OpenUploadTransferRequest initialOpen = initialScenario.Open;

        RestResponse<OpenUploadTransferResponse> initialOpened = await _dc.OpenUploadTransfer(initialOpen,this.WriteToken);
        Check.That((Int32)initialOpened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        using MemoryStream initialUpload = new(initialScenario.StreamPayload,false);
        PutTransferSegmentClientResponse? initialStored = await _dc.PutTransferSegment(CreatePutTransferSegmentRequest(initialOpen,0,initialScenario.StreamPayload),initialUpload,this.WriteToken);

        Check.That(initialStored).IsNotNull();
        Check.That((Int32)initialStored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(initialStored.Response).IsNotNull();

        RestResponse<CommitUploadTransferResponse> initialCommitted = await _dc.CommitUploadTransfer(new CommitUploadTransferRequest()
        {
            SessionID = initialOpen.SessionID,
            ItemID = initialOpen.ItemID,
            ExpectedStateVersion = initialStored.Response!.StateVersion,
        },this.WriteToken);

        Check.That((Int32)initialCommitted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(initialCommitted.Data).IsNotNull();
        Check.That(initialCommitted.Data!.Published).IsTrue();

        OpenUploadTransferRequest conflictingOpen = initialOpen with
        {
            SessionID = Guid.NewGuid(),
        };

        RestResponse<OpenUploadTransferResponse> conflictingOpened = await _dc.OpenUploadTransfer(conflictingOpen,this.WriteToken);
        Check.That((Int32)conflictingOpened.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        RestResponse<RemoveTransferResponse> removedInitial = await _dc.RemoveTransfer(new RemoveTransferRequest(){ SessionID = initialOpen.SessionID , ItemID = initialOpen.ItemID },this.WriteToken);
        Check.That((Int32)removedInitial.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        RestResponse<OpenUploadTransferResponse> reopenedAfterCleanup = await _dc.OpenUploadTransfer(conflictingOpen,this.WriteToken);
        Check.That((Int32)reopenedAfterCleanup.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        RestResponse<AbortTransferResponse> abortedRetry = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = conflictingOpen.SessionID , ItemID = conflictingOpen.ItemID },this.WriteToken);
        Check.That((Int32)abortedRetry.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        RestResponse<Guid> deletedFinal = await _dc.Delete(conflictingOpen.ItemID,this.WriteToken);
        Check.That((Int32)deletedFinal.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    private static Byte[] CreateSegmentedTransferPayload(Int32 length,Byte seed)
    {
        Byte[] payload = new Byte[length];

        for(Int32 i = 0;i < payload.Length;i++) { payload[i] = unchecked((Byte)(seed + i)); }

        return payload;
    }

    private static async Task<SegmentedTransferCommitScenario> CreateCommitScenario(Int32 streamLength,Byte streamSeed)
    {
        Byte[] streamPayload = CreateSegmentedTransferPayload(streamLength,streamSeed);
        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.SegmentedTransfer.{Guid.NewGuid():N}.bin");

        try
        {
            await File.WriteAllBytesAsync(filePath,streamPayload);

            BinaryItem item = new();
            item.SetFILE(filePath);
            Check.That(await item.SetContentStreamed(true)).IsTrue();

            Guid itemId = item.GetID()!.Value;
            Descriptor? descriptor = item.GetDescriptor();
            Byte[] objectPayload = item.Serialize();
            Byte[] objectSha512 = SHA512.HashData(objectPayload);
            Byte[] streamSha512;

            using(Stream? contentStream = item.GetContentStream())
            {
                streamSha512 = contentStream is null ? Array.Empty<Byte>() : SHA512.HashData(contentStream);
            }

            Check.That(descriptor).IsNotNull();
            Check.That(objectPayload.Length > 0).IsTrue();

            return new()
            {
                Open = new()
                {
                    SessionID = Guid.NewGuid(),
                    ItemID = itemId,
                    ObjectPayload = objectPayload,
                    ObjectSHA512 = objectSha512,
                    StreamSHA512 = streamSha512,
                    StreamLength = streamLength,
                    RequestedSegmentSizePolicy = new DataItemTransferSegmentSizePolicy() { MinSegmentBytes = 1,PreferredSegmentBytes = Math.Min(16,streamLength),MaxSegmentBytes = streamLength },
                    ObjectInfo = new DataItemTransferObjectInfo() { ObjectType = typeof(BinaryItem).FullName,ContentStreamed = true },
                },
                StreamPayload = streamPayload,
            };
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
        }
    }

    private static async Task<BinaryItem> CreateClientBinaryItem(String filePath , Byte[] payload)
    {
        await File.WriteAllBytesAsync(filePath,payload);

        BinaryItem item = new();
        item.SetFILE(filePath);
        Check.That(await item.SetContentStreamed(true)).IsTrue();
        return item;
    }

    private static void DeleteWorkingDirectory(String? workingDirectory)
    {
        if(String.IsNullOrWhiteSpace(workingDirectory) is false && Directory.Exists(workingDirectory)) { Directory.Delete(workingDirectory,true); }
    }

    private static void DeleteWorkingDirectoryForPath(String? path)
    {
        DeleteWorkingDirectory(String.IsNullOrWhiteSpace(path) ? null : Path.GetDirectoryName(path));
    }

    private static void AssertTransferProgressSequence(IReadOnlyList<DataItemTransferProgress> progress , Guid itemId , Guid sessionId , DataItemTransferMode mode , Int64 expectedRealizedBytes)
    {
        Check.That(progress.Count > 0).IsTrue();
        Check.That(progress.All(_ => _.ItemID == itemId)).IsTrue();
        Check.That(progress.All(_ => _.SessionID == sessionId)).IsTrue();
        Check.That(progress.All(_ => _.Mode == mode)).IsTrue();
        Check.That(progress.All(_ => _.Status is not DataItemTransferStatus.Aborted and not DataItemTransferStatus.Faulted)).IsTrue();
        Check.That(progress.All(_ => _.LatestRange is not null && _.LatestRange.Validate() && _.LatestRange.Length > 0)).IsTrue();
        Check.That(progress.All(_ => _.RealizedBytes >= 0 && _.RemainingBytes >= 0)).IsTrue();
        Check.That(progress.Zip(progress.Skip(1),(_0,_1) => _1.RealizedBytes >= _0.RealizedBytes && _1.RemainingBytes <= _0.RemainingBytes && _1.StateVersion >= _0.StateVersion).All(_ => _)).IsTrue();

        DataItemTransferProgress last = progress[^1];

        Check.That(last.RealizedBytes).IsEqualTo(expectedRealizedBytes);
        Check.That(last.RemainingBytes).IsEqualTo(0);
    }

    private static OpenUploadTransferRequest CreateOpenUploadRequest(Int64 streamLength = 64,Byte objectSeed = 0x21,Byte streamSeed = 0x31,Guid? itemId = null)
    {
        Byte[] objectPayload = CreateSegmentedTransferPayload(32,objectSeed);
        Byte[] streamPayload = CreateSegmentedTransferPayload(checked((Int32)streamLength),streamSeed);

        return new()
        {
            SessionID = Guid.NewGuid(),
            ItemID = itemId ?? Guid.NewGuid(),
            ObjectPayload = objectPayload,
            ObjectSHA512 = SHA512.HashData(objectPayload),
            StreamSHA512 = SHA512.HashData(streamPayload),
            StreamLength = streamLength,
            ObjectInfo = new DataItemTransferObjectInfo() { ObjectType = typeof(BinaryItem).FullName,ContentStreamed = true },
        };
    }

    private static PutTransferSegmentRequest CreatePutTransferSegmentRequest(OpenUploadTransferRequest request,Int64 offset,Byte[] payload,Int64 expectedStateVersion = 0)
    {
        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            Offset = offset,
            Length = payload.LongLength,
            ExpectedStateVersion = expectedStateVersion,
            SegmentSHA512 = SHA512.HashData(payload),
            StreamSHA512 = request.StreamSHA512,
            RequestID = Guid.NewGuid(),
        };
    }

    private sealed record SegmentedTransferCommitScenario
    {
        public OpenUploadTransferRequest Open { get; init; } = new();

        public Byte[] StreamPayload { get; init; } = Array.Empty<Byte>();
    }

}
