namespace KusDepot.Data;

/**<include file='IUniverse.xml' path='IUniverse/interface[@name="IUniverse"]/main/*'/>*/
public interface IUniverse : IActor
{
    /**<include file='IUniverse.xml' path='IUniverse/interface[@name="IUniverse"]/method[@name="Add"]/*'/>*/
    public Task<Boolean> Add(Guid? id , String? traceid , String? spanid);

    /**<include file='IUniverse.xml' path='IUniverse/interface[@name="IUniverse"]/method[@name="Exists"]/*'/>*/
    public Task<Boolean?> Exists(Guid? id , String? traceid , String? spanid);

    /**<include file='IUniverse.xml' path='IUniverse/interface[@name="IUniverse"]/method[@name="GetSize"]/*'/>*/
    public Task<Int32> GetSize(String? traceid , String? spanid);

    /**<include file='IUniverse.xml' path='IUniverse/interface[@name="IUniverse"]/method[@name="ListAll"]/*'/>*/
    public Task<HashSet<Guid>?> ListAll(String token , String? traceid , String? spanid);

    /**<include file='IUniverse.xml' path='IUniverse/interface[@name="IUniverse"]/method[@name="Remove"]/*'/>*/
    public Task<Boolean> Remove(Guid? id , String? traceid , String? spanid);

    /**<include file='IUniverse.xml' path='IUniverse/interface[@name="IUniverse"]/method[@name="Reset"]/*'/>*/
    public Task<Boolean> Reset(HashSet<Guid>? ids , String token , String? traceid , String? spanid);
}