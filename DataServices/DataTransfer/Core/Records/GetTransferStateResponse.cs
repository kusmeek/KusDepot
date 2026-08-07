namespace KusDepot.Data.Transfer;

/**<include file='GetTransferStateResponse.xml' path='GetTransferStateResponse/record[@name="GetTransferStateResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record GetTransferStateResponse
{
    /**<include file='GetTransferStateResponse.xml' path='GetTransferStateResponse/record[@name="GetTransferStateResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataItemTransferState State { get; init; } = new();
}