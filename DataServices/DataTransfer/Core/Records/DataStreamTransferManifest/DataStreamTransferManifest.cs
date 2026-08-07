namespace KusDepot.Data.Transfer;

/**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed partial record DataStreamTransferManifest
{
    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="AppendedLength"]/*'/>*/
    [JsonPropertyName("AppendedLength")] [JsonInclude]
    public Int64 AppendedLength { get; init; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="Created"]/*'/>*/
    [JsonPropertyName("Created")] [JsonInclude]
    public DateTimeOffset Created { get; init; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="FaultMessage"]/*'/>*/
    [JsonPropertyName("FaultMessage")] [JsonInclude]
    public String? FaultMessage { get; init; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="Mode"]/*'/>*/
    [JsonPropertyName("Mode")] [JsonInclude]
    public DataItemTransferMode Mode { get; init; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="ObjectInfo"]/*'/>*/
    [JsonPropertyName("ObjectInfo")] [JsonInclude]
    public DataItemTransferObjectInfo? ObjectInfo { get; init; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="ObjectPayload"]/*'/>*/
    [JsonPropertyName("ObjectPayload")] [JsonInclude]
    public Byte[] ObjectPayload { get; init; } = Array.Empty<Byte>();

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="ObjectSHA512"]/*'/>*/
    [JsonPropertyName("ObjectSHA512")] [JsonInclude]
    public Byte[] ObjectSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="SegmentSizePolicy"]/*'/>*/
    [JsonPropertyName("SegmentSizePolicy")] [JsonInclude]
    public DataItemTransferSegmentSizePolicy SegmentSizePolicy { get; init; } = new();

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="SourceSessionID"]/*'/>*/
    [JsonPropertyName("SourceSessionID")] [JsonInclude]
    public Guid? SourceSessionID { get; init; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="StateVersion"]/*'/>*/
    [JsonPropertyName("StateVersion")] [JsonInclude]
    public Int64 StateVersion { get; init; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="Status"]/*'/>*/
    [JsonPropertyName("Status")] [JsonInclude]
    public DataItemTransferStatus Status { get; init; }

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="StreamSHA512"]/*'/>*/
    [JsonPropertyName("StreamSHA512")] [JsonInclude]
    public Byte[] StreamSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='DataStreamTransferManifest.xml' path='DataStreamTransferManifest/record[@name="DataStreamTransferManifest"]/property[@name="Updated"]/*'/>*/
    [JsonPropertyName("Updated")] [JsonInclude]
    public DateTimeOffset Updated { get; init; }
}
