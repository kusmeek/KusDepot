namespace KusDepot.Data.Transfer;

/**<include file='GetStreamTransferSegment.xml' path='GetStreamTransferSegment/record[@name="GetStreamTransferSegmentRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record GetStreamTransferSegmentRequest
{
    /**<include file='GetStreamTransferSegment.xml' path='GetStreamTransferSegment/record[@name="GetStreamTransferSegmentRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='GetStreamTransferSegment.xml' path='GetStreamTransferSegment/record[@name="GetStreamTransferSegmentRequest"]/property[@name="Length"]/*'/>*/
    [JsonPropertyName("Length")] [JsonInclude]
    public Int64 Length { get; init; }

    /**<include file='GetStreamTransferSegment.xml' path='GetStreamTransferSegment/record[@name="GetStreamTransferSegmentRequest"]/property[@name="Offset"]/*'/>*/
    [JsonPropertyName("Offset")] [JsonInclude]
    public Int64 Offset { get; init; }

    /**<include file='GetStreamTransferSegment.xml' path='GetStreamTransferSegment/record[@name="GetStreamTransferSegmentRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}
