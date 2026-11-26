namespace KusDepot.Data.Transfer;

/**<include file='AbortTransfer.xml' path='AbortTransfer/record[@name="AbortTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record AbortTransferRequest
{
    /**<include file='AbortTransfer.xml' path='AbortTransfer/record[@name="AbortTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='AbortTransfer.xml' path='AbortTransfer/record[@name="AbortTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}

/**<include file='AbortTransfer.xml' path='AbortTransfer/record[@name="AbortTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record AbortTransferResponse
{
    /**<include file='AbortTransfer.xml' path='AbortTransfer/record[@name="AbortTransferResponse"]/property[@name="Aborted"]/*'/>*/
    [JsonPropertyName("Aborted")] [JsonInclude]
    public Boolean Aborted { get; init; }

    /**<include file='AbortTransfer.xml' path='AbortTransfer/record[@name="AbortTransferResponse"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='AbortTransfer.xml' path='AbortTransfer/record[@name="AbortTransferResponse"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}