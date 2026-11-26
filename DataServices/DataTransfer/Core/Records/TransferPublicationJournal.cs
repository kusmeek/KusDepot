namespace KusDepot.Data.Transfer;

/**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record TransferPublicationJournal
{
    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="Created"]/*'/>*/
    [JsonPropertyName("Created")] [JsonInclude]
    public DateTimeOffset Created { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="ExpectedStateVersion"]/*'/>*/
    [JsonPropertyName("ExpectedStateVersion")] [JsonInclude]
    public Int64 ExpectedStateVersion { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="CatalogName"]/*'/>*/
    [JsonPropertyName("CatalogName")] [JsonInclude]
    public String CatalogName { get; init; } = String.Empty;

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="Family"]/*'/>*/
    [JsonPropertyName("Family")] [JsonInclude]
    public TransferPublicationFamily Family { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="FinalObjectInfo"]/*'/>*/
    [JsonPropertyName("FinalObjectInfo")] [JsonInclude]
    public DataItemTransferObjectInfo? FinalObjectInfo { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="FinalObjectPayload"]/*'/>*/
    [JsonPropertyName("FinalObjectPayload")] [JsonInclude]
    public Byte[] FinalObjectPayload { get; init; } = Array.Empty<Byte>();

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="FinalStreamLength"]/*'/>*/
    [JsonPropertyName("FinalStreamLength")] [JsonInclude]
    public Int64? FinalStreamLength { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="Integrity"]/*'/>*/
    [JsonPropertyName("Integrity")] [JsonInclude]
    public TransferContentIntegrity? Integrity { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="FailureCount"]/*'/>*/
    [JsonPropertyName("FailureCount")] [JsonInclude]
    public Int32 FailureCount { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="ItemInstanceToken"]/*'/>*/
    [JsonPropertyName("ItemInstanceToken")] [JsonInclude]
    public Guid ItemInstanceToken { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="OperationID"]/*'/>*/
    [JsonPropertyName("OperationID")] [JsonInclude]
    public Guid OperationID { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="LastError"]/*'/>*/
    [JsonPropertyName("LastError")] [JsonInclude]
    public String? LastError { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="Phase"]/*'/>*/
    [JsonPropertyName("Phase")] [JsonInclude]
    public TransferPublicationPhase Phase { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='TransferPublicationJournal.xml' path='TransferPublicationJournal/record[@name="TransferPublicationJournal"]/property[@name="Updated"]/*'/>*/
    [JsonPropertyName("Updated")] [JsonInclude]
    public DateTimeOffset Updated { get; init; }
}
