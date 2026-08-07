using static KusDepot.Data.Services.DataTransfer.Strings;

namespace KusDepot.Data.Services.DataTransfer;

/**<include file='StreamTransferStorage.xml' path='StreamTransferStorage/class[@name="StreamTransferStorage"]/main/*'/>*/
public sealed class StreamTransferStorage : IStreamTransferStorage
{
    /**<include file='StreamTransferStorage.xml' path='StreamTransferStorage/class[@name="StreamTransferStorage"]/field[@name="options"]/*'/>*/
    private readonly TransferStorageOptions options;

    /**<include file='StreamTransferStorage.xml' path='StreamTransferStorage/class[@name="StreamTransferStorage"]/constructor[@name="StreamTransferStorage"]/*'/>*/
    public StreamTransferStorage(IOptions<TransferStorageOptions> options)
    {
        this.options = options?.Value ?? new TransferStorageOptions();
    }

    ///<inheritdoc/>
    public async Task<StreamTransferStorageSessionPaths?> CreateSession(DataStreamTransferManifest manifest , CancellationToken cancel = default)
    {
        try
        {
            if(manifest is null || manifest.Validate() is false) { Log.Error(StreamInvalidArgument); return null; }

            StreamTransferStorageSessionPaths paths = GetSessionPaths(manifest.SessionID);

            Directory.CreateDirectory(paths.SessionDirectoryPath);

            await File.WriteAllBytesAsync(paths.ObjectPath,manifest.ObjectPayload ?? Array.Empty<Byte>(),cancel).ConfigureAwait(false);

            if(await StreamTransferStorageCore.PrepareAppendStreamFile(paths.StreamPath,manifest.AppendedLength,cancel).ConfigureAwait(false) is false) { return null; }

            await StreamTransferStorageCore.SaveManifestAsync(paths.ManifestPath,manifest,cancel).ConfigureAwait(false);

            return paths;
        }
        catch ( Exception _ ) { Log.Error(_,StreamCreateSessionFail,manifest?.SessionID); return null; }
    }

