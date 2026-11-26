namespace KusDepot.Data.Transfer;

/**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record GetTransferIntegrityEvidenceRequest
{
    /**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceRequest"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceRequest"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceRequest"]/property[@name="TransferFamily"]/*'/>*/
    [JsonPropertyName("TransferFamily")] [JsonInclude]
    public DataControlTransferFamily TransferFamily { get; init; }

    /**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceRequest"]/property[@name="PrefixLength"]/*'/>*/
    [JsonPropertyName("PrefixLength")] [JsonInclude]
    public Int64 PrefixLength { get; init; }
}

/**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceResponse"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record GetTransferIntegrityEvidenceResponse
{
    /**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceResponse"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceResponse"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceResponse"]/property[@name="SourceSessionID"]/*'/>*/
    [JsonPropertyName("SourceSessionID")] [JsonInclude]
    public Guid? SourceSessionID { get; init; }

    /**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceResponse"]/property[@name="SourceStateVersion"]/*'/>*/
    [JsonPropertyName("SourceStateVersion")] [JsonInclude]
    public Int64? SourceStateVersion { get; init; }

    /**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceResponse"]/property[@name="Integrity"]/*'/>*/
    [JsonPropertyName("Integrity")] [JsonInclude]
    public TransferContentIntegrity Integrity { get; init; } = new();

    /**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceResponse"]/property[@name="Segments"]/*'/>*/
    [JsonPropertyName("Segments")] [JsonInclude]
    public TransferContentIntegritySegment[] Segments { get; init; } = Array.Empty<TransferContentIntegritySegment>();

    /**<include file='GetTransferIntegrityEvidence.xml' path='GetTransferIntegrityEvidence/record[@name="GetTransferIntegrityEvidenceResponse"]/property[@name="IsPublished"]/*'/>*/
    [JsonPropertyName("IsPublished")] [JsonInclude]
    public Boolean IsPublished { get; init; }
}
