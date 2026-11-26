namespace KusDepot.Data.Clients;

/**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/main/*'/>*/
internal sealed class DataControlClientWorkingDirectoryStorage
{
    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/field[@name="Storage"]/*'/>*/
    private readonly SegmentedTransferStorage Storage;

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/field[@name="StreamStorage"]/*'/>*/
    private readonly StreamTransferStorage StreamStorage;

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/constructor[@name="DataControlClientWorkingDirectoryStorage"]/*'/>*/
    public DataControlClientWorkingDirectoryStorage(DataControlClientWorkingDirectoryOptions? options = null)
    {
        this.RootPath = GetSegmentedStorageRootPath(options);

        this.StreamRootPath = GetStreamStorageRootPath(options);

        this.Storage = new SegmentedTransferStorage(Options.Create(new TransferStorageOptions(){ RootPath = this.RootPath }));

        this.StreamStorage = new StreamTransferStorage(Options.Create(new TransferStorageOptions(){ RootPath = this.StreamRootPath }));
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/property[@name="RootPath"]/*'/>*/
    public String RootPath { get; }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/property[@name="StreamRootPath"]/*'/>*/
    public String StreamRootPath { get; }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/field[@name="UploadSourceDescriptorFileName"]/*'/>*/
    private const String UploadSourceDescriptorFileName = "upload-source.json";

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="CreateSession"]/*'/>*/
    public Task<SegmentedTransferStorageSessionPaths?> CreateSession(DataItemTransferState state , Byte[] objectpayload , CancellationToken cancel = default)
    {
        return this.Storage.CreateSession(CreateManifest(state),objectpayload,cancel);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="CreateStreamSession"]/*'/>*/
    public Task<StreamTransferStorageSessionPaths?> CreateStreamSession(DataStreamTransferState state , Byte[] objectpayload , CancellationToken cancel = default)
    {
        return this.StreamStorage.CreateSession(CreateManifest(state,objectpayload),cancel);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="DeleteSession"]/*'/>*/
    public Task<Boolean> DeleteSession(Guid sessionid , CancellationToken cancel = default)
    {
        return this.Storage.DeleteSession(sessionid,cancel);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="DeleteStreamSession"]/*'/>*/
    public Task<Boolean> DeleteStreamSession(Guid sessionid , CancellationToken cancel = default)
    {
        return this.StreamStorage.DeleteSession(sessionid,cancel);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="GetSessionPaths"]/*'/>*/
    public SegmentedTransferStorageSessionPaths GetSessionPaths(Guid sessionid)
    {
        return this.Storage.GetSessionPaths(sessionid);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="GetStreamSessionPaths"]/*'/>*/
    public StreamTransferStorageSessionPaths GetStreamSessionPaths(Guid sessionid)
    {
        return this.StreamStorage.GetSessionPaths(sessionid);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="ReconcileStreamFollowFile"]/*'/>*/
    public Boolean ReconcileStreamFollowFile(Guid sessionid , Int64 checkpoint)
    {
        if(checkpoint < 0) { return false; }

        String path = this.GetStreamSessionPaths(sessionid).StreamPath;

        if(File.Exists(path) is false) { return false; }

        using FileStream stream = new(path,new FileStreamOptions(){ Access = FileAccess.Write , Mode = FileMode.Open,
            Share = FileShare.ReadWrite , Options = FileOptions.Asynchronous | FileOptions.SequentialScan });

        if(stream.Length < checkpoint) { return false; }

        if(stream.Length > checkpoint) { stream.SetLength(checkpoint); }

        return true;
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="LoadSessionInfo"]/*'/>*/
    public async Task<DataControlTransferSessionInfo?> LoadSessionInfo(Guid sessionid , CancellationToken cancel = default)
    {
        DataItemTransferManifest? manifest = await this.Storage.LoadManifest(sessionid,cancel).ConfigureAwait(false);

        if(manifest is null) { return null; }

        DataControlTransferSourceDescriptor? source = await this.LoadUploadSourceDescriptor(sessionid,cancel:cancel).ConfigureAwait(false);

        return CreateSessionInfo(manifest,this.GetSessionPaths(sessionid).SessionDirectoryPath,source);
    }

