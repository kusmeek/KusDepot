namespace TempDB;

/**<include file='ITempDB.xml' path='ITempDB/interface[@name="ITempDB"]/main/*'/>*/
public interface ITempDB : IActor
{
    /**<include file='ITempDB.xml' path='ITempDB/interface[@name="ITempDB"]/method[@name="Delete"]/*'/>*/
    public Task<Boolean> Delete(String? id , String? traceid , String? spanid);

    /**<include file='ITempDB.xml' path='ITempDB/interface[@name="ITempDB"]/method[@name="Exists"]/*'/>*/
    public Task<Boolean?> Exists(String? id , String? traceid , String? spanid);

    /**<include file='ITempDB.xml' path='ITempDB/interface[@name="ITempDB"]/method[@name="Get"]/*'/>*/
    public Task<Byte[]> Get(String? id , String? traceid , String? spanid);

    /**<include file='ITempDB.xml' path='ITempDB/interface[@name="ITempDB"]/method[@name="Store"]/*'/>*/
    public Task<Boolean> Store(String? id , Byte[]? it , String? traceid , String? spanid);
}