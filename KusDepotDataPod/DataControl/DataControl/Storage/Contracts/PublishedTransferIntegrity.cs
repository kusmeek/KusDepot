namespace DataPodServices.DataControl;

/**<include file='PublishedTransferIntegrity.xml' path='PublishedTransferIntegrity/record[@name="PublishedTransferIntegrity"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PublishedTransferIntegrity
{
    /**<include file='PublishedTransferIntegrity.xml' path='PublishedTransferIntegrity/record[@name="PublishedTransferIntegrity"]/property[@name="ObjectSHA512"]/*'/>*/
    [JsonPropertyName("ObjectSHA512")] [JsonInclude]
    public Byte[] ObjectSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='PublishedTransferIntegrity.xml' path='PublishedTransferIntegrity/record[@name="PublishedTransferIntegrity"]/property[@name="Integrity"]/*'/>*/
    [JsonPropertyName("Integrity")] [JsonInclude]
    public TransferContentIntegrity? Integrity { get; init; }

    /**<include file='PublishedTransferIntegrity.xml' path='PublishedTransferIntegrity/record[@name="PublishedTransferIntegrity"]/property[@name="Segments"]/*'/>*/
    [JsonPropertyName("Segments")] [JsonInclude]
    public TransferContentIntegritySegment[] Segments { get; init; } = Array.Empty<TransferContentIntegritySegment>();
}
