namespace DataPodServices.DataControl;

/**<include file='StreamTransferChunkState.xml' path='StreamTransferChunkState/record[@name="StreamTransferChunkState"]/main/*'/>*/
public sealed record StreamTransferChunkState
{
    /**<include file='StreamTransferChunkState.xml' path='StreamTransferChunkState/record[@name="StreamTransferChunkState"]/property[@name="SessionID"]/*'/>*/
    public Guid SessionID { get; init; }

    /**<include file='StreamTransferChunkState.xml' path='StreamTransferChunkState/record[@name="StreamTransferChunkState"]/property[@name="OperationID"]/*'/>*/
    public Guid OperationID { get; init; }

    /**<include file='StreamTransferChunkState.xml' path='StreamTransferChunkState/record[@name="StreamTransferChunkState"]/property[@name="Range"]/*'/>*/
    public DataItemTransferRange Range { get; init; } = new();

    /**<include file='StreamTransferChunkState.xml' path='StreamTransferChunkState/record[@name="StreamTransferChunkState"]/property[@name="Payload"]/*'/>*/
    public TransferBlobReference Payload { get; init; } = new();

    /**<include file='StreamTransferChunkState.xml' path='StreamTransferChunkState/record[@name="StreamTransferChunkState"]/property[@name="Created"]/*'/>*/
    public DateTimeOffset Created { get; init; }

    /**<include file='StreamTransferChunkState.xml' path='StreamTransferChunkState/record[@name="StreamTransferChunkState"]/property[@name="Ordinal"]/*'/>*/
    public Int64 Ordinal { get; init; }
}
