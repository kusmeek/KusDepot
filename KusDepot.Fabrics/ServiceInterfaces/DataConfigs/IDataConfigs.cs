namespace KusDepot.Data;

/**<include file='IDataConfigs.xml' path='IDataConfigs/interface[@name="IDataConfigs"]/main/*'/>*/
public interface IDataConfigs : IActor
{
    /**<include file='IDataConfigs.xml' path='IDataConfigs/interface[@name="IDataConfigs"]/method[@name="GetAuthorizedReadSilo"]/*'/>*/
    public Task<StorageSilo?> GetAuthorizedReadSilo(String token , String? traceid , String? spanid);

    /**<include file='IDataConfigs.xml' path='IDataConfigs/interface[@name="IDataConfigs"]/method[@name="GetAuthorizedWriteSilo"]/*'/>*/
    public Task<StorageSilo?> GetAuthorizedWriteSilo(String token , String? traceid , String? spanid);

    /**<include file='IDataConfigs.xml' path='IDataConfigs/interface[@name="IDataConfigs"]/method[@name="GetStorageSilos"]/*'/>*/
    public Task<HashSet<StorageSilo>?> GetStorageSilos(String token , String? traceid , String? spanid);

    /**<include file='IDataConfigs.xml' path='IDataConfigs/interface[@name="IDataConfigs"]/method[@name="SetStorageSilos"]/*'/>*/
    public Task<Boolean> SetStorageSilos(HashSet<StorageSilo>? silos , String token , String? traceid , String? spanid);
}