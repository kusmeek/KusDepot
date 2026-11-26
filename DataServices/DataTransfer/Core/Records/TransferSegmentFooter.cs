namespace KusDepot.Data.Transfer;

/**<include file='TransferSegmentFooter.xml' path='TransferSegmentFooter/record[@name="TransferSegmentFooter"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record TransferSegmentFooter
{
    /**<include file='TransferSegmentFooter.xml' path='TransferSegmentFooter/record[@name="TransferSegmentFooter"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='TransferSegmentFooter.xml' path='TransferSegmentFooter/record[@name="TransferSegmentFooter"]/property[@name="RequestedOffset"]/*'/>*/
    [JsonPropertyName("RequestedOffset")] [JsonInclude]
    public Int64 RequestedOffset { get; init; }

    /**<include file='TransferSegmentFooter.xml' path='TransferSegmentFooter/record[@name="TransferSegmentFooter"]/property[@name="RequestedLength"]/*'/>*/
    [JsonPropertyName("RequestedLength")] [JsonInclude]
    public Int64 RequestedLength { get; init; }

    /**<include file='TransferSegmentFooter.xml' path='TransferSegmentFooter/record[@name="TransferSegmentFooter"]/property[@name="ReturnedOffset"]/*'/>*/
    [JsonPropertyName("ReturnedOffset")] [JsonInclude]
    public Int64 ReturnedOffset { get; init; }

    /**<include file='TransferSegmentFooter.xml' path='TransferSegmentFooter/record[@name="TransferSegmentFooter"]/property[@name="SegmentSHA512"]/*'/>*/
    [JsonPropertyName("SegmentSHA512")] [JsonInclude]
    public Byte[] SegmentSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='TransferSegmentFooter.xml' path='TransferSegmentFooter/record[@name="TransferSegmentFooter"]/property[@name="ReturnedLength"]/*'/>*/
    [JsonPropertyName("ReturnedLength")] [JsonInclude]
    public Int64 ReturnedLength { get; init; }

    /**<include file='TransferSegmentFooter.xml' path='TransferSegmentFooter/record[@name="TransferSegmentFooter"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='TransferSegmentFooter.xml' path='TransferSegmentFooter/record[@name="TransferSegmentFooter"]/property[@name="StateVersion"]/*'/>*/
    [JsonPropertyName("StateVersion")] [JsonInclude]
    public Int64 StateVersion { get; init; }
}
