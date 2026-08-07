namespace KusDepot.Data.Transfer;

/**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record CompleteStreamTransferRequest
{
    /**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferRequest"]/property[@name="ExpectedStateVersion"]/*'/>*/
    [JsonPropertyName("ExpectedStateVersion")] [JsonInclude]
    public Int64 ExpectedStateVersion { get; init; }

    /**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferRequest"]/property[@name="FinalObjectInfo"]/*'/>*/
    [JsonPropertyName("FinalObjectInfo")] [JsonInclude]
    public DataItemTransferObjectInfo? FinalObjectInfo { get; init; }

    /**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferRequest"]/property[@name="FinalObjectPayload"]/*'/>*/
    [JsonPropertyName("FinalObjectPayload")] [JsonInclude]
    public Byte[] FinalObjectPayload { get; init; } = Array.Empty<Byte>();

    /**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferRequest"]/property[@name="FinalObjectSHA512"]/*'/>*/
    [JsonPropertyName("FinalObjectSHA512")] [JsonInclude]
    public Byte[] FinalObjectSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferRequest"]/property[@name="FinalStreamLength"]/*'/>*/
    [JsonPropertyName("FinalStreamLength")] [JsonInclude]
    public Int64? FinalStreamLength { get; init; }

    /**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferRequest"]/property[@name="FinalStreamSHA512"]/*'/>*/
    [JsonPropertyName("FinalStreamSHA512")] [JsonInclude]
    public Byte[] FinalStreamSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}

/**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record CompleteStreamTransferResponse
{
    /**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferResponse"]/property[@name="Published"]/*'/>*/
    [JsonPropertyName("Published")] [JsonInclude]
    public Boolean Published { get; init; }

    /**<include file='CompleteStreamTransfer.xml' path='CompleteStreamTransfer/record[@name="CompleteStreamTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataStreamTransferState State { get; init; } = new();
}
