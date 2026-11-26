namespace DataPodServices.DataControl;

/**<include file='TenantTransferStorageLimits.xml' path='TenantTransferStorageLimits/class[@name="TenantTransferStorageLimits"]/main/*'/>*/
internal static class TenantTransferStorageLimits
{
    /**<include file='TenantTransferStorageLimits.xml' path='TenantTransferStorageLimits/class[@name="TenantTransferStorageLimits"]/field[@name="MaximumBlockSizeBytes"]/*'/>*/
    public const Int64 MaximumBlockSizeBytes = 4000L * MiB;

    /**<include file='TenantTransferStorageLimits.xml' path='TenantTransferStorageLimits/class[@name="TenantTransferStorageLimits"]/field[@name="MaximumBlockCount"]/*'/>*/
    public const Int32 MaximumBlockCount = 50000;

    /**<include file='TenantTransferStorageLimits.xml' path='TenantTransferStorageLimits/class[@name="TenantTransferStorageLimits"]/method[@name="CountBlocks"]/*'/>*/
    public static Int64 CountBlocks(Int64 length , Int64 maximumBlockSizeBytes = MaximumBlockSizeBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumBlockSizeBytes);

        return 1 + (length - 1) / maximumBlockSizeBytes;
    }

    /**<include file='TenantTransferStorageLimits.xml' path='TenantTransferStorageLimits/class[@name="TenantTransferStorageLimits"]/method[@name="ValidateBudget"]/*'/>*/
    public static void ValidateBudget(Guid sessionid , Int64 currentBlocks , Int64 requestedBlocks)
    {
        if(currentBlocks < 0 || requestedBlocks < 0) { throw new InvalidOperationException("The publication block count is invalid."); }

        if(currentBlocks > MaximumBlockCount || requestedBlocks > MaximumBlockCount - currentBlocks)
        {
            throw new PublicationBlockLimitException(sessionid,currentBlocks,requestedBlocks);
        }
    }
}
