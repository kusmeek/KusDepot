namespace KusDepot.Data.Services.Blob;

/**<include file='IBlob.xml' path='IBlob/interface[@name="IBlob"]/main/*'/>*/
public interface IBlob
{
    /**<include file='IBlob.xml' path='IBlob/interface[@name="IBlob"]/method[@name="Initialize"]/*'/>*/
    Boolean Initialize(String? connection , String? id , String? version = null);

    /**<include file='IBlob.xml' path='IBlob/interface[@name="IBlob"]/method[@name="Get"]/*'/>*/
    Task<Byte[]?> Get(CancellationToken cancel = default);

    /**<include file='IBlob.xml' path='IBlob/interface[@name="IBlob"]/method[@name="Store"]/*'/>*/
    Task<Boolean> Store(Byte[]? it , CancellationToken cancel = default);

    /**<include file='IBlob.xml' path='IBlob/interface[@name="IBlob"]/method[@name="Delete"]/*'/>*/
    Task<Boolean> Delete(CancellationToken cancel = default);

    /**<include file='IBlob.xml' path='IBlob/interface[@name="IBlob"]/method[@name="Exists"]/*'/>*/
    Task<Boolean?> Exists(CancellationToken cancel = default);
}