namespace KusDepot.Data.Transfer;

/**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/class[@name="PublicationRecoveryArtifactKinds"]/main/*'/>*/
public static class PublicationRecoveryArtifactKinds
{
    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/class[@name="PublicationRecoveryArtifactKinds"]/field[@name="StagedObject"]/*'/>*/
    public const String StagedObject = "staged-object";

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/class[@name="PublicationRecoveryArtifactKinds"]/field[@name="SegmentedRange"]/*'/>*/
    public const String SegmentedRange = "segmented-range";

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/class[@name="PublicationRecoveryArtifactKinds"]/field[@name="StreamChunk"]/*'/>*/
    public const String StreamChunk = "stream-chunk";

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/class[@name="PublicationRecoveryArtifactKinds"]/field[@name="PublicationObjectRecoveryCopy"]/*'/>*/
    public const String PublicationObjectRecoveryCopy = "publication-object-recovery-copy";

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/class[@name="PublicationRecoveryArtifactKinds"]/field[@name="FinalObjectCandidate"]/*'/>*/
    public const String FinalObjectCandidate = "final-object-candidate";

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/class[@name="PublicationRecoveryArtifactKinds"]/field[@name="FinalStreamCandidate"]/*'/>*/
    public const String FinalStreamCandidate = "final-stream-candidate";

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/class[@name="PublicationRecoveryArtifactKinds"]/field[@name="FinalIntegrityCandidate"]/*'/>*/
    public const String FinalIntegrityCandidate = "final-integrity-candidate";
}

/**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="InspectPublicationRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record InspectPublicationRequest
{
    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="InspectPublicationRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }
}

/**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PublicationRecoveryFootprint
{
    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="InspectedAt"]/*'/>*/
    [JsonPropertyName("InspectedAt")] [JsonInclude]
    public DateTimeOffset InspectedAt { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="ExternalArtifactExistenceChecked"]/*'/>*/
    [JsonPropertyName("ExternalArtifactExistenceChecked")] [JsonInclude]
    public Boolean ExternalArtifactExistenceChecked { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="CatalogName"]/*'/>*/
    [JsonPropertyName("CatalogName")] [JsonInclude]
    public String CatalogName { get; init; } = String.Empty;

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="Family"]/*'/>*/
    [JsonPropertyName("Family")] [JsonInclude]
    public DataControlTransferFamily Family { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="Mode"]/*'/>*/
    [JsonPropertyName("Mode")] [JsonInclude]
    public DataItemTransferMode Mode { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="Status"]/*'/>*/
    [JsonPropertyName("Status")] [JsonInclude]
    public DataItemTransferStatus Status { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="SessionStateVersion"]/*'/>*/
    [JsonPropertyName("SessionStateVersion")] [JsonInclude]
    public Int64 SessionStateVersion { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="SessionUpdated"]/*'/>*/
    [JsonPropertyName("SessionUpdated")] [JsonInclude]
    public DateTimeOffset SessionUpdated { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="OperationID"]/*'/>*/
    [JsonPropertyName("OperationID")] [JsonInclude]
    public Guid? OperationID { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="Phase"]/*'/>*/
    [JsonPropertyName("Phase")] [JsonInclude]
    public TransferPublicationPhase? Phase { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="JournalUpdated"]/*'/>*/
    [JsonPropertyName("JournalUpdated")] [JsonInclude]
    public DateTimeOffset? JournalUpdated { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="FailureCount"]/*'/>*/
    [JsonPropertyName("FailureCount")] [JsonInclude]
    public Int32? FailureCount { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="LastError"]/*'/>*/
    [JsonPropertyName("LastError")] [JsonInclude]
    public String? LastError { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="ExpectedStateVersion"]/*'/>*/
    [JsonPropertyName("ExpectedStateVersion")] [JsonInclude]
    public Int64? ExpectedStateVersion { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="ExpectedObjectLength"]/*'/>*/
    [JsonPropertyName("ExpectedObjectLength")] [JsonInclude]
    public Int64? ExpectedObjectLength { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="ExpectedObjectSHA512"]/*'/>*/
    [JsonPropertyName("ExpectedObjectSHA512")] [JsonInclude]
    public Byte[]? ExpectedObjectSHA512 { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="ExpectedStreamLength"]/*'/>*/
    [JsonPropertyName("ExpectedStreamLength")] [JsonInclude]
    public Int64? ExpectedStreamLength { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="ExpectedStreamIntegrity"]/*'/>*/
    [JsonPropertyName("ExpectedStreamIntegrity")] [JsonInclude]
    public TransferContentIntegrity? ExpectedStreamIntegrity { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="StagingArtifacts"]/*'/>*/
    [JsonPropertyName("StagingArtifacts")] [JsonInclude]
    public PublicationRecoveryArtifact[] StagingArtifacts { get; init; } = Array.Empty<PublicationRecoveryArtifact>();

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="StagingPrefix"]/*'/>*/
    [JsonPropertyName("StagingPrefix")] [JsonInclude]
    public String StagingPrefix { get; init; } = String.Empty;

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="FinalArtifacts"]/*'/>*/
    [JsonPropertyName("FinalArtifacts")] [JsonInclude]
    public PublicationRecoveryArtifact[] FinalArtifacts { get; init; } = Array.Empty<PublicationRecoveryArtifact>();

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryFootprint"]/property[@name="IntegrityEvidence"]/*'/>*/
    [JsonPropertyName("IntegrityEvidence")] [JsonInclude]
    public TransferContentIntegrity? IntegrityEvidence { get; init; }
}

