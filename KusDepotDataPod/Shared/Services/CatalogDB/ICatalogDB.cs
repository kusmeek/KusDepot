namespace DataPodServices.CatalogDB;

[Alias("DataPodServices.ICatalogDB")]
/**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/main/*'/>*/
public interface ICatalogDB : IGrainWithStringKey
{
    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="AddUpdate"]/*'/>*/
    [Alias("AddUpdate")]
    Task<Boolean> AddUpdate([Immutable] Descriptor? descriptor , Guid itemInstanceToken , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="BeginDelete"]/*'/>*/
    [Alias("BeginDelete")]
    Task<CatalogItemIdentity?> BeginDelete(Guid itemID , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="FinalizeDelete"]/*'/>*/
    [Alias("FinalizeDelete")]
    Task<CatalogDeleteFinalizationResult> FinalizeDelete(Guid itemID , Guid itemInstanceToken , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="GetItemIdentity"]/*'/>*/
    [Alias("GetItemIdentity")]
    Task<CatalogItemIdentity?> GetItemIdentity(Guid itemID , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="GetItemState"]/*'/>*/
    [Alias("GetItemState")]
    Task<CatalogItemState?> GetItemState(Guid itemID , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="GetPendingDeletes"]/*'/>*/
    [Alias("GetPendingDeletes")]
    Task<CatalogPendingDeletePage> GetPendingDeletes(Guid? afterItemID , Int32 pageSize , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="Exists"]/*'/>*/
    [Alias("Exists")]
    Task<Boolean?> Exists(Guid? id , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchCommands"]/*'/>*/
    [Alias("SearchCommands")]
    Task<CommandResponse> SearchCommands([Immutable] CommandQuery? query , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchElements"]/*'/>*/
    [Alias("SearchElements")]
    Task<ElementResponse> SearchElements([Immutable] ElementQuery? query , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchMedia"]/*'/>*/
    [Alias("SearchMedia")]
    Task<MediaResponse> SearchMedia([Immutable] MediaQuery? query , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchNotes"]/*'/>*/
    [Alias("SearchNotes")]
    Task<NoteResponse> SearchNotes([Immutable] NoteQuery? query , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchServices"]/*'/>*/
    [Alias("SearchServices")]
    Task<ServiceResponse> SearchServices([Immutable] ServiceQuery? query , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    /**<include file='ICatalogDB.xml' path='ICatalogDB/interface[@name="ICatalogDB"]/method[@name="SearchTags"]/*'/>*/
    [Alias("SearchTags")]
    Task<TagResponse> SearchTags([Immutable] TagQuery? query , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);
}