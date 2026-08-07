namespace KusDepot.Data.Clients;

public sealed partial class DataControlClient
{
    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/property[@name="WorkingDirectoryOptions"]/*'/>*/
    public DataControlClientWorkingDirectoryOptions WorkingDirectoryOptions { get; init; } = new();

    ///<inheritdoc/>
    public async Task<DataControlDownloadCompletionResult?> Download(Guid itemid , String? token = null , DataControlDownloadOptions? options = null , IProgress<DataItemTransferProgress>? progress = null , CancellationToken cancel = default)
    {
        DataControlDownloadSession? session = await this.OpenDownload(itemid,token,options,cancel).ConfigureAwait(false);

        if(session is null) { return null; }

        await session.DownloadToCompletion(progress,cancel).ConfigureAwait(false);

        return await session.Materialize(cancel).ConfigureAwait(false);
    }

    ///<inheritdoc/>
    public async Task<DataControlDownloadSession?> OpenDownload(Guid itemid , String? token = null , DataControlDownloadOptions? options = null , CancellationToken cancel = default)
    {
        try
        {
            options ??= new();

            Guid? sourceSessionId = options.SourceSessionID;

            if(options.Mode is not DataControlDownloadMode.Committed && (sourceSessionId.HasValue is false || sourceSessionId.Value == Guid.Empty)) { return null; }

            if(options.Mode is DataControlDownloadMode.StreamFollow)
            {
                RestResponse<OpenFollowStreamTransferResponse> followed = await this.OpenFollowStreamTransfer(new()
                {
                    SessionID = Guid.NewGuid(),
                    ItemID = itemid,
                    SourceSessionID = sourceSessionId!.Value,
                },token,cancel).ConfigureAwait(false);

                if(followed.StatusCode != HttpStatusCode.OK || followed.Data is null) { return null; }

                DataControlClientWorkingDirectoryStorage streamStorage = new(this.WorkingDirectoryOptions);

                Byte[] objectPayload = followed.Data.ObjectPayload ?? Array.Empty<Byte>();

                if(await streamStorage.CreateStreamSession(followed.Data.State,objectPayload,cancel).ConfigureAwait(false) is null) { return null; }

                String followerStreamPath = streamStorage.GetStreamSessionPaths(followed.Data.State.SessionID).StreamPath;

                if(followed.Data.State.AppendedLength > 0)
                {
                    FileStream followerStream = new(followerStreamPath,
                        new FileStreamOptions(){ Access = FileAccess.Write , Mode = FileMode.Open , Share = FileShare.Read,
                        Options = FileOptions.Asynchronous | FileOptions.SequentialScan });

                    await using (followerStream.ConfigureAwait(false))
                    {
                        StreamSegmentDownloadInfo? initialSegment = await this.DownloadStreamTransferSegmentToStream(new()
                        {
                            SessionID = followed.Data.State.SessionID,
                            ItemID = itemid,
                            Offset = 0,
                            Length = followed.Data.State.AppendedLength,
                        },followerStream,token,cancel).ConfigureAwait(false);

                        if(initialSegment is null || initialSegment.StatusCode != HttpStatusCode.OK || initialSegment.Footer.ReturnedLength != followed.Data.State.AppendedLength) { return null; }

                        await followerStream.FlushAsync(cancel).ConfigureAwait(false);
                    }
                }

                DataStreamTransferManifest manifest = DataControlClientWorkingDirectoryStorage.CreateManifest(followed.Data.State,objectPayload);

                DataControlTransferSessionInfo streamSession = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                    manifest,streamStorage.GetStreamSessionPaths(followed.Data.State.SessionID).SessionDirectoryPath);

                if(options.BindLiveStreamItem)
                {
                    DataItem? liveItem = DataItem.Deserialize(objectPayload);

                    if(liveItem is not DataStreamItem dataStreamItem) { return null; }

                    BufferedFollowStream followStream = new(followerStreamPath,followed.Data.State.AppendedLength);

                    if(dataStreamItem.BindLiveContent(followStream) is false) { followStream.Dispose(); return null; }

                    streamSession = streamSession with { SupportsLiveFollowBinding = true };

                    return await this.CreateDownloadSessionAsync(streamStorage,token,streamSession,options.CompletionMode,options.MaxBytes,followStream,dataStreamItem,cancel).ConfigureAwait(false);
                }

                return await this.CreateDownloadSessionAsync(streamStorage,token,streamSession,options.CompletionMode,options.MaxBytes,null,null,cancel).ConfigureAwait(false);
            }

            OpenGetTransferRequest request = new()
            {
                SessionID = Guid.NewGuid(),
                ItemID = itemid,
                SourceSessionID = options.Mode is DataControlDownloadMode.Committed ? null : sourceSessionId,
            };

            RestResponse<OpenGetTransferResponse> opened = await this.OpenGetTransfer(request,token,cancel).ConfigureAwait(false);

            if(opened.StatusCode != HttpStatusCode.OK || opened.Data is null) { return null; }

            DataItemTransferState localState = opened.Data.State.Mode == DataItemTransferMode.ReadCommitted
                ? opened.Data.State with
                {
                    Status = opened.Data.State.StreamLength == 0 ? DataItemTransferStatus.Complete : DataItemTransferStatus.Open,

                    RealizedRanges = Array.Empty<DataItemTransferRange>()
                }
                : opened.Data.State;

            DataControlClientWorkingDirectoryStorage storage = new(this.WorkingDirectoryOptions);

            if(await storage.CreateSession(localState,opened.Data.ObjectPayload,cancel).ConfigureAwait(false) is null) { return null; }

            DataControlTransferSessionInfo session = 
                DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                    DataControlClientWorkingDirectoryStorage.CreateManifest(localState),
                    storage.GetSessionPaths(localState.SessionID).SessionDirectoryPath);

