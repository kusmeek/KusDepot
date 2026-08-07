namespace KusDepot.Data.Transfer;

/**<include file='ReOpenUploadTransfer.xml' path='ReOpenUploadTransfer/record[@name="ReOpenUploadTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ReOpenUploadTransferRequest
{
    /**<include file='ReOpenUploadTransfer.xml' path='ReOpenUploadTransfer/record[@name="ReOpenUploadTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='ReOpenUploadTransfer.xml' path='ReOpenUploadTransfer/record[@name="ReOpenUploadTransferRequest"]/property[@name="ObjectSHA512"]/*'/>*/
    [JsonPropertyName("ObjectSHA512")] [JsonInclude]
    public Byte[] ObjectSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='ReOpenUploadTransfer.xml' path='ReOpenUploadTransfer/record[@name="ReOpenUploadTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='ReOpenUploadTransfer.xml' path='ReOpenUploadTransfer/record[@name="ReOpenUploadTransferRequest"]/property[@name="StreamSHA512"]/*'/>*/
    [JsonPropertyName("StreamSHA512")] [JsonInclude]
    public Byte[] StreamSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='ReOpenUploadTransfer.xml' path='ReOpenUploadTransfer/record[@name="ReOpenUploadTransferRequest"]/property[@name="StreamLength"]/*'/>*/
    [JsonPropertyName("StreamLength")] [JsonInclude]
    public Int64 StreamLength { get; init; }
}

/**<include file='ReOpenUploadTransfer.xml' path='ReOpenUploadTransfer/record[@name="ReOpenUploadTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ReOpenUploadTransferResponse
{
    /**<include file='ReOpenUploadTransfer.xml' path='ReOpenUploadTransfer/record[@name="ReOpenUploadTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataItemTransferState State { get; init; } = new();
}