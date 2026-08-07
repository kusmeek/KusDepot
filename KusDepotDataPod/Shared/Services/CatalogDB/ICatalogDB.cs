namespace DataPodServices.CatalogDB;

[Alias("DataPodServices.ICatalogDB")]
public interface ICatalogDB : IGrainWithStringKey
{
    [Alias("AddUpdate")]
    Task<Boolean> AddUpdate([Immutable] Descriptor? descriptor , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("Exists")]
    Task<Boolean?> Exists([Immutable] Descriptor? descriptor , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("ExistsID")]
    Task<Boolean?> ExistsID([Immutable] Guid? id , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("Remove")]
    Task<Boolean> Remove([Immutable] Descriptor? descriptor , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("RemoveID")]
    Task<Boolean> RemoveID([Immutable] Guid? id , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("SearchCommands")]
    Task<CommandResponse> SearchCommands([Immutable] CommandQuery? query , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("SearchElements")]
    Task<ElementResponse> SearchElements([Immutable] ElementQuery? query , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("SearchMedia")]
    Task<MediaResponse> SearchMedia([Immutable] MediaQuery? query , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("SearchNotes")]
    Task<NoteResponse> SearchNotes([Immutable] NoteQuery? query , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("SearchServices")]
    Task<ServiceResponse> SearchServices([Immutable] ServiceQuery? query , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("SearchTags")]
    Task<TagResponse> SearchTags([Immutable] TagQuery? query , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);
}