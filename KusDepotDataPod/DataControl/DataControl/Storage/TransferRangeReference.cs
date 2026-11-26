namespace DataPodServices.DataControl;

/**<include file='TransferRangeReference.xml' path='TransferRangeReference/record[@name="TransferRangeReference"]/main/*'/>*/
internal sealed record TransferRangeReference
{
    /**<include file='TransferRangeReference.xml' path='TransferRangeReference/record[@name="TransferRangeReference"]/property[@name="Range"]/*'/>*/
    public DataItemTransferRange Range { get; init; } = new();

    /**<include file='TransferRangeReference.xml' path='TransferRangeReference/record[@name="TransferRangeReference"]/property[@name="Blob"]/*'/>*/
    public TenantTransferBlobReference Blob { get; init; } = new();
}