            return await this.CreateDownloadSessionAsync(storage,token,session,options.CompletionMode,options.MaxBytes,null,null,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenDownloadFail); if(NoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlUploadSession?> OpenUpload(DataItem item , String? token = null , DataControlUploadOptions? options = null , CancellationToken cancel = default)
    {
        try
        {
            if(item is null) { return null; } options ??= new();

            if(item.GetLocked()) { return null; }

            if(item is DataStreamItem)
            {
                Stream? streamSource = item.GetContentStream();

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

                RestResponse<OpenStreamTransferResponse> openedStream = await this.OpenStreamTransfer(streamRequest,token,cancel).ConfigureAwait(false);

                if(openedStream.StatusCode != HttpStatusCode.OK || openedStream.Data is null) { return null; }

                DataControlClientWorkingDirectoryStorage streamStorage = new(this.WorkingDirectoryOptions);

                if(await streamStorage.CreateStreamSession(openedStream.Data.State,objectPayload,cancel).ConfigureAwait(false) is null) { return null; }

                DataStreamTransferManifest manifest = DataControlClientWorkingDirectoryStorage.CreateManifest(openedStream.Data.State,objectPayload);

                DataControlTransferSessionInfo streamSession = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                    manifest,streamStorage.GetStreamSessionPaths(openedStream.Data.State.SessionID).SessionDirectoryPath);

                return new DataControlUploadSession(this,streamStorage,token,streamSession,streamSource,options.CompletionMode,options.MaxBytes,options.PublishFinalStreamHash);
            }

            Boolean contentStreamed = item.GetContentStreamed();

            var upload = await PrepareUploadAsync().ConfigureAwait(false);

            if(upload is null || upload.Value.descriptor.ID is null) { return null; }

            using Stream? source = null;

            PreparedUploadSource? preparedSource = contentStreamed ? await this.PrepareFiniteUploadSource(item,options,cancel).ConfigureAwait(false) : null;

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
                StreamSHA512 = contentStreamed ? upload.Value.streamSha512 : Array.Empty<Byte>(),
                StreamLength = streamLength,
                ObjectInfo = new DataItemTransferObjectInfo()
                {
                    ObjectType = item.GetType().FullName,
                    DataType = item.GetDataType(),
                    Name = item.GetName(),
                    ContentStreamed = item.GetContentStreamed(),
                },
            };

            RestResponse<OpenUploadTransferResponse> opened = await this.OpenUploadTransfer(request,token,cancel).ConfigureAwait(false);

            if(opened.StatusCode != HttpStatusCode.OK || opened.Data is null) { return null; }

            DataControlClientWorkingDirectoryStorage storage = new(this.WorkingDirectoryOptions); if(preparedSource?.Source?.CanSeek is true) { preparedSource.Source.Position = 0; }

            if(await storage.CreateSession(opened.Data.State,request.ObjectPayload,cancel).ConfigureAwait(false) is null) { return null; }

            if(contentStreamed && preparedSource!.Descriptor.Kind is DataControlTransferSourceKind.WorkingDirectoryCopy
                && await storage.SaveSourceStream(opened.Data.State.SessionID,preparedSource.Source!,streamLength,cancel).ConfigureAwait(false) is false) { return null; }

            if(contentStreamed && await storage.SaveUploadSourceDescriptor(opened.Data.State.SessionID,preparedSource!.Descriptor,cancel:cancel).ConfigureAwait(false) is false) { return null; }

            DataControlTransferSessionInfo session =
                DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                    DataControlClientWorkingDirectoryStorage.CreateManifest(opened.Data.State),
                    storage.GetSessionPaths(opened.Data.State.SessionID).SessionDirectoryPath,
                    preparedSource?.Descriptor);

