namespace KusDepot.Data.Transfer;

/**<include file='GetTransferSegment.xml' path='GetTransferSegment/record[@name="GetTransferSegmentRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record GetTransferSegmentRequest
{
    /**<include file='GetTransferSegment.xml' path='GetTransferSegment/record[@name="GetTransferSegmentRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='GetTransferSegment.xml' path='GetTransferSegment/record[@name="GetTransferSegmentRequest"]/property[@name="Length"]/*'/>*/
    [JsonPropertyName("Length")] [JsonInclude]
    public Int64 Length { get; init; }

    /**<include file='GetTransferSegment.xml' path='GetTransferSegment/record[@name="GetTransferSegmentRequest"]/property[@name="Offset"]/*'/>*/
    [JsonPropertyName("Offset")] [JsonInclude]
    public Int64 Offset { get; init; }

    /**<include file='GetTransferSegment.xml' path='GetTransferSegment/record[@name="GetTransferSegmentRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}