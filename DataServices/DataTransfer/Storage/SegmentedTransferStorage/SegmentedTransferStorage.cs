using static KusDepot.Data.Services.DataTransfer.Strings;

namespace KusDepot.Data.Services.DataTransfer;

/**<include file='SegmentedTransferStorage.xml' path='SegmentedTransferStorage/class[@name="SegmentedTransferStorage"]/main/*'/>*/
public sealed class SegmentedTransferStorage : ISegmentedTransferStorage
{
    /**<include file='SegmentedTransferStorage.xml' path='SegmentedTransferStorage/class[@name="SegmentedTransferStorage"]/field[@name="Options"]/*'/>*/
    private readonly TransferStorageOptions options;

    /**<include file='SegmentedTransferStorage.xml' path='SegmentedTransferStorage/class[@name="SegmentedTransferStorage"]/constructor[@name="SegmentedTransferStorage"]/*'/>*/
    public SegmentedTransferStorage(IOptions<TransferStorageOptions> options)
    {
        this.options = options?.Value ?? new TransferStorageOptions();
    }

    ///<inheritdoc/>
    public async Task<SegmentedTransferStorageSessionPaths?> CreateSession(DataItemTransferManifest manifest , Byte[] objectpayload , CancellationToken cancel = default)
    {
        try
        {
            if(manifest is null || manifest.Validate() is false || objectpayload is null) { Log.Error(InvalidArgument); return null; }

            SegmentedTransferStorageSessionPaths paths = GetSessionPaths(manifest.SessionID);

            Directory.CreateDirectory(paths.SessionDirectoryPath);

            await File.WriteAllBytesAsync(paths.ObjectPath,objectpayload,cancel).ConfigureAwait(false);

            if(await SegmentedTransferStorageCore.PrepareSparseStreamFile(paths.StreamPath,manifest.StreamLength,cancel).ConfigureAwait(false) is false) { return null; }

            await SegmentedTransferStorageCore.SaveManifestAsync(paths.ManifestPath,manifest.NormalizeRanges(),cancel).ConfigureAwait(false);

            return paths;
        }
        catch ( Exception _ ) { Log.Error(_,CreateSessionFail,manifest?.SessionID); return null; }
    }

