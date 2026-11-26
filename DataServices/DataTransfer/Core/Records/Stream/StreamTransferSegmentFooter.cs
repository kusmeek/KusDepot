namespace KusDepot.Data.Transfer;

/**<include file='StreamTransferSegmentFooter.xml' path='StreamTransferSegmentFooter/record[@name="StreamTransferSegmentFooter"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record StreamTransferSegmentFooter
{
    /**<include file='StreamTransferSegmentFooter.xml' path='StreamTransferSegmentFooter/record[@name="StreamTransferSegmentFooter"]/property[@name="AppendedLength"]/*'/>*/
    [JsonPropertyName("AppendedLength")] [JsonInclude]
    public Int64 AppendedLength { get; init; }

    /**<include file='StreamTransferSegmentFooter.xml' path='StreamTransferSegmentFooter/record[@name="StreamTransferSegmentFooter"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='StreamTransferSegmentFooter.xml' path='StreamTransferSegmentFooter/record[@name="StreamTransferSegmentFooter"]/property[@name="RequestedLength"]/*'/>*/
    [JsonPropertyName("RequestedLength")] [JsonInclude]
    public Int64 RequestedLength { get; init; }

    /**<include file='StreamTransferSegmentFooter.xml' path='StreamTransferSegmentFooter/record[@name="StreamTransferSegmentFooter"]/property[@name="RequestedOffset"]/*'/>*/
    [JsonPropertyName("RequestedOffset")] [JsonInclude]
    public Int64 RequestedOffset { get; init; }

    /**<include file='StreamTransferSegmentFooter.xml' path='StreamTransferSegmentFooter/record[@name="StreamTransferSegmentFooter"]/property[@name="ReturnedLength"]/*'/>*/
    [JsonPropertyName("ReturnedLength")] [JsonInclude]
    public Int64 ReturnedLength { get; init; }

    /**<include file='StreamTransferSegmentFooter.xml' path='StreamTransferSegmentFooter/record[@name="StreamTransferSegmentFooter"]/property[@name="ReturnedOffset"]/*'/>*/
    [JsonPropertyName("ReturnedOffset")] [JsonInclude]
    public Int64 ReturnedOffset { get; init; }

    /**<include file='StreamTransferSegmentFooter.xml' path='StreamTransferSegmentFooter/record[@name="StreamTransferSegmentFooter"]/property[@name="SegmentSHA512"]/*'/>*/
    [JsonPropertyName("SegmentSHA512")] [JsonInclude]
    public Byte[] SegmentSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='StreamTransferSegmentFooter.xml' path='StreamTransferSegmentFooter/record[@name="StreamTransferSegmentFooter"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='StreamTransferSegmentFooter.xml' path='StreamTransferSegmentFooter/record[@name="StreamTransferSegmentFooter"]/property[@name="StateVersion"]/*'/>*/
    [JsonPropertyName("StateVersion")] [JsonInclude]
    public Int64 StateVersion { get; init; }
}
