namespace KusDepot.Data.Transfer;

/**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PutStreamTransferSegmentRequest
{
    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentRequest"]/property[@name="ExpectedStateVersion"]/*'/>*/
    [JsonPropertyName("ExpectedStateVersion")] [JsonInclude]
    public Int64? ExpectedStateVersion { get; init; }

    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentRequest"]/property[@name="Length"]/*'/>*/
    [JsonPropertyName("Length")] [JsonInclude]
    public Int64 Length { get; init; }

    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentRequest"]/property[@name="Offset"]/*'/>*/
    [JsonPropertyName("Offset")] [JsonInclude]
    public Int64 Offset { get; init; }

    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentRequest"]/property[@name="RequestID"]/*'/>*/
    [JsonPropertyName("RequestID")] [JsonInclude]
    public Guid? RequestID { get; init; }

    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentRequest"]/property[@name="SegmentSHA512"]/*'/>*/
    [JsonPropertyName("SegmentSHA512")] [JsonInclude]
    public Byte[] SegmentSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentRequest"]/property[@name="StreamSHA512"]/*'/>*/
    [JsonPropertyName("StreamSHA512")] [JsonInclude]
    public Byte[] StreamSHA512 { get; init; } = Array.Empty<Byte>();
}

/**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PutStreamTransferSegmentResponse
{
    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentResponse"]/property[@name="Accepted"]/*'/>*/
    [JsonPropertyName("Accepted")] [JsonInclude]
    public Boolean Accepted { get; init; }

    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentResponse"]/property[@name="AppendedLength"]/*'/>*/
    [JsonPropertyName("AppendedLength")] [JsonInclude]
    public Int64 AppendedLength { get; init; }

    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentResponse"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentResponse"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='PutStreamTransferSegment.xml' path='PutStreamTransferSegment/record[@name="PutStreamTransferSegmentResponse"]/property[@name="StateVersion"]/*'/>*/
    [JsonPropertyName("StateVersion")] [JsonInclude]
    public Int64 StateVersion { get; init; }
}
