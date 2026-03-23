namespace DataPodServices.CatalogDB;

[Alias("DataPodServices.ICatalogDB")]
public interface ICatalogDB : IGrainWithStringKey
{
    [Alias("AddUpdate")]
    Task<Boolean> AddUpdate([Immutable] Descriptor? descriptor , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("Exists")]
    Task<Boolean?> Exists([Immutable] Descriptor? descriptor , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("ExistsID")]
    Task<Boolean?> ExistsID([Immutable] Guid? id , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("Remove")]
    Task<Boolean> Remove([Immutable] Descriptor? descriptor , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("RemoveID")]
    Task<Boolean> RemoveID([Immutable] Guid? id , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("SearchCommands")]
    Task<CommandResponse> SearchCommands([Immutable] CommandQuery? query , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("SearchElements")]
    Task<ElementResponse> SearchElements([Immutable] ElementQuery? query , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("SearchMedia")]
    Task<MediaResponse> SearchMedia([Immutable] MediaQuery? query , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("SearchNotes")]
    Task<NoteResponse> SearchNotes([Immutable] NoteQuery? query , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("SearchServices")]
    Task<ServiceResponse> SearchServices([Immutable] ServiceQuery? query , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("SearchTags")]
    Task<TagResponse> SearchTags([Immutable] TagQuery? query , [Immutable] String? traceid , [Immutable] String? spanid);
}