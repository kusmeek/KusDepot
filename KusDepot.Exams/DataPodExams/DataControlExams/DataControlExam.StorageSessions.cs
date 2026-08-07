namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test] [Order(33)]
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

    [Test] [Order(34)]
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
            Check.That(follower!.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadStaged);

            Int32 downloaded = await follower.DownloadToCurrent();

            Check.That(downloaded).IsStrictlyGreaterThan(0);
            Check.That(follower.Session.State.RealizedBytes).IsEqualTo(sourceUpload.Session.State.RealizedBytes);
            Check.That(follower.Session.State.Status).IsNotEqualTo(DataItemTransferStatus.Committed);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            DeleteWorkingDirectory(uploaderWorkingRoot);
            DeleteWorkingDirectory(consumerWorkingRoot);
        }
    }

    [Test] [Order(35)]
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
            Check.That(await uploadSession.Commit()).IsNotNull();
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

    [Test] [Order(36)]
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
            Check.That(segmentedUpload.Session.State.Status).IsEqualTo(DataItemTransferStatus.Committed);

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
            Check.That(streamUpload.Session.StreamState!.Status).IsEqualTo(DataItemTransferStatus.Committed);

            DataControlDownloadCompletionResult? streamDownload = await _dc.Download(streamItemId!.Value,this.ReadToken);

            Check.That(streamDownload).IsNotNull();
            Check.That(streamDownload!.Item).IsInstanceOf<DataStreamItem>();
            Check.That(File.Exists(streamDownload.StreamPath)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(streamDownload.StreamPath)).SequenceEqual(streamPayload)).IsTrue();

            RestResponse<Guid> deletedSegmented = await _dc.Delete(segmentedItemId,this.WriteToken);
            Check.That((Int32)deletedSegmented.StatusCode).IsEqualTo(StatusCodes.Status200OK);

            RestResponse<Guid> deletedStream = await _dc.Delete(streamItemId,this.WriteToken);
            Check.That((Int32)deletedStream.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test] [Order(37)]
    public async Task ClientOpenDownloadSupportsCommittedAndStreamFollowModes()
    {
        String workingRoot = CreateClientWorkingRoot();
        String followStreamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamFollowModes.{Guid.NewGuid():N}.bin");
        String committedStreamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamCommittedModes.{Guid.NewGuid():N}.bin");
        Byte[] followPayload = CreateSegmentedTransferPayload(512 * 1024,0x86);
        Byte[] committedPayload = CreateSegmentedTransferPayload(512 * 1024,0x88);
        Guid? followItemId = null;
        Guid? committedItemId = null;

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
            Check.That(follower.Session.StreamState!.Mode).IsEqualTo(DataItemTransferMode.StreamFollow);
            Check.That(follower.Session.StreamState.SourceSessionID).IsEqualTo(uploadSession.Session.SessionId);
            Check.That(follower.Session.StreamState.AppendedLength).IsEqualTo(uploadSession.Session.StreamState.AppendedLength);
            Check.That(await follower.Materialize()).IsNull();

            Check.That(await uploadSession.Abort()).IsTrue();

            await using FileStream committedStreamSource = new(committedStreamFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await committedStreamSource.WriteAsync(committedPayload);
            committedStreamSource.Position = 0;

            DataStreamItem committedItem = new(committedStreamSource);
            committedItemId = committedItem.GetID();

            DataControlUploadCompletionResult? committedUpload = await _dc.Upload(committedItem,this.WriteToken);

            Check.That(committedUpload).IsNotNull();
            Check.That(committedUpload!.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(committedUpload.Stream).IsNotNull();
            Check.That(committedUpload.Stream!.Published).IsTrue();

            DataControlDownloadSession? committedDownload = await _dc.OpenDownload(committedItemId!.Value,this.ReadToken);

            Check.That(committedDownload).IsNotNull();
            Check.That(committedDownload!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
            Check.That(committedDownload.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadCommitted);

            RestResponse<Guid> deletedCommitted = await _dc.Delete(committedItemId,this.WriteToken);
            Check.That((Int32)deletedCommitted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(followStreamFilePath)) { File.Delete(followStreamFilePath); }
            if(File.Exists(committedStreamFilePath)) { File.Delete(committedStreamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test] [Order(38)]
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
            Check.That(follower.Session.StreamState.AppendedLength).IsEqualTo(appendedLength);
            Check.That(follower.LiveItem).IsNotNull();
            Check.That(follower.LiveItem!.GetContentBindingMode()).IsEqualTo(DataStreamItemContentBindingMode.LiveSessionManaged);
            Check.That(follower.LiveItem.GetLiveContentStatus()).IsEqualTo(DataStreamItemLiveContentStatus.Open);

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

    [Test] [Order(39)]
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
            Check.That(follower.Session.StreamState.AppendedLength).IsEqualTo(appendedLength);
            Check.That(follower.LiveItem).IsNotNull();
            Check.That(follower.LiveItem!.GetContentBindingMode()).IsEqualTo(DataStreamItemContentBindingMode.LiveSessionManaged);
            Check.That(follower.LiveItem.GetLiveContentStatus()).IsEqualTo(DataStreamItemLiveContentStatus.Open);

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

    [Test] [Order(40)]
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

            RestResponse<Guid> deleted = await resumedClient.Delete(itemId,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test] [Order(41)]
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
            Check.That(follower!.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadStaged);
            Check.That(follower.Session.State.SourceSessionID).IsEqualTo(sourceUpload.Session.SessionId);

            using DataControlClient resumedClient = CreateClient(workingRoot);

            DataControlDownloadSession? resumed = await resumedClient.ResumeDownload(follower.Session.SessionId,this.ReadToken,new DataControlDownloadOptions());

            Check.That(resumed).IsNotNull();
            Check.That(resumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadStaged);
            Check.That(resumed.Session.State.SourceSessionID).IsEqualTo(sourceUpload.Session.SessionId);
            Check.That(await resumed.DownloadNext()).IsTrue();
            Check.That(resumed.Session.State.RealizedBytes).IsStrictlyGreaterThan(0);

            Check.That(await sourceUpload.UploadToCompletion()).IsStrictlyGreaterThan(0);
            Check.That(await sourceUpload.Commit()).IsNotNull();

            using DataControlClient committedResumeClient = CreateClient(workingRoot);

            DataControlDownloadSession? committedResumed = await committedResumeClient.ResumeDownload(follower.Session.SessionId,this.ReadToken,new DataControlDownloadOptions());

            Check.That(committedResumed).IsNotNull();
            Check.That(committedResumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadCommitted);
            Check.That(committedResumed.Session.State.SourceSessionID).IsNull();
            Check.That(committedResumed.Session.State.RealizedBytes).IsStrictlyGreaterThan(0);

            Int32 downloaded = await committedResumed.DownloadToCompletion();

            Check.That(downloaded >= 0).IsTrue();

            DataControlDownloadCompletionResult? materialized = await committedResumed.Materialize();

            Check.That(materialized).IsNotNull();
            Check.That(materialized!.Item).IsInstanceOf<BinaryItem>();
            Check.That((await File.ReadAllBytesAsync(materialized.StreamPath)).SequenceEqual(payload)).IsTrue();

            RestResponse<Guid> deleted = await client.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test] [Order(42)]
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

    [Test] [Order(43)]
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

            DataControlUploadCompletionResult? committed = await resumed.Commit();

            Check.That(committed).IsNotNull();
            Check.That(committed!.Segmented!.Published).IsTrue();

            RestResponse<Guid> deleted = await resumedClient.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test] [Order(44)]
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

    [Test] [Order(45)]
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

                followerSessionId = follower.Session.SessionId;

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
            Check.That(await sourceUpload.Commit()).IsNotNull();

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

            RestResponse<Guid> deleted = await uploader.Delete(itemId,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(uploaderWorkingRoot);
            DeleteWorkingDirectory(consumerWorkingRoot);
        }
    }

    [Test] [Order(46)]
    public async Task ClientCanResumeSegmentedFollowAcrossSeparateConsumerRestartsAndPromoteThroughCaughtUpCommitTransition()
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
                Check.That(follower!.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadStaged);

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
                Check.That(resumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadStaged);
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
                Check.That(resumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadStaged);
                Check.That(await resumed!.DownloadToCurrent()).IsGreaterOrEqualThan(0);
                thirdConsumerBytes = resumed.Session.State.RealizedBytes;
                Check.That(thirdConsumerBytes).IsStrictlyGreaterThan(secondConsumerBytes);
            }

            Check.That(sourceUpload.Session.State.RealizedBytes).IsEqualTo(thirdConsumerBytes);

            using(DataControlClient consumerCommitRace = CreateClient(consumerWorkingRoot))
            {
                DataControlDownloadSession? resumed = await consumerCommitRace.ResumeDownload(followerSessionId,this.ReadToken,new DataControlDownloadOptions());

                Check.That(resumed).IsNotNull();
                Check.That(resumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadStaged);
                Check.That(resumed.Session.State.Status).IsEqualTo(DataItemTransferStatus.Open);
                Check.That(resumed.Session.State.RealizedBytes).IsLessOrEqualThan(sourceUpload.Session.State.RealizedBytes);

                Task<Int32> pending = resumed.DownloadToCurrent(wait:TimeSpan.FromMilliseconds(50));

                await sourceUpload.UploadToCompletion();
                Check.That(await sourceUpload.Commit()).IsNotNull();

                Int32 downloadedAfterCommit = await pending;

                Check.That(downloadedAfterCommit).IsGreaterOrEqualThan(0);

                for(Int32 attempt = 0; attempt < 20 && resumed.Session.State.Mode != DataItemTransferMode.ReadCommitted; attempt++)
                {
                    Check.That(await resumed.DownloadToCurrent(wait:TimeSpan.FromMilliseconds(200))).IsGreaterOrEqualThan(0);
                }

                Check.That(resumed.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadCommitted);
                Check.That(resumed.Session.State.Status).IsOneOf([DataItemTransferStatus.Committed,DataItemTransferStatus.Complete]);
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
                Check.That(resumed!.Session.State.Mode).IsEqualTo(DataItemTransferMode.ReadCommitted);
                Check.That(await resumed.DownloadToCompletion()).IsGreaterOrEqualThan(0);

                DataControlDownloadCompletionResult? materialized = await resumed.Materialize();

                Check.That(materialized).IsNotNull();
                Check.That(materialized!.Item).IsInstanceOf<BinaryItem>();
                Check.That((await File.ReadAllBytesAsync(materialized.StreamPath)).SequenceEqual(payload)).IsTrue();
            }

            RestResponse<Guid> deleted = await uploader.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            DeleteWorkingDirectory(uploaderWorkingRoot);
            DeleteWorkingDirectory(consumerWorkingRoot);
        }
    }

    [Test] [Order(47)]
    public async Task ClientStreamCommitCanReplaceFinalObjectTypeWithMetadataOnlyBinaryItem()
    {
        String workingRoot = CreateClientWorkingRoot();
        String streamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamCommitFinalBinary.{Guid.NewGuid():N}.bin");
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

            BinaryItem finalItem = new(content:null,file:null,name:"Committed Binary Metadata",type:DataTypes.ZIP);

            DataControlUploadCompletionResult? committed = await session.Commit(finalitem:finalItem);

            Check.That(committed).IsNotNull();
            Check.That(finalItem.GetID()).IsEqualTo(itemId);

            DataControlDownloadCompletionResult? downloaded = await client.Download(itemId!.Value,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsInstanceOf<BinaryItem>();
            Check.That(downloaded.Item!.GetID()).IsEqualTo(itemId);
            Check.That(downloaded.Item.GetName()).IsEqualTo("Committed Binary Metadata");
            Check.That(downloaded.Item.GetDataType()).IsEqualTo(DataTypes.ZIP);
            Check.That(File.Exists(downloaded.Item.GetFILE()!)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(downloaded.Item.GetFILE()!)).SequenceEqual(streamPayload)).IsTrue();

            RestResponse<Guid> deleted = await client.Delete(itemId,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test] [Order(48)]
    public async Task ClientStreamCommitRejectsFinalItemThatStillExposesContent()
    {
        String workingRoot = CreateClientWorkingRoot();
        String streamFilePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.StreamCommitRejectContent.{Guid.NewGuid():N}.bin");
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

            Check.ThatAsyncCode(() => session.Commit(finalitem:invalidFinalItem)).Throws<InvalidOperationException>();

            Check.That(await session.Abort()).IsTrue();
        }
        finally
        {
            if(File.Exists(streamFilePath)) { File.Delete(streamFilePath); }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test] [Order(49)]
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

            RestResponse<DataControlServerSessionInfo[]> sessions = await client.GetServerSessions(new GetTransferSessionsRequest()
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

    [Test] [Order(50)]
    public async Task ClientCanMaterializeCommittedSegmentedDownloadIntoExternalDirectory()
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

            RestResponse<Guid> deleted = await client.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(segmentedFilePath)) { File.Delete(segmentedFilePath); }
            DeleteWorkingDirectory(workingRoot);
            DeleteWorkingDirectory(destinationRoot);
        }
    }

    [Test] [Order(51)]
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

                RestResponse<Guid> deleted = await resumedClient.Delete(itemId,this.WriteToken);
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

    [Test] [Order(52)]
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
            Check.That(await uploadSession.Commit()).IsNotNull();
            published = true;
            Check.That(await follower.DownloadToCompletion()).IsGreaterOrEqualThan(0);
            Check.That(follower.Session.StreamState).IsNotNull();
            Check.That(follower.Session.StreamState!.Status).IsEqualTo(DataItemTransferStatus.Committed);

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

    private DataControlClient CreateClient(String workingRoot)
    {
        return new(this.EndpointLocator!,this.Certificate!)
        {
            WorkingDirectoryOptions = new DataControlClientWorkingDirectoryOptions(){ RootPath = workingRoot }
        };
    }

    private static String CreateClientWorkingRoot()
    {
        return Path.Combine(Path.GetTempPath(),"KusDepot","ClientExams",Guid.NewGuid().ToString("N"));
    }
}
