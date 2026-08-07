namespace KusDepot.Data.Services.DataTransfer;

/**<include file='SegmentedTransferStorageSessionPaths.xml' path='SegmentedTransferStorageSessionPaths/record[@name="SegmentedTransferStorageSessionPaths"]/main/*'/>*/
public sealed record SegmentedTransferStorageSessionPaths
{
    /**<include file='SegmentedTransferStorageSessionPaths.xml' path='SegmentedTransferStorageSessionPaths/record[@name="SegmentedTransferStorageSessionPaths"]/property[@name="ManifestPath"]/*'/>*/
    public String ManifestPath { get; init; } = String.Empty;

    /**<include file='SegmentedTransferStorageSessionPaths.xml' path='SegmentedTransferStorageSessionPaths/record[@name="SegmentedTransferStorageSessionPaths"]/property[@name="ObjectPath"]/*'/>*/
    public String ObjectPath { get; init; } = String.Empty;

    /**<include file='SegmentedTransferStorageSessionPaths.xml' path='SegmentedTransferStorageSessionPaths/record[@name="SegmentedTransferStorageSessionPaths"]/property[@name="SessionDirectoryPath"]/*'/>*/
    public String SessionDirectoryPath { get; init; } = String.Empty;

    /**<include file='SegmentedTransferStorageSessionPaths.xml' path='SegmentedTransferStorageSessionPaths/record[@name="SegmentedTransferStorageSessionPaths"]/property[@name="StreamPath"]/*'/>*/
    public String StreamPath { get; init; } = String.Empty;
}