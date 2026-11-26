namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test]
    public async Task SegmentedTransferRoundTrip()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);

        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open);

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

        DataControlResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

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

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(aborted.Data).IsNotNull();
        Check.That(aborted.Data!.Aborted).IsTrue();
    }

    [Test]
    public async Task SegmentedTransferAbortAllowsCompleteUpload()
    {
        using DataControlClient client = new(this.EndpointLocator!);
        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:16);

        try
        {
            DataControlResponse<OpenUploadTransferResponse> opened = await client.OpenUploadTransfer(open,this.WriteToken);
            Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            Byte[] payload = CreateSegmentedTransferPayload(16,0x55);
            using MemoryStream upload = new(payload,false);
            PutTransferSegmentClientResponse? stored = await client.PutTransferSegment(CreatePutTransferSegmentRequest(open,0,payload),upload,this.WriteToken);

            Check.That(stored).IsNotNull();
            Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            DataControlResponse<GetTransferStateResponse> state = await client.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
            Check.That(state.Data).IsNotNull();
            Check.That(state.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Complete);

            DataControlResponse<AbortTransferResponse> aborted = await client.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
            Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(aborted.Data).IsNotNull();
            Check.That(aborted.Data!.Aborted).IsTrue();
        }
        finally
        {
            try { await client.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken); }
            catch { }

            try { await client.Delete(open.ItemID,this.WriteToken); }
            catch { }
        }
    }

    [Test]
    public async Task SegmentedTransferAbortRejectsPublishedUpload()
    {
        using DataControlClient client = new(this.EndpointLocator!);
        SegmentedTransferPublishScenario scenario = await CreatePublishScenario(streamLength:128 * 1024,streamSeed:0x56);
        OpenUploadTransferRequest open = scenario.Open;

        try
        {
            DataControlResponse<OpenUploadTransferResponse> opened = await client.OpenUploadTransfer(open,this.WriteToken);
            Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            using MemoryStream upload = new(scenario.StreamPayload,false);
            PutTransferSegmentClientResponse? stored = await client.PutTransferSegment(CreatePutTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);
            Check.That(stored).IsNotNull();

            DataControlResponse<PublishSegmentedTransferResponse> published = await client.PublishSegmentedTransfer(new PublishSegmentedTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored!.Response!.StateVersion },this.WriteToken);
            Check.That((Int32)published.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            DataControlResponse<AbortTransferResponse> aborted = await client.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
            Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

            DataControlResponse<GetTransferStateResponse> state = await client.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
            Check.That(state.Data).IsNotNull();
            Check.That(state.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Published);
        }
        finally
        {
            try { await client.RemoveTransfer(new RemoveTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken); }
            catch { }

            try { await client.Delete(open.ItemID,this.WriteToken); }
            catch { }
        }
    }

    [Test]
    public async Task SegmentedTransferRejectsOverlap()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);
        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

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

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task SegmentedTransferRejectsStaleExpectedStateVersion()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);
        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

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

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task SegmentedTransferPublishCanRetryAfterStaleExpectedStateVersion()
    {
        using DataControlClient client = new(this.EndpointLocator!);
        SegmentedTransferPublishScenario scenario = await CreatePublishScenario(streamLength:64 * 1024,streamSeed:0x71);
        OpenUploadTransferRequest open = scenario.Open;

        try
        {
            DataControlResponse<OpenUploadTransferResponse> opened = await client.OpenUploadTransfer(open,this.WriteToken);
            Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            DataControlResponse<PublishSegmentedTransferResponse> incomplete = await client.PublishSegmentedTransfer(new PublishSegmentedTransferRequest()
            {
                SessionID = open.SessionID,
                ItemID = open.ItemID,
                ExpectedStateVersion = opened.Data!.State.StateVersion,
            },this.WriteToken);
            Check.That((Int32)incomplete.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

            using MemoryStream upload = new(scenario.StreamPayload,false);
            PutTransferSegmentClientResponse? stored = await client.PutTransferSegment(CreatePutTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);
            Check.That(stored).IsNotNull();
            Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            DataControlResponse<PublishSegmentedTransferResponse> stale = await client.PublishSegmentedTransfer(new PublishSegmentedTransferRequest()
            {
                SessionID = open.SessionID,
                ItemID = open.ItemID,
                ExpectedStateVersion = stored.Response!.StateVersion - 1,
            },this.WriteToken);
            Check.That((Int32)stale.StatusCode).IsEqualTo(StatusCodes.Status412PreconditionFailed);

            DataControlResponse<PublishSegmentedTransferResponse> corrected = await client.PublishSegmentedTransfer(new PublishSegmentedTransferRequest()
            {
                SessionID = open.SessionID,
                ItemID = open.ItemID,
                ExpectedStateVersion = stored.Response.StateVersion,
            },this.WriteToken);
            Check.That((Int32)corrected.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(corrected.Data).IsNotNull();
            Check.That(corrected.Data!.Published).IsTrue();
        }
        finally
        {
            try { await client.RemoveTransfer(new RemoveTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken); }
            catch { }

            try { await client.Delete(open.ItemID,this.WriteToken); }
            catch { }
        }
    }

    [Test]
    public async Task SegmentedTransferPublishPublishesAndRetainsSessionUntilRemove()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        SegmentedTransferPublishScenario scenario = await CreatePublishScenario(streamLength:5*1024*1024,streamSeed:0x40);
        OpenUploadTransferRequest open = scenario.Open;
        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

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

        DataControlResponse<PublishSegmentedTransferResponse> published = await _dc.PublishSegmentedTransfer(new PublishSegmentedTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored.Response.StateVersion },this.WriteToken);

        Check.That((Int32)published.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(published.Data).IsNotNull();
        Check.That(published.Data!.Published).IsTrue();
        Check.That(published.Data.State.Status).IsEqualTo(DataItemTransferStatus.Published);

        DataControlResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Published);
        Check.That(state.Data.State.StateVersion).IsEqualTo(published.Data.State.StateVersion);
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

        DataControlResponse<RemoveTransferResponse> removed = await _dc.RemoveTransfer(new RemoveTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)removed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(removed.Data).IsNotNull();
        Check.That(removed.Data!.Removed).IsTrue();

        DataControlResponse<GetTransferStateResponse> removedState = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)removedState.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);

        DataControlResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task SegmentedTransferReOpenReturnsCurrentPartialState()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:96);
        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

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
            StreamLength = open.StreamLength,
        };

        DataControlResponse<ReOpenUploadTransferResponse> reopened = await _dc.ReOpenUploadTransfer(reopenRequest,this.WriteToken);

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

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task SegmentedTransferAbortRemovesStateAndRejectsReOpen()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);
        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(aborted.Data).IsNotNull();
        Check.That(aborted.Data!.Aborted).IsTrue();

        DataControlResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);

        DataControlResponse<ReOpenUploadTransferResponse> reopened = await _dc.ReOpenUploadTransfer(new ReOpenUploadTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ObjectSHA512 = open.ObjectSHA512,
            StreamLength = open.StreamLength,
        },this.WriteToken);

        Check.That((Int32)reopened.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task SegmentedTransferPublishRetainsSessionAndRejectsReOpenInPublishedState()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        SegmentedTransferPublishScenario scenario = await CreatePublishScenario(streamLength:1024*1024,streamSeed:0x48);
        OpenUploadTransferRequest open = scenario.Open;
        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] fullPayload = scenario.StreamPayload;
        PutTransferSegmentRequest storeRequest = CreatePutTransferSegmentRequest(open,0,fullPayload);

        using MemoryStream upload = new(fullPayload,false);
        PutTransferSegmentClientResponse? stored = await _dc.PutTransferSegment(storeRequest,upload,this.WriteToken);

        Check.That(stored).IsNotNull();
        Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(stored.Response).IsNotNull();

        DataControlResponse<PublishSegmentedTransferResponse> published = await _dc.PublishSegmentedTransfer(new PublishSegmentedTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored.Response!.StateVersion },this.WriteToken);

        Check.That((Int32)published.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(published.Data).IsNotNull();
        Check.That(published.Data!.Published).IsTrue();

        DataControlResponse<ReOpenUploadTransferResponse> reopened = await _dc.ReOpenUploadTransfer(new ReOpenUploadTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ObjectSHA512 = open.ObjectSHA512,
            StreamLength = open.StreamLength,
        },this.WriteToken);

        Check.That((Int32)reopened.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        DataControlResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Published);

        DataControlResponse<RemoveTransferResponse> removed = await _dc.RemoveTransfer(new RemoveTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)removed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(removed.Data).IsNotNull();
        Check.That(removed.Data!.Removed).IsTrue();

        DataControlResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task ClientUploadCanPublishThroughOpenUpload()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

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

            DataControlUploadCompletionResult? published = await session.Publish();

            Check.That(published).IsNotNull();
            Check.That(published!.Segmented!.Published).IsTrue();
            Check.That(published.Segmented.State.Status).IsEqualTo(DataItemTransferStatus.Published);

            DataControlResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
        }
    }

    [Test]
    public async Task ClientDownloadCanMaterializePublishedItem()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        SegmentedTransferPublishScenario scenario = await CreatePublishScenario(streamLength:1024 * 1024,streamSeed:0x66);
        OpenUploadTransferRequest open = scenario.Open;

        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);
        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        using MemoryStream upload = new(scenario.StreamPayload,false);
        PutTransferSegmentClientResponse? stored = await _dc.PutTransferSegment(CreatePutTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);

        Check.That(stored).IsNotNull();
        Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(stored.Response).IsNotNull();

        DataControlResponse<PublishSegmentedTransferResponse> published = await _dc.PublishSegmentedTransfer(new PublishSegmentedTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored.Response!.StateVersion },this.WriteToken);

        Check.That((Int32)published.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(published.Data).IsNotNull();
        Check.That(published.Data!.Published).IsTrue();

        DataControlDownloadSession? session = await _dc.OpenDownload(open.ItemID,this.ReadToken);

        Check.That(session).IsNotNull();
        Check.That(session!.Session.State.Mode).IsEqualTo(DataItemTransferMode.PublishedRead);

        Int32 downloaded = await session.DownloadToCompletion();
        Check.That(downloaded > 0).IsTrue();

        DataControlDownloadCompletionResult? materialized = await session.Materialize();

        Check.That(materialized).IsNotNull();
        Check.That(materialized!.Item).IsNotNull();
        Check.That(Guid.Equals(open.ItemID,materialized.Item!.GetID())).IsTrue();
        Check.That(File.Exists(materialized.StreamPath)).IsTrue();
        Check.That((await File.ReadAllBytesAsync(materialized.StreamPath)).SequenceEqual(scenario.StreamPayload)).IsTrue();

        DataControlResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task ReOpeningPublishedReadSessionRejectsDeletedPublishedItem()
    {
        using DataControlClient client = new(this.EndpointLocator!);

        SegmentedTransferPublishScenario scenario = await CreatePublishScenario(streamLength:1024 * 1024,streamSeed:0x68);
        OpenUploadTransferRequest open = scenario.Open;

        DataControlResponse<OpenUploadTransferResponse> opened = await client.OpenUploadTransfer(open,this.WriteToken);
        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        using MemoryStream upload = new(scenario.StreamPayload,false);
        PutTransferSegmentClientResponse? stored = await client.PutTransferSegment(CreatePutTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);
        Check.That(stored).IsNotNull();

        DataControlResponse<PublishSegmentedTransferResponse> published = await client.PublishSegmentedTransfer(new PublishSegmentedTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = stored!.Response!.StateVersion,
        },this.WriteToken);

        Check.That((Int32)published.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(published.Data!.Published).IsTrue();

        DataControlDownloadSession? download = await client.OpenDownload(open.ItemID,this.ReadToken);
        Check.That(download).IsNotNull();

        DataControlResponse<Guid> deleted = await client.Delete(open.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlResponse<ReOpenGetTransferResponse> reopened = await client.ReOpenGetTransfer(new ReOpenGetTransferRequest()
        {
            SessionID = download!.Session.SessionId,
            ItemID = open.ItemID,
        },this.ReadToken);

        Check.That((Int32)reopened.StatusCode).IsNotEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task ClientUploadAndDownloadHelpersRoundTripPublishedContent()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.ClientRoundTrip.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(1024 * 1024,0x6A);
        String? downloadedStreamPath = null;

        try
        {
            BinaryItem item = await CreateClientBinaryItem(filePath,payload);

            DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken);

            Check.That(uploaded).IsNotNull();
            Check.That(uploaded!.Segmented!.Published).IsTrue();
            Check.That(uploaded.Session.State.Status).IsEqualTo(DataItemTransferStatus.Published);

            DataControlDownloadCompletionResult? downloaded = await _dc.Download(item.GetID()!.Value,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsNotNull();
            Check.That(Guid.Equals(item.GetID(),downloaded.Item!.GetID())).IsTrue();
            Check.That(File.Exists(downloaded.StreamPath)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(downloaded.StreamPath)).SequenceEqual(payload)).IsTrue();

            downloadedStreamPath = downloaded.StreamPath;

            DataControlResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }
    }

    [Test]
    public async Task ClientUploadAndDownloadHelpersRoundTripSerializedOnlyFiniteContent()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        Byte[] payload = CreateSegmentedTransferPayload(1024,0x6C);

        BinaryItem item = new(payload);

        DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken);

        Check.That(uploaded).IsNotNull();
        Check.That(uploaded!.Segmented!.Published).IsTrue();
        Check.That(uploaded.Session.State.Status).IsEqualTo(DataItemTransferStatus.Published);
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

        DataControlResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task ClientDownloadMaterializeRequiresCompletedSession()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        SegmentedTransferPublishScenario scenario = await CreatePublishScenario(streamLength:1024 * 1024,streamSeed:0x6B);
        OpenUploadTransferRequest open = scenario.Open;
        String? workingDirectory = null;

        try
        {
            DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);
            Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            using MemoryStream upload = new(scenario.StreamPayload,false);
            PutTransferSegmentClientResponse? stored = await _dc.PutTransferSegment(CreatePutTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);

            Check.That(stored).IsNotNull();
            Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(stored.Response).IsNotNull();

            DataControlResponse<PublishSegmentedTransferResponse> published = await _dc.PublishSegmentedTransfer(new PublishSegmentedTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored.Response!.StateVersion },this.WriteToken);

            Check.That((Int32)published.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(published.Data).IsNotNull();
            Check.That(published.Data!.Published).IsTrue();

            DataControlDownloadSession? session = await _dc.OpenDownload(open.ItemID,this.ReadToken);

            Check.That(session).IsNotNull();
            workingDirectory = session!.Session.WorkingDirectory;
            Check.That(Directory.Exists(workingDirectory)).IsTrue();
            Check.That(await session.Materialize()).IsNull();

            Int32 downloaded = await session.DownloadToCompletion();
            Check.That(downloaded > 0).IsTrue();
            Check.That(await session.Materialize()).IsNotNull();

            DataControlResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            DeleteWorkingDirectory(workingDirectory);
        }
    }

    [Test]
    public async Task ClientUploadAbortRemovesLocalWorkingDirectory()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

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

            DataControlResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = session.Session.SessionId , ItemID = session.Session.ItemId },this.WriteToken);
            Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
        }
    }

    [Test]
    public async Task ClientUploadDeleteLocalStateRemovesWorkingDirectoryWithoutAbortingRemoteSession()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

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

            DataControlResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = session.Session.SessionId , ItemID = session.Session.ItemId },this.WriteToken);
            Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(state.Data).IsNotNull();
            Check.That(state.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Open);

            Boolean aborted = await session.Abort();

            Check.That(aborted).IsTrue();

            DataControlResponse<GetTransferStateResponse> abortedState = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = session.Session.SessionId , ItemID = session.Session.ItemId },this.WriteToken);
            Check.That((Int32)abortedState.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
        }
    }

    [Test]
    public async Task ClientDownloadDeleteLocalStateRemovesWorkingDirectoryWithoutDeletingPublishedItem()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        SegmentedTransferPublishScenario scenario = await CreatePublishScenario(streamLength:1024 * 1024,streamSeed:0x6E);
        OpenUploadTransferRequest open = scenario.Open;
        String? reopenedWorkingDirectory = null;

        try
        {
            DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);
            Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            using MemoryStream upload = new(scenario.StreamPayload,false);
            PutTransferSegmentClientResponse? stored = await _dc.PutTransferSegment(CreatePutTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);

            Check.That(stored).IsNotNull();
            Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(stored.Response).IsNotNull();

            DataControlResponse<PublishSegmentedTransferResponse> published = await _dc.PublishSegmentedTransfer(new PublishSegmentedTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID , ExpectedStateVersion = stored.Response!.StateVersion },this.WriteToken);

            Check.That((Int32)published.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(published.Data).IsNotNull();
            Check.That(published.Data!.Published).IsTrue();

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

            DataControlResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            DeleteWorkingDirectory(reopenedWorkingDirectory);
        }
    }

    [Test]
    public async Task ClientUploadHelperReportsProgress()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

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
            Check.That(uploaded.Session.State.Status).IsEqualTo(DataItemTransferStatus.Published);

            workingDirectory = uploaded.Session.WorkingDirectory;

            Check.That(reporter.Latest).IsNotNull();
            AssertTransferProgressSequence(progress,item.GetID()!.Value,uploaded.Session.SessionId,DataItemTransferMode.Upload,payload.LongLength);

            DataControlResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
            DeleteWorkingDirectory(workingDirectory);
        }
    }

    [Test]
    public async Task ClientDownloadHelperReportsProgress()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

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

            AssertTransferProgressSequence(progress,item.GetID()!.Value,downloaded.Session.SessionId,DataItemTransferMode.PublishedRead,payload.LongLength);

            DataControlResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
            DeleteWorkingDirectory(uploadedWorkingDirectory);
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }
    }

    [Test]
    public async Task SegmentedTransferPublishRetainedSessionBlocksNewWriterOpen()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        SegmentedTransferPublishScenario initialScenario = await CreatePublishScenario(streamLength:256 * 1024,streamSeed:0x91);
        OpenUploadTransferRequest initialOpen = initialScenario.Open;

        DataControlResponse<OpenUploadTransferResponse> initialOpened = await _dc.OpenUploadTransfer(initialOpen,this.WriteToken);
        Check.That((Int32)initialOpened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        using MemoryStream initialUpload = new(initialScenario.StreamPayload,false);
        PutTransferSegmentClientResponse? initialStored = await _dc.PutTransferSegment(CreatePutTransferSegmentRequest(initialOpen,0,initialScenario.StreamPayload),initialUpload,this.WriteToken);

        Check.That(initialStored).IsNotNull();
        Check.That((Int32)initialStored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(initialStored.Response).IsNotNull();

        DataControlResponse<PublishSegmentedTransferResponse> initialPublished = await _dc.PublishSegmentedTransfer(new PublishSegmentedTransferRequest()
        {
            SessionID = initialOpen.SessionID,
            ItemID = initialOpen.ItemID,
            ExpectedStateVersion = initialStored.Response!.StateVersion,
        },this.WriteToken);

        Check.That((Int32)initialPublished.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(initialPublished.Data).IsNotNull();
        Check.That(initialPublished.Data!.Published).IsTrue();

        OpenUploadTransferRequest conflictingOpen = initialOpen with
        {
            SessionID = Guid.NewGuid(),
        };

        DataControlResponse<OpenUploadTransferResponse> conflictingOpened = await _dc.OpenUploadTransfer(conflictingOpen,this.WriteToken);
        Check.That((Int32)conflictingOpened.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        DataControlResponse<RemoveTransferResponse> removedInitial = await _dc.RemoveTransfer(new RemoveTransferRequest(){ SessionID = initialOpen.SessionID , ItemID = initialOpen.ItemID },this.WriteToken);
        Check.That((Int32)removedInitial.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlResponse<OpenUploadTransferResponse> reopenedAfterCleanup = await _dc.OpenUploadTransfer(conflictingOpen,this.WriteToken);
        Check.That((Int32)reopenedAfterCleanup.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlResponse<AbortTransferResponse> abortedRetry = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = conflictingOpen.SessionID , ItemID = conflictingOpen.ItemID },this.WriteToken);
        Check.That((Int32)abortedRetry.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlResponse<Guid> deletedFinal = await _dc.Delete(conflictingOpen.ItemID,this.WriteToken);
        Check.That((Int32)deletedFinal.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task ConcurrentSegmentedPublishAndDeleteAreSerializedByItemLock()
    {
        SegmentedTransferPublishScenario scenario = await CreatePublishScenario(streamLength:128 * 1024,streamSeed:0x92);
        OpenUploadTransferRequest open = scenario.Open;
        using DataControlClient setup = new(this.EndpointLocator!,this.WriteToken);
        using CatalogClient catalog = new(this.CatalogEndpointLocator!,this.ReadToken);

        DataControlResponse<OpenUploadTransferResponse> opened = await setup.OpenUploadTransfer(open,this.WriteToken);
        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(opened.Data).IsNotNull();

        using MemoryStream payload = new(scenario.StreamPayload,false);
        PutTransferSegmentClientResponse? uploaded = await setup.PutTransferSegment(
            CreatePutTransferSegmentRequest(open,0,scenario.StreamPayload),payload,this.WriteToken);

        Check.That(uploaded).IsNotNull();
        Check.That((Int32)uploaded!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(uploaded.Response).IsNotNull();

        PublishSegmentedTransferRequest publishRequest = new()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = uploaded.Response!.StateVersion,
        };

        TaskCompletionSource start = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Task<DataControlResponse<PublishSegmentedTransferResponse>> publish = Task.Run(async () =>
        {
            await start.Task;
            using DataControlClient client = new(this.EndpointLocator!,this.WriteToken);
            return await client.PublishSegmentedTransfer(publishRequest,this.WriteToken);
        });

        Task<DataControlResponse<Guid>> delete = Task.Run(async () =>
        {
            await start.Task;
            using DataControlClient client = new(this.EndpointLocator!,this.WriteToken);
            return await client.Delete(open.ItemID,this.WriteToken);
        });

        try
        {
            start.SetResult();

            await Task.WhenAll(publish,delete);

            Check.That((Int32)publish.Result.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(publish.Result.Data).IsNotNull();
            Check.That(publish.Result.Data!.Published).IsTrue();
            Check.That(new[] { StatusCodes.Status200OK,StatusCodes.Status404NotFound }.Contains((Int32)delete.Result.StatusCode)).IsTrue();

            CatalogResponse<ElementResponse> result = await catalog.SearchElements(new ElementQuery { ID = open.ItemID },this.ReadToken);
            Check.That((Int32)result.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(result.Data).IsNotNull();
            Check.That(result.Data!.Elements.Length).IsEqualTo((Int32)delete.Result.StatusCode == StatusCodes.Status200OK ? 0 : 1);
        }
        finally
        {
            try { await setup.Delete(open.ItemID,this.WriteToken); }
            catch { }

            try { await setup.RemoveTransfer(new RemoveTransferRequest { ItemID = open.ItemID,SessionID = open.SessionID },this.WriteToken); }
            catch { }
        }
    }

    [Test]
    public async Task ConcurrentDuplicateSegmentedPublicationProducesOneCatalogItem()
    {
        SegmentedTransferPublishScenario scenario = await CreatePublishScenario(streamLength:128 * 1024,streamSeed:0x93);
        OpenUploadTransferRequest open = scenario.Open;
        using DataControlClient setup = new(this.EndpointLocator!,this.WriteToken);
        using CatalogClient catalog = new(this.CatalogEndpointLocator!,this.ReadToken);

        DataControlResponse<OpenUploadTransferResponse> opened = await setup.OpenUploadTransfer(open,this.WriteToken);
        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(opened.Data).IsNotNull();

        using MemoryStream payload = new(scenario.StreamPayload,false);
        PutTransferSegmentClientResponse? uploaded = await setup.PutTransferSegment(
            CreatePutTransferSegmentRequest(open,0,scenario.StreamPayload),payload,this.WriteToken);

        Check.That(uploaded).IsNotNull();
        Check.That((Int32)uploaded!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(uploaded.Response).IsNotNull();

        PublishSegmentedTransferRequest request = new()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = uploaded.Response!.StateVersion,
        };

        TaskCompletionSource start = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Task<DataControlResponse<PublishSegmentedTransferResponse>>[] retries = Enumerable.Range(0,2).Select(async _ =>
        {
            await start.Task;
            using DataControlClient client = new(this.EndpointLocator!,this.WriteToken);
            return await client.PublishSegmentedTransfer(request,this.WriteToken);
        }).ToArray();

        try
        {
            start.SetResult();

            DataControlResponse<PublishSegmentedTransferResponse>[] results = await Task.WhenAll(retries);

            Check.That(results.Count(response => (Int32)response.StatusCode == StatusCodes.Status200OK)).IsEqualTo(1);
            Check.That(results.Count(response => (Int32)response.StatusCode == StatusCodes.Status409Conflict)).IsEqualTo(1);

            Task<DataControlResponse<PublishSegmentedTransferResponse>>[] laterRetries = Enumerable.Range(0,2).Select(_ =>
                setup.PublishSegmentedTransfer(request,this.WriteToken)).ToArray();
            DataControlResponse<PublishSegmentedTransferResponse>[] laterResults = await Task.WhenAll(laterRetries);

            Check.That(laterResults.All(response => (Int32)response.StatusCode == StatusCodes.Status409Conflict)).IsTrue();

            CatalogResponse<ElementResponse> catalogResult = await catalog.SearchElements(new ElementQuery { ID = open.ItemID },this.ReadToken);
            Check.That((Int32)catalogResult.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(catalogResult.Data).IsNotNull();
            Check.That(catalogResult.Data!.Elements.Length).IsEqualTo(1);
        }
        finally
        {
            try { await setup.Delete(open.ItemID,this.WriteToken); }
            catch { }

            try { await setup.RemoveTransfer(new RemoveTransferRequest { ItemID = open.ItemID,SessionID = open.SessionID },this.WriteToken); }
            catch { }
        }
    }

    [Test]
    public async Task SegmentedTransferAcceptsIdenticalReplayAtCommittedRange()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);
        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] payload = CreateSegmentedTransferPayload(16,0xA8);
        PutTransferSegmentRequest request = CreatePutTransferSegmentRequest(open,8,payload);
        using MemoryStream firstPayload = new(payload,false);
        PutTransferSegmentClientResponse? first = await _dc.PutTransferSegment(request,firstPayload,this.WriteToken);

        Check.That(first).IsNotNull();
        Check.That((Int32)first!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(first.Response).IsNotNull();
        Check.That(first.Response!.Accepted).IsTrue();

        using MemoryStream replayPayload = new(payload,false);
        PutTransferSegmentClientResponse? replay = await _dc.PutTransferSegment(request,replayPayload,this.WriteToken);

        Check.That(replay).IsNotNull();
        Check.That((Int32)replay!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(replay.Response).IsNotNull();
        Check.That(replay.Response!.Accepted).IsTrue();
        Check.That(replay.Response.Replayed).IsTrue();
        Check.That(replay.Response.StateVersion).IsEqualTo(first.Response.StateVersion);

        DataControlResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That(state.Data!.State.StateVersion).IsEqualTo(first.Response.StateVersion);
        Check.That(state.Data.State.RealizedBytes).IsEqualTo(payload.Length);

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task SegmentedTransferRejectsDifferentPayloadAtCommittedRange()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);
        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] committedPayload = CreateSegmentedTransferPayload(16,0xA9);
        PutTransferSegmentRequest request = CreatePutTransferSegmentRequest(open,8,committedPayload);
        using MemoryStream firstPayload = new(committedPayload,false);
        PutTransferSegmentClientResponse? first = await _dc.PutTransferSegment(request,firstPayload,this.WriteToken);

        Check.That(first).IsNotNull();
        Check.That((Int32)first!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(first.Response).IsNotNull();

        Byte[] conflictingPayload = CreateSegmentedTransferPayload(16,0xAA);
        PutTransferSegmentRequest conflictingRequest = request with { SegmentSHA512 = SHA512.HashData(conflictingPayload) };
        using MemoryStream conflictingStream = new(conflictingPayload,false);
        PutTransferSegmentClientResponse? conflict = await _dc.PutTransferSegment(conflictingRequest,conflictingStream,this.WriteToken);

        Check.That(conflict).IsNotNull();
        Check.That((Int32)conflict!.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        DataControlResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That(state.Data!.State.StateVersion).IsEqualTo(first.Response!.StateVersion);
        Check.That(state.Data.State.RealizedBytes).IsEqualTo(committedPayload.Length);

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task SegmentedTransferRejectsHashMismatch()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);
        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] payload = CreateSegmentedTransferPayload(16,0xA1);
        PutTransferSegmentRequest request = CreatePutTransferSegmentRequest(open,0,payload);
        request = request with { SegmentSHA512 = SHA512.HashData(CreateSegmentedTransferPayload(payload.Length,0xA2)) };

        using MemoryStream upload = new(payload,false);
        PutTransferSegmentClientResponse? response = await _dc.PutTransferSegment(request,upload,this.WriteToken);

        Check.That(response).IsNotNull();
        Check.That((Int32)response!.StatusCode).IsEqualTo(StatusCodes.Status422UnprocessableEntity);

        DataControlResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.RealizedBytes).IsEqualTo(0);

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task SegmentedTransferRejectsPayloadLengthMismatch()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:64);
        DataControlResponse<OpenUploadTransferResponse> opened = await _dc.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] payload = CreateSegmentedTransferPayload(16,0xA4);
        PutTransferSegmentRequest request = CreatePutTransferSegmentRequest(open,0,payload);
        request = request with { Length = payload.LongLength + 1 };

        Int32 statusCode = await this.SendMalformedTransferEnvelopeAsync("/PutTransferSegment",request,payload,payload.LongLength,this.WriteToken);

        Check.That(statusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        DataControlResponse<GetTransferStateResponse> state = await _dc.GetTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.RealizedBytes).IsEqualTo(0);

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    private static Byte[] CreateSegmentedTransferPayload(Int32 length,Byte seed)
    {
        Byte[] payload = new Byte[length];

        for(Int32 i = 0;i < payload.Length;i++) { payload[i] = unchecked((Byte)(seed + i)); }

        return payload;
    }

    private static async Task<SegmentedTransferPublishScenario> CreatePublishScenario(Int32 streamLength,Byte streamSeed)
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
        if(mode is DataItemTransferMode.Upload)
        {
            Check.That(progress.All(_ => _.LatestRange is not null && _.LatestRange.Validate() && _.LatestRange.Length > 0)).IsTrue();
        }
        else
        {
            Check.That(progress[0].LatestRange).IsNull();
            Check.That(progress.Skip(1).All(_ => _.LatestRange is not null && _.LatestRange.Validate() && _.LatestRange.Length > 0)).IsTrue();
        }
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
            RequestID = Guid.NewGuid(),
        };
    }

    private sealed record SegmentedTransferPublishScenario
    {
        public OpenUploadTransferRequest Open { get; init; } = new();

        public Byte[] StreamPayload { get; init; } = Array.Empty<Byte>();
    }

}
