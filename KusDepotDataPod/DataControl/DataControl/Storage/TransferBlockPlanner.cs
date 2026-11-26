namespace DataPodServices.DataControl;

/**<include file='TransferBlockPlanner.xml' path='TransferBlockPlanner/class[@name="TransferBlockPlanner"]/main/*'/>*/
internal static class TransferBlockPlanner
{
    /**<include file='TransferBlockPlanner.xml' path='TransferBlockPlanner/class[@name="TransferBlockPlanner"]/method[@name="Create"]/*'/>*/
    public static TransferBlockPlan Create(IReadOnlyList<TransferRangeReference> logicalRanges , Int64 maximumBlockSizeBytes = TenantTransferStorageLimits.MaximumBlockSizeBytes)
    {
        ArgumentNullException.ThrowIfNull(logicalRanges);

        if(maximumBlockSizeBytes <= 0 || maximumBlockSizeBytes > TenantTransferStorageLimits.MaximumBlockSizeBytes) { throw new ArgumentOutOfRangeException(nameof(maximumBlockSizeBytes),"The maximum block size must be positive and within the storage limit."); }

        TransferRangeReference[] ordered = logicalRanges.OrderBy(static item => item?.Range.Offset ?? -1).ToArray();
        List<TransferBlock> blocks = new();
        Int64 expectedOffset = 0;
        Int32 blockIndex = 0;

        foreach((TransferRangeReference reference,Int32 segmentIndex) in ordered.Select(static (item,index) => (item,index)))
        {
            ValidateReference(reference,segmentIndex);

            DataItemTransferRange range = reference.Range;

            if(range.Offset != expectedOffset)
            {
                throw new ArgumentException("Logical transfer ranges must provide contiguous coverage beginning at offset zero.",nameof(logicalRanges));
            }

            Int64 newBlocks = range.Length is 0 ? 0 : TenantTransferStorageLimits.CountBlocks(range.Length,maximumBlockSizeBytes);

            TenantTransferStorageLimits.ValidateBudget(Guid.Empty,blocks.Count,newBlocks);

            Int64 sourceOffset = 0;

            while(sourceOffset < range.Length)
            {
                Int64 remaining = checked(range.Length - sourceOffset);
                Int64 length = Math.Min(remaining,maximumBlockSizeBytes);
                Int64 destinationOffset = checked(range.Offset + sourceOffset);

                blocks.Add(new TransferBlock
                {
                    BlockID = CreateBlockID(blockIndex),
                    DestinationOffset = destinationOffset,
                    Length = length,
                    LogicalSegmentIndex = segmentIndex,
                    LogicalRange = range,
                    Source = reference.Blob,
                    SourceOffset = sourceOffset,
                });

                sourceOffset = checked(sourceOffset + length);
                blockIndex = checked(blockIndex + 1);
            }

            expectedOffset = checked(range.Offset + range.Length);
        }

        return new TransferBlockPlan
        {
            Blocks = blocks,
            LogicalRanges = ordered,
            TotalLength = expectedOffset,
        };
    }

    /**<include file='TransferBlockPlanner.xml' path='TransferBlockPlanner/class[@name="TransferBlockPlanner"]/method[@name="CreateBlockID"]/*'/>*/
    private static String CreateBlockID(Int32 blockIndex)
    {
        String value = String.Concat("tenant-transfer-block-",blockIndex.ToString("D12",CultureInfo.InvariantCulture));

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    /**<include file='TransferBlockPlanner.xml' path='TransferBlockPlanner/class[@name="TransferBlockPlanner"]/method[@name="ValidateReference"]/*'/>*/
    private static void ValidateReference(TransferRangeReference? reference , Int32 segmentIndex)
    {
        if(reference is null) { throw new ArgumentException(String.Concat("Logical range reference ",segmentIndex.ToString(CultureInfo.InvariantCulture)," is required."),nameof(reference)); }

        if(!reference.Range.Validate()) { throw new ArgumentException(String.Concat("Logical range reference ",segmentIndex.ToString(CultureInfo.InvariantCulture)," is invalid."),nameof(reference)); }

        if(String.IsNullOrWhiteSpace(reference.Blob.BlobName)) { throw new ArgumentException(String.Concat("Logical range reference ",segmentIndex.ToString(CultureInfo.InvariantCulture)," has no source blob name."),nameof(reference)); }

        if(reference.Blob.Length != reference.Range.Length) { throw new ArgumentException(String.Concat("Logical range reference ",segmentIndex.ToString(CultureInfo.InvariantCulture)," does not match its source blob length."),nameof(reference)); }
    }
}
