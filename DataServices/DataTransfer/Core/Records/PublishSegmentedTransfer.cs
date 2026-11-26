namespace KusDepot.Data.Transfer;

/**<include file='PublishSegmentedTransfer.xml' path='PublishSegmentedTransfer/record[@name="PublishSegmentedTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PublishSegmentedTransferRequest
{
    /**<include file='PublishSegmentedTransfer.xml' path='PublishSegmentedTransfer/record[@name="PublishSegmentedTransferRequest"]/property[@name="ExpectedStateVersion"]/*'/>*/
    [JsonPropertyName("ExpectedStateVersion")] [JsonInclude]
    public Int64 ExpectedStateVersion { get; init; }

    /**<include file='PublishSegmentedTransfer.xml' path='PublishSegmentedTransfer/record[@name="PublishSegmentedTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='PublishSegmentedTransfer.xml' path='PublishSegmentedTransfer/record[@name="PublishSegmentedTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}

/**<include file='PublishSegmentedTransfer.xml' path='PublishSegmentedTransfer/record[@name="PublishSegmentedTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PublishSegmentedTransferResponse
{
    /**<include file='PublishSegmentedTransfer.xml' path='PublishSegmentedTransfer/record[@name="PublishSegmentedTransferResponse"]/property[@name="Published"]/*'/>*/
    [JsonPropertyName("Published")] [JsonInclude]
    public Boolean Published { get; init; }

    /**<include file='PublishSegmentedTransfer.xml' path='PublishSegmentedTransfer/record[@name="PublishSegmentedTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataItemTransferState State { get; init; } = new();
}
