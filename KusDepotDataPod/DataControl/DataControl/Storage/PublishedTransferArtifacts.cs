namespace DataPodServices.DataControl;

/**<include file='PublishedTransferArtifacts.xml' path='PublishedTransferArtifacts/record[@name="PublishedTransferArtifacts"]/main/*'/>*/
internal sealed record PublishedTransferArtifacts
{
    /**<include file='PublishedTransferArtifacts.xml' path='PublishedTransferArtifacts/record[@name="PublishedTransferArtifacts"]/property[@name="Object"]/*'/>*/
    public PublishedArtifactReference Object { get; init; } = new();

    /**<include file='PublishedTransferArtifacts.xml' path='PublishedTransferArtifacts/record[@name="PublishedTransferArtifacts"]/property[@name="Stream"]/*'/>*/
    public PublishedArtifactReference? Stream { get; init; }

    /**<include file='PublishedTransferArtifacts.xml' path='PublishedTransferArtifacts/record[@name="PublishedTransferArtifacts"]/property[@name="Integrity"]/*'/>*/
    public PublishedArtifactReference Integrity { get; init; } = new();
}
