namespace KusDepot.Data;

/**<include file='IBlob.xml' path='IBlob/interface[@name="IBlob"]/main/*'/>*/
public interface IBlob : IActor
{
    /**<include file='IBlob.xml' path='IBlob/interface[@name="IBlob"]/method[@name="Delete"]/*'/>*/
    public Task<Boolean> Delete(String? connection , String? id , String? version , String? traceid , String? spanid);

    /**<include file='IBlob.xml' path='IBlob/interface[@name="IBlob"]/method[@name="Exists"]/*'/>*/
    public Task<Boolean?> Exists(String? connection , String? id , String? version , String? traceid , String? spanid);

    /**<include file='IBlob.xml' path='IBlob/interface[@name="IBlob"]/method[@name="Get"]/*'/>*/
    public Task<Byte[]?> Get(String? connection , String? id , String? version , String? traceid , String? spanid);

    /**<include file='IBlob.xml' path='IBlob/interface[@name="IBlob"]/method[@name="Store"]/*'/>*/
    public Task<Boolean> Store(String? connection , String? id , Byte[]? it , String? traceid , String? spanid);
}