    public async Task<DataControlTransferSessionInfo?> LoadStreamSessionInfo(Guid sessionid , CancellationToken cancel = default)
    {
        DataStreamTransferManifest? manifest = await this.StreamStorage.LoadManifest(sessionid,cancel).ConfigureAwait(false);

        if(manifest is null) { return null; }

        DataControlTransferSourceDescriptor? source = await this.LoadUploadSourceDescriptor(sessionid,stream:true,cancel:cancel).ConfigureAwait(false);

        return CreateSessionInfo(manifest,this.GetStreamSessionPaths(sessionid).SessionDirectoryPath,source);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="LoadState"]/*'/>*/
    public async Task<DataItemTransferState?> LoadState(Guid sessionid , CancellationToken cancel = default)
    {
        DataItemTransferManifest? manifest = await this.Storage.LoadManifest(sessionid,cancel).ConfigureAwait(false);

        return manifest?.ToState();
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="LoadStreamState"]/*'/>*/
    public async Task<DataStreamTransferState?> LoadStreamState(Guid sessionid , CancellationToken cancel = default)
    {
        DataStreamTransferManifest? manifest = await this.StreamStorage.LoadManifest(sessionid,cancel).ConfigureAwait(false);

        return manifest?.ToState();
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="ReadRange"]/*'/>*/
    public Task<Stream?> ReadRange(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default)
    {
        return this.Storage.ReadRange(sessionid,range,cancel);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="ReadObjectPayload"]/*'/>*/
    public Task<Byte[]?> ReadObjectPayload(Guid sessionid , CancellationToken cancel = default)
    {
        return this.Storage.ReadObjectPayload(sessionid,cancel);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="ReplaceSegmentedSessionIdentity"]/*'/>*/
    public async Task<DataControlTransferSessionInfo?> ReplaceSegmentedSessionIdentity(Guid sessionid , DataItemTransferState replacementstate , CancellationToken cancel = default)
    {
        try
        {
            if(sessionid == Guid.Empty || replacementstate.SessionID == Guid.Empty) { return null; }

            if(sessionid == replacementstate.SessionID)
            {
                if(await this.SaveState(replacementstate,cancel).ConfigureAwait(false) is false) { return null; }

                DataControlTransferSourceDescriptor? existingSource = await this.LoadUploadSourceDescriptor(replacementstate.SessionID,cancel:cancel).ConfigureAwait(false);

                return CreateSessionInfo(CreateManifest(replacementstate),this.GetSessionPaths(replacementstate.SessionID).SessionDirectoryPath,existingSource);
            }

            SegmentedTransferStorageSessionPaths sourcePaths = this.GetSessionPaths(sessionid);

            if(Directory.Exists(sourcePaths.SessionDirectoryPath) is false) { return null; }

            DataItemTransferState? originalState = await this.LoadState(sessionid,cancel).ConfigureAwait(false);

            if(originalState is null) { return null; }

            SegmentedTransferStorageSessionPaths targetPaths = this.GetSessionPaths(replacementstate.SessionID);

            if(Directory.Exists(targetPaths.SessionDirectoryPath)) { return null; }

            cancel.ThrowIfCancellationRequested();

            Directory.Move(sourcePaths.SessionDirectoryPath,targetPaths.SessionDirectoryPath);

            if(await this.SaveState(replacementstate,cancel).ConfigureAwait(false) is false)
            {
                await this.RestoreMovedSegmentedSessionAsync(sourcePaths,targetPaths,originalState,CancellationToken.None).ConfigureAwait(false);

                return null;
            }

            DataControlTransferSourceDescriptor? source = await this.LoadUploadSourceDescriptor(replacementstate.SessionID,cancel:cancel).ConfigureAwait(false);

            return CreateSessionInfo(CreateManifest(replacementstate),targetPaths.SessionDirectoryPath,source);
        }
        catch { return null; }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="ReplaceStreamSessionIdentity"]/*'/>*/
    public async Task<DataControlTransferSessionInfo?> ReplaceStreamSessionIdentity(Guid sessionid , DataStreamTransferState replacementstate , CancellationToken cancel = default)
    {
        try
        {
            if(sessionid == Guid.Empty || replacementstate.SessionID == Guid.Empty) { return null; }

            if(sessionid == replacementstate.SessionID)
            {
                Byte[] objectPayload = await this.ReadStreamObjectPayload(replacementstate.SessionID,cancel).ConfigureAwait(false) ?? Array.Empty<Byte>();

                if(await this.SaveState(replacementstate,objectPayload,cancel).ConfigureAwait(false) is false) { return null; }

                DataControlTransferSourceDescriptor? existingSource = await this.LoadUploadSourceDescriptor(replacementstate.SessionID,stream:true,cancel:cancel).ConfigureAwait(false);

                return CreateSessionInfo(CreateManifest(replacementstate,objectPayload),this.GetStreamSessionPaths(replacementstate.SessionID).SessionDirectoryPath,existingSource);
            }

            StreamTransferStorageSessionPaths sourcePaths = this.GetStreamSessionPaths(sessionid);

            if(Directory.Exists(sourcePaths.SessionDirectoryPath) is false) { return null; }

            DataStreamTransferState? originalState = await this.LoadStreamState(sessionid,cancel).ConfigureAwait(false);

            if(originalState is null) { return null; }

            Byte[] objectPayloadForMove = await this.ReadStreamObjectPayload(sessionid,cancel).ConfigureAwait(false) ?? Array.Empty<Byte>();

            if(objectPayloadForMove.Length == 0) { return null; }

            StreamTransferStorageSessionPaths targetPaths = this.GetStreamSessionPaths(replacementstate.SessionID);

            if(Directory.Exists(targetPaths.SessionDirectoryPath)) { return null; }

            cancel.ThrowIfCancellationRequested();

            Directory.Move(sourcePaths.SessionDirectoryPath,targetPaths.SessionDirectoryPath);

            if(await this.SaveState(replacementstate,objectPayloadForMove,cancel).ConfigureAwait(false) is false)
            {
                await this.RestoreMovedStreamSessionAsync(sourcePaths,targetPaths,originalState,objectPayloadForMove,CancellationToken.None).ConfigureAwait(false);

                return null;
            }

            DataControlTransferSourceDescriptor? source = await this.LoadUploadSourceDescriptor(replacementstate.SessionID,stream:true,cancel:cancel).ConfigureAwait(false);

            return CreateSessionInfo(CreateManifest(replacementstate,objectPayloadForMove),targetPaths.SessionDirectoryPath,source);
        }
        catch { return null; }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="ReplaceStreamSessionWithSegmentedIdentity"]/*'/>*/
    public async Task<DataControlTransferSessionInfo?> ReplaceStreamSessionWithSegmentedIdentity(Guid sessionid , DataItemTransferState replacementstate , CancellationToken cancel = default)
    {
        try
        {
            if(sessionid == Guid.Empty || replacementstate.SessionID == Guid.Empty) { return null; }

            StreamTransferStorageSessionPaths sourcePaths = this.GetStreamSessionPaths(sessionid);

            if(Directory.Exists(sourcePaths.SessionDirectoryPath) is false) { return null; }

            Byte[] objectPayload = await this.ReadStreamObjectPayload(sessionid,cancel).ConfigureAwait(false) ?? Array.Empty<Byte>();

            if(objectPayload.Length == 0) { return null; }

            SegmentedTransferStorageSessionPaths targetPaths = this.GetSessionPaths(replacementstate.SessionID);

            cancel.ThrowIfCancellationRequested();

            Boolean createdTarget = Directory.Exists(targetPaths.SessionDirectoryPath) is false;

            SegmentedTransferStorageSessionPaths? created = createdTarget
                ? await this.CreateSession(replacementstate,objectPayload,cancel).ConfigureAwait(false)
                : targetPaths;

            if(created is null) { return null; }

            if(createdTarget is false)
            {
                Byte[]? existingObjectPayload = await this.ReadObjectPayload(replacementstate.SessionID,cancel).ConfigureAwait(false);

                if(existingObjectPayload is null || existingObjectPayload.Length == 0) { return null; }

                if(await this.SaveState(replacementstate,cancel).ConfigureAwait(false) is false) { return null; }
            }

            if(File.Exists(sourcePaths.StreamPath))
            {
                FileStream sourceStream = new(sourcePaths.StreamPath,new FileStreamOptions(){ Access = FileAccess.Read , Mode = FileMode.Open,
                    Share = FileShare.ReadWrite , Options = FileOptions.Asynchronous | FileOptions.SequentialScan });

                await using (sourceStream.ConfigureAwait(false))
                {
                    if(await this.SaveSourceStream(replacementstate.SessionID,sourceStream,sourceStream.Length,cancel).ConfigureAwait(false) is false)
                    {
                        if(createdTarget) { await this.DeleteSession(replacementstate.SessionID,cancel).ConfigureAwait(false); }

                        return null;
                    }
                }
            }

            DataControlTransferSourceDescriptor? sourceDescriptor = await this.LoadUploadSourceDescriptor(sessionid,stream:true,cancel:cancel).ConfigureAwait(false);

            if(sourceDescriptor is not null && await this.SaveUploadSourceDescriptor(replacementstate.SessionID,sourceDescriptor,cancel:cancel).ConfigureAwait(false) is false)
            {
                if(createdTarget) { await this.DeleteSession(replacementstate.SessionID,cancel).ConfigureAwait(false); }

                return null;
            }

            if(await this.DeleteStreamSession(sessionid,cancel).ConfigureAwait(false) is false)
            {
                if(createdTarget) { await this.DeleteSession(replacementstate.SessionID,cancel).ConfigureAwait(false); }

                return null;
            }

            return CreateSessionInfo(CreateManifest(replacementstate),created.SessionDirectoryPath,sourceDescriptor);
        }
        catch { return null; }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="ReadStreamObjectPayload"]/*'/>*/
    public async Task<Byte[]?> ReadStreamObjectPayload(Guid sessionid , CancellationToken cancel = default)
    {
        Stream? payload = await this.StreamStorage.ReadObjectPayload(sessionid,cancel).ConfigureAwait(false);

        if(payload is null) { return null; }

        await using (payload.ConfigureAwait(false))
        {
            using MemoryStream buffered = new();

            await payload.CopyToAsync(buffered,cancel).ConfigureAwait(false);

            return buffered.ToArray();
        }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="ReadStreamRange"]/*'/>*/
    public Task<Stream?> ReadStreamRange(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default)
    {
        return this.StreamStorage.ReadRange(sessionid,range,cancel);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="SaveSourceStream"]/*'/>*/
    public async Task<Boolean> SaveSourceStream(Guid sessionid , Stream payload , Int64 length , CancellationToken cancel = default)
    {
        try
        {
            if(payload is null || length < 0) { return false; }

            SegmentedTransferStorageSessionPaths paths = this.GetSessionPaths(sessionid);

            Directory.CreateDirectory(paths.SessionDirectoryPath);

            FileStream stream = new(paths.StreamPath,
                new FileStreamOptions(){ Access = FileAccess.Write , Mode = FileMode.Open,
                Share = FileShare.Read , Options = FileOptions.Asynchronous | FileOptions.RandomAccess });

            await using (stream.ConfigureAwait(false))
            {
                stream.Position = 0;

                Byte[] buffer = ArrayPool<Byte>.Shared.Rent(80*KiB); Int64 remaining = length;

                try
                {
                    while(remaining > 0)
                    {
                        Int32 requested = (Int32)Math.Min(buffer.Length,remaining);

                        Int32 read = await payload.ReadAsync(buffer.AsMemory(0,requested),cancel).ConfigureAwait(false);

                        if(read <= 0) { return false; }

                        await stream.WriteAsync(buffer.AsMemory(0,read),cancel).ConfigureAwait(false);

                        remaining -= read;
                    }
                }
                finally
                {
                    ArrayPool<Byte>.Shared.Return(buffer);
                }

                await stream.FlushAsync(cancel).ConfigureAwait(false);

                return remaining == 0;
            }
        }
        catch { return false; }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="LoadUploadSourceDescriptor"]/*'/>*/
    public async Task<DataControlTransferSourceDescriptor?> LoadUploadSourceDescriptor(Guid sessionid , Boolean stream = false , CancellationToken cancel = default)
    {
        try
        {
            String path = this.GetUploadSourceDescriptorPath(sessionid,stream);

            if(File.Exists(path) is false) { return null; }

            String json = await File.ReadAllTextAsync(path,cancel).ConfigureAwait(false);

            if(String.IsNullOrWhiteSpace(json)) { return null; }

            return JsonSerializer.Deserialize<DataControlTransferSourceDescriptor>(json);
        }
        catch { return null; }
    }


    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="OpenUploadSourceRange"]/*'/>*/
    public Task<Stream?> OpenUploadSourceRange(Guid sessionid , DataControlTransferSourceDescriptor? descriptor , DataItemTransferRange range , CancellationToken cancel = default)
    {
        try
        {
            if(descriptor is null || range.Validate() is false) { return Task.FromResult<Stream?>(null); }

            String? path = descriptor.Kind switch
            {
                DataControlTransferSourceKind.ExternalFile => descriptor.Path,

                DataControlTransferSourceKind.WorkingDirectoryCopy => this.GetSessionPaths(sessionid).StreamPath,

                _ => null,
            };

            if(String.IsNullOrWhiteSpace(path) || File.Exists(path) is false) { return Task.FromResult<Stream?>(null); }

            cancel.ThrowIfCancellationRequested();

            Stream stream = new BoundedReadStream(new FileStream(path,
                new FileStreamOptions(){ Access = FileAccess.Read , Mode = FileMode.Open , Share = FileShare.ReadWrite,
                Options = FileOptions.Asynchronous | FileOptions.RandomAccess }),range.Offset,range.Length,leaveopen:false);

            return Task.FromResult<Stream?>(stream);
        }
        catch { return Task.FromResult<Stream?>(null); }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="SaveUploadSourceDescriptor"]/*'/>*/
    public async Task<Boolean> SaveUploadSourceDescriptor(Guid sessionid , DataControlTransferSourceDescriptor descriptor , Boolean stream = false , CancellationToken cancel = default)
    {
        try
        {
            if(descriptor is null) { return false; }

            String path = this.GetUploadSourceDescriptorPath(sessionid,stream);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            Byte[] payload = JsonSerializer.SerializeToUtf8Bytes(descriptor);

            await SaveFileAtomicallyAsync(path,payload,cancel).ConfigureAwait(false);

            return true;
        }
        catch { return false; }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="SaveFileAtomicallyAsync"]/*'/>*/
    private static async Task SaveFileAtomicallyAsync(String path , ReadOnlyMemory<Byte> payload , CancellationToken cancel)
    {
        String directory = Path.GetDirectoryName(path)!;
        String temporaryPath = Path.Combine(directory,$".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");

        try
        {
            await using (FileStream temporary = new(temporaryPath,new FileStreamOptions()
            {
                Access = FileAccess.Write,
                Mode = FileMode.CreateNew,
                Share = FileShare.None,
                Options = FileOptions.Asynchronous | FileOptions.WriteThrough,
            }))
            {
                await temporary.WriteAsync(payload,cancel).ConfigureAwait(false);

                await temporary.FlushAsync(cancel).ConfigureAwait(false);

                temporary.Flush(flushToDisk:true);
            }

            File.Move(temporaryPath,path,overwrite:true);
        }
        finally
        {
            if(File.Exists(temporaryPath)) { File.Delete(temporaryPath); }
        }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="RestoreMovedSegmentedSessionAsync"]/*'/>*/
    private async Task RestoreMovedSegmentedSessionAsync(SegmentedTransferStorageSessionPaths sourcePaths , SegmentedTransferStorageSessionPaths targetPaths , DataItemTransferState originalstate , CancellationToken cancel)
    {
        try
        {
            if(Directory.Exists(sourcePaths.SessionDirectoryPath) is false && Directory.Exists(targetPaths.SessionDirectoryPath))
            {
                Directory.Move(targetPaths.SessionDirectoryPath,sourcePaths.SessionDirectoryPath);

                if(await this.SaveState(originalstate,cancel).ConfigureAwait(false) is false)
                {
                    KusDepotLog.Error(LocalSessionPersistenceFail,originalstate.SessionID,originalstate.ItemID,"RestoreSegmentedMigration");
                }
            }
        }
        catch(Exception error) { KusDepotLog.Error(error,"Restore segmented session migration failed"); }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="RestoreMovedStreamSessionAsync"]/*'/>*/
    private async Task RestoreMovedStreamSessionAsync(StreamTransferStorageSessionPaths sourcePaths , StreamTransferStorageSessionPaths targetPaths , DataStreamTransferState originalstate , Byte[] objectpayload , CancellationToken cancel)
    {
        try
        {
            if(Directory.Exists(sourcePaths.SessionDirectoryPath) is false && Directory.Exists(targetPaths.SessionDirectoryPath))
            {
                Directory.Move(targetPaths.SessionDirectoryPath,sourcePaths.SessionDirectoryPath);

                if(await this.SaveState(originalstate,objectpayload,cancel).ConfigureAwait(false) is false)
                {
                    KusDepotLog.Error(LocalSessionPersistenceFail,originalstate.SessionID,originalstate.ItemID,"RestoreStreamMigration");
                }
            }
        }
        catch(Exception error) { KusDepotLog.Error(error,"Restore stream session migration failed"); }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="OpenPersistedRange"]/*'/>*/
    public Task<Stream?> OpenPersistedRange(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default)
    {
        try
        {
            if(range.Validate() is false) { return Task.FromResult<Stream?>(null); }

            SegmentedTransferStorageSessionPaths paths = this.GetSessionPaths(sessionid);

            if(File.Exists(paths.StreamPath) is false) { return Task.FromResult<Stream?>(null); }

            cancel.ThrowIfCancellationRequested();

            Stream stream = new BoundedReadStream(new FileStream(paths.StreamPath,
                new FileStreamOptions(){ Access = FileAccess.Read , Mode = FileMode.Open , Share = FileShare.ReadWrite,
                Options = FileOptions.Asynchronous | FileOptions.RandomAccess }),range.Offset,range.Length,leaveopen:false);

            return Task.FromResult<Stream?>(stream);
        }
        catch { return Task.FromResult<Stream?>(null); }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="SaveState"]/*'/>*/
    public Task<Boolean> SaveState(DataItemTransferState state , CancellationToken cancel = default)
    {
        return this.Storage.SaveManifest(CreateManifest(state),cancel);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="SaveStreamState"]/*'/>*/
    public async Task<Boolean> SaveState(DataStreamTransferState state , Byte[]? objectpayload = null , CancellationToken cancel = default)
    {
        Byte[] persistedPayload = objectpayload ?? Array.Empty<Byte>();

        if(await this.StreamStorage.SaveManifest(CreateManifest(state,persistedPayload),cancel).ConfigureAwait(false) is false)
        {
            return false;
        }

        if(objectpayload is null)
        {
            return true;
        }

        return await this.StreamStorage.SaveObjectPayload(state.SessionID,objectpayload,cancel).ConfigureAwait(false);        
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="WriteRange"]/*'/>*/
    public Task<SegmentedTransferWriteResult> WriteRange(Guid sessionid , DataItemTransferRange range , Stream payload , CancellationToken cancel = default)
    {
        return this.Storage.WriteRange(sessionid,range,payload,cancel);
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="CreateManifest"]/*'/>*/
    public static DataItemTransferManifest CreateManifest(DataItemTransferState state)
    {
        return new DataItemTransferManifest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            SourceSessionID = state.SourceSessionID,
            ObjectSHA512 = state.ObjectSHA512,
            Integrity = state.Integrity,
            StreamLength = state.StreamLength,
            Status = state.Status,
            Mode = state.Mode,
            StateVersion = state.StateVersion,
            Created = state.Created,
            Updated = state.Updated,
            SegmentSizePolicy = state.SegmentSizePolicy,
            RealizedRanges = state.RealizedRanges,
            ObjectInfo = state.ObjectInfo,
            FaultMessage = state.FaultMessage,
        }.NormalizeRanges();
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="CreateStreamManifest"]/*'/>*/
    public static DataStreamTransferManifest CreateManifest(DataStreamTransferState state , Byte[] objectpayload)
    {
        return new DataStreamTransferManifest()
        {
            AppendedLength = state.AppendedLength,
            Created = state.Created,
            FaultMessage = state.FaultMessage,
            ItemID = state.ItemID,
            Mode = state.Mode,
            ObjectInfo = state.ObjectInfo,
            ObjectPayload = objectpayload,
            ObjectSHA512 = state.ObjectSHA512,
            SegmentSizePolicy = state.SegmentSizePolicy,
            SessionID = state.SessionID,
            SourceSessionID = state.SourceSessionID,
            StateVersion = state.StateVersion,
            Status = state.Status,
            Integrity = state.Integrity,
            Updated = state.Updated,
        };
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="CreateSessionInfo"]/*'/>*/
    public static DataControlTransferSessionInfo CreateSessionInfo(DataItemTransferManifest manifest , String workingdirectory , DataControlTransferSourceDescriptor? uploadsource = null)
    {
        DataItemTransferState state = manifest.ToState();

        return new()
        {
            SessionId = manifest.SessionID,
            ItemId = manifest.ItemID,
            WorkingDirectory = workingdirectory,
            UploadSource = uploadsource,
            State = state,
            SegmentedState = state,
            TransferFamily = DataControlTransferFamily.Segmented,
        };
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="CreateStreamSessionInfo"]/*'/>*/
    public static DataControlTransferSessionInfo CreateSessionInfo(DataStreamTransferManifest manifest , String workingdirectory , DataControlTransferSourceDescriptor? uploadsource = null)
    {
        return new()
        {
            SessionId = manifest.SessionID,
            ItemId = manifest.ItemID,
            WorkingDirectory = workingdirectory,
            UploadSource = uploadsource,
            StreamState = manifest.ToState(),
            TransferFamily = DataControlTransferFamily.Stream,
        };
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="EnumerateSegmentedSessionIDs"]/*'/>*/
    public IEnumerable<Guid> EnumerateSegmentedSessionIDs() => EnumerateSessionIDs(this.RootPath);

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="EnumerateStreamSessionIDs"]/*'/>*/
    public IEnumerable<Guid> EnumerateStreamSessionIDs() => EnumerateSessionIDs(this.StreamRootPath);

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="EnumerateSessionIDs"]/*'/>*/
    private static IEnumerable<Guid> EnumerateSessionIDs(String rootpath)
    {
        if(Directory.Exists(rootpath) is false) { yield break; }

        foreach(String directory in Directory.EnumerateDirectories(rootpath))
        {
            String name = Path.GetFileName(directory);

            if(Guid.TryParse(name,out Guid sessionId)) { yield return sessionId; }
        }
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="GetSegmentedStorageRootPath"]/*'/>*/
    private static String GetSegmentedStorageRootPath(DataControlClientWorkingDirectoryOptions? options)
    {
        String rootPath = options?.RootPath ?? Path.Combine(Path.GetTempPath(),"KusDepot","DataControlClient");

        return Path.Combine(Path.GetFullPath(rootPath),"Segments");
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="GetStreamStorageRootPath"]/*'/>*/
    private static String GetStreamStorageRootPath(DataControlClientWorkingDirectoryOptions? options)
    {
        String rootPath = options?.RootPath ?? Path.Combine(Path.GetTempPath(),"KusDepot","DataControlClient");

        return Path.Combine(Path.GetFullPath(rootPath),"Streams");
    }

    /**<include file='DataControlClientWorkingDirectoryStorage.xml' path='DataControlClientWorkingDirectoryStorage/class[@name="DataControlClientWorkingDirectoryStorage"]/method[@name="GetUploadSourceDescriptorPath"]/*'/>*/
    private String GetUploadSourceDescriptorPath(Guid sessionid , Boolean stream = false)
    {
        return Path.Combine(stream ? this.GetStreamSessionPaths(sessionid).SessionDirectoryPath : this.GetSessionPaths(sessionid).SessionDirectoryPath,UploadSourceDescriptorFileName);
    }
}
