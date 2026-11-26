namespace DataPodServices.DataControl;

/**<include file='TenantTransferBlobReference.xml' path='TenantTransferBlobReference/record[@name="TenantTransferBlobReference"]/main/*'/>*/
internal sealed record TenantTransferBlobReference
{
    /**<include file='TenantTransferBlobReference.xml' path='TenantTransferBlobReference/record[@name="TenantTransferBlobReference"]/property[@name="BlobName"]/*'/>*/
    public String BlobName { get; init; } = String.Empty;

    /**<include file='TenantTransferBlobReference.xml' path='TenantTransferBlobReference/record[@name="TenantTransferBlobReference"]/property[@name="Length"]/*'/>*/
    public Int64 Length { get; init; }

    /**<include file='TenantTransferBlobReference.xml' path='TenantTransferBlobReference/record[@name="TenantTransferBlobReference"]/property[@name="SHA512"]/*'/>*/
    public Byte[] SHA512 { get; init; } = Array.Empty<Byte>();
}
