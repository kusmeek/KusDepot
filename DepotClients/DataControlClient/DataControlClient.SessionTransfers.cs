namespace KusDepot.Data.Clients;

public sealed partial class DataControlClient
{
    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="OpenDownloadCleanupTimeout"]/*'/>*/
    private static readonly TimeSpan OpenDownloadCleanupTimeout = TimeSpan.FromSeconds(10);

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="OpenUploadCleanupTimeout"]/*'/>*/
    private static readonly TimeSpan OpenUploadCleanupTimeout = TimeSpan.FromSeconds(10);

    ///<inheritdoc/>
    public async Task<DataControlDownloadCompletionResult?> Download(Guid itemid , String? token = null , DataControlDownloadOptions? options = null , IProgress<DataItemTransferProgress>? progress = null , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        DataControlDownloadSession? session = await this.OpenDownload(itemid,token,options,cancel).ConfigureAwait(false);

        if(session is null) { return null; }

        await using (session.ConfigureAwait(false))
        {
            try
            {
                await session.DownloadToCompletion(progress,cancel).ConfigureAwait(false);
            }
            catch(OperationCanceledException) when(cancel.IsCancellationRequested)
            {
                if(session.Session.TransferFamily is DataControlTransferFamily.Segmented)
                {
                    progress?.Report(DataControlDownloadSession.CreateProgressSnapshot(session.Session.State));
                }
                else if(session.Session.StreamState is { } streamState)
                {
                    progress?.Report(DataControlDownloadSession.CreateProgressSnapshot(streamState));
                }

                throw;
            }

            return await session.Materialize(cancel).ConfigureAwait(false);
        }
    }

    ///<inheritdoc/>
    public async Task<DataControlDownloadSession?> OpenDownload(Guid itemid , String? token = null , DataControlDownloadOptions? options = null , CancellationToken cancel = default)
    {
        DataControlClientWorkingDirectoryStorage? cleanupStorage = null;
        Guid cleanupSessionId = Guid.Empty; Boolean remoteSessionOpened = false;
        DataControlTransferFamily? cleanupTransferFamily = null;
        BufferedFollowStream? cleanupFollowStream = null;
        Boolean ownershipTransferred = false;

        try
        {
            options ??= new(); if(options.DownloadRequestSizeBytes <= 0) { return null; }

            Guid? sourceSessionId = options.SourceSessionID;

            if(options.Mode is not DataControlDownloadMode.PublishedRead && (sourceSessionId.HasValue is false || sourceSessionId.Value == Guid.Empty)) { return null; }

            if(options.Mode is DataControlDownloadMode.StreamFollow)
            {
                OpenFollowStreamTransferRequest followRequest = new()
                {
                    SessionID = Guid.NewGuid(),
                    ItemID = itemid,
                    SourceSessionID = sourceSessionId!.Value,
                };

                cleanupSessionId = followRequest.SessionID; cleanupTransferFamily = DataControlTransferFamily.Stream;

                DataControlResponse<OpenFollowStreamTransferResponse> followed = await this.OpenFollowStreamTransfer(followRequest,token,cancel).ConfigureAwait(false);

                if(followed.StatusCode != HttpStatusCode.OK || followed.Data is null) { return null; }

                remoteSessionOpened = true; cleanupSessionId = followed.Data.State.SessionID;

                DataControlClientWorkingDirectoryStorage streamStorage = new(this.Options.WorkingDirectoryOptions); cleanupStorage = streamStorage;

                Byte[] objectPayload = followed.Data.ObjectPayload ?? Array.Empty<Byte>();

                DataStreamTransferState localStreamState = followed.Data.State with { AppendedLength = 0 };

                if(await streamStorage.CreateStreamSession(localStreamState,objectPayload,cancel).ConfigureAwait(false) is null) { return null; }

                String followerStreamPath = streamStorage.GetStreamSessionPaths(localStreamState.SessionID).StreamPath;

                DataStreamTransferManifest manifest = DataControlClientWorkingDirectoryStorage.CreateManifest(localStreamState,objectPayload);

                DataControlTransferSessionInfo streamSession = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                    manifest,streamStorage.GetStreamSessionPaths(localStreamState.SessionID).SessionDirectoryPath);

                if(options.BindLiveStreamItem)
                {
                    DataItem? liveItem = DataItem.Deserialize(objectPayload);

                    if(liveItem is not DataStreamItem dataStreamItem) { return null; }

                    BufferedFollowStream followStream = new(followerStreamPath,0);

                    cleanupFollowStream = followStream;

                    if(dataStreamItem.BindLiveContent(followStream) is false) { return null; }

                    streamSession = streamSession with { SupportsLiveFollowBinding = true };

                    DataControlDownloadSession liveDownloadSession = await this.CreateDownloadSessionAsync(streamStorage,token,streamSession,followStream,dataStreamItem,options.DownloadRequestSizeBytes,cancel).ConfigureAwait(false);

                    ownershipTransferred = true; cleanupFollowStream = null;

                    return liveDownloadSession;
                }

                DataControlDownloadSession resultWithoutLiveBinding = await this.CreateDownloadSessionAsync(streamStorage,token,streamSession,downloadRequestSizeBytes:options.DownloadRequestSizeBytes,cancel:cancel).ConfigureAwait(false);

                ownershipTransferred = true;

                return resultWithoutLiveBinding;
            }

            OpenGetTransferRequest request = new()
            {
                SessionID = Guid.NewGuid(),
                ItemID = itemid,
                SourceSessionID = options.Mode is DataControlDownloadMode.PublishedRead ? null : sourceSessionId,
            };

            DataControlResponse<OpenGetTransferResponse> opened = await this.OpenGetTransfer(request,token,cancel).ConfigureAwait(false);

            if(opened.StatusCode != HttpStatusCode.OK || opened.Data is null) { return null; }

            remoteSessionOpened = true; cleanupSessionId = opened.Data.State.SessionID;

            DataItemTransferState localState = opened.Data.State.Mode == DataItemTransferMode.PublishedRead
                ? opened.Data.State with
                {
                    Status = opened.Data.State.StreamLength == 0 ? DataItemTransferStatus.Complete : DataItemTransferStatus.Open,

                    RealizedRanges = Array.Empty<DataItemTransferRange>()
                }
                : opened.Data.State;

            DataControlClientWorkingDirectoryStorage storage = new(this.Options.WorkingDirectoryOptions);

            cleanupStorage = storage; cleanupSessionId = request.SessionID;

            cleanupTransferFamily = DataControlTransferFamily.Segmented;

            if(await storage.CreateSession(localState,opened.Data.ObjectPayload,cancel).ConfigureAwait(false) is null) { return null; }

            DataControlTransferSessionInfo session = 
                DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                    DataControlClientWorkingDirectoryStorage.CreateManifest(localState),
                    storage.GetSessionPaths(localState.SessionID).SessionDirectoryPath);

            DataControlDownloadSession result = await this.CreateDownloadSessionAsync(storage,token,session,downloadRequestSizeBytes:options.DownloadRequestSizeBytes,cancel:cancel).ConfigureAwait(false);

            ownershipTransferred = true;

            return result;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenDownloadFail); if(NoExceptions) { return null; } throw; }
        finally
        {
            if(ownershipTransferred is false && remoteSessionOpened && cleanupSessionId != Guid.Empty)
            {
                await this.CleanupFailedDownloadOpenAsync(cleanupStorage,cleanupSessionId,itemid,cleanupTransferFamily,cleanupFollowStream,token).ConfigureAwait(false);
            }
        }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="CleanupFailedDownloadOpenAsync"]/*'/>*/
    private async Task CleanupFailedDownloadOpenAsync(DataControlClientWorkingDirectoryStorage? storage , Guid sessionid , Guid itemid , DataControlTransferFamily? transferfamily , BufferedFollowStream? followstream , String? token)
    {
        try
        {
            followstream?.Dispose();
        }
        catch ( Exception cleanupError )
        {
            KusDepotLog.Error(cleanupError,OpenDownloadFail);
        }

        using CancellationTokenSource remoteCleanup = new(OpenDownloadCleanupTimeout);

        try
        {
            DataControlResponse<CloseTransferSessionResponse> closed = await this.CloseTransferSession(new CloseTransferSessionRequest()
            {
                SessionID = sessionid,
                ItemID = itemid,
            },this.ResolveToken(token),remoteCleanup.Token).ConfigureAwait(false);

            if(closed.StatusCode != HttpStatusCode.OK || closed.Data?.Closed is not true)
            {
                KusDepotLog.Error(TransferOpenCleanupFail,sessionid,itemid,"CloseDownloadTransfer");
            }
        }
        catch ( Exception cleanupError )
        {
            KusDepotLog.Error(cleanupError,OpenDownloadFail);
        }

        if(storage is null || transferfamily is null) { return; }

        using CancellationTokenSource localCleanup = new(OpenDownloadCleanupTimeout);

        try
        {
            Boolean deleted = transferfamily is DataControlTransferFamily.Stream
                ? await storage.DeleteStreamSession(sessionid,localCleanup.Token).ConfigureAwait(false)
                : await storage.DeleteSession(sessionid,localCleanup.Token).ConfigureAwait(false);

            if(deleted is false)
            {
                KusDepotLog.Error(TransferOpenCleanupFail,sessionid,itemid,"DeleteDownloadSession");
            }
        }
        catch ( Exception cleanupError )
        {
            KusDepotLog.Error(cleanupError,OpenDownloadFail);
        }
    }

    ///<inheritdoc/>
    public async Task<DataControlUploadSession?> OpenUpload(DataItem item , String? token = null , DataControlUploadOptions? options = null , CancellationToken cancel = default)
    {
        Stream? streamSource = null;
        Boolean streamSourceTransferred = false;
        PreparedUploadSource? preparedSource = null;
        DataControlClientWorkingDirectoryStorage? cleanupStorage = null;
        Guid cleanupSessionId = Guid.Empty; Guid cleanupItemId = Guid.Empty;
        DataControlTransferFamily? cleanupTransferFamily = null;
        Boolean ownershipTransferred = false;

        try
        {
            if(item is null) { return null; } options ??= new();

            if(item.GetLocked()) { return null; }

            if(item is DataStreamItem)
            {
                streamSource = item.GetContentStream();

                if(streamSource is null) { return null; }

                if(streamSource.CanSeek) { streamSource.Position = 0; }

                Byte[] objectPayload = item.Serialize();

                if(objectPayload.Length == 0) { return null; }

                Byte[] objectHash = objectPayload.Length >= LargeObjectPayloadSize
                    ? await ComputeSHA512Async(objectPayload,MiB,cancel).ConfigureAwait(false)
                    : SHA512.HashData(objectPayload);

                Guid? itemId = item.GetID();

                if(!itemId.HasValue || itemId.Value == Guid.Empty) { return null; }

                OpenStreamTransferRequest streamRequest = new()
                {
                    SessionID = Guid.NewGuid(),
                    ItemID = itemId.Value,
                    ObjectPayload = objectPayload,
                    ObjectSHA512 = objectHash,
                    RequestedSegmentSizePolicy = options.RequestedSegmentSizePolicy,
                    ObjectInfo = new DataItemTransferObjectInfo()
                    {
                        ObjectType = item.GetType().FullName,
                        DataType = item.GetDataType(),
                        Name = item.GetName(),
                        ContentStreamed = true,
                    },
                };

                cleanupSessionId = streamRequest.SessionID; cleanupItemId = streamRequest.ItemID; cleanupTransferFamily = DataControlTransferFamily.Stream;

                DataControlResponse<OpenStreamTransferResponse> openedStream = await this.OpenStreamTransfer(streamRequest,token,cancel).ConfigureAwait(false);

                if(openedStream.StatusCode != HttpStatusCode.OK || openedStream.Data is null) { return null; }

                cleanupSessionId = openedStream.Data.State.SessionID;

                DataControlClientWorkingDirectoryStorage streamStorage = new(this.Options.WorkingDirectoryOptions); cleanupStorage = streamStorage;

                if(await streamStorage.CreateStreamSession(openedStream.Data.State,objectPayload,cancel).ConfigureAwait(false) is null) { return null; }

                DataStreamTransferManifest manifest = DataControlClientWorkingDirectoryStorage.CreateManifest(openedStream.Data.State,objectPayload);

                DataControlTransferSessionInfo streamSession = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                    manifest,streamStorage.GetStreamSessionPaths(openedStream.Data.State.SessionID).SessionDirectoryPath);

                DataControlUploadSession streamUploadSession = new(this,streamStorage,token,streamSession,streamSource,options.CompletionMode,options.MaxBytes);

                streamSourceTransferred = true; ownershipTransferred = true;

                return streamUploadSession;
            }

            Boolean contentStreamed = item.GetContentStreamed();

            var upload = await PrepareUploadAsync().ConfigureAwait(false);

            if(upload is null || upload.Value.descriptor.ID is null) { return null; }

            preparedSource = contentStreamed ? await this.PrepareFiniteUploadSource(item,options,cancel).ConfigureAwait(false) : null;

            Int64 streamLength = 0;

            if(contentStreamed)
            {
                if(preparedSource is null) { return null; }

                streamLength = preparedSource.Length;

                if(streamLength <= 0) { return null; }
            }

            OpenUploadTransferRequest request = new()
            {
                SessionID = Guid.NewGuid(),
                ItemID = upload.Value.descriptor.ID.Value,
                ObjectPayload = upload.Value.objectPayload,
                ObjectSHA512 = upload.Value.objectSha512,
                RequestedSegmentSizePolicy = options.RequestedSegmentSizePolicy,
                StreamLength = streamLength,
                ObjectInfo = new DataItemTransferObjectInfo()
                {
                    ObjectType = item.GetType().FullName,
                    DataType = item.GetDataType(),
                    Name = item.GetName(),
                    ContentStreamed = item.GetContentStreamed(),
                },
            };

            cleanupSessionId = request.SessionID; cleanupItemId = request.ItemID;
            cleanupTransferFamily = DataControlTransferFamily.Segmented;

            DataControlResponse<OpenUploadTransferResponse> opened = await this.OpenUploadTransfer(request,token,cancel).ConfigureAwait(false);

            if(opened.StatusCode != HttpStatusCode.OK || opened.Data is null) { return null; }

            cleanupSessionId = opened.Data.State.SessionID;

            DataControlClientWorkingDirectoryStorage storage = new(this.Options.WorkingDirectoryOptions); if(preparedSource?.Source?.CanSeek is true) { preparedSource.Source.Position = 0; }
            cleanupStorage = storage;

            if(await storage.CreateSession(opened.Data.State,request.ObjectPayload,cancel).ConfigureAwait(false) is null) { return null; }

            if(contentStreamed && preparedSource!.Descriptor.Kind is DataControlTransferSourceKind.WorkingDirectoryCopy
                && await storage.SaveSourceStream(opened.Data.State.SessionID,preparedSource.Source!,streamLength,cancel).ConfigureAwait(false) is false) { return null; }

            if(contentStreamed && await storage.SaveUploadSourceDescriptor(opened.Data.State.SessionID,preparedSource!.Descriptor,cancel:cancel).ConfigureAwait(false) is false) { return null; }

            DataControlTransferSessionInfo session =
                DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                    DataControlClientWorkingDirectoryStorage.CreateManifest(opened.Data.State),
                    storage.GetSessionPaths(opened.Data.State.SessionID).SessionDirectoryPath,
                    preparedSource?.Descriptor);

            DataControlUploadSession uploadSession = new(this,storage,token,session,completionmode:options.CompletionMode,maxbytes:options.MaxBytes);

            ownershipTransferred = true;

            return uploadSession;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenUploadFail); if(NoExceptions) { return null; } throw; }
        finally
        {
            await this.CleanupFailedUploadOpenAsync(cleanupStorage,cleanupSessionId,cleanupItemId,cleanupTransferFamily,ownershipTransferred,streamSource,streamSourceTransferred,preparedSource?.Source,token).ConfigureAwait(false);
        }

        async Task<(Descriptor descriptor, Byte[] objectPayload, Byte[] objectSha512)?> PrepareUploadAsync()
        {
            Descriptor? descriptor = item.GetDescriptor(); if(descriptor is null) { return null; }

            Byte[] objectPayload = item.Serialize(); if(objectPayload is not { Length: > 0 }) { return null; }

            using MemoryStream objectStream = new(objectPayload,writable:false);

            Task<Byte[]> objectHashTask = SHA512.HashDataAsync(objectStream,cancel).AsTask();

            return (descriptor,objectPayload,await objectHashTask.ConfigureAwait(false));
        }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="CleanupFailedUploadOpenAsync"]/*'/>*/
    private async Task CleanupFailedUploadOpenAsync(DataControlClientWorkingDirectoryStorage? storage , Guid sessionid , Guid itemid , DataControlTransferFamily? transferfamily , Boolean ownershipTransferred , Stream? streamsource , Boolean streamSourceTransferred , Stream? preparedsource , String? token)
    {
        if(streamsource is not null && streamSourceTransferred is false)
        {
            try
            {
                if(streamsource is IAsyncDisposable d) { await d.DisposeAsync().ConfigureAwait(false); } else { streamsource.Dispose(); }
            }
            catch ( Exception cleanupError )
            {
                KusDepotLog.Error(cleanupError,OpenUploadFail);
            }
        }

        if(preparedsource is not null)
        {
            try
            {
                if(preparedsource is IAsyncDisposable d) { await d.DisposeAsync().ConfigureAwait(false); } else { preparedsource.Dispose(); }
            }
            catch ( Exception cleanupError )
            {
                KusDepotLog.Error(cleanupError,OpenUploadFail);
            }
        }

        if(ownershipTransferred || sessionid == Guid.Empty || itemid == Guid.Empty) { return; }

        using CancellationTokenSource remoteCleanup = new(OpenUploadCleanupTimeout);

        try
        {
            DataControlResponse<AbortTransferResponse> aborted = await this.AbortTransfer(new AbortTransferRequest()
            {
                SessionID = sessionid,
                ItemID = itemid,
            },this.ResolveToken(token),remoteCleanup.Token).ConfigureAwait(false);

            if(aborted.StatusCode != HttpStatusCode.OK || aborted.Data?.Aborted is not true)
            {
                if(aborted.StatusCode != HttpStatusCode.NotFound)
                {
                    KusDepotLog.Error(TransferOpenCleanupFail,sessionid,itemid,"AbortUploadTransfer");
                }
            }
        }
        catch ( Exception cleanupError )
        {
            KusDepotLog.Error(cleanupError,OpenUploadFail);
        }

        if(storage is null || transferfamily is null) { return; }

        using CancellationTokenSource localCleanup = new(OpenUploadCleanupTimeout);

        try
        {
            Boolean deleted = transferfamily is DataControlTransferFamily.Stream
                ? await storage.DeleteStreamSession(sessionid,localCleanup.Token).ConfigureAwait(false)
                : await storage.DeleteSession(sessionid,localCleanup.Token).ConfigureAwait(false);

            if(deleted is false)
            {
                KusDepotLog.Error(TransferOpenCleanupFail,sessionid,itemid,"DeleteUploadSession");
            }
        }
        catch ( Exception cleanupError )
        {
            KusDepotLog.Error(cleanupError,OpenUploadFail);
        }
    }

    ///<inheritdoc/>
    public async Task<DataControlUploadCompletionResult?> Upload(DataItem item , String? token = null , DataControlUploadOptions? options = null , IProgress<DataItemTransferProgress>? progress = null , CancellationToken cancel = default)
    {
        DataControlUploadSession? session = await this.OpenUpload(item,token,options,cancel).ConfigureAwait(false); if(session is null) { return null; }

        await using (session.ConfigureAwait(false))
        {
            await session.UploadToCompletion(progress,cancel).ConfigureAwait(false);

            return await session.Publish(cancel:cancel).ConfigureAwait(false);
        }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="PrepareFiniteUploadSource"]/*'/>*/
    private async Task<PreparedUploadSource?> PrepareFiniteUploadSource(DataItem item , DataControlUploadOptions options , CancellationToken cancel)
    {
        String? stableFilePath = item.GetFILE();

        if(String.IsNullOrWhiteSpace(stableFilePath) is false && File.Exists(stableFilePath))
        {
            FileInfo file = new(Path.GetFullPath(stableFilePath));

            return new(null,new DataControlTransferSourceDescriptor(){ Kind = DataControlTransferSourceKind.ExternalFile , Path = file.FullName , Length = file.Length },file.Length);
        }

        Stream? source = item.GetContentStream(); if(source is null) { return null; }

        if(source.CanSeek) { source.Position = 0; }

        String rootPath = String.IsNullOrWhiteSpace(options.TemporaryStagingPath) ? Path.GetTempPath() : options.TemporaryStagingPath!;

        Directory.CreateDirectory(rootPath);

        String path = Path.Combine(rootPath,$"KusDepot.DataControlClient.{Guid.NewGuid():N}.upload.tmp");

        FileStream staged = new(path,FileMode.CreateNew,FileAccess.ReadWrite,FileShare.None,524288,FileOptions.Asynchronous | FileOptions.DeleteOnClose | FileOptions.SequentialScan);

        try
        {
            await source.CopyToAsync(staged,cancel).ConfigureAwait(false);

            staged.Position = 0;

            await source.DisposeAsync().ConfigureAwait(false);

            return new(staged,new DataControlTransferSourceDescriptor(){ Kind = DataControlTransferSourceKind.WorkingDirectoryCopy , Length = staged.Length },staged.Length);
        }
        catch
        {
            await staged.DisposeAsync().ConfigureAwait(false);

            await source.DisposeAsync().ConfigureAwait(false);

            throw;
        }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/record[@name="PreparedUploadSource"]/main/*'/>*/
    private sealed record PreparedUploadSource
    {
        /**<include file='DataControlClient.xml' path='DataControlClient/record[@name="PreparedUploadSource"]/property[@name="Source"]/*'/>*/
        internal Stream? Source { get; init; }

        /**<include file='DataControlClient.xml' path='DataControlClient/record[@name="PreparedUploadSource"]/property[@name="Descriptor"]/*'/>*/
        internal DataControlTransferSourceDescriptor Descriptor { get; init; }
        
        /**<include file='DataControlClient.xml' path='DataControlClient/record[@name="PreparedUploadSource"]/property[@name="Length"]/*'/>*/
        internal Int64 Length { get; init; }

        /**<include file='DataControlClient.xml' path='DataControlClient/record[@name="PreparedUploadSource"]/constructor[@name="Constructor"]/*'/>*/
        internal PreparedUploadSource(Stream? source , DataControlTransferSourceDescriptor descriptor , Int64 length)
        {
            this.Source = source; this.Descriptor = descriptor; this.Length = length;
        }
    }
}