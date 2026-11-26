namespace DataPodServices.DataControl;

/**<include file='PublishedArtifactReference.xml' path='PublishedArtifactReference/record[@name="PublishedArtifactReference"]/main/*'/>*/
internal sealed record PublishedArtifactReference
{
    /**<include file='PublishedArtifactReference.xml' path='PublishedArtifactReference/record[@name="PublishedArtifactReference"]/property[@name="BlobName"]/*'/>*/
    public String BlobName { get; init; } = String.Empty;

    /**<include file='PublishedArtifactReference.xml' path='PublishedArtifactReference/record[@name="PublishedArtifactReference"]/property[@name="Length"]/*'/>*/
    public Int64 Length { get; init; }

    /**<include file='PublishedArtifactReference.xml' path='PublishedArtifactReference/record[@name="PublishedArtifactReference"]/property[@name="SHA512"]/*'/>*/
    public Byte[] SHA512 { get; init; } = Array.Empty<Byte>();
}
