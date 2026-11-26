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

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/property[@name="Replayed"]/*'/>*/
    [JsonPropertyName("Replayed")] [JsonInclude]
    public Boolean Replayed { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/property[@name="StateVersion"]/*'/>*/
    [JsonPropertyName("StateVersion")] [JsonInclude]
    public Int64 StateVersion { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/property[@name="Updated"]/*'/>*/
    [JsonPropertyName("Updated")] [JsonInclude]
    public DateTimeOffset? Updated { get; init; }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/field[@name="StrictParsingOptions"]/*'/>*/
    private static readonly JsonSerializerOptions StrictParsingOptions = new()
    {
        AllowDuplicateProperties = false,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { RequireResponseProperties } },
    };

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/method[@name="TryParseWithRequiredFields"]/*'/>*/
    public static Boolean TryParseWithRequiredFields(String? content , [NotNullWhen(true)] out PutTransferSegmentResponse? response)
    {
        response = null;

        if(String.IsNullOrWhiteSpace(content)) { return false; }

        try
        {
            PutTransferSegmentResponse? parsed = JsonSerializer.Deserialize<PutTransferSegmentResponse>(content,StrictParsingOptions);

            if(parsed?.Range is null) { return false; }

            response = parsed; return true;
        }
        catch ( JsonException ) { return false; }
    }

    /**<include file='PutTransferSegment.xml' path='PutTransferSegment/record[@name="PutTransferSegmentResponse"]/method[@name="RequireResponseProperties"]/*'/>*/
    private static void RequireResponseProperties(JsonTypeInfo typeInfo)
    {
        foreach(JsonPropertyInfo property in typeInfo.Properties)
        {
            if(typeInfo.Type == typeof(PutTransferSegmentResponse) && property.Name is nameof(Accepted) or nameof(CompletedCoverage) or nameof(ItemID) or nameof(Range) or nameof(Replayed) or nameof(SessionID) or nameof(StateVersion) or nameof(Updated))
            {
                property.IsRequired = true;
            }
            else if(typeInfo.Type == typeof(DataItemTransferRange) && property.Name is nameof(DataItemTransferRange.Offset) or nameof(DataItemTransferRange.Length))
            {
                property.IsRequired = true;
            }
        }
    }
}
