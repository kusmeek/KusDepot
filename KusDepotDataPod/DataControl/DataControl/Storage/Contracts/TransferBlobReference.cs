namespace DataPodServices.DataControl;

/**<include file='TransferBlobReference.xml' path='TransferBlobReference/record[@name="TransferBlobReference"]/main/*'/>*/
public sealed record TransferBlobReference
{
    /**<include file='TransferBlobReference.xml' path='TransferBlobReference/record[@name="TransferBlobReference"]/property[@name="BlobName"]/*'/>*/
    public String BlobName { get; init; } = String.Empty;

    /**<include file='TransferBlobReference.xml' path='TransferBlobReference/record[@name="TransferBlobReference"]/property[@name="Length"]/*'/>*/
    public Int64 Length { get; init; }

    /**<include file='TransferBlobReference.xml' path='TransferBlobReference/record[@name="TransferBlobReference"]/property[@name="SHA512"]/*'/>*/
    public Byte[] SHA512 { get; init; } = Array.Empty<Byte>();
}
