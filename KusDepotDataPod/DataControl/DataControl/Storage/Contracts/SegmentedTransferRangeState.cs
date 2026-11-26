namespace DataPodServices.DataControl;

/**<include file='SegmentedTransferRangeState.xml' path='SegmentedTransferRangeState/record[@name="SegmentedTransferRangeState"]/main/*'/>*/
public sealed record SegmentedTransferRangeState
{
    /**<include file='SegmentedTransferRangeState.xml' path='SegmentedTransferRangeState/record[@name="SegmentedTransferRangeState"]/property[@name="SessionID"]/*'/>*/
    public Guid SessionID { get; init; }

    /**<include file='SegmentedTransferRangeState.xml' path='SegmentedTransferRangeState/record[@name="SegmentedTransferRangeState"]/property[@name="Range"]/*'/>*/
    public DataItemTransferRange Range { get; init; } = new();

    /**<include file='SegmentedTransferRangeState.xml' path='SegmentedTransferRangeState/record[@name="SegmentedTransferRangeState"]/property[@name="Payload"]/*'/>*/
    public TransferBlobReference Payload { get; init; } = new();

    /**<include file='SegmentedTransferRangeState.xml' path='SegmentedTransferRangeState/record[@name="SegmentedTransferRangeState"]/property[@name="Created"]/*'/>*/
    public DateTimeOffset Created { get; init; }

    /**<include file='SegmentedTransferRangeState.xml' path='SegmentedTransferRangeState/record[@name="SegmentedTransferRangeState"]/property[@name="Ordinal"]/*'/>*/
    public Int64 Ordinal { get; init; }
}