    ///<inheritdoc/>
    public Task<Boolean> DeleteSession(Guid sessionid , CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            SegmentedTransferStorageSessionPaths paths = GetSessionPaths(sessionid);

            if(Directory.Exists(paths.SessionDirectoryPath)) { Directory.Delete(paths.SessionDirectoryPath,true); }

            return Task.FromResult(true);
        }
        catch ( Exception _ ) { Log.Error(_,DeleteSessionFail,sessionid); return Task.FromResult(false); }
    }

    ///<inheritdoc/>
    public SegmentedTransferStorageSessionPaths GetSessionPaths(Guid sessionid)
    {
        String sessionDirectoryPath = SegmentedTransferStorageCore.GetStorageDirectoryPath(this.options.RootPath,SegmentedTransferStorageCore.CreateSessionStorageKey(sessionid));

        return new()
        {
            SessionDirectoryPath = sessionDirectoryPath,

            ManifestPath = Path.Combine(sessionDirectoryPath,DataItemTransferManifest.ManifestFileName),

            ObjectPath = Path.Combine(sessionDirectoryPath,DataItemTransferManifest.ObjectFileName),

            StreamPath = Path.Combine(sessionDirectoryPath,DataItemTransferManifest.StreamFileName),
        };
    }

    ///<inheritdoc/>
    public async Task<DataItemTransferManifest?> LoadManifest(Guid sessionid , CancellationToken cancel = default)
    {
        try
        {
            SegmentedTransferStorageSessionPaths paths = GetSessionPaths(sessionid);

            if(File.Exists(paths.ManifestPath) is false) { return null; }

            Byte[]? data = await SegmentedTransferStorageCore.ReadAllBytesSafeAsync(paths.ManifestPath,sessionid,LoadManifestFail,cancel).ConfigureAwait(false);

            return data is null ? null : DataItemTransferManifest.Deserialize(data)?.NormalizeRanges();
        }
        catch ( Exception _ ) { Log.Error(_,LoadManifestFail,sessionid); return null; }
    }

    ///<inheritdoc/>
    public Task<Byte[]?> ReadObjectPayload(Guid sessionid , CancellationToken cancel = default)
    {
        return SegmentedTransferStorageCore.ReadAllBytesSafeAsync(GetSessionPaths(sessionid).ObjectPath,sessionid,ReadObjectFail,cancel);
    }

    ///<inheritdoc/>
    public async Task<Stream?> ReadRange(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default)
    {
        try
        {
            if(range.Validate() is false) { Log.Error(InvalidArgument); return null; }

            DataItemTransferManifest? manifest = await LoadManifest(sessionid,cancel).ConfigureAwait(false);

            if(manifest is null || manifest.CanReadRange(range) is false) { return null; }

            SegmentedTransferStorageSessionPaths paths = GetSessionPaths(sessionid);

            if(File.Exists(paths.StreamPath) is false) { return null; }

            cancel.ThrowIfCancellationRequested();

            return SegmentedTransferStorageCore.OpenRangeReadStream(paths.StreamPath,range);
        }
        catch ( Exception _ ) { Log.Error(_,ReadRangeFail,sessionid,range.Offset,range.Length); return null; }
    }

    ///<inheritdoc/>
    public async Task<Boolean> SaveManifest(DataItemTransferManifest manifest , CancellationToken cancel = default)
    {
        try
        {
            if(manifest is null || manifest.Validate() is false) { Log.Error(InvalidArgument); return false; }

            SegmentedTransferStorageSessionPaths paths = GetSessionPaths(manifest.SessionID);

            Directory.CreateDirectory(paths.SessionDirectoryPath);

            await SegmentedTransferStorageCore.SaveManifestAsync(paths.ManifestPath,manifest.NormalizeRanges(),cancel).ConfigureAwait(false);

            return true;
        }
        catch ( Exception _ ) { Log.Error(_,SaveManifestFail,manifest?.SessionID); return false; }
    }

    ///<inheritdoc/>
    public async Task<SegmentedTransferWriteResult> WriteRange(Guid sessionid , DataItemTransferRange range , Stream payload , CancellationToken cancel = default)
    {
        try
        {
            if(range.Validate() is false || payload is null)
            {
                Log.Error(InvalidArgument);

                return SegmentedTransferWriteResult.Create(range,accepted:false);
            }

            DataItemTransferManifest? manifest = await LoadManifest(sessionid,cancel).ConfigureAwait(false);

            if(manifest is null || manifest.CanWriteRange(range) is false) { return SegmentedTransferWriteResult.Create(range,accepted:false); }

            SegmentedTransferStorageSessionPaths paths = GetSessionPaths(sessionid);

            Directory.CreateDirectory(paths.SessionDirectoryPath);

            if(await SegmentedTransferStorageCore.PrepareSparseStreamFile(paths.StreamPath,manifest.StreamLength,cancel).ConfigureAwait(false) is false) { return SegmentedTransferWriteResult.Create(range,accepted:false); }

            using FileStream stream = SegmentedTransferStorageCore.OpenStream(paths.StreamPath,FileMode.Open,FileAccess.ReadWrite,FileShare.Read);

            DataItemTransferRange[] realizedRanges = manifest.NormalizeRanges().RealizedRanges;

            if(HasRealizedOverlap(realizedRanges,range))
            {
                Log.Error(OverlapConflict,sessionid,range.Offset,range.Length);

                return SegmentedTransferWriteResult.Create(range,accepted:false);
            }

            stream.Position = range.Offset;

            if(await SegmentedTransferStorageCore.WritePayloadAsync(stream,payload,range.Length,cancel).ConfigureAwait(false) is false) { return SegmentedTransferWriteResult.Create(range,accepted:false); }

            await stream.FlushAsync(cancel).ConfigureAwait(false);

            return SegmentedTransferWriteResult.Create(range,accepted:true);
        }
        catch ( Exception _ )
        {
            Log.Error(_,WriteRangeFail,sessionid,range.Offset,range.Length);

            return SegmentedTransferWriteResult.Create(range,accepted:false);
        }
    }

    /**<include file='SegmentedTransferStorage.xml' path='SegmentedTransferStorage/class[@name="SegmentedTransferStorage"]/method[@name="HasRealizedOverlap"]/*'/>*/
    private static Boolean HasRealizedOverlap(DataItemTransferRange[] realizedranges , DataItemTransferRange incomingrange)
    {
        return realizedranges is not null && realizedranges.Any(_ => _.Intersects(incomingrange));
    }
}