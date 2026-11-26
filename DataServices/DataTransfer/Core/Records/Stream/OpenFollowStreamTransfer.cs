namespace KusDepot.Data.Transfer;

/**<include file='OpenFollowStreamTransfer.xml' path='OpenFollowStreamTransfer/record[@name="OpenFollowStreamTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record OpenFollowStreamTransferRequest
{
    /**<include file='OpenFollowStreamTransfer.xml' path='OpenFollowStreamTransfer/record[@name="OpenFollowStreamTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='OpenFollowStreamTransfer.xml' path='OpenFollowStreamTransfer/record[@name="OpenFollowStreamTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='OpenFollowStreamTransfer.xml' path='OpenFollowStreamTransfer/record[@name="OpenFollowStreamTransferRequest"]/property[@name="SourceSessionID"]/*'/>*/
    [JsonPropertyName("SourceSessionID")] [JsonInclude]
    public Guid SourceSessionID { get; init; }
}

/**<include file='OpenFollowStreamTransfer.xml' path='OpenFollowStreamTransfer/record[@name="OpenFollowStreamTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record OpenFollowStreamTransferResponse
{
    /**<include file='OpenFollowStreamTransfer.xml' path='OpenFollowStreamTransfer/record[@name="OpenFollowStreamTransferResponse"]/property[@name="ObjectPayload"]/*'/>*/
    [JsonPropertyName("ObjectPayload")] [JsonInclude]
    public Byte[] ObjectPayload { get; init; } = Array.Empty<Byte>();

    /**<include file='OpenFollowStreamTransfer.xml' path='OpenFollowStreamTransfer/record[@name="OpenFollowStreamTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataStreamTransferState State { get; init; } = new();
}
