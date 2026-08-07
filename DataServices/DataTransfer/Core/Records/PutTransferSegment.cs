namespace KusDepot.Data.Transfer;

/**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PutTransferSegmentRequest
{
    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentRequest"]/property[@name="ExpectedStateVersion"]/*'/>*/
    [JsonPropertyName("ExpectedStateVersion")] [JsonInclude]
    public Int64? ExpectedStateVersion { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentRequest"]/property[@name="Length"]/*'/>*/
    [JsonPropertyName("Length")] [JsonInclude]
    public Int64 Length { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentRequest"]/property[@name="Offset"]/*'/>*/
    [JsonPropertyName("Offset")] [JsonInclude]
    public Int64 Offset { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentRequest"]/property[@name="SegmentSHA512"]/*'/>*/
    [JsonPropertyName("SegmentSHA512")] [JsonInclude]
    public Byte[] SegmentSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentRequest"]/property[@name="StreamSHA512"]/*'/>*/
    [JsonPropertyName("StreamSHA512")] [JsonInclude]
    public Byte[] StreamSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentRequest"]/property[@name="RequestID"]/*'/>*/
    [JsonPropertyName("RequestID")] [JsonInclude]
    public Guid? RequestID { get; init; }
}

/**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PutTransferSegmentResponse
{
    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/property[@name="Accepted"]/*'/>*/
    [JsonPropertyName("Accepted")] [JsonInclude]
    public Boolean Accepted { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/property[@name="CompletedCoverage"]/*'/>*/
    [JsonPropertyName("CompletedCoverage")] [JsonInclude]
    public Boolean CompletedCoverage { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/property[@name="Range"]/*'/>*/
    [JsonPropertyName("Range")] [JsonInclude]
    public DataItemTransferRange Range { get; init; } = new();

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/property[@name="StateVersion"]/*'/>*/
    [JsonPropertyName("StateVersion")] [JsonInclude]
    public Int64 StateVersion { get; init; }
}
