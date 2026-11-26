namespace DataPodServices.DataControl;

/**<include file='TenantTransferStorageContext.xml' path='TenantTransferStorageContext/interface[@name="ITenantTransferStorageContext"]/main/*'/>*/
internal interface ITenantTransferStorageContext
{
    /**<include file='TenantTransferStorageContext.xml' path='TenantTransferStorageContext/interface[@name="ITenantTransferStorageContext"]/property[@name="CatalogName"]/*'/>*/
    String CatalogName { get; }

    /**<include file='TenantTransferStorageContext.xml' path='TenantTransferStorageContext/interface[@name="ITenantTransferStorageContext"]/method[@name="Set"]/*'/>*/
    void Set(StorageSilo silo);

    /**<include file='TenantTransferStorageContext.xml' path='TenantTransferStorageContext/interface[@name="ITenantTransferStorageContext"]/method[@name="Get"]/*'/>*/
    ITenantTransferStorage Get(Guid sessionid , Guid itemid , Guid itemInstanceToken);
}

/**<include file='TenantTransferStorageContext.xml' path='TenantTransferStorageContext/class[@name="TenantTransferStorageContext"]/main/*'/>*/
internal sealed class TenantTransferStorageContext : ITenantTransferStorageContext
{
    /**<include file='TenantTransferStorageContext.xml' path='TenantTransferStorageContext/class[@name="TenantTransferStorageContext"]/field[@name="factory"]/*'/>*/
    private readonly ITenantTransferStorageFactory factory;

    /**<include file='TenantTransferStorageContext.xml' path='TenantTransferStorageContext/class[@name="TenantTransferStorageContext"]/field[@name="silo"]/*'/>*/
    private StorageSilo? silo;

    /**<include file='TenantTransferStorageContext.xml' path='TenantTransferStorageContext/class[@name="TenantTransferStorageContext"]/property[@name="CatalogName"]/*'/>*/
    public String CatalogName => (this.silo ?? throw new InvalidOperationException("Tenant transfer storage has not been selected.")).CatalogName;

    /**<include file='TenantTransferStorageContext.xml' path='TenantTransferStorageContext/class[@name="TenantTransferStorageContext"]/constructor[@name="TenantTransferStorageContext"]/*'/>*/
    public TenantTransferStorageContext(ITenantTransferStorageFactory factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /**<include file='TenantTransferStorageContext.xml' path='TenantTransferStorageContext/class[@name="TenantTransferStorageContext"]/method[@name="Set"]/*'/>*/
    public void Set(StorageSilo silo)
    {
        this.silo = silo ?? throw new ArgumentNullException(nameof(silo));
    }

    /**<include file='TenantTransferStorageContext.xml' path='TenantTransferStorageContext/class[@name="TenantTransferStorageContext"]/method[@name="Get"]/*'/>*/
    public ITenantTransferStorage Get(Guid sessionid , Guid itemid , Guid itemInstanceToken)
    {
        StorageSilo selected = this.silo ?? throw new InvalidOperationException("Tenant transfer storage has not been selected.");

        return this.factory.Create(selected,sessionid,itemid,itemInstanceToken);
    }
}
