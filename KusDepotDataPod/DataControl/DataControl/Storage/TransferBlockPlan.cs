namespace DataPodServices.DataControl;

/**<include file='TransferBlockPlan.xml' path='TransferBlockPlan/record[@name="TransferBlockPlan"]/main/*'/>*/
internal sealed record TransferBlockPlan
{
    /**<include file='TransferBlockPlan.xml' path='TransferBlockPlan/record[@name="TransferBlockPlan"]/property[@name="Blocks"]/*'/>*/
    public IReadOnlyList<TransferBlock> Blocks { get; init; } = Array.Empty<TransferBlock>();

    /**<include file='TransferBlockPlan.xml' path='TransferBlockPlan/record[@name="TransferBlockPlan"]/property[@name="LogicalRanges"]/*'/>*/
    public IReadOnlyList<TransferRangeReference> LogicalRanges { get; init; } = Array.Empty<TransferRangeReference>();

    /**<include file='TransferBlockPlan.xml' path='TransferBlockPlan/record[@name="TransferBlockPlan"]/property[@name="TotalLength"]/*'/>*/
    public Int64 TotalLength { get; init; }
}
