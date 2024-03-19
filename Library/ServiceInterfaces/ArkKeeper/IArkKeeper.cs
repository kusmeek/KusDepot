namespace KusDepot.Data;

/**<include file='IArkKeeper.xml' path='IArkKeeper/interface[@name="IArkKeeper"]/main/*'/>*/
public interface IArkKeeper : IActor
{
    /**<include file='IArkKeeper.xml' path='IArkKeeper/interface[@name="IArkKeeper"]/method[@name="AddUpdate"]/*'/>*/
    public Task<Boolean> AddUpdate(Descriptor? descriptor , String? traceid , String? spanid);

    /**<include file='IArkKeeper.xml' path='IArkKeeper/interface[@name="IArkKeeper"]/method[@name="Exists"]/*'/>*/
    public Task<Boolean?> Exists(Descriptor? descriptor , String? traceid , String? spanid);

    /**<include file='IArkKeeper.xml' path='IArkKeeper/interface[@name="IArkKeeper"]/method[@name="ExistsID"]/*'/>*/
    public Task<Boolean?> ExistsID(Guid? id , String? traceid , String? spanid);

    /**<include file='IArkKeeper.xml' path='IArkKeeper/interface[@name="IArkKeeper"]/method[@name="GetArk"]/*'/>*/
    public Task<Byte[]?> GetArk(String? traceid , String? spanid);

    /**<include file='IArkKeeper.xml' path='IArkKeeper/interface[@name="IArkKeeper"]/method[@name="Remove"]/*'/>*/
    public Task<Boolean> Remove(Descriptor? descriptor , String? traceid , String? spanid);

    /**<include file='IArkKeeper.xml' path='IArkKeeper/interface[@name="IArkKeeper"]/method[@name="RemoveID"]/*'/>*/
    public Task<Boolean> RemoveID(Guid? id , String? traceid , String? spanid);

    /**<include file='IArkKeeper.xml' path='IArkKeeper/interface[@name="IArkKeeper"]/method[@name="StoreArk"]/*'/>*/
    public Task<Boolean> StoreArk(Byte[]? ark , String? traceid , String? spanid);
}