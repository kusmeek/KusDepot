namespace KusDepot.Data.Transfer;

/**<include file='GetStreamTransferStateResponse.xml' path='GetStreamTransferStateResponse/record[@name="GetStreamTransferStateResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record GetStreamTransferStateResponse
{
    /**<include file='GetStreamTransferStateResponse.xml' path='GetStreamTransferStateResponse/record[@name="GetStreamTransferStateResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataStreamTransferState State { get; init; } = new();
}
