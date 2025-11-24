namespace KusDepot.Data;

/**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/main/*'/>*/
public interface ICatalogDB : IActor
{
    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="AddUpdate"]/*'/>*/
    public Task<Boolean> AddUpdate(Descriptor? descriptor , String? traceid , String? spanid);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="Exists"]/*'/>*/
    public Task<Boolean?> Exists(Descriptor? descriptor , String? traceid , String? spanid);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="ExistsID"]/*'/>*/
    public Task<Boolean?> ExistsID(Guid? id , String? traceid , String? spanid);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="Remove"]/*'/>*/
    public Task<Boolean> Remove(Descriptor? descriptor , String? traceid , String? spanid);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="RemoveID"]/*'/>*/
    public Task<Boolean> RemoveID(Guid? id , String? traceid , String? spanid);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchCommands"]/*'/>*/
    public Task<CommandResponse> SearchCommands(CommandQuery? query , String? traceid , String? spanid);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchElements"]/*'/>*/
    public Task<ElementResponse> SearchElements(ElementQuery? query , String? traceid , String? spanid);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchMedia"]/*'/>*/
    public Task<MediaResponse> SearchMedia(MediaQuery? query , String? traceid , String? spanid);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchNotes"]/*'/>*/
    public Task<NoteResponse> SearchNotes(NoteQuery? query , String? traceid , String? spanid);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchServices"]/*'/>*/
    public Task<ServiceResponse> SearchServices(ServiceQuery? query , String? traceid , String? spanid);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchTags"]/*'/>*/
    public Task<TagResponse> SearchTags(TagQuery? query , String? traceid , String? spanid);
}