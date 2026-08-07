namespace KusDepot.Data.Transfer;

/**<include file='ReOpenGetTransfer.xml' path='ReOpenGetTransfer/record[@name="ReOpenGetTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ReOpenGetTransferRequest
{
    /**<include file='ReOpenGetTransfer.xml' path='ReOpenGetTransfer/record[@name="ReOpenGetTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='ReOpenGetTransfer.xml' path='ReOpenGetTransfer/record[@name="ReOpenGetTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='ReOpenGetTransfer.xml' path='ReOpenGetTransfer/record[@name="ReOpenGetTransferRequest"]/property[@name="StreamSHA512"]/*'/>*/
    [JsonPropertyName("StreamSHA512")] [JsonInclude]
    public Byte[] StreamSHA512 { get; init; } = Array.Empty<Byte>();
}

/**<include file='ReOpenGetTransfer.xml' path='ReOpenGetTransfer/record[@name="ReOpenGetTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ReOpenGetTransferResponse
{
    /**<include file='ReOpenGetTransfer.xml' path='ReOpenGetTransfer/record[@name="ReOpenGetTransferResponse"]/property[@name="ObjectPayload"]/*'/>*/
    [JsonPropertyName("ObjectPayload")] [JsonInclude]
    public Byte[] ObjectPayload { get; init; } = Array.Empty<Byte>();

    /**<include file='ReOpenGetTransfer.xml' path='ReOpenGetTransfer/record[@name="ReOpenGetTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataItemTransferState State { get; init; } = new();
}
