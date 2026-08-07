namespace KusDepot.Data.Transfer;

/**<include file='ReOpenStreamTransfer.xml' path='ReOpenStreamTransfer/record[@name="ReOpenStreamTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ReOpenStreamTransferRequest
{
    /**<include file='ReOpenStreamTransfer.xml' path='ReOpenStreamTransfer/record[@name="ReOpenStreamTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='ReOpenStreamTransfer.xml' path='ReOpenStreamTransfer/record[@name="ReOpenStreamTransferRequest"]/property[@name="ObjectSHA512"]/*'/>*/
    [JsonPropertyName("ObjectSHA512")] [JsonInclude]
    public Byte[] ObjectSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='ReOpenStreamTransfer.xml' path='ReOpenStreamTransfer/record[@name="ReOpenStreamTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='ReOpenStreamTransfer.xml' path='ReOpenStreamTransfer/record[@name="ReOpenStreamTransferRequest"]/property[@name="StreamSHA512"]/*'/>*/
    [JsonPropertyName("StreamSHA512")] [JsonInclude]
    public Byte[] StreamSHA512 { get; init; } = Array.Empty<Byte>();
}

/**<include file='ReOpenStreamTransfer.xml' path='ReOpenStreamTransfer/record[@name="ReOpenStreamTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ReOpenStreamTransferResponse
{
    /**<include file='ReOpenStreamTransfer.xml' path='ReOpenStreamTransfer/record[@name="ReOpenStreamTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataStreamTransferState State { get; init; } = new();
}
