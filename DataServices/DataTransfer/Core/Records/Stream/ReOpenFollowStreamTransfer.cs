namespace KusDepot.Data.Transfer;

/**<include file='ReOpenFollowStreamTransfer.xml' path='ReOpenFollowStreamTransfer/record[@name="ReOpenFollowStreamTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ReOpenFollowStreamTransferRequest
{
    /**<include file='ReOpenFollowStreamTransfer.xml' path='ReOpenFollowStreamTransfer/record[@name="ReOpenFollowStreamTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='ReOpenFollowStreamTransfer.xml' path='ReOpenFollowStreamTransfer/record[@name="ReOpenFollowStreamTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='ReOpenFollowStreamTransfer.xml' path='ReOpenFollowStreamTransfer/record[@name="ReOpenFollowStreamTransferRequest"]/property[@name="SourceSessionID"]/*'/>*/
    [JsonPropertyName("SourceSessionID")] [JsonInclude]
    public Guid SourceSessionID { get; init; }
}

/**<include file='ReOpenFollowStreamTransfer.xml' path='ReOpenFollowStreamTransfer/record[@name="ReOpenFollowStreamTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ReOpenFollowStreamTransferResponse
{
    /**<include file='ReOpenFollowStreamTransfer.xml' path='ReOpenFollowStreamTransfer/record[@name="ReOpenFollowStreamTransferResponse"]/property[@name="ObjectPayload"]/*'/>*/
    [JsonPropertyName("ObjectPayload")] [JsonInclude]
    public Byte[] ObjectPayload { get; init; } = Array.Empty<Byte>();

    /**<include file='ReOpenFollowStreamTransfer.xml' path='ReOpenFollowStreamTransfer/record[@name="ReOpenFollowStreamTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataStreamTransferState State { get; init; } = new();
}
