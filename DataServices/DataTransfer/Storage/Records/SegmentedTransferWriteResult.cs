namespace KusDepot.Data.Services.DataTransfer;

/**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/main/*'/>*/
public sealed record SegmentedTransferWriteResult
{
    /**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/property[@name="Accepted"]/*'/>*/
    public Boolean Accepted { get; init; }

    /**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/property[@name="CompletedCoverage"]/*'/>*/
    public Boolean CompletedCoverage { get; init; }

    /**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/property[@name="Updated"]/*'/>*/
    public DateTimeOffset? Updated { get; init; }

    /**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/property[@name="Range"]/*'/>*/
    public DataItemTransferRange Range { get; init; } = new();

    /**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/property[@name="Replayed"]/*'/>*/
    public Boolean Replayed { get; init; }

    /**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/property[@name="StateVersion"]/*'/>*/
    public Int64 StateVersion { get; init; }

    /**<include file='SegmentedTransferWriteResult.xml' path='SegmentedTransferWriteResult/record[@name="SegmentedTransferWriteResult"]/method[@name="Create"]/*'/>*/
    public static SegmentedTransferWriteResult Create(DataItemTransferRange range , Boolean accepted , Int64 stateversion = 0 , Boolean replayed = false)
    {
        return new()
        {
            Accepted = accepted,
            Replayed = replayed,
            Range = range,
            StateVersion = stateversion,
        };
    }
}
