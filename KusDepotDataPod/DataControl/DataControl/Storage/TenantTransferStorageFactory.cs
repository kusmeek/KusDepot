namespace DataPodServices.DataControl;

/**<include file='TenantTransferStorageFactory.xml' path='TenantTransferStorageFactory/interface[@name="ITenantTransferStorageFactory"]/main/*'/>*/
internal interface ITenantTransferStorageFactory
{
    /**<include file='TenantTransferStorageFactory.xml' path='TenantTransferStorageFactory/interface[@name="ITenantTransferStorageFactory"]/method[@name="Create"]/*'/>*/
    ITenantTransferStorage Create(StorageSilo silo , Guid sessionid , Guid itemid , Guid itemInstanceToken);

    /**<include file='TenantTransferStorageFactory.xml' path='TenantTransferStorageFactory/interface[@name="ITenantTransferStorageFactory"]/method[@name="DeleteInstanceTransfersAsync"]/*'/>*/
    Task<Boolean> DeleteInstanceTransfersAsync(StorageSilo silo , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default);
}

/**<include file='TenantTransferStorageFactory.xml' path='TenantTransferStorageFactory/class[@name="TenantTransferStorageFactory"]/main/*'/>*/
internal sealed class TenantTransferStorageFactory : ITenantTransferStorageFactory
{
    /**<include file='TenantTransferStorageFactory.xml' path='TenantTransferStorageFactory/class[@name="TenantTransferStorageFactory"]/method[@name="Create"]/*'/>*/
    public ITenantTransferStorage Create(StorageSilo silo , Guid sessionid , Guid itemid , Guid itemInstanceToken)
    {
        return new TenantTransferStorage(silo,sessionid,itemid,itemInstanceToken);
    }

    /// <inheritdoc/>
    public Task<Boolean> DeleteInstanceTransfersAsync(StorageSilo silo , Guid itemid , Guid itemInstanceToken , CancellationToken cancel = default)
    {
        return TenantTransferStorage.DeleteInstanceTransfersAsync(silo,itemid,itemInstanceToken,cancel);
    }
}
