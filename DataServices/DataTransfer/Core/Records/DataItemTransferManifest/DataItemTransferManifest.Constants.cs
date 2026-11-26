namespace KusDepot.Data.Transfer;

public sealed partial record DataItemTransferManifest
{
    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/field[@name="ManifestFileName"]/*'/>*/
    public const String ManifestFileName = "manifest.json";

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/field[@name="ObjectFileName"]/*'/>*/
    public const String ObjectFileName = "object.bin";

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/field[@name="StreamFileName"]/*'/>*/
    public const String StreamFileName = "stream.bin";
}
