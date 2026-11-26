namespace DataPodServices.DataControl;

/**<include file='TransferIntegrityEvidenceBuilder.xml' path='TransferIntegrityEvidenceBuilder/class[@name="TransferIntegrityEvidenceBuilder"]/main/*'/>*/
internal static class TransferIntegrityEvidenceBuilder
{
    /**<include file='TransferIntegrityEvidenceBuilder.xml' path='TransferIntegrityEvidenceBuilder/class[@name="TransferIntegrityEvidenceBuilder"]/method[@name="BuildAsync"]/*'/>*/
    public static async Task<(TransferContentIntegrity Integrity,TransferContentIntegritySegment[] Segments)?> BuildAsync(
        IEnumerable<TransferContentIntegritySegment> sourceSegments , Int64 prefixLength,
        Func<TransferContentIntegritySegment,Int64,CancellationToken,Task<Byte[]>>? hashPartialSegment,
        CancellationToken cancel)
    {
        if(sourceSegments is null || prefixLength < 0) { return null; }

        List<TransferContentIntegritySegment> prefixSegments = new(); Int64 expectedOffset = 0;

        foreach(TransferContentIntegritySegment segment in sourceSegments.OrderBy(item => item.Offset))
        {
            if(expectedOffset == prefixLength) { break; }

            if(segment is null || segment.Offset != expectedOffset || segment.Length <= 0 || segment.SHA512 is null || segment.SHA512.Length != 64) { return null; }

            Int64 length = Math.Min(segment.Length,prefixLength - expectedOffset);

            Byte[] hash = segment.SHA512;

            if(length != segment.Length)
            {
                if(hashPartialSegment is null) { return null; }

                hash = await hashPartialSegment(segment,length,cancel);

                if(hash is null || hash.Length != 64) { return null; }
            }

            prefixSegments.Add(new() { Offset = expectedOffset , Length = length , SHA512 = hash });

            expectedOffset = checked(expectedOffset + length);
        }

        if(expectedOffset != prefixLength) { return null; }

        TransferContentIntegrity integrity;

        try { integrity = TransferContentIntegrityCalculator.Compute(prefixSegments); } catch ( ArgumentException ) { return null; }

        return (integrity,prefixSegments.ToArray());
    }

    /**<include file='TransferIntegrityEvidenceBuilder.xml' path='TransferIntegrityEvidenceBuilder/class[@name="TransferIntegrityEvidenceBuilder"]/method[@name="HashPartialSegmentAsync"]/*'/>*/
    public static async Task<Byte[]> HashPartialSegmentAsync(
        TransferContentIntegritySegment segment , Int64 prefixLength,
        Func<DataItemTransferRange,CancellationToken,Task<Stream?>> openSegment,
        CancellationToken cancel)
    {
        if(segment is null || segment.Length <= 0 || prefixLength <= 0 || prefixLength >= segment.Length || segment.SHA512 is null || segment.SHA512.Length != 64) { throw new ArgumentException("The partial integrity segment is invalid.",nameof(segment)); }

        await using Stream? payload = await openSegment(new DataItemTransferRange { Offset = segment.Offset , Length = segment.Length },cancel);

        if(payload is null) { throw new IOException("The source integrity segment could not be read."); }

        using IncrementalHash fullHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);

        using IncrementalHash prefixHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);

        Byte[] buffer = new Byte[64*KiB]; Int64 readTotal = 0;

        while(readTotal < segment.Length)
        {
            Int32 requested = (Int32)Math.Min(buffer.Length,segment.Length - readTotal);

            Int32 read = await payload.ReadAsync(buffer.AsMemory(0,requested),cancel);

            if(read == 0) { throw new EndOfStreamException("The source integrity segment ended before its declared length."); }

            fullHash.AppendData(buffer,0,read);

            Int64 prefixRemaining = prefixLength - readTotal;

            if(prefixRemaining > 0) { prefixHash.AppendData(buffer,0,(Int32)Math.Min(read,prefixRemaining)); }

            readTotal += read;
        }

        Byte[] actualFullHash = fullHash.GetHashAndReset();

        if(actualFullHash.AsSpan().SequenceEqual(segment.SHA512) is false) { throw new InvalidDataException("The source integrity segment does not match its persisted SHA-512 digest."); }

        return prefixHash.GetHashAndReset();
    }
}
