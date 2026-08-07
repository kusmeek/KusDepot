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

    /**<include file='StreamTransferAppendResult.xml' path='StreamTransferAppendResult/record[@name="StreamTransferAppendResult"]/method[@name="Create"]/*'/>*/
    public static StreamTransferAppendResult Create(DataItemTransferRange range , Boolean accepted , Int64 appendedlength)
    {
        return new()
        {
            Accepted = accepted,
            AppendedLength = appendedlength,
            Range = range,
        };
    }
}
