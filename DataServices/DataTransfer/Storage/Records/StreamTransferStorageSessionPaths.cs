namespace KusDepot.Data.Services.DataTransfer;

/**<include file='StreamTransferStorageSessionPaths.xml' path='StreamTransferStorageSessionPaths/record[@name="StreamTransferStorageSessionPaths"]/main/*'/>*/
public sealed record StreamTransferStorageSessionPaths
{
    /**<include file='StreamTransferStorageSessionPaths.xml' path='StreamTransferStorageSessionPaths/record[@name="StreamTransferStorageSessionPaths"]/property[@name="ManifestPath"]/*'/>*/
    public String ManifestPath { get; init; } = String.Empty;

    /**<include file='StreamTransferStorageSessionPaths.xml' path='StreamTransferStorageSessionPaths/record[@name="StreamTransferStorageSessionPaths"]/property[@name="ObjectPath"]/*'/>*/
    public String ObjectPath { get; init; } = String.Empty;

    /**<include file='StreamTransferStorageSessionPaths.xml' path='StreamTransferStorageSessionPaths/record[@name="StreamTransferStorageSessionPaths"]/property[@name="SessionDirectoryPath"]/*'/>*/
    public String SessionDirectoryPath { get; init; } = String.Empty;

    /**<include file='StreamTransferStorageSessionPaths.xml' path='StreamTransferStorageSessionPaths/record[@name="StreamTransferStorageSessionPaths"]/property[@name="StreamPath"]/*'/>*/
    public String StreamPath { get; init; } = String.Empty;
}
