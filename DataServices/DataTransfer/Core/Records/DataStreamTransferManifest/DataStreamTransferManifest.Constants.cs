namespace KusDepot.Data.Transfer;

public sealed partial record DataStreamTransferManifest
{
    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/field[@name="ManifestFileName"]/*'/>*/
    public const String ManifestFileName = "manifest.json";

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/field[@name="ObjectFileName"]/*'/>*/
    public const String ObjectFileName = "object.bin";

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/field[@name="StreamFileName"]/*'/>*/
    public const String StreamFileName = "stream.bin";
}
