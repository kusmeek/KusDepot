namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test]
    public async Task StreamTransferRoundTrip()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenStreamTransferRequest open = CreateOpenStreamTransferRequest();

        DataControlResponse<OpenStreamTransferResponse> opened = await _dc.OpenStreamTransfer(open);

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

        DataControlResponse<GetStreamTransferStateResponse> state = await _dc.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

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

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(aborted.Data).IsNotNull();
        Check.That(aborted.Data!.Aborted).IsTrue();
    }

    [Test]
    public async Task StreamTransferAbortRejectsPublishedUpload()
    {
        using DataControlClient client = new(this.EndpointLocator!);
        StreamTransferPublishScenario scenario = await CreateStreamTransferPublishScenario(128 * 1024,0x78);
        OpenStreamTransferRequest open = scenario.Open;

        try
        {
            DataControlResponse<OpenStreamTransferResponse> opened = await client.OpenStreamTransfer(open,this.WriteToken);
            Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            using MemoryStream upload = new(scenario.StreamPayload,false);
            PutStreamTransferSegmentClientResponse? stored = await client.PutStreamTransferSegment(CreatePutStreamTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);
            Check.That(stored).IsNotNull();

            DataControlResponse<PublishStreamTransferResponse> published = await client.PublishStreamTransfer(new PublishStreamTransferRequest()
            {
                SessionID = open.SessionID,
                ItemID = open.ItemID,
                ExpectedStateVersion = stored!.Response!.StateVersion,
                FinalObjectInfo = open.ObjectInfo,
                FinalObjectPayload = open.ObjectPayload,
                FinalStreamLength = scenario.StreamPayload.LongLength,
            },this.WriteToken);
            Check.That((Int32)published.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            DataControlResponse<AbortTransferResponse> aborted = await client.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
            Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

            DataControlResponse<GetStreamTransferStateResponse> state = await client.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
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
    public async Task StreamTransferFollowAndReOpenSynchronizesFollowerState()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenStreamTransferRequest open = CreateOpenStreamTransferRequest();
        DataControlResponse<OpenStreamTransferResponse> opened = await _dc.OpenStreamTransfer(open,this.WriteToken);

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

        DataControlResponse<OpenFollowStreamTransferResponse> followed = await _dc.OpenFollowStreamTransfer(follow,this.ReadToken);

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

        DataControlResponse<ReOpenFollowStreamTransferResponse> reopened = await _dc.ReOpenFollowStreamTransfer(new ReOpenFollowStreamTransferRequest()
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

        DataControlResponse<AbortTransferResponse> abortedFollower = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = follow.SessionID , ItemID = follow.ItemID },this.ReadToken);
        Check.That((Int32)abortedFollower.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        DataControlResponse<AbortTransferResponse> abortedSource = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)abortedSource.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task StreamTransferPublishRetainsSessionAndRejectsReOpenInPublishedState()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        StreamTransferPublishScenario scenario = await CreateStreamTransferPublishScenario(1024 * 1024,0x75);
        OpenStreamTransferRequest open = scenario.Open;
        DataControlResponse<OpenStreamTransferResponse> opened = await _dc.OpenStreamTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(opened.Data).IsNotNull();

        using MemoryStream upload = new(scenario.StreamPayload,false);
        PutStreamTransferSegmentClientResponse? stored = await _dc.PutStreamTransferSegment(CreatePutStreamTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);

        Check.That(stored).IsNotNull();
        Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(stored.Response).IsNotNull();
        Check.That(stored.Response!.Accepted).IsTrue();

        DataControlResponse<PublishStreamTransferResponse> published = await _dc.PublishStreamTransfer(new PublishStreamTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = stored.Response.StateVersion,
            FinalObjectInfo = open.ObjectInfo,
            FinalObjectPayload = open.ObjectPayload,
            FinalStreamLength = scenario.StreamPayload.LongLength,
        },this.WriteToken);

        Check.That((Int32)published.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(published.Data).IsNotNull();
        Check.That(published.Data!.Published).IsTrue();
        Check.That(published.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Published);
        Check.That(published.Data.State.AppendedLength).IsEqualTo(scenario.StreamPayload.LongLength);

        DataControlResponse<GetStreamTransferStateResponse> state = await _dc.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.Status).IsEqualTo(DataItemTransferStatus.Published);
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

        DataControlResponse<ReOpenStreamTransferResponse> reopened = await _dc.ReOpenStreamTransfer(new ReOpenStreamTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ObjectSHA512 = open.ObjectSHA512,
        },this.WriteToken);

        Check.That((Int32)reopened.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        DataControlResponse<RemoveTransferResponse> removed = await _dc.RemoveTransfer(new RemoveTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)removed.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(removed.Data).IsNotNull();
        Check.That(removed.Data!.Removed).IsTrue();

        DataControlResponse<GetStreamTransferStateResponse> removedState = await _dc.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)removedState.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);

        DataControlResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task StreamTransferPublishCanRetryAfterStaleExpectedStateVersion()
    {
        using DataControlClient client = new(this.EndpointLocator!);
        StreamTransferPublishScenario scenario = await CreateStreamTransferPublishScenario(64 * 1024,0x76);
        OpenStreamTransferRequest open = scenario.Open;

        try
        {
            DataControlResponse<OpenStreamTransferResponse> opened = await client.OpenStreamTransfer(open,this.WriteToken);
            Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            using MemoryStream upload = new(scenario.StreamPayload,false);
            PutStreamTransferSegmentClientResponse? stored = await client.PutStreamTransferSegment(CreatePutStreamTransferSegmentRequest(open,0,scenario.StreamPayload),upload,this.WriteToken);
            Check.That(stored).IsNotNull();
            Check.That((Int32)stored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            PublishStreamTransferRequest publishRequest = new()
            {
                SessionID = open.SessionID,
                ItemID = open.ItemID,
                ExpectedStateVersion = stored.Response!.StateVersion - 1,
                FinalObjectInfo = open.ObjectInfo,
                FinalObjectPayload = open.ObjectPayload,
                FinalStreamLength = scenario.StreamPayload.LongLength,
            };

            DataControlResponse<PublishStreamTransferResponse> stale = await client.PublishStreamTransfer(publishRequest,this.WriteToken);
            Check.That((Int32)stale.StatusCode).IsEqualTo(StatusCodes.Status412PreconditionFailed);

            publishRequest = publishRequest with { ExpectedStateVersion = stored.Response.StateVersion };

            DataControlResponse<PublishStreamTransferResponse> wrongLength = await client.PublishStreamTransfer(publishRequest with { FinalStreamLength = scenario.StreamPayload.LongLength + 1 },this.WriteToken);
            Check.That((Int32)wrongLength.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

            DataControlResponse<PublishStreamTransferResponse> corrected = await client.PublishStreamTransfer(publishRequest,this.WriteToken);
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
    public async Task ClientStreamUploadAndDownloadRoundTripPublishedContent()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

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
            Check.That(uploaded.Session.StreamState!.Status).IsEqualTo(DataItemTransferStatus.Published);

            uploadedWorkingDirectory = uploaded.Session.WorkingDirectory;

            DataControlDownloadCompletionResult? downloaded = await _dc.Download(item.GetID()!.Value,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsNotNull();
            Check.That(Guid.Equals(item.GetID(),downloaded.Item!.GetID())).IsTrue();
            Check.That(downloaded.Item).IsInstanceOf<DataStreamItem>();
            Check.That(File.Exists(downloaded.StreamPath)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(downloaded.StreamPath)).SequenceEqual(payload)).IsTrue();

            downloadedStreamPath = downloaded.StreamPath;

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
    public async Task StreamTransferCompleteRetainedSessionBlocksNewWriterOpen()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        StreamTransferPublishScenario initialScenario = await CreateStreamTransferPublishScenario(256 * 1024,0x81);
        OpenStreamTransferRequest initialOpen = initialScenario.Open;

        DataControlResponse<OpenStreamTransferResponse> initialOpened = await _dc.OpenStreamTransfer(initialOpen,this.WriteToken);
        Check.That((Int32)initialOpened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        using MemoryStream initialUpload = new(initialScenario.StreamPayload,false);
        PutStreamTransferSegmentClientResponse? initialStored = await _dc.PutStreamTransferSegment(CreatePutStreamTransferSegmentRequest(initialOpen,0,initialScenario.StreamPayload),initialUpload,this.WriteToken);

        Check.That(initialStored).IsNotNull();
        Check.That((Int32)initialStored!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(initialStored.Response).IsNotNull();

        DataControlResponse<PublishStreamTransferResponse> initialCompleted = await _dc.PublishStreamTransfer(new PublishStreamTransferRequest()
        {
            SessionID = initialOpen.SessionID,
            ItemID = initialOpen.ItemID,
            ExpectedStateVersion = initialStored.Response!.StateVersion,
            FinalObjectInfo = initialOpen.ObjectInfo,
            FinalObjectPayload = initialOpen.ObjectPayload,
            FinalStreamLength = initialScenario.StreamPayload.LongLength
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

        DataControlResponse<OpenStreamTransferResponse> conflictingOpened = await _dc.OpenStreamTransfer(conflictingOpen,this.WriteToken);
        Check.That((Int32)conflictingOpened.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        DataControlResponse<RemoveTransferResponse> removedInitial = await _dc.RemoveTransfer(new RemoveTransferRequest(){ SessionID = initialOpen.SessionID , ItemID = initialOpen.ItemID },this.WriteToken);
        Check.That((Int32)removedInitial.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlResponse<OpenStreamTransferResponse> reopenedAfterCleanup = await _dc.OpenStreamTransfer(conflictingOpen,this.WriteToken);
        Check.That((Int32)reopenedAfterCleanup.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlResponse<AbortTransferResponse> abortedRetry = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = conflictingOpen.SessionID , ItemID = conflictingOpen.ItemID },this.WriteToken);
        Check.That((Int32)abortedRetry.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(abortedRetry.Data).IsNotNull();
        Check.That(abortedRetry.Data!.Aborted).IsTrue();

        DataControlResponse<Guid> deleted = await _dc.Delete(conflictingOpen.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task ConcurrentStreamPublishAndDeleteAreSerializedByItemLock()
    {
        StreamTransferPublishScenario scenario = await CreateStreamTransferPublishScenario(128 * 1024,0x82);
        OpenStreamTransferRequest open = scenario.Open;
        using DataControlClient setup = new(this.EndpointLocator!,this.WriteToken);
        using CatalogClient catalog = new(this.CatalogEndpointLocator!,this.ReadToken);

        DataControlResponse<OpenStreamTransferResponse> opened = await setup.OpenStreamTransfer(open,this.WriteToken);
        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(opened.Data).IsNotNull();

        using MemoryStream payload = new(scenario.StreamPayload,false);
        PutStreamTransferSegmentClientResponse? uploaded = await setup.PutStreamTransferSegment(
            CreatePutStreamTransferSegmentRequest(open,0,scenario.StreamPayload),payload,this.WriteToken);

        Check.That(uploaded).IsNotNull();
        Check.That((Int32)uploaded!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(uploaded.Response).IsNotNull();

        PublishStreamTransferRequest publishRequest = new()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = uploaded.Response!.StateVersion,
            FinalObjectInfo = open.ObjectInfo,
            FinalObjectPayload = open.ObjectPayload,
            FinalStreamLength = scenario.StreamPayload.LongLength,
        };

        TaskCompletionSource start = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Task<DataControlResponse<PublishStreamTransferResponse>> publish = Task.Run(async () =>
        {
            await start.Task;
            using DataControlClient client = new(this.EndpointLocator!,this.WriteToken);
            return await client.PublishStreamTransfer(publishRequest,this.WriteToken);
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

            if((Int32)delete.Result.StatusCode == StatusCodes.Status200OK)
            {
                Check.That((Int32)result.StatusCode).IsEqualTo(StatusCodes.Status200OK);
                Check.That(result.Data).IsNotNull();
                Check.That(result.Data!.Elements.Length).IsEqualTo(0);
            }
            else
            {
                Check.That((Int32)result.StatusCode).IsEqualTo(StatusCodes.Status200OK);
                Check.That(result.Data).IsNotNull();
                Check.That(result.Data!.Elements.Length).IsEqualTo(1);
            }
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
    public async Task ConcurrentDuplicateStreamPublicationProducesOneCatalogItem()
    {
        StreamTransferPublishScenario scenario = await CreateStreamTransferPublishScenario(128 * 1024,0x83);
        OpenStreamTransferRequest open = scenario.Open;
        using DataControlClient setup = new(this.EndpointLocator!,this.WriteToken);
        using CatalogClient catalog = new(this.CatalogEndpointLocator!,this.ReadToken);

        DataControlResponse<OpenStreamTransferResponse> opened = await setup.OpenStreamTransfer(open,this.WriteToken);
        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(opened.Data).IsNotNull();

        using MemoryStream payload = new(scenario.StreamPayload,false);
        PutStreamTransferSegmentClientResponse? uploaded = await setup.PutStreamTransferSegment(
            CreatePutStreamTransferSegmentRequest(open,0,scenario.StreamPayload),payload,this.WriteToken);

        Check.That(uploaded).IsNotNull();
        Check.That((Int32)uploaded!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(uploaded.Response).IsNotNull();

        PublishStreamTransferRequest request = new()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = uploaded.Response!.StateVersion,
            FinalObjectInfo = open.ObjectInfo,
            FinalObjectPayload = open.ObjectPayload,
            FinalStreamLength = scenario.StreamPayload.LongLength,
        };

        TaskCompletionSource start = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Task<DataControlResponse<PublishStreamTransferResponse>>[] retries = Enumerable.Range(0,2).Select(async _ =>
        {
            await start.Task;
            using DataControlClient client = new(this.EndpointLocator!,this.WriteToken);
            return await client.PublishStreamTransfer(request,this.WriteToken);
        }).ToArray();

        try
        {
            start.SetResult();

            DataControlResponse<PublishStreamTransferResponse>[] results = await Task.WhenAll(retries);

            Check.That(results.Count(response => (Int32)response.StatusCode == StatusCodes.Status200OK)).IsEqualTo(1);
            Check.That(results.Count(response => (Int32)response.StatusCode == StatusCodes.Status409Conflict)).IsEqualTo(1);

            Task<DataControlResponse<PublishStreamTransferResponse>>[] laterRetries = Enumerable.Range(0,2).Select(_ =>
                setup.PublishStreamTransfer(request,this.WriteToken)).ToArray();
            DataControlResponse<PublishStreamTransferResponse>[] laterResults = await Task.WhenAll(laterRetries);

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
    public async Task EmptyStreamTransferPublishesReadableZeroLengthStream()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        StreamTransferPublishScenario scenario = await CreateStreamTransferPublishScenario(0,0x77);
        OpenStreamTransferRequest open = scenario.Open;
        DataControlResponse<OpenStreamTransferResponse> opened = await _dc.OpenStreamTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(opened.Data).IsNotNull();

        DataControlResponse<PublishStreamTransferResponse> published = await _dc.PublishStreamTransfer(new PublishStreamTransferRequest()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            ExpectedStateVersion = opened.Data!.State.StateVersion,
            FinalObjectInfo = open.ObjectInfo,
            FinalObjectPayload = open.ObjectPayload,
            FinalStreamLength = 0
        },this.WriteToken);

        Check.That((Int32)published.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(published.Data).IsNotNull();
        Check.That(published.Data!.Published).IsTrue();
        Check.That(published.Data.State.Status).IsEqualTo(DataItemTransferStatus.Published);
        Check.That(published.Data.State.AppendedLength).IsEqualTo(0);

        String? downloadedStreamPath = null;

        try
        {
            DataControlDownloadCompletionResult? downloaded = await _dc.Download(open.ItemID,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsNotNull();
            Check.That(downloaded.Item).IsInstanceOf<DataStreamItem>();
            Check.That(File.Exists(downloaded.StreamPath)).IsTrue();
            Check.That(new FileInfo(downloaded.StreamPath).Length).IsEqualTo(0);

            downloadedStreamPath = downloaded.StreamPath;
        }
        finally
        {
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }

        DataControlResponse<RemoveTransferResponse> removed = await _dc.RemoveTransfer(new RemoveTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)removed.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlResponse<Guid> deleted = await _dc.Delete(open.ItemID,this.WriteToken);
        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task StreamTransferAcceptsIdenticalReplayAtCommittedOffset()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenStreamTransferRequest open = CreateOpenStreamTransferRequest();
        DataControlResponse<OpenStreamTransferResponse> opened = await _dc.OpenStreamTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(opened.Data).IsNotNull();

        Byte[] payload = CreateSegmentedTransferPayload(16,0xAA);
        PutStreamTransferSegmentRequest originalRequest = CreatePutStreamTransferSegmentRequest(open,0,payload);

        using MemoryStream firstPayload = new(payload,false);
        PutStreamTransferSegmentClientResponse? first = await _dc.PutStreamTransferSegment(originalRequest,firstPayload,this.WriteToken);

        Check.That(first).IsNotNull();
        Check.That((Int32)first!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(first.Response).IsNotNull();

        using MemoryStream replayPayload = new(payload,false);
        PutStreamTransferSegmentClientResponse? replay = await _dc.PutStreamTransferSegment(originalRequest with { RequestID = Guid.NewGuid() },replayPayload,this.WriteToken);

        Check.That(replay).IsNotNull();
        Check.That((Int32)replay!.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(replay.Response).IsNotNull();
        Check.That(replay.Response!.Accepted).IsTrue();
        Check.That(replay.Response.AppendedLength).IsEqualTo(payload.Length);
        Check.That(replay.Response.Replayed).IsTrue();
        Check.That(replay.Response.StateVersion).IsEqualTo(first.Response!.StateVersion);

        DataControlResponse<GetStreamTransferStateResponse> state = await _dc.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.AppendedLength).IsEqualTo(payload.Length);
        Check.That(state.Data.State.StateVersion).IsEqualTo(first.Response.StateVersion);

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task StreamTransferRejectsDifferentPayloadAtCommittedOffset()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenStreamTransferRequest open = CreateOpenStreamTransferRequest();
        DataControlResponse<OpenStreamTransferResponse> opened = await _dc.OpenStreamTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] committedPayload = CreateSegmentedTransferPayload(16,0xAB);
        using MemoryStream firstPayload = new(committedPayload,false);
        PutStreamTransferSegmentClientResponse? first = await _dc.PutStreamTransferSegment(CreatePutStreamTransferSegmentRequest(open,0,committedPayload),firstPayload,this.WriteToken);

        Check.That(first).IsNotNull();
        Check.That((Int32)first!.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] conflictingPayload = CreateSegmentedTransferPayload(16,0xAC);
        PutStreamTransferSegmentRequest conflictingRequest = CreatePutStreamTransferSegmentRequest(open,0,conflictingPayload,first.Response!.StateVersion);
        using MemoryStream retryPayload = new(conflictingPayload,false);
        PutStreamTransferSegmentClientResponse? retry = await _dc.PutStreamTransferSegment(conflictingRequest,retryPayload,this.WriteToken);

        Check.That(retry).IsNotNull();
        Check.That((Int32)retry!.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        DataControlResponse<GetStreamTransferStateResponse> state = await _dc.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.AppendedLength).IsEqualTo(committedPayload.Length);
        Check.That(state.Data.State.StateVersion).IsEqualTo(first.Response.StateVersion);

        DataControlResponse<AbortTransferResponse> aborted = await _dc.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);
        Check.That((Int32)aborted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
    }

    [Test]
    public async Task StreamTransferRejectsHashMismatch()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenStreamTransferRequest open = CreateOpenStreamTransferRequest();
        DataControlResponse<OpenStreamTransferResponse> opened = await _dc.OpenStreamTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] payload = CreateSegmentedTransferPayload(16,0xA5);
        PutStreamTransferSegmentRequest request = CreatePutStreamTransferSegmentRequest(open,0,payload);
        request = request with { SegmentSHA512 = SHA512.HashData(CreateSegmentedTransferPayload(payload.Length,0xA6)) };

        using MemoryStream upload = new(payload,false);
        PutStreamTransferSegmentClientResponse? response = await _dc.PutStreamTransferSegment(request,upload,this.WriteToken);

        Check.That(response).IsNotNull();
        Check.That((Int32)response!.StatusCode).IsEqualTo(StatusCodes.Status422UnprocessableEntity);

        DataControlResponse<GetStreamTransferStateResponse> state = await _dc.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.AppendedLength).IsEqualTo(0);
    }

    [Test]
    public async Task StreamTransferRejectsPayloadLengthMismatch()
    {
        using DataControlClient _dc = new(this.EndpointLocator!);

        OpenStreamTransferRequest open = CreateOpenStreamTransferRequest();
        DataControlResponse<OpenStreamTransferResponse> opened = await _dc.OpenStreamTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Byte[] payload = CreateSegmentedTransferPayload(16,0xA8);
        PutStreamTransferSegmentRequest request = CreatePutStreamTransferSegmentRequest(open,0,payload);
        request = request with { Length = payload.LongLength + 1 };

        Int32 statusCode = await this.SendMalformedTransferEnvelopeAsync("/PutStreamTransferSegment",request,payload,payload.LongLength,this.WriteToken);

        Check.That(statusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        DataControlResponse<GetStreamTransferStateResponse> state = await _dc.GetStreamTransferState(new DataItemTransferIdentity(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken);

        Check.That((Int32)state.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(state.Data).IsNotNull();
        Check.That(state.Data!.State.AppendedLength).IsEqualTo(0);
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
            RequestID = Guid.NewGuid()
        };
    }

    private static async Task<StreamTransferPublishScenario> CreateStreamTransferPublishScenario(Int32 streamLength,Byte streamSeed)
    {
        Byte[] streamPayload = CreateSegmentedTransferPayload(streamLength,streamSeed);
        Int32 boundedStreamLength = Math.Max(1,streamLength);

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
                RequestedSegmentSizePolicy = streamLength == 0 ? null : new DataItemTransferSegmentSizePolicy() { MinSegmentBytes = 1,PreferredSegmentBytes = Math.Min(16,boundedStreamLength),MaxSegmentBytes = boundedStreamLength },
                ObjectInfo = new DataItemTransferObjectInfo() { ObjectType = typeof(DataStreamItem).FullName,ContentStreamed = true },
            },
            StreamPayload = streamPayload,
        };
    }

    private sealed record StreamTransferPublishScenario
    {
        public OpenStreamTransferRequest Open { get; init; } = new();

        public Byte[] StreamPayload { get; init; } = Array.Empty<Byte>();
    }
}
