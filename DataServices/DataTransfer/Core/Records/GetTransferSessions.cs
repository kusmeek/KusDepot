namespace KusDepot.Data.Transfer;

/**<include file='GetTransferSessions.xml' path='GetTransferSessions/record[@name="GetTransferSessionsRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record GetTransferSessionsRequest
{
    /**<include file='GetTransferSessions.xml' path='GetTransferSessions/record[@name="GetTransferSessionsRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid? ItemID { get; init; }

    /**<include file='GetTransferSessions.xml' path='GetTransferSessions/record[@name="GetTransferSessionsRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid? SessionID { get; init; }

    /**<include file='GetTransferSessions.xml' path='GetTransferSessions/record[@name="GetTransferSessionsRequest"]/property[@name="Status"]/*'/>*/
    [JsonPropertyName("Status")] [JsonInclude]
    public DataItemTransferStatus? Status { get; init; }

    /**<include file='GetTransferSessions.xml' path='GetTransferSessions/record[@name="GetTransferSessionsRequest"]/property[@name="TransferFamily"]/*'/>*/
    [JsonPropertyName("TransferFamily")] [JsonInclude]
    public DataControlTransferFamily? TransferFamily { get; init; }
}