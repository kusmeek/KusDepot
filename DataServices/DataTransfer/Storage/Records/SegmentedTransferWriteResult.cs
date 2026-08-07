namespace KusDepot.Data.Services.DataTransfer;

/**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/main/*'/>*/
public sealed record SegmentedTransferWriteResult
{
    /**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/property[@name="Accepted"]/*'/>*/
    public Boolean Accepted { get; init; }

    /**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/property[@name="Range"]/*'/>*/
    public DataItemTransferRange Range { get; init; } = new();

    /**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/method[@name="Create"]/*'/>*/
    public static SegmentedTransferWriteResult Create(DataItemTransferRange range , Boolean accepted)
    {
        return new()
        {
            Accepted = accepted,

            Range = range,
        };
    }
}
