namespace DataPodServices.DataControl;

/**<include file='TransferBlock.xml' path='TransferBlock/record[@name="TransferBlock"]/main/*'/>*/
internal sealed record TransferBlock
{
    /**<include file='TransferBlock.xml' path='TransferBlock/record[@name="TransferBlock"]/property[@name="BlockID"]/*'/>*/
    public String BlockID { get; init; } = String.Empty;

    /**<include file='TransferBlock.xml' path='TransferBlock/record[@name="TransferBlock"]/property[@name="DestinationOffset"]/*'/>*/
    public Int64 DestinationOffset { get; init; }

    /**<include file='TransferBlock.xml' path='TransferBlock/record[@name="TransferBlock"]/property[@name="Length"]/*'/>*/
    public Int64 Length { get; init; }

    /**<include file='TransferBlock.xml' path='TransferBlock/record[@name="TransferBlock"]/property[@name="LogicalSegmentIndex"]/*'/>*/
    public Int32 LogicalSegmentIndex { get; init; }

    /**<include file='TransferBlock.xml' path='TransferBlock/record[@name="TransferBlock"]/property[@name="LogicalRange"]/*'/>*/
    public DataItemTransferRange LogicalRange { get; init; } = new();

    /**<include file='TransferBlock.xml' path='TransferBlock/record[@name="TransferBlock"]/property[@name="Source"]/*'/>*/
    public TenantTransferBlobReference Source { get; init; } = new();

    /**<include file='TransferBlock.xml' path='TransferBlock/record[@name="TransferBlock"]/property[@name="SourceOffset"]/*'/>*/
    public Int64 SourceOffset { get; init; }
}
