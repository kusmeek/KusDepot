namespace KusDepot.Data.Transfer;

/**<include file='OpenUploadTransfer.xml' path='OpenUploadTransfer/record[@name="OpenUploadTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record OpenUploadTransferRequest
{
    /**<include file='OpenUploadTransfer.xml' path='OpenUploadTransfer/record[@name="OpenUploadTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='OpenUploadTransfer.xml' path='OpenUploadTransfer/record[@name="OpenUploadTransferRequest"]/property[@name="ObjectInfo"]/*'/>*/
    [JsonPropertyName("ObjectInfo")] [JsonInclude]
    public DataItemTransferObjectInfo? ObjectInfo { get; init; }

    /**<include file='OpenUploadTransfer.xml' path='OpenUploadTransfer/record[@name="OpenUploadTransferRequest"]/property[@name="ObjectPayload"]/*'/>*/
    [JsonPropertyName("ObjectPayload")] [JsonInclude]
    public Byte[] ObjectPayload { get; init; } = Array.Empty<Byte>();

    /**<include file='OpenUploadTransfer.xml' path='OpenUploadTransfer/record[@name="OpenUploadTransferRequest"]/property[@name="ObjectSHA512"]/*'/>*/
    [JsonPropertyName("ObjectSHA512")] [JsonInclude]
    public Byte[] ObjectSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='OpenUploadTransfer.xml' path='OpenUploadTransfer/record[@name="OpenUploadTransferRequest"]/property[@name="RequestedSegmentSizePolicy"]/*'/>*/
    [JsonPropertyName("RequestedSegmentSizePolicy")] [JsonInclude]
    public DataItemTransferSegmentSizePolicy? RequestedSegmentSizePolicy { get; init; }

    /**<include file='OpenUploadTransfer.xml' path='OpenUploadTransfer/record[@name="OpenUploadTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='OpenUploadTransfer.xml' path='OpenUploadTransfer/record[@name="OpenUploadTransferRequest"]/property[@name="StreamLength"]/*'/>*/
    [JsonPropertyName("StreamLength")] [JsonInclude]
    public Int64 StreamLength { get; init; }

    /**<include file='OpenUploadTransfer.xml' path='OpenUploadTransfer/record[@name="OpenUploadTransferRequest"]/property[@name="StreamSHA512"]/*'/>*/
    [JsonPropertyName("StreamSHA512")] [JsonInclude]
    public Byte[] StreamSHA512 { get; init; } = Array.Empty<Byte>();
}

/**<include file='OpenUploadTransfer.xml' path='OpenUploadTransfer/record[@name="OpenUploadTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record OpenUploadTransferResponse
{
    /**<include file='OpenUploadTransfer.xml' path='OpenUploadTransfer/record[@name="OpenUploadTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataItemTransferState State { get; init; } = new();
}
