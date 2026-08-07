namespace KusDepot.Data.Transfer;

/**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed partial record DataItemTransferManifest
{
    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="Created"]/*'/>*/
    [JsonPropertyName("Created")] [JsonInclude]
    public DateTimeOffset Created { get; init; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="FaultMessage"]/*'/>*/
    [JsonPropertyName("FaultMessage")] [JsonInclude]
    public String? FaultMessage { get; init; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="Mode"]/*'/>*/
    [JsonPropertyName("Mode")] [JsonInclude]
    public DataItemTransferMode Mode { get; init; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="ObjectInfo"]/*'/>*/
    [JsonPropertyName("ObjectInfo")] [JsonInclude]
    public DataItemTransferObjectInfo? ObjectInfo { get; init; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="ObjectSHA512"]/*'/>*/
    [JsonPropertyName("ObjectSHA512")] [JsonInclude]
    public Byte[] ObjectSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="RealizedRanges"]/*'/>*/
    [JsonPropertyName("RealizedRanges")] [JsonInclude]
    public DataItemTransferRange[] RealizedRanges { get; init; } = Array.Empty<DataItemTransferRange>();

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="SegmentSizePolicy"]/*'/>*/
    [JsonPropertyName("SegmentSizePolicy")] [JsonInclude]
    public DataItemTransferSegmentSizePolicy SegmentSizePolicy { get; init; } = new();

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="SourceSessionID"]/*'/>*/
    [JsonPropertyName("SourceSessionID")] [JsonInclude]
    public Guid? SourceSessionID { get; init; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="StateVersion"]/*'/>*/
    [JsonPropertyName("StateVersion")] [JsonInclude]
    public Int64 StateVersion { get; init; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="Status"]/*'/>*/
    [JsonPropertyName("Status")] [JsonInclude]
    public DataItemTransferStatus Status { get; init; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="StreamLength"]/*'/>*/
    [JsonPropertyName("StreamLength")] [JsonInclude]
    public Int64 StreamLength { get; init; }

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="StreamSHA512"]/*'/>*/
    [JsonPropertyName("StreamSHA512")] [JsonInclude]
    public Byte[] StreamSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='DataItemTransferManifest.xml' path='DataItemTransferManifest/record[@name="DataItemTransferManifest"]/property[@name="Updated"]/*'/>*/
    [JsonPropertyName("Updated")] [JsonInclude]
    public DateTimeOffset Updated { get; init; }
}
