namespace KusDepot.Data.Transfer;

/**<include file='DataItemTransferProgress.xml' path='DataItemTransferProgress/record[@name="DataItemTransferProgress"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record DataItemTransferProgress
{
    /**<include file='DataItemTransferProgress.xml' path='DataItemTransferProgress/record[@name="DataItemTransferProgress"]/property[@name="LatestAppendedLength"]/*'/>*/
    [JsonPropertyName("LatestAppendedLength")] [JsonInclude]
    public Int64? LatestAppendedLength { get; init; }

    /**<include file='DataItemTransferProgress.xml' path='DataItemTransferProgress/record[@name="DataItemTransferProgress"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='DataItemTransferProgress.xml' path='DataItemTransferProgress/record[@name="DataItemTransferProgress"]/property[@name="LatestRange"]/*'/>*/
    [JsonPropertyName("LatestRange")] [JsonInclude]
    public DataItemTransferRange? LatestRange { get; init; }

    /**<include file='DataItemTransferProgress.xml' path='DataItemTransferProgress/record[@name="DataItemTransferProgress"]/property[@name="Mode"]/*'/>*/
    [JsonPropertyName("Mode")] [JsonInclude]
    public DataItemTransferMode Mode { get; init; }

    /**<include file='DataItemTransferProgress.xml' path='DataItemTransferProgress/record[@name="DataItemTransferProgress"]/property[@name="Status"]/*'/>*/
    [JsonPropertyName("Status")] [JsonInclude]
    public DataItemTransferStatus Status { get; init; }

    /**<include file='DataItemTransferProgress.xml' path='DataItemTransferProgress/record[@name="DataItemTransferProgress"]/property[@name="RealizedBytes"]/*'/>*/
    [JsonPropertyName("RealizedBytes")] [JsonInclude]
    public Int64 RealizedBytes { get; init; }

    /**<include file='DataItemTransferProgress.xml' path='DataItemTransferProgress/record[@name="DataItemTransferProgress"]/property[@name="RemainingBytes"]/*'/>*/
    [JsonPropertyName("RemainingBytes")] [JsonInclude]
    public Int64? RemainingBytes { get; init; }

    /**<include file='DataItemTransferProgress.xml' path='DataItemTransferProgress/record[@name="DataItemTransferProgress"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='DataItemTransferProgress.xml' path='DataItemTransferProgress/record[@name="DataItemTransferProgress"]/property[@name="StateVersion"]/*'/>*/
    [JsonPropertyName("StateVersion")] [JsonInclude]
    public Int64 StateVersion { get; init; }

    /**<include file='DataItemTransferProgress.xml' path='DataItemTransferProgress/record[@name="DataItemTransferProgress"]/property[@name="TransferFamily"]/*'/>*/
    [JsonPropertyName("TransferFamily")] [JsonInclude]
    public DataControlTransferFamily TransferFamily { get; init; }
}