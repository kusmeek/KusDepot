namespace KusDepot.Data.Transfer;

/**<include file='OpenGetTransfer.xml' path='OpenGetTransfer/record[@name="OpenGetTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record OpenGetTransferRequest
{
    /**<include file='OpenGetTransfer.xml' path='OpenGetTransfer/record[@name="OpenGetTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='OpenGetTransfer.xml' path='OpenGetTransfer/record[@name="OpenGetTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='OpenGetTransfer.xml' path='OpenGetTransfer/record[@name="OpenGetTransferRequest"]/property[@name="SourceSessionID"]/*'/>*/
    [JsonPropertyName("SourceSessionID")] [JsonInclude]
    public Guid? SourceSessionID { get; init; }
}

/**<include file='OpenGetTransfer.xml' path='OpenGetTransfer/record[@name="OpenGetTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record OpenGetTransferResponse
{
    /**<include file='OpenGetTransfer.xml' path='OpenGetTransfer/record[@name="OpenGetTransferResponse"]/property[@name="ObjectPayload"]/*'/>*/
    [JsonPropertyName("ObjectPayload")] [JsonInclude]
    public Byte[] ObjectPayload { get; init; } = Array.Empty<Byte>();

    /**<include file='OpenGetTransfer.xml' path='OpenGetTransfer/record[@name="OpenGetTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataItemTransferState State { get; init; } = new();
}
