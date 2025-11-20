namespace KusDepot.Data;

/**<include file='IManagement.xml' path='IManagement/interface[@name="IManagement"]/main/*'/>*/
public interface IManagement : IActor
{
    /**<include file='IManagement.xml' path='IManagement/interface[@name="IManagement"]/method[@name="BackupDataConfigs"]/*'/>*/
    public Task<Boolean> BackupDataConfigs(String connection , String certificate , String token , String? traceid , String? spanid);

    /**<include file='IManagement.xml' path='IManagement/interface[@name="IManagement"]/method[@name="RestoreDataConfigs"]/*'/>*/
    public Task<Boolean> RestoreDataConfigs(String connection , String certificate , String token , String? traceid , String? spanid);
}