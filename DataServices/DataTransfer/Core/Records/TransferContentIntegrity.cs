namespace KusDepot.Data.Transfer;

/**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/record[@name="TransferContentIntegrity"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record TransferContentIntegrity
{
    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/record[@name="TransferContentIntegrity"]/property[@name="Algorithm"]/*'/>*/
    [JsonPropertyName("Algorithm")] [JsonInclude]
    public String Algorithm { get; init; } = TransferContentIntegrityCalculator.Algorithm;

    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/record[@name="TransferContentIntegrity"]/property[@name="Digest"]/*'/>*/
    [JsonPropertyName("Digest")] [JsonInclude]
    public Byte[] Digest { get; init; } = Array.Empty<Byte>();

    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/record[@name="TransferContentIntegrity"]/property[@name="Length"]/*'/>*/
    [JsonPropertyName("Length")] [JsonInclude]
    public Int64 Length { get; init; }

    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/record[@name="TransferContentIntegrity"]/property[@name="SegmentCount"]/*'/>*/
    [JsonPropertyName("SegmentCount")] [JsonInclude]
    public Int64 SegmentCount { get; init; }

    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/record[@name="TransferContentIntegrity"]/method[@name="Validate"]/*'/>*/
    public Boolean Validate()
    {
        return String.Equals(this.Algorithm,TransferContentIntegrityCalculator.Algorithm,StringComparison.Ordinal)
            && this.Digest.Length == 64
            && this.Length >= 0
            && this.SegmentCount >= 0;
    }
}

/**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/record[@name="TransferContentIntegritySegment"]/main/*'/>*/
public sealed record TransferContentIntegritySegment
{
    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/record[@name="TransferContentIntegritySegment"]/property[@name="Offset"]/*'/>*/
    public Int64 Offset { get; init; }

    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/record[@name="TransferContentIntegritySegment"]/property[@name="Length"]/*'/>*/
    public Int64 Length { get; init; }

    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/record[@name="TransferContentIntegritySegment"]/property[@name="SHA512"]/*'/>*/
    public Byte[] SHA512 { get; init; } = Array.Empty<Byte>();
}

/**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/class[@name="TransferContentIntegrityCalculator"]/main/*'/>*/
public static class TransferContentIntegrityCalculator
{
    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/class[@name="TransferContentIntegrityCalculator"]/field[@name="Algorithm"]/*'/>*/
    public const String Algorithm = "Sha512IntegrityTreeV1";

    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/class[@name="TransferContentIntegrityCalculator"]/field[@name="LeafDomain"]/*'/>*/
    private static readonly Byte[] LeafDomain = "KusDepot.Sha512IntegrityTreeV1.Leaf"u8.ToArray();

    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/class[@name="TransferContentIntegrityCalculator"]/field[@name="NodeDomain"]/*'/>*/
    private static readonly Byte[] NodeDomain = "KusDepot.Sha512IntegrityTreeV1.Node"u8.ToArray();

    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/class[@name="TransferContentIntegrityCalculator"]/field[@name="EmptyDomain"]/*'/>*/
    private static readonly Byte[] EmptyDomain = "KusDepot.Sha512IntegrityTreeV1.Empty"u8.ToArray();

    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/class[@name="TransferContentIntegrityCalculator"]/method[@name="Compute"]/*'/>*/
    public static TransferContentIntegrity Compute(IEnumerable<TransferContentIntegritySegment> segments)
    {
        ArgumentNullException.ThrowIfNull(segments);

        TransferContentIntegritySegment[] ordered = segments.OrderBy(item => item.Offset).ToArray();

        List<Byte[]> leaves = new(ordered.Length);

        Int64 totalLength = 0; Int64 expectedOffset = 0;

        Span<Byte> header = stackalloc Byte[sizeof(Int64) * 2];

        foreach(TransferContentIntegritySegment segment in ordered)
        {
            ArgumentNullException.ThrowIfNull(segment);

            if(segment.Offset != expectedOffset || segment.Length <= 0 || segment.SHA512.Length != 64)
            {
                throw new ArgumentException("Integrity segments must be non-empty, contiguous, and SHA-512 verified.",nameof(segments));
            }

            WriteInt64BigEndian(header[..sizeof(Int64)],segment.Offset);
            WriteInt64BigEndian(header[sizeof(Int64)..],segment.Length);

            using IncrementalHash leaf = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);

            leaf.AppendData(LeafDomain);
            leaf.AppendData(header);
            leaf.AppendData(segment.SHA512);
            leaves.Add(leaf.GetHashAndReset());

            expectedOffset = checked(expectedOffset + segment.Length);

            totalLength = expectedOffset;
        }

        Byte[] digest = leaves.Count == 0 ? SHA512.HashData(EmptyDomain) : ComputeRoot(leaves);

        return new TransferContentIntegrity { Digest = digest , Length = totalLength , SegmentCount = ordered.LongLength };
    }

    /**<include file='TransferContentIntegrity.xml' path='TransferContentIntegrity/class[@name="TransferContentIntegrityCalculator"]/method[@name="ComputeRoot"]/*'/>*/
    private static Byte[] ComputeRoot(IReadOnlyList<Byte[]> nodes)
    {
        List<Byte[]> current = nodes.ToList();

        while(current.Count > 1)
        {
            List<Byte[]> next = new((current.Count + 1) / 2);

            for(Int32 i = 0 ; i < current.Count ; i += 2)
            {
                Byte[] right = i + 1 < current.Count ? current[i + 1] : current[i];

                using IncrementalHash node = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);

                node.AppendData(NodeDomain);
                node.AppendData(current[i]);
                node.AppendData(right);
                next.Add(node.GetHashAndReset());
            }

            current = next;
        }

        return current[0];
    }
}
