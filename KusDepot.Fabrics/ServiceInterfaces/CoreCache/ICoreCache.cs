namespace KusDepot.Data;

/**<include file='ICoreCache.xml' path='ICoreCache/interface[@name="ICoreCache"]/main/*'/>*/
public interface ICoreCache : IActor
{
    /**<include file='ICoreCache.xml' path='ICoreCache/interface[@name="ICoreCache"]/method[@name="Delete"]/*'/>*/
    public Task<Boolean> Delete(String? id , String? traceid , String? spanid);

    /**<include file='ICoreCache.xml' path='ICoreCache/interface[@name="ICoreCache"]/method[@name="Exists"]/*'/>*/
    public Task<Boolean?> Exists(String? id , String? traceid , String? spanid);

    /**<include file='ICoreCache.xml' path='ICoreCache/interface[@name="ICoreCache"]/method[@name="Get"]/*'/>*/
    public Task<String?> Get(String? id , String? traceid , String? spanid);

    /**<include file='ICoreCache.xml' path='ICoreCache/interface[@name="ICoreCache"]/method[@name="Store"]/*'/>*/
    public Task<Boolean> Store(String? id , String? it , String? traceid , String? spanid);
}