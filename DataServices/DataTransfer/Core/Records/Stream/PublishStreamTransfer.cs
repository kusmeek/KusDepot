namespace KusDepot.Data.Transfer;

/**<include file='PublishStreamTransfer.xml' path='PublishStreamTransfer/record[@name="PublishStreamTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PublishStreamTransferRequest
{
    /**<include file='PublishStreamTransfer.xml' path='PublishStreamTransfer/record[@name="PublishStreamTransferRequest"]/property[@name="ExpectedStateVersion"]/*'/>*/
    [JsonPropertyName("ExpectedStateVersion")] [JsonInclude]
    public Int64 ExpectedStateVersion { get; init; }

    /**<include file='PublishStreamTransfer.xml' path='PublishStreamTransfer/record[@name="PublishStreamTransferRequest"]/property[@name="FinalObjectInfo"]/*'/>*/
    [JsonPropertyName("FinalObjectInfo")] [JsonInclude]
    public DataItemTransferObjectInfo? FinalObjectInfo { get; init; }

    /**<include file='PublishStreamTransfer.xml' path='PublishStreamTransfer/record[@name="PublishStreamTransferRequest"]/property[@name="FinalObjectPayload"]/*'/>*/
    [JsonPropertyName("FinalObjectPayload")] [JsonInclude]
    public Byte[] FinalObjectPayload { get; init; } = Array.Empty<Byte>();

    /**<include file='PublishStreamTransfer.xml' path='PublishStreamTransfer/record[@name="PublishStreamTransferRequest"]/property[@name="FinalStreamLength"]/*'/>*/
    [JsonPropertyName("FinalStreamLength")] [JsonInclude]
    public Int64? FinalStreamLength { get; init; }

    /**<include file='PublishStreamTransfer.xml' path='PublishStreamTransfer/record[@name="PublishStreamTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='PublishStreamTransfer.xml' path='PublishStreamTransfer/record[@name="PublishStreamTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}

/**<include file='PublishStreamTransfer.xml' path='PublishStreamTransfer/record[@name="PublishStreamTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PublishStreamTransferResponse
{
    /**<include file='PublishStreamTransfer.xml' path='PublishStreamTransfer/record[@name="PublishStreamTransferResponse"]/property[@name="Published"]/*'/>*/
    [JsonPropertyName("Published")] [JsonInclude]
    public Boolean Published { get; init; }

    /**<include file='PublishStreamTransfer.xml' path='PublishStreamTransfer/record[@name="PublishStreamTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataStreamTransferState State { get; init; } = new();
}
