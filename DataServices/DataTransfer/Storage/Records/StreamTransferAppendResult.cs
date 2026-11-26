namespace KusDepot.Data.Services.DataTransfer;

/**<include file='StreamTransferAppendResult.xml' path='StreamTransferAppendResult/record[@name="StreamTransferAppendResult"]/main/*'/>*/
public sealed record StreamTransferAppendResult
{
    /**<include file='StreamTransferAppendResult.xml' path='StreamTransferAppendResult/record[@name="StreamTransferAppendResult"]/property[@name="Accepted"]/*'/>*/
    public Boolean Accepted { get; init; }

    /**<include file='StreamTransferAppendResult.xml' path='StreamTransferAppendResult/record[@name="StreamTransferAppendResult"]/property[@name="AppendedLength"]/*'/>*/
    public Int64 AppendedLength { get; init; }

    /**<include file='StreamTransferAppendResult.xml' path='StreamTransferAppendResult/record[@name="StreamTransferAppendResult"]/property[@name="Range"]/*'/>*/
    public DataItemTransferRange Range { get; init; } = new();

    /**<include file='StreamTransferAppendResult.xml' path='StreamTransferAppendResult/record[@name="StreamTransferAppendResult"]/property[@name="Replayed"]/*'/>*/
    public Boolean Replayed { get; init; }

    /**<include file='StreamTransferAppendResult.xml' path='StreamTransferAppendResult/record[@name="StreamTransferAppendResult"]/property[@name="StateVersion"]/*'/>*/
    public Int64 StateVersion { get; init; }

    /**<include file='StreamTransferAppendResult.xml' path='StreamTransferAppendResult/record[@name="StreamTransferAppendResult"]/method[@name="Create"]/*'/>*/
    public static StreamTransferAppendResult Create(DataItemTransferRange range , Boolean accepted , Int64 appendedlength , Int64 stateversion = 0 , Boolean replayed = false)
    {
        return new()
        {
            Accepted = accepted,
            AppendedLength = appendedlength,
            Range = range,
            Replayed = replayed,
            StateVersion = stateversion,
        };
    }
}
