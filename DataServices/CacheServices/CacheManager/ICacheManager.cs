namespace KusDepot.Data.Services.CoreCache;

/**<include file='ICacheManager.xml' path='ICacheManager/interface[@name="ICacheManager"]/main/*'/>*/
public interface ICacheManager
{
    /**<include file='ICacheManager.xml' path='ICacheManager/interface[@name="ICacheManager"]/method[@name="Delete"]/*'/>*/
    Task<Boolean> Delete(String? id , String? traceid , String? spanid);

    /**<include file='ICacheManager.xml' path='ICacheManager/interface[@name="ICacheManager"]/method[@name="Exists"]/*'/>*/
    Task<Boolean?> Exists(String? id , String? traceid , String? spanid);

    /**<include file='ICacheManager.xml' path='ICacheManager/interface[@name="ICacheManager"]/method[@name="Get"]/*'/>*/
    Task<String?> Get(String? id , String? traceid , String? spanid);

    /**<include file='ICacheManager.xml' path='ICacheManager/interface[@name="ICacheManager"]/method[@name="Store"]/*'/>*/
    Task<Boolean> Store(String? id , String? it , String? traceid , String? spanid);

    /**<include file='ICacheManager.xml' path='ICacheManager/interface[@name="ICacheManager"]/method[@name="EnqueueItem"]/*'/>*/
    void EnqueueItem(String? id , String? it , String? dt , String? ds);
}