    ///<inheritdoc/>
    public Task<Boolean> DeleteSession(Guid sessionid , CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            StreamTransferStorageSessionPaths paths = GetSessionPaths(sessionid);

            if(Directory.Exists(paths.SessionDirectoryPath)) { Directory.Delete(paths.SessionDirectoryPath,true); }

            return Task.FromResult(true);
        }
        catch ( Exception _ ) { Log.Error(_,StreamDeleteSessionFail,sessionid); return Task.FromResult(false); }
    }

    ///<inheritdoc/>
    public StreamTransferStorageSessionPaths GetSessionPaths(Guid sessionid)
    {
        String sessionDirectoryPath = StreamTransferStorageCore.GetStorageDirectoryPath(this.options.RootPath,StreamTransferStorageCore.CreateSessionStorageKey(sessionid));

        return new()
        {
            SessionDirectoryPath = sessionDirectoryPath,
            ManifestPath = Path.Combine(sessionDirectoryPath,DataStreamTransferManifest.ManifestFileName),
            ObjectPath = Path.Combine(sessionDirectoryPath,DataStreamTransferManifest.ObjectFileName),
            StreamPath = Path.Combine(sessionDirectoryPath,DataStreamTransferManifest.StreamFileName),
        };
    }

    ///<inheritdoc/>
    public async Task<DataStreamTransferManifest?> LoadManifest(Guid sessionid , CancellationToken cancel = default)
    {
        try
        {
            StreamTransferStorageSessionPaths paths = GetSessionPaths(sessionid);

            if(File.Exists(paths.ManifestPath) is false) { return null; }

            Byte[]? data = await StreamTransferStorageCore.ReadAllBytesSafeAsync(paths.ManifestPath,sessionid,StreamLoadManifestFail,cancel).ConfigureAwait(false);

            return data is null ? null : DataStreamTransferManifest.Deserialize(data);
        }
        catch ( Exception _ ) { Log.Error(_,StreamLoadManifestFail,sessionid); return null; }
    }

    ///<inheritdoc/>
    public async Task<Stream?> ReadObjectPayload(Guid sessionid , CancellationToken cancel = default)
    {
        Byte[]? payload = await StreamTransferStorageCore.ReadAllBytesSafeAsync(GetSessionPaths(sessionid).ObjectPath,sessionid,StreamReadObjectFail,cancel).ConfigureAwait(false);

        return payload is null ? null : new MemoryStream(payload,writable:false);
    }

    ///<inheritdoc/>
    public async Task<Stream?> ReadRange(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default)
    {
        try
        {
            if(range.Validate() is false) { Log.Error(StreamInvalidArgument); return null; }

            DataStreamTransferManifest? manifest = await LoadManifest(sessionid,cancel).ConfigureAwait(false);

            if(manifest is null || manifest.CanReadRange(range) is false) { return null; }

            StreamTransferStorageSessionPaths paths = GetSessionPaths(sessionid);

            if(File.Exists(paths.StreamPath) is false) { return null; }

            cancel.ThrowIfCancellationRequested();

            return StreamTransferStorageCore.OpenRangeReadStream(paths.StreamPath,range);
        }
        catch ( Exception _ ) { Log.Error(_,StreamReadRangeFail,sessionid,range.Offset,range.Length); return null; }
    }

    ///<inheritdoc/>
    public async Task<Boolean> SaveManifest(DataStreamTransferManifest manifest , CancellationToken cancel = default)
    {
        try
        {
            if(manifest is null || manifest.Validate() is false) { Log.Error(StreamInvalidArgument); return false; }

            StreamTransferStorageSessionPaths paths = GetSessionPaths(manifest.SessionID);

            Directory.CreateDirectory(paths.SessionDirectoryPath);

            await StreamTransferStorageCore.SaveManifestAsync(paths.ManifestPath,manifest,cancel).ConfigureAwait(false);

            return true;
        }
        catch ( Exception _ ) { Log.Error(_,StreamSaveManifestFail,manifest?.SessionID); return false; }
    }

    ///<inheritdoc/>
    public async Task<Boolean> SaveObjectPayload(Guid sessionid , Byte[] objectpayload , CancellationToken cancel = default)
    {
        try
        {
            if(sessionid == Guid.Empty || objectpayload is null) { Log.Error(StreamInvalidArgument); return false; }

            StreamTransferStorageSessionPaths paths = GetSessionPaths(sessionid);

            Directory.CreateDirectory(paths.SessionDirectoryPath);

            await File.WriteAllBytesAsync(paths.ObjectPath,objectpayload,cancel).ConfigureAwait(false);

            return true;
        }
        catch ( Exception _ ) { Log.Error(_,StreamSaveObjectFail,sessionid); return false; }
    }

    ///<inheritdoc/>
    public async Task<StreamTransferAppendResult> WriteTail(Guid sessionid , DataItemTransferRange range , Stream payload , CancellationToken cancel = default)
    {
        try
        {
            if(range.Validate() is false || payload is null)
            {
                Log.Error(StreamInvalidArgument);

                return StreamTransferAppendResult.Create(range,accepted:false,appendedlength:-1);
            }

            DataStreamTransferManifest? manifest = await LoadManifest(sessionid,cancel).ConfigureAwait(false);

            if(manifest is null || manifest.CanWriteRange(range) is false)
            {
                return StreamTransferAppendResult.Create(range,accepted:false,appendedlength:manifest?.AppendedLength ?? -1);
            }

            StreamTransferStorageSessionPaths paths = GetSessionPaths(sessionid);

            Directory.CreateDirectory(paths.SessionDirectoryPath);

            if(await StreamTransferStorageCore.PrepareAppendStreamFile(paths.StreamPath,manifest.AppendedLength,cancel).ConfigureAwait(false) is false)
            {
                return StreamTransferAppendResult.Create(range,accepted:false,appendedlength:manifest.AppendedLength);
            }

            using FileStream stream = StreamTransferStorageCore.OpenStream(paths.StreamPath,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.Read);

            if(stream.Length != manifest.AppendedLength) { stream.SetLength(manifest.AppendedLength); }

            stream.Position = manifest.AppendedLength;

            if(await StreamTransferStorageCore.WritePayloadAsync(stream,payload,range.Length,cancel).ConfigureAwait(false) is false)
            {
                stream.SetLength(manifest.AppendedLength);

                await stream.FlushAsync(cancel).ConfigureAwait(false);

                return StreamTransferAppendResult.Create(range,accepted:false,appendedlength:manifest.AppendedLength);
            }

            Int64 appendedLength = manifest.AppendedLength + range.Length;

            stream.SetLength(appendedLength);

            await stream.FlushAsync(cancel).ConfigureAwait(false);

            return StreamTransferAppendResult.Create(range,accepted:true,appendedlength:appendedLength);
        }
        catch ( Exception _ )
        {
            Log.Error(_,StreamWriteTailFail,sessionid,range.Offset,range.Length);

            return StreamTransferAppendResult.Create(range,accepted:false,appendedlength:-1);
        }
    }
}
