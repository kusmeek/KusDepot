namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test]
    public async Task ClientOpenUploadRoutesSegmentedAndStreamFamilies()
    {
        String workingRoot = CreateClientWorkingRoot();
        String segmentedFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.SegmentedOpen.{Guid.NewGuid():N}.bin");
        String streamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamOpen.{Guid.NewGuid():N}.bin");
        Byte[] segmentedPayload = CreateSegmentedTransferPayload(256 * 1024,0x82);
        Byte[] streamPayload = CreateSegmentedTransferPayload(256 * 1024,0x83);

        try
        {
            using DataControlClient _dc = CreateClient(workingRoot);

            BinaryItem segmentedItem = await CreateClientBinaryItem(segmentedFilePath,segmentedPayload);

            await using FileStream streamSource = new(streamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await streamSource.WriteAsync(streamPayload);
            streamSource.Position = 0;

            DataStreamItem streamItem = new(streamSource);

            DataControlUploadSession? segmentedSession = await _dc.OpenUpload(segmentedItem,this.WriteToken);
            DataControlUploadSession? streamSession = await _dc.OpenUpload(streamItem,this.WriteToken);

            Check.That(segmentedSession).IsNotNull();
            Check.That(segmentedSession!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
            Check.That(segmentedSession.Session.State.Mode).IsEqualTo(DataItemTransferMode.Upload);
            Check.That(Directory.Exists(segmentedSession.Session.WorkingDirectory)).IsTrue();

            Check.That(streamSession).IsNotNull();
            Check.That(streamSession!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(streamSession.Session.StreamState).IsNotNull();
            Check.That(streamSession.Session.StreamState!.Mode).IsEqualTo(DataItemTransferMode.StreamUpload);
            Check.That(Directory.Exists(streamSession.Session.WorkingDirectory)).IsTrue();

            Check.That(await segmentedSession.Abort()).IsTrue();
            Check.That(await streamSession.Abort()).IsTrue();
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientDownloadToCurrentReturnsWhenOpenSegmentedFollowHasNoMoreCurrentlyAvailableRanges()
    {
        String uploaderWorkingRoot = CreateClientWorkingRoot();
        String consumerWorkingRoot = CreateClientWorkingRoot();
        String segmentedFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.SegmentedFollowCurrent.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(1024 * 1024,0x91);

        try
        {
            using DataControlClient uploader = CreateClient(uploaderWorkingRoot);
            using DataControlClient consumer = CreateClient(consumerWorkingRoot);

            BinaryItem item = await CreateClientBinaryItem(segmentedFilePath,payload);

            DataControlUploadSession? sourceUpload = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(sourceUpload).IsNotNull();
            Check.That(await sourceUpload!.UploadNext()).IsTrue();
            Check.That(sourceUpload.Session.State.RealizedBytes).IsStrictlyGreaterThan(0);

            DataControlDownloadSession? follower = await consumer.OpenDownload(item.GetID()!.Value,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StagedFollow,
                SourceSessionID = sourceUpload.Session.SessionId,
            });

            Check.That(follower).IsNotNull();
            Check.That(follower!.Session.State.Mode).IsEqualTo(DataItemTransferMode.StagedRead);

            Int32 downloaded = await follower.DownloadToCurrent();

            Check.That(downloaded).IsStrictlyGreaterThan(0);
            Check.That(follower.Session.State.RealizedBytes).IsEqualTo(sourceUpload.Session.State.RealizedBytes);
            Check.That(follower.Session.State.Status).IsNotEqualTo(DataItemTransferStatus.Published);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            DeleteWorkingDirectory(uploaderWorkingRoot);
            DeleteWorkingDirectory(consumerWorkingRoot);
        }
    }

    [Test]
    public async Task ClientDownloadToCompletionContinuesWaitingAcrossFutureStreamFollowUpdates()
    {
        String workingRoot = CreateClientWorkingRoot();
        String followerRoot = CreateClientWorkingRoot();
        Guid? publishedItemId = null;
        Boolean published = false;

        try
        {
            using DataControlClient uploader = CreateClient(workingRoot);
            using DataControlClient followerClient = CreateClient(followerRoot);
            using ControllableProducerStream producer = new();

            DataStreamItem item = new(producer);
            Guid itemId = item.GetID()!.Value;
            publishedItemId = itemId;

            DataControlUploadSession? uploadSession = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(uploadSession).IsNotNull();
            Check.That(uploadSession!.Session.StreamState).IsNotNull();

            Int32 chunkLength = ResolveLiveFollowChunkLength(uploadSession.Session.StreamState!);
            Byte[] firstSegment = CreateSegmentedTransferPayload(chunkLength,0x98);
            Byte[] secondSegment = CreateSegmentedTransferPayload(chunkLength,0x99);

            DataControlDownloadSession? follower = await followerClient.OpenDownload(itemId,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = uploadSession.Session.SessionId,
                BindLiveStreamItem = true,
            });

            Check.That(follower).IsNotNull();

            await producer.PublishAsync(firstSegment);
            Check.That(await uploadSession.UploadNext()).IsTrue();

            Task<Int32> completionTask = follower!.DownloadToCompletion();

            Check.That(await RemainsIncompleteAsync(completionTask)).IsTrue();

            await producer.PublishAsync(secondSegment);
            Check.That(await uploadSession.UploadNext()).IsTrue();

            Check.That(await RemainsIncompleteAsync(completionTask)).IsTrue();

            producer.Complete();
            Check.That(await uploadSession.UploadToCompletion()).IsGreaterOrEqualThan(0);
            Check.That(await uploadSession.Publish()).IsNotNull();
            published = true;

            Check.That(await completionTask).IsStrictlyGreaterThan(0);

            DataControlDownloadCompletionResult? materialized = await follower.Materialize();

            Check.That(materialized).IsNotNull();
            Check.That(new FileInfo(materialized!.StreamPath).Length).IsEqualTo((Int64)firstSegment.Length + secondSegment.Length);
        }
        finally
        {
            if(published && publishedItemId.HasValue)
            {
                try
                {
                    using DataControlClient cleanupClient = CreateClient(workingRoot);
                    _ = await cleanupClient.Delete(publishedItemId.Value,this.WriteToken);
                }
                catch { }
            }

            DeleteWorkingDirectory(workingRoot);
            DeleteWorkingDirectory(followerRoot);
        }
    }

    [Test]
    public async Task ClientUploadAndDownloadHelpersRoundTripAcrossFamilies()
    {
        String workingRoot = CreateClientWorkingRoot();
        String segmentedFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.SegmentedRoundTrip.{Guid.NewGuid():N}.bin");
        String streamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamRoundTrip.{Guid.NewGuid():N}.bin");
        Byte[] segmentedPayload = CreateSegmentedTransferPayload(1024 * 1024,0x84);
        Byte[] streamPayload = CreateSegmentedTransferPayload(1024 * 1024,0x85);
        Guid? segmentedItemId = null;
        Guid? streamItemId = null;

        try
        {
            using DataControlClient _dc = CreateClient(workingRoot);

            BinaryItem segmentedItem = await CreateClientBinaryItem(segmentedFilePath,segmentedPayload);
            segmentedItemId = segmentedItem.GetID();

            DataControlUploadCompletionResult? segmentedUpload = await _dc.Upload(segmentedItem,this.WriteToken);

            Check.That(segmentedUpload).IsNotNull();
            Check.That(segmentedUpload!.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
            Check.That(segmentedUpload.Segmented!.Published).IsTrue();
            Check.That(segmentedUpload.Session.State.Status).IsEqualTo(DataItemTransferStatus.Published);

            DataControlDownloadCompletionResult? segmentedDownload = await _dc.Download(segmentedItemId!.Value,this.ReadToken);

            Check.That(segmentedDownload).IsNotNull();
            Check.That(segmentedDownload!.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
            Check.That(segmentedDownload.Item).IsInstanceOf<BinaryItem>();
            Check.That(File.Exists(segmentedDownload.StreamPath)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(segmentedDownload.StreamPath)).SequenceEqual(segmentedPayload)).IsTrue();

            await using FileStream streamSource = new(streamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await streamSource.WriteAsync(streamPayload);
            streamSource.Position = 0;

            DataStreamItem streamItem = new(streamSource);
            streamItemId = streamItem.GetID();

            DataControlUploadCompletionResult? streamUpload = await _dc.Upload(streamItem,this.WriteToken);

            Check.That(streamUpload).IsNotNull();
            Check.That(streamUpload!.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(streamUpload.Stream).IsNotNull();
            Check.That(streamUpload.Stream!.Published).IsTrue();
            Check.That(streamUpload.Session.StreamState).IsNotNull();
            Check.That(streamUpload.Session.StreamState!.Status).IsEqualTo(DataItemTransferStatus.Published);

            DataControlDownloadCompletionResult? streamDownload = await _dc.Download(streamItemId!.Value,this.ReadToken);

            Check.That(streamDownload).IsNotNull();
            Check.That(streamDownload!.Item).IsInstanceOf<DataStreamItem>();
            Check.That(File.Exists(streamDownload.StreamPath)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(streamDownload.StreamPath)).SequenceEqual(streamPayload)).IsTrue();

            DataControlResponse<Guid> deletedSegmented = await _dc.Delete(segmentedItemId,this.WriteToken);
            Check.That((Int32)deletedSegmented.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            DataControlResponse<Guid> deletedStream = await _dc.Delete(streamItemId,this.WriteToken);
            Check.That((Int32)deletedStream.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientOpenDownloadSupportsPublishedAndStreamFollowModes()
    {
        String workingRoot = CreateClientWorkingRoot();
        String followStreamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamFollowModes.{Guid.NewGuid():N}.bin");
        String publishedStreamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamPublishedModes.{Guid.NewGuid():N}.bin");
        Byte[] followPayload = CreateSegmentedTransferPayload(512 * 1024,0x86);
        Byte[] publishedPayload = CreateSegmentedTransferPayload(512 * 1024,0x88);
        Guid? followItemId = null;
        Guid? publishedItemId = null;

        try
        {
            using DataControlClient _dc = CreateClient(workingRoot);

            await using FileStream followStreamSource = new(followStreamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await followStreamSource.WriteAsync(followPayload);
            followStreamSource.Position = 0;

            DataStreamItem followItem = new(followStreamSource);
            followItemId = followItem.GetID();

            DataControlUploadSession? uploadSession = await _dc.OpenUpload(followItem,this.WriteToken,new DataControlUploadOptions(){ MaxBytes = followPayload.Length / 2L });

            Check.That(uploadSession).IsNotNull();
            Check.That(uploadSession!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);

            Int32 partiallyUploaded = await uploadSession.UploadToCompletion();

            Check.That(partiallyUploaded > 0).IsTrue();
            Check.That(uploadSession.Session.StreamState).IsNotNull();
            Check.That(uploadSession.Session.StreamState!.AppendedLength > 0).IsTrue();
            Check.That(uploadSession.Session.StreamState.AppendedLength < followPayload.LongLength).IsTrue();

            DataControlDownloadSession? follower = await _dc.OpenDownload(followItemId!.Value,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = uploadSession.Session.SessionId,
            });

            Check.That(follower).IsNotNull();
            Check.That(follower!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(follower.Session.StreamState).IsNotNull();
            Check.That(follower.Session.StreamState!.AppendedLength).IsEqualTo(0);
            Check.That(follower.Session.StreamState!.Mode).IsEqualTo(DataItemTransferMode.StreamFollow);
            Check.That(follower.Session.StreamState.SourceSessionID).IsEqualTo(uploadSession.Session.SessionId);
            Check.That(await follower.Materialize()).IsNull();
            Check.That(await follower.DownloadToCurrent()).IsGreaterOrEqualThan(0);
            Check.That(follower.Session.StreamState!.AppendedLength).IsEqualTo(uploadSession.Session.StreamState.AppendedLength);

            Check.That(await uploadSession.Abort()).IsTrue();

            await using FileStream publishedStreamSource = new(publishedStreamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await publishedStreamSource.WriteAsync(publishedPayload);
            publishedStreamSource.Position = 0;

            DataStreamItem publishedItem = new(publishedStreamSource);
            publishedItemId = publishedItem.GetID();

            DataControlUploadCompletionResult? publishedUpload = await _dc.Upload(publishedItem,this.WriteToken);

            Check.That(publishedUpload).IsNotNull();
            Check.That(publishedUpload!.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(publishedUpload.Stream).IsNotNull();
            Check.That(publishedUpload.Stream!.Published).IsTrue();

            DataControlDownloadSession? publishedDownload = await _dc.OpenDownload(publishedItemId!.Value,this.ReadToken);

            Check.That(publishedDownload).IsNotNull();
            Check.That(publishedDownload!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
            Check.That(publishedDownload.Session.State.Mode).IsEqualTo(DataItemTransferMode.PublishedRead);

            DataControlResponse<Guid> deletedPublished = await _dc.Delete(publishedItemId,this.WriteToken);
            Check.That((Int32)deletedPublished.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(followStreamFilePath)) { File.Delete(followStreamFilePath); }
            if(File.Exists(publishedStreamFilePath)) { File.Delete(publishedStreamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientStreamFollowCanOpenFromNewClientUsingSameWorkingDirectoryRoot()
    {
        String workingRoot = CreateClientWorkingRoot();
        String streamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamResume.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(256 * 1024,0x87);
        Guid? itemId = null;
        Guid sourceSessionId = Guid.Empty;
        Int64 appendedLength = 0;

        try
        {
            using DataControlClient uploader = CreateClient(workingRoot);

            await using FileStream streamSource = new(streamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await streamSource.WriteAsync(payload);
            streamSource.Position = 0;

            DataStreamItem item = new(streamSource);
            itemId = item.GetID();

            DataControlUploadSession? uploadSession = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(uploadSession).IsNotNull();
            Int32 uploaded = await uploadSession!.UploadToCompletion();

            Check.That(uploaded > 0).IsTrue();
            Check.That(uploadSession.Session.StreamState).IsNotNull();

            sourceSessionId = uploadSession.Session.SessionId;
            appendedLength = uploadSession.Session.StreamState!.AppendedLength;
            Check.That(appendedLength).IsEqualTo(payload.LongLength);

            using DataControlClient resumedClient = CreateClient(workingRoot);

            IReadOnlyList<DataControlTransferSessionInfo> localSessions = await resumedClient.GetLocalSessions();

            Check.That(localSessions.Any(session => session.SessionId == uploadSession.Session.SessionId)).IsTrue();

            DataControlDownloadSession? follower = await resumedClient.OpenDownload(itemId!.Value,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = sourceSessionId,
                BindLiveStreamItem = true,
            });

            Check.That(follower).IsNotNull();
            Check.That(follower!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(follower.Session.SupportsLiveFollowBinding).IsTrue();
            Check.That(follower.Session.StreamState).IsNotNull();
            Check.That(follower.Session.StreamState!.SourceSessionID).IsEqualTo(sourceSessionId);
            Check.That(follower.Session.StreamState.AppendedLength).IsEqualTo(0);
            Check.That(follower.LiveItem).IsNotNull();
            Check.That(follower.LiveItem!.GetContentBindingMode()).IsEqualTo(DataStreamItemContentBindingMode.LiveSessionManaged);
            Check.That(follower.LiveItem.GetLiveContentStatus()).IsEqualTo(DataStreamItemLiveContentStatus.Open);

            Check.That(await follower.DownloadToCurrent()).IsGreaterOrEqualThan(0);
            Check.That(follower.Session.StreamState!.AppendedLength).IsEqualTo(appendedLength);

            DataControlDownloadCompletionResult? materialized = await follower.Materialize();

            Check.That(materialized).IsNotNull();
            Check.That(materialized!.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(materialized.Item).IsInstanceOf<DataStreamItem>();
            Check.That(materialized.LiveItem).IsSameReferenceAs(follower.LiveItem);
            Check.That((await File.ReadAllBytesAsync(materialized.StreamPath)).SequenceEqual(payload.AsSpan()[..checked((Int32)appendedLength)])).IsTrue();

            Check.That(await uploadSession.Abort()).IsTrue();
        }
        finally
        {
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientStreamFollowCanOpenFromNewClientUsingDifferentWorkingDirectoryRoot()
    {
        String uploaderWorkingRoot = CreateClientWorkingRoot();
        String followerWorkingRoot = CreateClientWorkingRoot();
        String streamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamFollowSeparateRoot.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(256 * 1024,0x8A);
        Guid? itemId = null;
        Guid sourceSessionId = Guid.Empty;
        Int64 appendedLength = 0;

        try
        {
            using DataControlClient uploader = CreateClient(uploaderWorkingRoot);

            await using FileStream streamSource = new(streamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await streamSource.WriteAsync(payload);
            streamSource.Position = 0;

            DataStreamItem item = new(streamSource);
            itemId = item.GetID();

            DataControlUploadSession? uploadSession = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(uploadSession).IsNotNull();
            Int32 uploaded = await uploadSession!.UploadToCompletion();

            Check.That(uploaded > 0).IsTrue();
            Check.That(uploadSession.Session.StreamState).IsNotNull();

            sourceSessionId = uploadSession.Session.SessionId;
            appendedLength = uploadSession.Session.StreamState!.AppendedLength;
            Check.That(appendedLength).IsEqualTo(payload.LongLength);

            using DataControlClient followerClient = CreateClient(followerWorkingRoot);

            IReadOnlyList<DataControlTransferSessionInfo> followerLocalSessions = await followerClient.GetLocalSessions();

            Check.That(followerLocalSessions.Any()).IsFalse();

            DataControlDownloadSession? follower = await followerClient.OpenDownload(itemId!.Value,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = sourceSessionId,
                BindLiveStreamItem = true,
            });

            Check.That(follower).IsNotNull();
            Check.That(follower!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(follower.Session.SupportsLiveFollowBinding).IsTrue();
            Check.That(follower.Session.StreamState).IsNotNull();
            Check.That(follower.Session.StreamState!.SourceSessionID).IsEqualTo(sourceSessionId);
            Check.That(follower.Session.StreamState.AppendedLength).IsEqualTo(0);
            Check.That(follower.LiveItem).IsNotNull();
            Check.That(follower.LiveItem!.GetContentBindingMode()).IsEqualTo(DataStreamItemContentBindingMode.LiveSessionManaged);
            Check.That(follower.LiveItem.GetLiveContentStatus()).IsEqualTo(DataStreamItemLiveContentStatus.Open);

            Check.That(await follower.DownloadToCurrent()).IsGreaterOrEqualThan(0);
            Check.That(follower.Session.StreamState!.AppendedLength).IsEqualTo(appendedLength);

            DataControlDownloadCompletionResult? materialized = await follower.Materialize();

            Check.That(materialized).IsNotNull();
            Check.That(materialized!.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(materialized.Item).IsInstanceOf<DataStreamItem>();
            Check.That(materialized.LiveItem).IsSameReferenceAs(follower.LiveItem);
            Check.That((await File.ReadAllBytesAsync(materialized.StreamPath)).SequenceEqual(payload.AsSpan()[..checked((Int32)appendedLength)])).IsTrue();

            Check.That(await uploadSession.Abort()).IsTrue();
        }
        finally
        {
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(uploaderWorkingRoot);
            DeleteWorkingDirectory(followerWorkingRoot);
        }
    }

    [Test]
    public async Task ClientCanResumeSegmentedDownloadFromLocalSessionInventory()
    {
        String workingRoot = CreateClientWorkingRoot();
        String segmentedFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.ResumeSegmented.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(256 * 1024,0x89);
        Guid? itemId = null;
        Guid downloadSessionId = Guid.Empty;

        try
        {
            using DataControlClient uploader = CreateClient(workingRoot);

            BinaryItem item = await CreateClientBinaryItem(segmentedFilePath,payload);
            itemId = item.GetID();

            DataControlUploadCompletionResult? uploaded = await uploader.Upload(item,this.WriteToken);

            Check.That(uploaded).IsNotNull();

            DataControlDownloadSession? opened = await uploader.OpenDownload(itemId!.Value,this.ReadToken);

            Check.That(opened).IsNotNull();

            downloadSessionId = opened!.Session.SessionId;

            using DataControlClient resumedClient = CreateClient(workingRoot);

            DataControlTransferSessionInfo? local = await resumedClient.GetLocalSession(downloadSessionId);

            Check.That(local).IsNotNull();
            Check.That(local!.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);

            DataControlDownloadSession? resumed = await resumedClient.ResumeDownload(downloadSessionId,this.ReadToken,new DataControlDownloadOptions());

            Check.That(resumed).IsNotNull();
            Check.That(resumed!.Session.ItemId).IsEqualTo(itemId!.Value);

            Int32 downloaded = await resumed.DownloadToCompletion();

            Check.That(downloaded >= 0).IsTrue();

            DataControlDownloadCompletionResult? materialized = await resumed.Materialize();

            Check.That(materialized).IsNotNull();
            Check.That(materialized!.Item).IsInstanceOf<BinaryItem>();
            Check.That(materialized.StreamPath.StartsWith(local!.WorkingDirectory,StringComparison.OrdinalIgnoreCase)).IsTrue();

            DataControlDownloadCompletionResult? repeated = await resumed.Materialize();

            Check.That(repeated).IsNotNull();
            Check.That(repeated!.StreamPath).IsEqualTo(materialized.StreamPath);

            using(DataControlClient recoveryClient = CreateClient(workingRoot))
            {
                DataControlDownloadSession? recovered = await recoveryClient.ResumeDownload(downloadSessionId,this.ReadToken,new DataControlDownloadOptions());

                Check.That(recovered).IsNotNull();
                Check.That(await recovered!.DownloadToCompletion()).IsGreaterOrEqualThan(0);

                DataControlDownloadCompletionResult? recoveredMaterialization = await recovered.Materialize();

                Check.That(recoveredMaterialization).IsNotNull();
                Check.That(recoveredMaterialization!.StreamPath).IsEqualTo(materialized.StreamPath);
                Check.That((await File.ReadAllBytesAsync(recoveredMaterialization.StreamPath)).SequenceEqual(payload)).IsTrue();
            }

            DataControlResponse<Guid> deleted = await resumedClient.Delete(itemId,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientPublishedReadResumeReusesCompletedLocalRanges()
    {
        String workingRoot = CreateClientWorkingRoot();
        String sourcePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.ResumePublishedRead.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(10 * 1024 * 1024,0xA7);
        Guid itemId = Guid.Empty;
        Guid sessionId = Guid.Empty;

        try
        {
            using DataControlClient client = CreateClient(workingRoot);
            BinaryItem item = await CreateClientBinaryItem(sourcePath,payload);
            itemId = item.GetID()!.Value;
            Check.That(await client.Upload(item,this.WriteToken)).IsNotNull();

            DataControlDownloadSession? download = await client.OpenDownload(itemId,this.ReadToken,new DataControlDownloadOptions()
            {
                DownloadRequestSizeBytes = 1024 * 1024,
            });

            Check.That(download).IsNotNull();
            sessionId = download!.Session.SessionId;
            Check.That(await download.DownloadNext()).IsTrue();
            Int64 completedBeforeRestart = download.Session.State.RealizedBytes;
            Check.That(completedBeforeRestart).IsEqualTo(1024 * 1024);

            DataControlResponse<GetTransferStateResponse> serverState = await client.GetTransferState(new DataItemTransferIdentity()
            {
                SessionID = sessionId,
                ItemID = itemId,
            },this.ReadToken);

            Check.That((Int32)serverState.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(serverState.Data!.State.RealizedBytes).IsEqualTo(0);

            using DataControlClient resumedClient = CreateClient(workingRoot);
            DataControlTransferSessionInfo? checkpoint = await resumedClient.GetLocalSession(sessionId);
            Check.That(checkpoint).IsNotNull();
            Check.That(checkpoint!.State.RealizedBytes).IsEqualTo(completedBeforeRestart);

            DataControlDownloadSession? resumed = await resumedClient.ResumeDownload(sessionId,this.ReadToken,new DataControlDownloadOptions()
            {
                DownloadRequestSizeBytes = 1024 * 1024,
            });

            Check.That(resumed).IsNotNull();
            Check.That(resumed!.Session.SessionId).IsEqualTo(sessionId);
            Check.That(resumed.Session.State.RealizedBytes).IsEqualTo(completedBeforeRestart);
            Check.That(await resumed.DownloadNext()).IsTrue();
            Check.That(resumed.Session.State.RealizedBytes).IsEqualTo(completedBeforeRestart + 1024 * 1024);

            Check.That(await resumed.DownloadToCompletion()).IsGreaterOrEqualThan(0);
            DataControlDownloadCompletionResult? materialized = await resumed.Materialize();
            Check.That(materialized).IsNotNull();
            Check.That((await File.ReadAllBytesAsync(materialized!.StreamPath)).SequenceEqual(payload)).IsTrue();

            Check.That((Int32)(await resumedClient.Delete(itemId,this.WriteToken)).StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(sourcePath)) { File.Delete(sourcePath); }
            if(itemId != Guid.Empty)
            {
                using DataControlClient cleanup = CreateClient(workingRoot);
                _ = await cleanup.Delete(itemId,this.WriteToken);
            }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientCanResumeSegmentedFollowDownloadFromLocalSessionInventory()
    {
        String workingRoot = CreateClientWorkingRoot();
        String segmentedFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.ResumeStagedFollow.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(10 * 1024 * 1024,0x8D);

        try
        {
            using DataControlClient client = CreateClient(workingRoot);

            BinaryItem item = await CreateClientBinaryItem(segmentedFilePath,payload);

            DataControlUploadSession? sourceUpload = await client.OpenUpload(item,this.WriteToken);

            Check.That(sourceUpload).IsNotNull();
            Check.That(await sourceUpload!.UploadNext()).IsTrue();
            Check.That(sourceUpload.Session.State.RealizedBytes > 0).IsTrue();
            Check.That(sourceUpload.Session.State.RealizedBytes < payload.LongLength).IsTrue();

            DataControlDownloadSession? follower = await client.OpenDownload(item.GetID()!.Value,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StagedFollow,
                SourceSessionID = sourceUpload.Session.SessionId,
            });

            Check.That(follower).IsNotNull();
            Check.That(follower!.Session.State.Mode).IsEqualTo(DataItemTransferMode.StagedRead);
            Check.That(follower.Session.State.SourceSessionID).IsEqualTo(sourceUpload.Session.SessionId);

            using DataControlClient resumedClient = CreateClient(workingRoot);

            DataControlDownloadSession? resumed = await resumedClient.ResumeDownload(follower.Session.SessionId,this.ReadToken,new DataControlDownloadOptions());

            Check.That(resumed).IsNotNull();
            Check.That(resumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.StagedRead);
            Check.That(resumed.Session.State.SourceSessionID).IsEqualTo(sourceUpload.Session.SessionId);
            Check.That(await resumed.DownloadNext()).IsTrue();
            Check.That(resumed.Session.State.RealizedBytes).IsStrictlyGreaterThan(0);

            Check.That(await sourceUpload.UploadToCompletion()).IsStrictlyGreaterThan(0);
            Check.That(await sourceUpload.Publish()).IsNotNull();

            using DataControlClient publishedResumeClient = CreateClient(workingRoot);

            DataControlDownloadSession? publishedResumed = await publishedResumeClient.ResumeDownload(follower.Session.SessionId,this.ReadToken,new DataControlDownloadOptions());

            Check.That(publishedResumed).IsNotNull();
            Check.That(publishedResumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.PublishedRead);
            Check.That(publishedResumed.Session.State.SourceSessionID).IsNull();
            Check.That(publishedResumed.Session.State.RealizedBytes).IsStrictlyGreaterThan(0);

            Int32 downloaded = await publishedResumed.DownloadToCompletion();

            Check.That(downloaded >= 0).IsTrue();

            DataControlDownloadCompletionResult? materialized = await publishedResumed.Materialize();

            Check.That(materialized).IsNotNull();
            Check.That(materialized!.Item).IsInstanceOf<BinaryItem>();
            Check.That((await File.ReadAllBytesAsync(materialized.StreamPath)).SequenceEqual(payload)).IsTrue();

            DataControlResponse<Guid> deleted = await client.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientDownloadCancellationReportsSessionAndRetainsSegmentedSessionForResume()
    {
        String workingRoot = CreateClientWorkingRoot();
        String sourcePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.CancelDownloadSegmented.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(10 * 1024 * 1024,0xA4);
        Guid itemId = Guid.Empty;
        Guid sessionId = Guid.Empty;

        try
        {
            using DataControlClient client = CreateClient(workingRoot);
            BinaryItem item = await CreateClientBinaryItem(sourcePath,payload);
            itemId = item.GetID()!.Value;
            Check.That(await client.Upload(item,this.WriteToken)).IsNotNull();

            using CancellationTokenSource cancel = new();
            TransferProgressCollector progress = new(update =>
            {
                if(update.LatestRange is not null) { cancel.Cancel(); }
            });

            Check.ThatAsyncCode(async () => await client.Download(itemId,this.ReadToken,progress:progress,cancel:cancel.Token))
                .Throws<OperationCanceledException>();

            Check.That(progress.Updates.Count).IsGreaterOrEqualThan(2);
            Check.That(progress.Updates[0].SessionID).IsNotEqualTo(Guid.Empty);
            Check.That(progress.Updates[0].LatestRange).IsNull();
            Check.That(progress.Updates[1].LatestRange).IsNotNull();
            sessionId = progress.Updates[0].SessionID;

            DataControlTransferSessionInfo? checkpoint = await client.GetLocalSession(sessionId);
            Check.That(checkpoint).IsNotNull();
            Check.That(checkpoint!.State.RealizedBytes).IsStrictlyGreaterThan(0);
            Check.That(checkpoint.State.MissingRanges.Length).IsStrictlyGreaterThan(0);

            DataControlDownloadSession? resumed = await client.ResumeDownload(sessionId,this.ReadToken);
            Check.That(resumed).IsNotNull();
            Check.That(await resumed!.DownloadToCompletion()).IsGreaterOrEqualThan(0);
            DataControlDownloadCompletionResult? materialized = await resumed.Materialize();
            Check.That(materialized).IsNotNull();
            Check.That((await File.ReadAllBytesAsync(materialized!.StreamPath)).SequenceEqual(payload)).IsTrue();

            Check.That((Int32)(await client.Delete(itemId,this.WriteToken)).StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(sourcePath)) { File.Delete(sourcePath); }
            if(itemId != Guid.Empty)
            {
                using DataControlClient cleanup = CreateClient(workingRoot);
                _ = await cleanup.Delete(itemId,this.WriteToken);
            }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientDownloadCancellationRetainsActualStreamPrefixForResume()
    {
        String uploaderRoot = CreateClientWorkingRoot();
        String followerRoot = CreateClientWorkingRoot();
        String sourcePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.CancelDownloadStream.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(10 * 1024 * 1024,0xA5);
        Guid itemId = Guid.Empty;
        Guid followerSessionId = Guid.Empty;
        Int64 downloadedBytes = 0;

        try
        {
            using DataControlClient uploader = CreateClient(uploaderRoot);
            await using FileStream source = new(sourcePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await source.WriteAsync(payload);
            source.Position = 0;

            DataStreamItem item = new(source);
            itemId = item.GetID()!.Value;
            DataControlUploadSession? upload = await uploader.OpenUpload(item,this.WriteToken);
            Check.That(upload).IsNotNull();
            Check.That(await upload!.UploadToCompletion()).IsStrictlyGreaterThan(0);

            using DataControlClient consumer = CreateClient(followerRoot);
            TransferProgressCollector progress = new();
            using CancellationTokenSource cancel = new();

            Task<DataControlDownloadCompletionResult?> downloadTask = consumer.Download(itemId,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = upload.Session.SessionId,
            },progress,cancel.Token);

            DataItemTransferProgress initial = await progress.FirstUpdate.WaitAsync(TimeSpan.FromSeconds(10));
            Check.That(initial.LatestRange).IsNull();
            Check.That(initial.RealizedBytes).IsEqualTo(0);
            followerSessionId = initial.SessionID;

            DataItemTransferProgress appended = await WaitForProgressAsync(progress,
                update => update.TransferFamily is DataControlTransferFamily.Stream && update.RealizedBytes > 0,
                TimeSpan.FromSeconds(10));

            downloadedBytes = appended.RealizedBytes;
            Check.That(appended.LatestAppendedLength).IsEqualTo(upload.Session.StreamState!.AppendedLength);
            cancel.Cancel();

            Check.ThatAsyncCode(async () => await downloadTask).Throws<OperationCanceledException>();

            DataControlTransferSessionInfo? checkpoint = await consumer.GetLocalSession(followerSessionId);
            Check.That(checkpoint).IsNotNull();
            Check.That(checkpoint!.StreamState!.AppendedLength).IsEqualTo(downloadedBytes);
            Check.That(new FileInfo(Path.Combine(checkpoint.WorkingDirectory,DataStreamTransferManifest.StreamFileName)).Length)
                .IsEqualTo(downloadedBytes);
            Check.That(progress.Updates.Any(update => update.SessionID == followerSessionId && update.RealizedBytes == downloadedBytes)).IsTrue();

            DataControlDownloadSession? resumed = await consumer.ResumeDownload(followerSessionId,this.ReadToken,new DataControlDownloadOptions()
            {
                BindLiveStreamItem = true,
            });
            Check.That(resumed).IsNotNull();
            Check.That(resumed!.Session.StreamState!.AppendedLength).IsEqualTo(downloadedBytes);
            Check.That(await resumed.DownloadToCurrent()).IsGreaterOrEqualThan(0);
            DataControlDownloadCompletionResult? result = await resumed.Materialize();
            Check.That(result).IsNotNull();
            Check.That((await File.ReadAllBytesAsync(result!.StreamPath)).SequenceEqual(payload.AsSpan()[..checked((Int32)upload.Session.StreamState!.AppendedLength)])).IsTrue();

            Check.That(await upload.Abort()).IsTrue();
        }
        finally
        {
            if(File.Exists(sourcePath)) { File.Delete(sourcePath); }
            DeleteWorkingDirectory(uploaderRoot);
            DeleteWorkingDirectory(followerRoot);
        }
    }

    private static async Task<DataItemTransferProgress> WaitForProgressAsync(TransferProgressCollector progress , Func<DataItemTransferProgress,Boolean> predicate , TimeSpan timeout)
    {
        DateTimeOffset deadline = DateTimeOffset.UtcNow + timeout;

        while(DateTimeOffset.UtcNow < deadline)
        {
            DataItemTransferProgress? match = progress.Updates.FirstOrDefault(predicate);

            if(match is not null) { return match; }

            await Task.Delay(10);
        }

        throw new TimeoutException("Expected download progress was not reported before the timeout.");
    }

    [Test]
    public async Task ClientCanResumeStreamFollowDownloadWithLiveBinding()
    {
        String workingRoot = CreateClientWorkingRoot();
        String streamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.ResumeStreamFollow.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(256 * 1024,0x8E);

        try
        {
            using DataControlClient client = CreateClient(workingRoot);

            await using FileStream streamSource = new(streamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await streamSource.WriteAsync(payload);
            streamSource.Position = 0;

            DataStreamItem item = new(streamSource);

            DataControlUploadSession? sourceUpload = await client.OpenUpload(item,this.WriteToken);

            Check.That(sourceUpload).IsNotNull();
            Check.That(await sourceUpload!.UploadToCompletion()).IsStrictlyGreaterThan(0);

            DataControlDownloadSession? follower = await client.OpenDownload(item.GetID()!.Value,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = sourceUpload.Session.SessionId,
            });

            Check.That(follower).IsNotNull();
            Check.That(follower!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(follower.Session.StreamState).IsNotNull();

            using DataControlClient resumedClient = CreateClient(workingRoot);

            DataControlDownloadSession? resumed = await resumedClient.ResumeDownload(follower.Session.SessionId,this.ReadToken,new DataControlDownloadOptions()
            {
                BindLiveStreamItem = true,
            });

            Check.That(resumed).IsNotNull();
            Check.That(resumed!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(resumed.Session.SupportsLiveFollowBinding).IsTrue();
            Check.That(resumed.LiveItem).IsNotNull();
            Check.That(resumed.LiveItem!.GetContentBindingMode()).IsEqualTo(DataStreamItemContentBindingMode.LiveSessionManaged);

            Check.That(await resumed.DownloadToCurrent()).IsGreaterOrEqualThan(0);

            DataControlDownloadCompletionResult? materialized = await resumed.Materialize();

            Check.That(materialized).IsNotNull();
            Check.That(materialized!.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(materialized.Item).IsInstanceOf<DataStreamItem>();
            Check.That(materialized.LiveItem).IsSameReferenceAs(resumed.LiveItem);
            Check.That((await File.ReadAllBytesAsync(materialized.StreamPath)).SequenceEqual(payload)).IsTrue();

            Check.That(await sourceUpload.Abort()).IsTrue();
        }
        finally
        {
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientCanResumeSegmentedUploadFromStableFileSource()
    {
        String workingRoot = CreateClientWorkingRoot();
        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.ResumeUploadStable.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(10 * 1024 * 1024,0x8B);

        try
        {
            using DataControlClient uploader = CreateClient(workingRoot);

            BinaryItem item = await CreateClientBinaryItem(filePath,payload);

            DataControlUploadSession? opened = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(opened).IsNotNull();
            Check.That(opened!.Session.UploadSource).IsNotNull();
            Check.That(opened.Session.UploadSource!.Kind).IsEqualTo(DataControlTransferSourceKind.ExternalFile);
            Check.That(opened.Session.UploadSource.Path).IsEqualTo(Path.GetFullPath(filePath));

            Check.That(await opened.UploadNext()).IsTrue();
            Check.That(opened.Session.State.RealizedBytes > 0).IsTrue();
            Check.That(opened.Session.State.RealizedBytes < payload.LongLength).IsTrue();

            using DataControlClient resumedClient = CreateClient(workingRoot);

            DataControlUploadSession? resumed = await resumedClient.ResumeUpload(opened.Session.SessionId,this.WriteToken);

            Check.That(resumed).IsNotNull();
            Check.That(resumed!.Session.UploadSource).IsNotNull();
            Check.That(resumed.Session.UploadSource!.Kind).IsEqualTo(DataControlTransferSourceKind.ExternalFile);

            Int32 resumedUploaded = await resumed.UploadToCompletion();

            Check.That(resumedUploaded > 0).IsTrue();

            DataControlUploadCompletionResult? published = await resumed.Publish();

            Check.That(published).IsNotNull();
            Check.That(published!.Segmented!.Published).IsTrue();

            DataControlResponse<Guid> deleted = await resumedClient.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientResumeSegmentedUploadFailsWhenExternalFileIsGone()
    {
        String workingRoot = CreateClientWorkingRoot();
        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.ResumeUploadMissingFile.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(512 * 1024,0x8C);

        try
        {
            using DataControlClient uploader = CreateClient(workingRoot);

            BinaryItem item = await CreateClientBinaryItem(filePath,payload);

            DataControlUploadSession? opened = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(opened).IsNotNull();
            Check.That(opened!.Session.UploadSource).IsNotNull();
            Check.That(opened.Session.UploadSource!.Kind).IsEqualTo(DataControlTransferSourceKind.ExternalFile);

            Check.That(await opened.UploadNext()).IsTrue();

            String itemPath = item.GetFILE()!;

            if(File.Exists(itemPath)) { File.Delete(itemPath); }

            using DataControlClient resumedClient = CreateClient(workingRoot);

            DataControlUploadSession? resumed = await resumedClient.ResumeUpload(opened.Session.SessionId,this.WriteToken);

            Check.That(resumed).IsNull();
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientCanResumeStreamFollowAcrossSeparateConsumerRestartsUsingLocalInventory()
    {
        String uploaderWorkingRoot = CreateClientWorkingRoot();
        String consumerWorkingRoot = CreateClientWorkingRoot();
        String streamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamFollowResumeSeparateRoots.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(10 * 1024 * 1024,0x8F);
        Guid? itemId = null;
        Guid followerSessionId = Guid.Empty;
        Int64 firstConsumerLength = 0;
        Int64 secondConsumerLength = 0;

        try
        {
            using DataControlClient uploader = CreateClient(uploaderWorkingRoot);

            await using FileStream streamSource = new(streamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await streamSource.WriteAsync(payload);
            streamSource.Position = 0;

            DataStreamItem item = new(streamSource);
            itemId = item.GetID();

            DataControlUploadSession? sourceUpload = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(sourceUpload).IsNotNull();
            Check.That(await sourceUpload!.UploadNext()).IsTrue();
            Check.That(sourceUpload.Session.StreamState).IsNotNull();
            Check.That(sourceUpload.Session.StreamState!.AppendedLength).IsStrictlyGreaterThan(0);
            Check.That(sourceUpload.Session.StreamState.AppendedLength).IsStrictlyLessThan(payload.LongLength);

            using(DataControlClient consumer1 = CreateClient(consumerWorkingRoot))
            {
                DataControlDownloadSession? follower = await consumer1.OpenDownload(itemId!.Value,this.ReadToken,new DataControlDownloadOptions()
                {
                    Mode = DataControlDownloadMode.StreamFollow,
                    SourceSessionID = sourceUpload.Session.SessionId,
                    BindLiveStreamItem = true,
                });

                Check.That(follower).IsNotNull();
                Check.That(follower!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
                Check.That(follower.Session.StreamState).IsNotNull();
                Check.That(follower.Session.StreamState!.AppendedLength).IsEqualTo(0);

                followerSessionId = follower.Session.SessionId;
                Check.That(await follower.DownloadToCurrent()).IsGreaterOrEqualThan(0);

                DataControlDownloadCompletionResult? firstMaterialized = await follower.Materialize();

                Check.That(firstMaterialized).IsNotNull();
                firstConsumerLength = new FileInfo(firstMaterialized!.StreamPath).Length;
                Check.That(firstConsumerLength).IsEqualTo(sourceUpload.Session.StreamState.AppendedLength);
            }

            Check.That(await sourceUpload.UploadNext()).IsTrue();
            Check.That(sourceUpload.Session.StreamState!.AppendedLength).IsStrictlyGreaterThan(firstConsumerLength);

            using(DataControlClient consumer2 = CreateClient(consumerWorkingRoot))
            {
                IReadOnlyList<DataControlTransferSessionInfo> localSessions = await consumer2.GetLocalSessions();
                Check.That(localSessions.Any(_ => _.SessionId == followerSessionId)).IsTrue();

                DataControlDownloadSession? resumed = await consumer2.ResumeDownload(followerSessionId,this.ReadToken,new DataControlDownloadOptions()
                {
                    BindLiveStreamItem = true,
                });

                Check.That(resumed).IsNotNull();
                Check.That(await resumed!.DownloadToCurrent()).IsGreaterOrEqualThan(0);

                DataControlDownloadCompletionResult? secondMaterialized = await resumed.Materialize();

                Check.That(secondMaterialized).IsNotNull();
                secondConsumerLength = new FileInfo(secondMaterialized!.StreamPath).Length;
                Check.That(secondConsumerLength).IsStrictlyGreaterThan(firstConsumerLength);
            }

            Check.That(await sourceUpload.UploadNext()).IsTrue();
            Check.That(sourceUpload.Session.StreamState!.AppendedLength).IsStrictlyGreaterThan(secondConsumerLength);

            using(DataControlClient consumer3 = CreateClient(consumerWorkingRoot))
            {
                DataControlDownloadSession? resumed = await consumer3.ResumeDownload(followerSessionId,this.ReadToken,new DataControlDownloadOptions()
                {
                    BindLiveStreamItem = true,
                });

                Check.That(resumed).IsNotNull();
                Check.That(await resumed!.DownloadToCurrent()).IsGreaterOrEqualThan(0);

                DataControlDownloadCompletionResult? thirdMaterialized = await resumed.Materialize();

                Check.That(thirdMaterialized).IsNotNull();
                Check.That(new FileInfo(thirdMaterialized!.StreamPath).Length).IsStrictlyGreaterThan(secondConsumerLength);
            }

            Check.That(await sourceUpload.UploadToCompletion()).IsStrictlyGreaterThan(0);
            Check.That(await sourceUpload.Publish()).IsNotNull();

            using(DataControlClient consumer4 = CreateClient(consumerWorkingRoot))
            {
                DataControlDownloadSession? resumed = await consumer4.ResumeDownload(followerSessionId,this.ReadToken,new DataControlDownloadOptions()
                {
                    BindLiveStreamItem = true,
                });

                Check.That(resumed).IsNotNull();
                Check.That(await resumed!.DownloadToCurrent()).IsGreaterOrEqualThan(0);

                DataControlDownloadCompletionResult? finalMaterialized = await resumed.Materialize();

                Check.That(finalMaterialized).IsNotNull();
                Check.That(finalMaterialized!.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
                Check.That((await File.ReadAllBytesAsync(finalMaterialized.StreamPath)).SequenceEqual(payload)).IsTrue();
            }

            DataControlResponse<Guid> deleted = await uploader.Delete(itemId,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(uploaderWorkingRoot);
            DeleteWorkingDirectory(consumerWorkingRoot);
        }
    }

    [Test]
    public async Task ClientCanResumeSegmentedFollowAcrossSeparateConsumerRestartsAndPromoteThroughCaughtUpPublishTransition()
    {
        String uploaderWorkingRoot = CreateClientWorkingRoot();
        String consumerWorkingRoot = CreateClientWorkingRoot();
        String segmentedFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.SegmentedFollowResumeSeparateRoots.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(10 * 1024 * 1024,0x90);
        Guid followerSessionId = Guid.Empty;
        Int64 firstConsumerBytes = 0;
        Int64 secondConsumerBytes = 0;
        Int64 thirdConsumerBytes = 0;

        try
        {
            using DataControlClient uploader = CreateClient(uploaderWorkingRoot);

            BinaryItem item = await CreateClientBinaryItem(segmentedFilePath,payload);

            DataControlUploadSession? sourceUpload = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(sourceUpload).IsNotNull();
            Check.That(await sourceUpload!.UploadNext()).IsTrue();
            Check.That(sourceUpload.Session.State.RealizedBytes).IsStrictlyGreaterThan(0);
            Check.That(sourceUpload.Session.State.RealizedBytes).IsStrictlyLessThan(payload.LongLength);

            using(DataControlClient consumer1 = CreateClient(consumerWorkingRoot))
            {
                DataControlDownloadSession? follower = await consumer1.OpenDownload(item.GetID()!.Value,this.ReadToken,new DataControlDownloadOptions()
                {
                    Mode = DataControlDownloadMode.StagedFollow,
                    SourceSessionID = sourceUpload.Session.SessionId,
                });

                Check.That(follower).IsNotNull();
                Check.That(follower!.Session.State.Mode).IsEqualTo(DataItemTransferMode.StagedRead);

                followerSessionId = follower.Session.SessionId;

                Check.That(await follower.DownloadNext()).IsTrue();
                firstConsumerBytes = follower.Session.State.RealizedBytes;
                Check.That(firstConsumerBytes).IsStrictlyGreaterThan(0);
            }

            Check.That(await sourceUpload.UploadNext()).IsTrue();
            Check.That(sourceUpload.Session.State.RealizedBytes).IsStrictlyGreaterThan(firstConsumerBytes);

            using(DataControlClient consumer2 = CreateClient(consumerWorkingRoot))
            {
                IReadOnlyList<DataControlTransferSessionInfo> localSessions = await consumer2.GetLocalSessions();
                Check.That(localSessions.Any(_ => _.SessionId == followerSessionId)).IsTrue();

                DataControlDownloadSession? resumed = await consumer2.ResumeDownload(followerSessionId,this.ReadToken,new DataControlDownloadOptions());

                Check.That(resumed).IsNotNull();
                Check.That(resumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.StagedRead);
                Check.That(resumed.Session.State.RealizedBytes).IsEqualTo(firstConsumerBytes);
                Check.That(resumed.Session.State.MissingRanges.First().Offset).IsEqualTo(firstConsumerBytes);
                Check.That(await resumed.DownloadToCurrent()).IsGreaterOrEqualThan(0);
                secondConsumerBytes = resumed.Session.State.RealizedBytes;
                Check.That(secondConsumerBytes).IsStrictlyGreaterThan(firstConsumerBytes);
            }

            Check.That(await sourceUpload.UploadNext()).IsTrue();
            Check.That(sourceUpload.Session.State.RealizedBytes).IsStrictlyGreaterThan(secondConsumerBytes);

            using(DataControlClient consumer3 = CreateClient(consumerWorkingRoot))
            {
                DataControlDownloadSession? resumed = await consumer3.ResumeDownload(followerSessionId,this.ReadToken,new DataControlDownloadOptions());

                Check.That(resumed).IsNotNull();
                Check.That(resumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.StagedRead);
                Check.That(await resumed!.DownloadToCurrent()).IsGreaterOrEqualThan(0);
                thirdConsumerBytes = resumed.Session.State.RealizedBytes;
                Check.That(thirdConsumerBytes).IsStrictlyGreaterThan(secondConsumerBytes);
            }

            Check.That(sourceUpload.Session.State.RealizedBytes).IsEqualTo(thirdConsumerBytes);

            using(DataControlClient consumerPublishRace = CreateClient(consumerWorkingRoot))
            {
                DataControlDownloadSession? resumed = await consumerPublishRace.ResumeDownload(followerSessionId,this.ReadToken,new DataControlDownloadOptions());

                Check.That(resumed).IsNotNull();
                Check.That(resumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.StagedRead);
                Check.That(resumed.Session.State.Status).IsEqualTo(DataItemTransferStatus.Open);
                Check.That(resumed.Session.State.RealizedBytes).IsLessOrEqualThan(sourceUpload.Session.State.RealizedBytes);

                Task<Int32> pending = resumed.DownloadToCurrent(wait:TimeSpan.FromMilliseconds(50));

                await sourceUpload.UploadToCompletion();
                Check.That(await sourceUpload.Publish()).IsNotNull();

                Int32 downloadedAfterPublish = await pending;

                Check.That(downloadedAfterPublish).IsGreaterOrEqualThan(0);

                for(Int32 attempt = 0; attempt < 20 && resumed.Session.State.Mode != DataItemTransferMode.PublishedRead; attempt++)
                {
                    Check.That(await resumed.DownloadToCurrent(wait:TimeSpan.FromMilliseconds(200))).IsGreaterOrEqualThan(0);
                }

                Check.That(resumed.Session.State.Mode).IsEqualTo(DataItemTransferMode.PublishedRead);
                Check.That(resumed.Session.State.Status).IsOneOf([DataItemTransferStatus.Published,DataItemTransferStatus.Complete]);
                Check.That(await resumed.DownloadToCompletion()).IsGreaterOrEqualThan(0);

                DataControlDownloadCompletionResult? materialized = await resumed.Materialize();

                Check.That(materialized).IsNotNull();
                Check.That(materialized!.Item).IsInstanceOf<BinaryItem>();
                Check.That((await File.ReadAllBytesAsync(materialized.StreamPath)).SequenceEqual(payload)).IsTrue();
            }

            using(DataControlClient consumer4 = CreateClient(consumerWorkingRoot))
            {
                DataControlDownloadSession? resumed = await consumer4.ResumeDownload(followerSessionId,this.ReadToken,new DataControlDownloadOptions());

                Check.That(resumed).IsNotNull();
                Check.That(resumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.PublishedRead);
                Check.That(await resumed.DownloadToCompletion()).IsGreaterOrEqualThan(0);

                DataControlDownloadCompletionResult? materialized = await resumed.Materialize();

                Check.That(materialized).IsNotNull();
                Check.That(materialized!.Item).IsInstanceOf<BinaryItem>();
                Check.That((await File.ReadAllBytesAsync(materialized.StreamPath)).SequenceEqual(payload)).IsTrue();
            }

            DataControlResponse<Guid> deleted = await uploader.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            DeleteWorkingDirectory(uploaderWorkingRoot);
            DeleteWorkingDirectory(consumerWorkingRoot);
        }
    }

    [Test]
    public async Task ClientStreamPublishCanReplaceFinalObjectTypeWithMetadataOnlyBinaryItem()
    {
        String workingRoot = CreateClientWorkingRoot();
        String streamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamPublishFinalBinary.{Guid.NewGuid():N}.bin");
        Byte[] streamPayload = CreateSegmentedTransferPayload(512 * 1024,0x8D);
        Guid? itemId = null;

        try
        {
            using DataControlClient client = CreateClient(workingRoot);

            await using FileStream streamSource = new(streamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await streamSource.WriteAsync(streamPayload);
            streamSource.Position = 0;

            DataStreamItem streamItem = new(streamSource);
            itemId = streamItem.GetID();

            DataControlUploadSession? session = await client.OpenUpload(streamItem,this.WriteToken);

            Check.That(session).IsNotNull();
            Check.That(await session!.UploadToCompletion()).IsStrictlyGreaterThan(0);

            BinaryItem finalItem = new(content:null,file:null,name:"Published Binary Metadata",type:DataTypes.ZIP);

            DataControlUploadCompletionResult? published = await session.Publish(finalitem:finalItem);

            Check.That(published).IsNotNull();
            Check.That(finalItem.GetID()).IsEqualTo(itemId);

            DataControlDownloadCompletionResult? downloaded = await client.Download(itemId!.Value,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsInstanceOf<BinaryItem>();
            Check.That(downloaded.Item!.GetID()).IsEqualTo(itemId);
            Check.That(downloaded.Item.GetName()).IsEqualTo("Published Binary Metadata");
            Check.That(downloaded.Item.GetDataType()).IsEqualTo(DataTypes.ZIP);
            Check.That(File.Exists(downloaded.Item.GetFILE()!)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(downloaded.Item.GetFILE()!)).SequenceEqual(streamPayload)).IsTrue();

            DataControlResponse<Guid> deleted = await client.Delete(itemId,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientStreamPublishRejectsFinalItemThatStillExposesContent()
    {
        String workingRoot = CreateClientWorkingRoot();
        String streamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamPublishRejectContent.{Guid.NewGuid():N}.bin");
        Byte[] streamPayload = CreateSegmentedTransferPayload(256 * 1024,0x8E);
        Guid? itemId = null;

        try
        {
            using DataControlClient client = CreateClient(workingRoot);

            await using FileStream streamSource = new(streamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await streamSource.WriteAsync(streamPayload);
            streamSource.Position = 0;

            DataStreamItem streamItem = new(streamSource);
            itemId = streamItem.GetID();

            DataControlUploadSession? session = await client.OpenUpload(streamItem,this.WriteToken);

            Check.That(session).IsNotNull();
            Check.That(await session!.UploadToCompletion()).IsStrictlyGreaterThan(0);

            BinaryItem invalidFinalItem = new(content:new Byte[]{ 0x01,0x02,0x03 },file:null,name:"Invalid Final Binary",type:DataTypes.BIN);

            Check.ThatAsyncCode(async () => await session.Publish(finalitem:invalidFinalItem)).Throws<InvalidOperationException>();

            Check.That(await session.Abort()).IsTrue();
        }
        finally
        {
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientGetServerSessionsReturnsSegmentedAndStreamInventory()
    {
        String workingRoot = CreateClientWorkingRoot();
        String segmentedFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.ServerSessions.Segmented.{Guid.NewGuid():N}.bin");
        String streamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.ServerSessions.Stream.{Guid.NewGuid():N}.bin");
        Byte[] segmentedPayload = CreateSegmentedTransferPayload(256 * 1024,0xA1);
        Byte[] streamPayload = CreateSegmentedTransferPayload(256 * 1024,0xA2);
        DataControlUploadSession? segmentedSession = null;
        DataControlUploadSession? streamSession = null;

        try
        {
            using DataControlClient client = CreateClient(workingRoot);

            BinaryItem segmentedItem = await CreateClientBinaryItem(segmentedFilePath,segmentedPayload);

            await using FileStream streamSource = new(streamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await streamSource.WriteAsync(streamPayload);
            await streamSource.FlushAsync();
            streamSource.Position = 0;

            DataStreamItem streamItem = new(streamSource);

            segmentedSession = await client.OpenUpload(segmentedItem,this.WriteToken);
            streamSession = await client.OpenUpload(streamItem,this.WriteToken);

            Check.That(segmentedSession).IsNotNull();
            Check.That(streamSession).IsNotNull();

            DataControlResponse<DataControlServerSessionInfo[]> sessions = await client.GetServerSessions(new GetTransferSessionsRequest()
            {
                Status = DataItemTransferStatus.Open,
            },this.AdminToken);

            Check.That((Int32)sessions.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(sessions.Data).IsNotNull();

            DataControlServerSessionInfo? segmented = sessions.Data!.FirstOrDefault(_ => _.SessionID == segmentedSession!.Session.SessionId);
            DataControlServerSessionInfo? stream = sessions.Data!.FirstOrDefault(_ => _.SessionID == streamSession!.Session.SessionId);

            Check.That(segmented).IsNotNull();
            Check.That(segmented!.ItemID).IsEqualTo(segmentedSession!.Session.ItemId);
            Check.That(segmented.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
            Check.That(segmented.SegmentedState).IsNotNull();
            Check.That(segmented.StreamState).IsNull();
            Check.That(segmented.SegmentedState!.Mode).IsEqualTo(DataItemTransferMode.Upload);

            Check.That(stream).IsNotNull();
            Check.That(stream!.ItemID).IsEqualTo(streamSession!.Session.ItemId);
            Check.That(stream.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(stream.SegmentedState).IsNull();
            Check.That(stream.StreamState).IsNotNull();
            Check.That(stream.StreamState!.Mode).IsEqualTo(DataItemTransferMode.StreamUpload);

            if(segmentedSession is not null) { _ = await segmentedSession.Abort(); }
            if(streamSession is not null) { _ = await streamSession.Abort(); }
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientCanMaterializePublishedSegmentedDownloadIntoExternalDirectory()
    {
        String workingRoot = CreateClientWorkingRoot();
        String segmentedFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.MaterializeExternalSegmented.Source.{Guid.NewGuid():N}.bin");
        String destinationRoot = Path.Combine(Path.GetTempPath(),"KusDepot","ClientExams","MaterializeExternalSegmented",Guid.NewGuid().ToString("N"));
        Byte[] payload = CreateSegmentedTransferPayload(256 * 1024,0xA3);

        try
        {
            using DataControlClient client = CreateClient(workingRoot);

            BinaryItem item = await CreateClientBinaryItem(segmentedFilePath,payload);

            DataControlUploadCompletionResult? uploaded = await client.Upload(item,this.WriteToken);

            Check.That(uploaded).IsNotNull();

            DataControlDownloadSession? session = await client.OpenDownload(item.GetID()!.Value,this.ReadToken);

            Check.That(session).IsNotNull();
            Check.That(await session!.DownloadToCompletion()).IsGreaterOrEqualThan(0);

            DataControlDownloadCompletionResult? materialized = await session.Materialize(destinationRoot);

            Check.That(materialized).IsNotNull();
            Check.That(materialized!.Item).IsInstanceOf<BinaryItem>();
            Check.That(materialized.StreamPath.StartsWith(Path.GetFullPath(destinationRoot),StringComparison.OrdinalIgnoreCase)).IsTrue();
            Check.That(File.Exists(materialized.StreamPath)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(materialized.StreamPath)).SequenceEqual(payload)).IsTrue();

            BinaryItem binary = (BinaryItem)materialized.Item!;
            Check.That(String.IsNullOrWhiteSpace(binary.GetObjectFILE())).IsFalse();
            Check.That(binary.GetObjectFILE()!.StartsWith(Path.GetFullPath(destinationRoot),StringComparison.OrdinalIgnoreCase)).IsTrue();
            Check.That(File.Exists(binary.GetObjectFILE()!)).IsTrue();

            DataControlResponse<Guid> deleted = await client.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            DeleteWorkingDirectory(workingRoot);
            DeleteWorkingDirectory(destinationRoot);
        }
    }

    [Test]
    public async Task ClientRetiresLocalSegmentedSessionAfterExternalMaterialize()
    {
        String workingRoot = CreateClientWorkingRoot();
        String segmentedFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.MaterializeExternalRetire.Source.{Guid.NewGuid():N}.bin");
        String destinationRoot = Path.Combine(Path.GetTempPath(),"KusDepot","ClientExams","MaterializeExternalRetire",Guid.NewGuid().ToString("N"));
        Byte[] payload = CreateSegmentedTransferPayload(256 * 1024,0xA4);
        Guid downloadSessionId = Guid.Empty;
        String? exportedStreamPath = null;
        String? exportedObjectPath = null;
        String? sessionWorkingDirectory = null;
        Guid? itemId = null;

        try
        {
            using(DataControlClient client = CreateClient(workingRoot))
            {
                BinaryItem item = await CreateClientBinaryItem(segmentedFilePath,payload);
                itemId = item.GetID();

                DataControlUploadCompletionResult? uploaded = await client.Upload(item,this.WriteToken);

                Check.That(uploaded).IsNotNull();

                DataControlDownloadSession? session = await client.OpenDownload(itemId!.Value,this.ReadToken);

                Check.That(session).IsNotNull();
                downloadSessionId = session!.Session.SessionId;
                sessionWorkingDirectory = session.Session.WorkingDirectory;
                Check.That(await session.DownloadToCompletion()).IsGreaterOrEqualThan(0);

                String preferredStreamPath = Path.Combine(destinationRoot,Path.GetFileName(item.GetFILE()!));

                Directory.CreateDirectory(destinationRoot);
                await File.WriteAllTextAsync(preferredStreamPath,"conflicting destination content");

                DataControlDownloadCompletionResult? rejected = await session.Materialize(destinationRoot);

                Check.That(rejected).IsNull();
                Check.That(Directory.Exists(sessionWorkingDirectory!)).IsTrue();
                Check.That(await File.ReadAllTextAsync(preferredStreamPath)).IsEqualTo("conflicting destination content");

                File.Delete(preferredStreamPath);

                DataControlDownloadCompletionResult? materialized = await session.Materialize(destinationRoot);

                Check.That(materialized).IsNotNull();
                exportedStreamPath = materialized!.StreamPath;
                exportedObjectPath = ((BinaryItem)materialized.Item!).GetObjectFILE();
                Check.That(exportedStreamPath!.StartsWith(Path.GetFullPath(destinationRoot),StringComparison.OrdinalIgnoreCase)).IsTrue();
                Check.That(exportedObjectPath!.StartsWith(Path.GetFullPath(destinationRoot),StringComparison.OrdinalIgnoreCase)).IsTrue();
                Check.That(File.Exists(exportedStreamPath)).IsTrue();
                Check.That(File.Exists(exportedObjectPath)).IsTrue();
                Check.That((await File.ReadAllBytesAsync(exportedStreamPath)).SequenceEqual(payload)).IsTrue();
                Check.That(sessionWorkingDirectory).IsNotNull();
                Check.That(Directory.Exists(sessionWorkingDirectory!)).IsFalse();
            }

            using(DataControlClient resumedClient = CreateClient(workingRoot))
            {
                DataControlDownloadSession? resumed = await resumedClient.ResumeDownload(downloadSessionId,this.ReadToken,new DataControlDownloadOptions());

                Check.That(resumed).IsNull();

                Check.That(exportedStreamPath).IsNotNull();
                Check.That(exportedObjectPath).IsNotNull();
                Check.That(File.Exists(exportedStreamPath!)).IsTrue();
                Check.That(File.Exists(exportedObjectPath!)).IsTrue();

                DataControlResponse<Guid> deleted = await resumedClient.Delete(itemId,this.WriteToken);
                Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            }
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            DeleteWorkingDirectory(workingRoot);
            DeleteWorkingDirectory(destinationRoot);
        }
    }

    [Test]
    public async Task ClientRefusesExternalMaterializeForActiveStreamFollowSession()
    {
        String workingRoot = CreateClientWorkingRoot();
        String followerRoot = CreateClientWorkingRoot();
        String destinationRoot = Path.Combine(Path.GetTempPath(),"KusDepot","ClientExams","MaterializeExternalActiveStreamFollow",Guid.NewGuid().ToString("N"));
        Guid? publishedItemId = null;
        Boolean published = false;

        try
        {
            using DataControlClient uploader = CreateClient(workingRoot);
            using DataControlClient followerClient = CreateClient(followerRoot);
            using ControllableProducerStream producer = new();

            DataStreamItem item = new(producer);
            Guid itemId = item.GetID()!.Value;
            publishedItemId = itemId;

            DataControlUploadSession? uploadSession = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(uploadSession).IsNotNull();
            Check.That(uploadSession!.Session.StreamState).IsNotNull();

            Int32 chunkLength = ResolveLiveFollowChunkLength(uploadSession.Session.StreamState!);
            Byte[] firstSegment = CreateSegmentedTransferPayload(chunkLength,0xA5);

            DataControlDownloadSession? follower = await followerClient.OpenDownload(itemId,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = uploadSession.Session.SessionId,
                BindLiveStreamItem = true,
            });

            Check.That(follower).IsNotNull();
            Check.That(follower!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(follower.LiveItem).IsNotNull();

            await producer.PublishAsync(firstSegment);
            Check.That(await uploadSession.UploadNext()).IsTrue();
            Check.That(await follower.DownloadToCurrent(wait:TimeSpan.Zero)).IsGreaterOrEqualThan(0);
            Check.That(follower.Session.StreamState).IsNotNull();
            Check.That(follower.Session.StreamState!.Status).IsEqualTo(DataItemTransferStatus.Open);

            DataControlDownloadCompletionResult? inPlace = await follower.Materialize();

            Check.That(inPlace).IsNotNull();
            Check.That(File.Exists(inPlace!.StreamPath)).IsTrue();
            Check.That(inPlace.StreamPath.StartsWith(Path.GetFullPath(destinationRoot),StringComparison.OrdinalIgnoreCase)).IsFalse();

            DataControlDownloadCompletionResult? externalized = await follower.Materialize(destinationRoot);

            Check.That(externalized).IsNull();
            Check.That(Directory.Exists(destinationRoot)).IsFalse();

            producer.Complete();
            Check.That(await uploadSession.UploadToCompletion()).IsGreaterOrEqualThan(0);
            Check.That(await uploadSession.Publish()).IsNotNull();
            published = true;
            Check.That(await follower.DownloadToCompletion()).IsGreaterOrEqualThan(0);
            Check.That(follower.Session.StreamState).IsNotNull();
            Check.That(follower.Session.StreamState!.Status).IsEqualTo(DataItemTransferStatus.Published);

            DataControlDownloadCompletionResult? exported = await follower.Materialize(destinationRoot);

            Check.That(exported).IsNotNull();
            Check.That(exported!.StreamPath.StartsWith(Path.GetFullPath(destinationRoot),StringComparison.OrdinalIgnoreCase)).IsTrue();
            Check.That(File.Exists(exported.StreamPath)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(exported.StreamPath)).SequenceEqual(firstSegment)).IsTrue();
            Check.That(String.IsNullOrWhiteSpace(exported.Item!.GetObjectFILE())).IsFalse();
            Check.That(exported.Item.GetObjectFILE()!.StartsWith(Path.GetFullPath(destinationRoot),StringComparison.OrdinalIgnoreCase)).IsTrue();
            Check.That(File.Exists(exported.Item.GetObjectFILE()!)).IsTrue();
        }
        finally
        {
            if(published && publishedItemId.HasValue)
            {
                try
                {
                    using DataControlClient cleanupClient = CreateClient(workingRoot);
                    _ = await cleanupClient.Delete(publishedItemId.Value,this.WriteToken);
                }
                catch { }
            }

            DeleteWorkingDirectory(workingRoot);
            DeleteWorkingDirectory(followerRoot);
            DeleteWorkingDirectory(destinationRoot);
        }
    }

    [TestCase(false)] [TestCase(true)]
    public async Task ClientResumeStreamUploadDoesNotReplayAcknowledgedPrefix(Boolean appendUnsentTail)
    {
        String workingRoot = CreateClientWorkingRoot();
        Byte[] payload = CreateSegmentedTransferPayload(32 * 1024,0xB1);
        Byte[] unsentTail = CreateSegmentedTransferPayload(24 * 1024,0xB2);
        Guid? itemId = null;
        Guid sessionId = Guid.Empty;
        String? sessionDirectory = null;
        Boolean published = false;
        DataControlUploadSession? activeUpload = null;

        try
        {
            using DataControlClient uploader = CreateClient(workingRoot);
            using MemoryStream source = new(payload,writable:false);

            DataStreamItem item = new(source);
            itemId = item.GetID();

            activeUpload = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(activeUpload).IsNotNull();
            sessionId = activeUpload!.Session.SessionId;
            sessionDirectory = activeUpload.Session.WorkingDirectory;

            Check.That(await activeUpload!.UploadNext()).IsTrue();
            Check.That(activeUpload.Session.StreamState!.AppendedLength).IsEqualTo(payload.LongLength);

            await activeUpload.DisposeAsync();
            activeUpload = null;

            if(appendUnsentTail)
            {
                String spoolPath = Path.Combine(sessionDirectory!,DataStreamTransferManifest.StreamFileName);

                using FileStream spool = new(spoolPath,FileMode.Append,FileAccess.Write,FileShare.Read);
                await spool.WriteAsync(unsentTail);
            }

            using DataControlClient resumedClient = CreateClient(workingRoot);

            activeUpload = await resumedClient.ResumeUpload(sessionId,this.WriteToken);

            Check.That(activeUpload).IsNotNull();

            Int32 resumedSegments = await activeUpload!.UploadToCompletion();

            Check.That(resumedSegments).IsEqualTo(appendUnsentTail ? 1 : 0);
            Check.That(activeUpload.Session.StreamState!.AppendedLength).IsEqualTo(payload.LongLength + (appendUnsentTail ? unsentTail.LongLength : 0));

            DataControlUploadCompletionResult? completed = await activeUpload.Publish();

            Check.That(completed).IsNotNull();
            Check.That(completed!.Stream!.Published).IsTrue();
            published = true;

            DataControlDownloadCompletionResult? downloaded = await resumedClient.Download(itemId!.Value,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Byte[] expectedPayload = appendUnsentTail ? payload.Concat(unsentTail).ToArray() : payload;
            Check.That((await File.ReadAllBytesAsync(downloaded!.StreamPath)).SequenceEqual(expectedPayload)).IsTrue();

            DataControlResponse<Guid> deleted = await resumedClient.Delete(itemId.Value,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            published = false;
        }
        finally
        {
            if(activeUpload is not null) { await activeUpload.DisposeAsync(); }

            using DataControlClient cleanupClient = CreateClient(workingRoot);

            if(published && itemId.HasValue)
            {
                _ = await cleanupClient.Delete(itemId.Value,this.WriteToken);
            }
            else if(itemId.HasValue && sessionId != Guid.Empty)
            {
                _ = await cleanupClient.AbortTransfer(new AbortTransferRequest(){ SessionID = sessionId , ItemID = itemId.Value },this.WriteToken);
            }

            DeleteWorkingDirectory(workingRoot);
        }
    }

    private DataControlClient CreateClient(String workingRoot)
    {
        return new(this.EndpointLocator!,options:new DataControlClientOptions
        {
            WorkingDirectoryOptions = new DataControlClientWorkingDirectoryOptions(){ RootPath = workingRoot }
        });
    }

    private static String CreateClientWorkingRoot()
    {
        return Path.Combine(Path.GetTempPath(),"KusDepot","ClientExams",Guid.NewGuid().ToString("N"));
    }
}
