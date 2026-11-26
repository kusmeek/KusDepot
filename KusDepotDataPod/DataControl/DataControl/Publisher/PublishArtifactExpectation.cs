namespace DataPodServices.DataControl;

internal sealed record PublishArtifactExpectation
{
    public Byte[] ObjectSHA512 { get; init; } = Array.Empty<Byte>();

    public Int64 ObjectLength { get; init; }

    public Boolean PublishStream { get; init; }

    public Int64 StreamLength { get; init; }

    public TransferContentIntegrity? StreamIntegrity { get; init; }

    public IReadOnlyList<TransferContentIntegritySegment>? StreamSegments { get; init; }
}
