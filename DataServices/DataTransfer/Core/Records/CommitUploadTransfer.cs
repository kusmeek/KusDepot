namespace KusDepot.Data.Transfer;

/**<include file='CommitUploadTransfer.xml' path='CommitUploadTransfer/record[@name="CommitUploadTransferRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record CommitUploadTransferRequest
{
    /**<include file='CommitUploadTransfer.xml' path='CommitUploadTransfer/record[@name="CommitUploadTransferRequest"]/property[@name="ExpectedStateVersion"]/*'/>*/
    [JsonPropertyName("ExpectedStateVersion")] [JsonInclude]
    public Int64 ExpectedStateVersion { get; init; }

    /**<include file='CommitUploadTransfer.xml' path='CommitUploadTransfer/record[@name="CommitUploadTransferRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='CommitUploadTransfer.xml' path='CommitUploadTransfer/record[@name="CommitUploadTransferRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}

/**<include file='CommitUploadTransfer.xml' path='CommitUploadTransfer/record[@name="CommitUploadTransferResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record CommitUploadTransferResponse
{
    /**<include file='CommitUploadTransfer.xml' path='CommitUploadTransfer/record[@name="CommitUploadTransferResponse"]/property[@name="Published"]/*'/>*/
    [JsonPropertyName("Published")] [JsonInclude]
    public Boolean Published { get; init; }

    /**<include file='CommitUploadTransfer.xml' path='CommitUploadTransfer/record[@name="CommitUploadTransferResponse"]/property[@name="State"]/*'/>*/
    [JsonPropertyName("State")] [JsonInclude]
    public DataItemTransferState State { get; init; } = new();
}
