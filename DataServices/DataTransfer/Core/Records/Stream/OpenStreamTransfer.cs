namespace KusDepot.Data.Transfer;

/**<include file='OpenStreamTransfer.xml' path='OpenStreamTransfer/record[@name="OpenStreamTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record OpenStreamTransferRequest
{
    /**<include file='OpenStreamTransfer.xml' path='OpenStreamTransfer/record[@name="OpenStreamTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='OpenStreamTransfer.xml' path='OpenStreamTransfer/record[@name="OpenStreamTransferRequest"]/property[@name="ObjectInfo"]/*'/>*/
    [JsonPropertyName("ObjectInfo")] [JsonInclude]
    public DataItemTransferObjectInfo? ObjectInfo { get; init; }

    /**<include file='OpenStreamTransfer.xml' path='OpenStreamTransfer/record[@name="OpenStreamTransferRequest"]/property[@name="ObjectPayload"]/*'/>*/
    [JsonPropertyName("ObjectPayload")] [JsonInclude]
    public Byte[] ObjectPayload { get; init; } = Array.Empty<Byte>();

    /**<include file='OpenStreamTransfer.xml' path='OpenStreamTransfer/record[@name="OpenStreamTransferRequest"]/property[@name="ObjectSHA512"]/*'/>*/
    [JsonPropertyName("ObjectSHA512")] [JsonInclude]
    public Byte[] ObjectSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='OpenStreamTransfer.xml' path='OpenStreamTransfer/record[@name="OpenStreamTransferRequest"]/property[@name="RequestedSegmentSizePolicy"]/*'/>*/
    [JsonPropertyName("RequestedSegmentSizePolicy")] [JsonInclude]
    public DataItemTransferSegmentSizePolicy? RequestedSegmentSizePolicy { get; init; }

    /**<include file='OpenStreamTransfer.xml' path='OpenStreamTransfer/record[@name="OpenStreamTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}

/**<include file='OpenStreamTransfer.xml' path='OpenStreamTransfer/record[@name="OpenStreamTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record OpenStreamTransferResponse
{
    /**<include file='OpenStreamTransfer.xml' path='OpenStreamTransfer/record[@name="OpenStreamTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataStreamTransferState State { get; init; } = new();
}
