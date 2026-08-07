namespace KusDepot.Data.Transfer;

/**<include file='DataItemTransferIdentity.xml' path='DataItemTransferIdentity/record[@name="DataItemTransferIdentity"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record DataItemTransferIdentity
{
    /**<include file='DataItemTransferIdentity.xml' path='DataItemTransferIdentity/record[@name="DataItemTransferIdentity"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='DataItemTransferIdentity.xml' path='DataItemTransferIdentity/record[@name="DataItemTransferIdentity"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}