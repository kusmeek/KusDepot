namespace DataPodServices.DataControl;

/**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/main/*'/>*/
public sealed record TransferSessionState
{
    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="SessionID"]/*'/>*/
    public Guid SessionID { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="ItemID"]/*'/>*/
    public Guid ItemID { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="ItemInstanceToken"]/*'/>*/
    public Guid ItemInstanceToken { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="Family"]/*'/>*/
    public DataControlTransferFamily Family { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="Mode"]/*'/>*/
    public DataItemTransferMode Mode { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="Status"]/*'/>*/
    public DataItemTransferStatus Status { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="StateVersion"]/*'/>*/
    public Int64 StateVersion { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="PlannedBlockCount"]/*'/>*/
    public Int64 PlannedBlockCount { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="CoveredBytes"]/*'/>*/
    public Int64 CoveredBytes { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="Created"]/*'/>*/
    public DateTimeOffset Created { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="Updated"]/*'/>*/
    public DateTimeOffset Updated { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="LastActivity"]/*'/>*/
    public DateTimeOffset LastActivity { get; init; }

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="ManifestJson"]/*'/>*/
    public String ManifestJson { get; init; } = String.Empty;

    /**<include file='TransferSessionState.xml' path='TransferSessionState/record[@name="TransferSessionState"]/property[@name="ObjectPayload"]/*'/>*/
    public TransferBlobReference? ObjectPayload { get; init; }
}
