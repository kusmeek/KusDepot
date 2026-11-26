namespace KusDepot.Data.Transfer;

/**<include file='CloseTransferSession.xml' path='CloseTransferSession/record[@name="CloseTransferSessionRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record CloseTransferSessionRequest
{
    /**<include file='CloseTransferSession.xml' path='CloseTransferSession/record[@name="CloseTransferSessionRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='CloseTransferSession.xml' path='CloseTransferSession/record[@name="CloseTransferSessionRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}

/**<include file='CloseTransferSession.xml' path='CloseTransferSession/record[@name="CloseTransferSessionResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record CloseTransferSessionResponse
{
    /**<include file='CloseTransferSession.xml' path='CloseTransferSession/record[@name="CloseTransferSessionResponse"]/property[@name="Closed"]/*'/>*/
    [JsonPropertyName("Closed")] [JsonInclude]
    public Boolean Closed { get; init; }

    /**<include file='CloseTransferSession.xml' path='CloseTransferSession/record[@name="CloseTransferSessionResponse"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='CloseTransferSession.xml' path='CloseTransferSession/record[@name="CloseTransferSessionResponse"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}
