namespace KusDepot.Data.Services.DataTransfer;

/**<include file='DataItemTransferRegistry.xml' path='DataItemTransferRegistry/class[@name="DataItemTransferRegistry"]/main/*'/>*/
public sealed class DataItemTransferRegistry : IDataItemTransferRegistry
{
    /**<include file='DataItemTransferRegistry.xml' path='DataItemTransferRegistry/class[@name="DataItemTransferRegistry"]/field[@name="SegmentedStorage"]/*'/>*/
    private readonly ISegmentedTransferStorage segmentedStorage;

    /**<include file='DataItemTransferRegistry.xml' path='DataItemTransferRegistry/class[@name="DataItemTransferRegistry"]/field[@name="StreamStorage"]/*'/>*/
    private readonly IStreamTransferStorage streamStorage;

    /**<include file='DataItemTransferRegistry.xml' path='DataItemTransferRegistry/class[@name="DataItemTransferRegistry"]/constructor[@name="DataItemTransferRegistry"]/*'/>*/
    public DataItemTransferRegistry(ISegmentedTransferStorage segmentedstorage , IStreamTransferStorage streamstorage)
    {
        this.segmentedStorage = segmentedstorage; this.streamStorage = streamstorage;
    }

    ///<inheritdoc/>
    public async Task<DataItemTransferOpenConflict?> FindOpenConflictByItemId(Guid itemid , CancellationToken cancel = default)
    {
        if(itemid == Guid.Empty) { return null; }

        DataItemTransferOpenConflict? segmented = await FindSegmentedOpenConflictByItemId(itemid,cancel).ConfigureAwait(false);

        DataItemTransferOpenConflict? stream = await FindStreamOpenConflictByItemId(itemid,cancel).ConfigureAwait(false);

        if(segmented is null) { return stream; } if(stream is null) { return segmented; }

        return segmented.Updated >= stream.Updated ? segmented : stream;
    }

    /**<include file='DataItemTransferRegistry.xml' path='DataItemTransferRegistry/class[@name="DataItemTransferRegistry"]/method[@name="FindSegmentedOpenConflictByItemId"]/*'/>*/
    private async Task<DataItemTransferOpenConflict?> FindSegmentedOpenConflictByItemId(Guid itemid , CancellationToken cancel)
    {
        String root = Path.GetDirectoryName(this.segmentedStorage.GetSessionPaths(Guid.NewGuid()).SessionDirectoryPath) ?? String.Empty;

        if(String.IsNullOrWhiteSpace(root) || Directory.Exists(root) is false) { return null; }

        DataItemTransferOpenConflict? best = null;

        foreach(String path in Directory.EnumerateFiles(root,DataItemTransferManifest.ManifestFileName,SearchOption.AllDirectories))
        {
            cancel.ThrowIfCancellationRequested();

            DataItemTransferManifest? manifest = DataItemTransferManifest.Load(path)?.NormalizeRanges();

            if(manifest is null || manifest.ItemID != itemid || BlocksWriterOpen(manifest.Mode,manifest.Status) is false) { continue; }

            DataItemTransferOpenConflict candidate = new()
            {
                SessionID = manifest.SessionID,
                ItemID = manifest.ItemID,
                TransferFamily = DataControlTransferFamily.Segmented,
                Mode = manifest.Mode,
                Status = manifest.Status,
                Updated = manifest.Updated,
            };

            if(best is null || candidate.Updated > best.Updated) { best = candidate; }
        }

        return best;
    }

    /**<include file='DataItemTransferRegistry.xml' path='DataItemTransferRegistry/class[@name="DataItemTransferRegistry"]/method[@name="FindStreamOpenConflictByItemId"]/*'/>*/
    private async Task<DataItemTransferOpenConflict?> FindStreamOpenConflictByItemId(Guid itemid , CancellationToken cancel)
    {
        String root = Path.GetDirectoryName(this.streamStorage.GetSessionPaths(Guid.NewGuid()).SessionDirectoryPath) ?? String.Empty;

        if(String.IsNullOrWhiteSpace(root) || Directory.Exists(root) is false) { return null; }

        DataItemTransferOpenConflict? best = null;

        foreach(String path in Directory.EnumerateFiles(root,DataStreamTransferManifest.ManifestFileName,SearchOption.AllDirectories))
        {
            cancel.ThrowIfCancellationRequested();

            DataStreamTransferManifest? manifest = DataStreamTransferManifest.Load(path);

            if(manifest is null || manifest.ItemID != itemid || BlocksWriterOpen(manifest.Mode,manifest.Status) is false) { continue; }

            DataItemTransferOpenConflict candidate = new()
            {
                SessionID = manifest.SessionID,
                ItemID = manifest.ItemID,
                TransferFamily = DataControlTransferFamily.Stream,
                Mode = manifest.Mode,
                Status = manifest.Status,
                Updated = manifest.Updated,
            };

            if(best is null || candidate.Updated > best.Updated) { best = candidate; }
        }

        return best;
    }

    /**<include file='DataItemTransferRegistry.xml' path='DataItemTransferRegistry/class[@name="DataItemTransferRegistry"]/method[@name="BlocksWriterOpen"]/*'/>*/
    private static Boolean BlocksWriterOpen(DataItemTransferMode mode , DataItemTransferStatus status)
    {
        if(mode is not DataItemTransferMode.Upload and not DataItemTransferMode.StreamUpload) { return false; }

        return status is not DataItemTransferStatus.Aborted and not DataItemTransferStatus.Faulted;
    }
}