/**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryArtifact"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PublicationRecoveryArtifact
{
    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryArtifact"]/property[@name="Kind"]/*'/>*/
    [JsonPropertyName("Kind")] [JsonInclude]
    public String Kind { get; init; } = String.Empty;

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryArtifact"]/property[@name="BlobName"]/*'/>*/
    [JsonPropertyName("BlobName")] [JsonInclude]
    public String BlobName { get; init; } = String.Empty;

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryArtifact"]/property[@name="Length"]/*'/>*/
    [JsonPropertyName("Length")] [JsonInclude]
    public Int64? Length { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryArtifact"]/property[@name="SHA512"]/*'/>*/
    [JsonPropertyName("SHA512")] [JsonInclude]
    public Byte[]? SHA512 { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryArtifact"]/property[@name="OperationID"]/*'/>*/
    [JsonPropertyName("OperationID")] [JsonInclude]
    public Guid? OperationID { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PublicationRecoveryArtifact"]/property[@name="Offset"]/*'/>*/
    [JsonPropertyName("Offset")] [JsonInclude]
    public Int64? Offset { get; init; }
}

/**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PurgePublicationRequest
{
    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationRequest"]/property[@name="CatalogName"]/*'/>*/
    [JsonPropertyName("CatalogName")] [JsonInclude]
    public String CatalogName { get; init; } = String.Empty;

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationRequest"]/property[@name="ExpectedSessionStateVersion"]/*'/>*/
    [JsonPropertyName("ExpectedSessionStateVersion")] [JsonInclude]
    public Int64 ExpectedSessionStateVersion { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationRequest"]/property[@name="ExpectedOperationID"]/*'/>*/
    [JsonPropertyName("ExpectedOperationID")] [JsonInclude]
    public Guid? ExpectedOperationID { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationRequest"]/property[@name="ExpectedPhase"]/*'/>*/
    [JsonPropertyName("ExpectedPhase")] [JsonInclude]
    public TransferPublicationPhase? ExpectedPhase { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationRequest"]/property[@name="ExpectedJournalUpdated"]/*'/>*/
    [JsonPropertyName("ExpectedJournalUpdated")] [JsonInclude]
    public DateTimeOffset? ExpectedJournalUpdated { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationRequest"]/property[@name="ConfirmPurge"]/*'/>*/
    [JsonPropertyName("ConfirmPurge")] [JsonInclude]
    public Boolean ConfirmPurge { get; init; }
}

/**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PurgePublicationResponse
{
    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationResponse"]/property[@name="Purged"]/*'/>*/
    [JsonPropertyName("Purged")] [JsonInclude]
    public Boolean Purged { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationResponse"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationResponse"]/property[@name="OperationID"]/*'/>*/
    [JsonPropertyName("OperationID")] [JsonInclude]
    public Guid? OperationID { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationResponse"]/property[@name="StagingArtifactsDeleted"]/*'/>*/
    [JsonPropertyName("StagingArtifactsDeleted")] [JsonInclude]
    public Boolean StagingArtifactsDeleted { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationResponse"]/property[@name="DatabaseStateDeleted"]/*'/>*/
    [JsonPropertyName("DatabaseStateDeleted")] [JsonInclude]
    public Boolean DatabaseStateDeleted { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationResponse"]/property[@name="FinalArtifactsDeleted"]/*'/>*/
    [JsonPropertyName("FinalArtifactsDeleted")] [JsonInclude]
    public Boolean FinalArtifactsDeleted { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationResponse"]/property[@name="CatalogDataDeleted"]/*'/>*/
    [JsonPropertyName("CatalogDataDeleted")] [JsonInclude]
    public Boolean CatalogDataDeleted { get; init; }

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationResponse"]/property[@name="FinalArtifactCandidatesLeftUntouched"]/*'/>*/
    [JsonPropertyName("FinalArtifactCandidatesLeftUntouched")] [JsonInclude]
    public PublicationRecoveryArtifact[] FinalArtifactCandidatesLeftUntouched { get; init; } = Array.Empty<PublicationRecoveryArtifact>();

    /**<include file='PublicationRecoveryRecords.xml' path='PublicationRecoveryRecords/record[@name="PurgePublicationResponse"]/property[@name="Message"]/*'/>*/
    [JsonPropertyName("Message")] [JsonInclude]
    public String Message { get; init; } = String.Empty;
}
