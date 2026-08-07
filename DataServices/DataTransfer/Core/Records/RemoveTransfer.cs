namespace KusDepot.Data.Transfer;

/**<include file='RemoveTransfer.xml' path='RemoveTransfer/record[@name="RemoveTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record RemoveTransferRequest
{
    /**<include file='RemoveTransfer.xml' path='RemoveTransfer/record[@name="RemoveTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='RemoveTransfer.xml' path='RemoveTransfer/record[@name="RemoveTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}

/**<include file='RemoveTransfer.xml' path='RemoveTransfer/record[@name="RemoveTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record RemoveTransferResponse
{
    /**<include file='RemoveTransfer.xml' path='RemoveTransfer/record[@name="RemoveTransferResponse"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='RemoveTransfer.xml' path='RemoveTransfer/record[@name="RemoveTransferResponse"]/property[@name="Removed"]/*'/>*/
    [JsonPropertyName("Removed")] [JsonInclude]
    public Boolean Removed { get; init; }

    /**<include file='RemoveTransfer.xml' path='RemoveTransfer/record[@name="RemoveTransferResponse"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}