            return new DataControlUploadSession(this,storage,token,session,completionmode:options.CompletionMode,maxbytes:options.MaxBytes,publishfinalstreamhash:options.PublishFinalStreamHash,finalstreamsha512:preparedSource?.FinalStreamSHA512);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,OpenUploadFail); if(NoExceptions) { return null; } throw; }

        async Task<(Descriptor descriptor, Byte[] objectPayload, Byte[] objectSha512, Byte[] streamSha512)?> PrepareUploadAsync()
        {
            Descriptor? descriptor = item.GetDescriptor(); if(descriptor is null) { return null; }

            Byte[] objectPayload = item.Serialize(); if(objectPayload is not { Length: > 0 }) { return null; }

            using MemoryStream objectStream = new(objectPayload,writable:false);

            Task<Byte[]> objectHashTask = SHA512.HashDataAsync(objectStream,cancel).AsTask();

            using Stream? contentStream = item.GetContentStream();

            if(contentStream is not null)
            {
                Task<Byte[]> streamHashTask = SHA512.HashDataAsync(contentStream,cancel).AsTask();

                await Task.WhenAll(objectHashTask,streamHashTask).ConfigureAwait(false);

                return (descriptor,objectPayload,objectHashTask.Result,streamHashTask.Result);
            }

            return (descriptor,objectPayload,await objectHashTask.ConfigureAwait(false),Array.Empty<Byte>());
        }
    }

    ///<inheritdoc/>
    public async Task<DataControlUploadCompletionResult?> Upload(DataItem item , String? token = null , DataControlUploadOptions? options = null , IProgress<DataItemTransferProgress>? progress = null , CancellationToken cancel = default)
    {
        DataControlUploadSession? session = await this.OpenUpload(item,token,options,cancel).ConfigureAwait(false); if(session is null) { return null; }

        await session.UploadToCompletion(progress,cancel).ConfigureAwait(false);

        return await session.Commit(cancel:cancel).ConfigureAwait(false);
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
            Byte[] finalStreamSHA512;

            if(options.PublishFinalStreamHash)
            {
                HashingWriteStream hashing = new(staged,leaveopen:true);

                await using (hashing.ConfigureAwait(false))
                {
                    await source.CopyToAsync(hashing,cancel).ConfigureAwait(false);

                    finalStreamSHA512 = hashing.GetHashAndReset();
                }
            }
            else
            {
                await source.CopyToAsync(staged,cancel).ConfigureAwait(false);

                finalStreamSHA512 = Array.Empty<Byte>();
            }

            staged.Position = 0;

            await source.DisposeAsync().ConfigureAwait(false);

            return new(staged,new DataControlTransferSourceDescriptor(){ Kind = DataControlTransferSourceKind.WorkingDirectoryCopy , Length = staged.Length },staged.Length,finalStreamSHA512);
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

        /**<include file='DataControlClient.xml' path='DataControlClient/record[@name="PreparedUploadSource"]/property[@name="FinalStreamSHA512"]/*'/>*/
        internal Byte[] FinalStreamSHA512 { get; init; } = Array.Empty<Byte>();

        /**<include file='DataControlClient.xml' path='DataControlClient/record[@name="PreparedUploadSource"]/constructor[@name="Constructor"]/*'/>*/
        internal PreparedUploadSource(Stream? source , DataControlTransferSourceDescriptor descriptor , Int64 length , Byte[]? finalstreamsha512 = null)
        {
            this.Source = source; this.Descriptor = descriptor; this.Length = length; this.FinalStreamSHA512 = finalstreamsha512 ?? Array.Empty<Byte>();
        }
    }
}