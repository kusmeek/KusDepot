namespace KusDepot.Data.Clients;

/**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/main/*'/>*/
public interface ICatalogClient : IDisposable
{
    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="EnumerateElementsAsync"]/*'/>*/
    IAsyncEnumerable<KusDepot.Data.Models.Element> EnumerateElementsAsync(ElementQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="EnumerateCommandsAsync"]/*'/>*/
    IAsyncEnumerable<KusDepot.Data.Models.Command> EnumerateCommandsAsync(CommandQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="EnumerateServicesAsync"]/*'/>*/
    IAsyncEnumerable<KusDepot.Data.Models.Service> EnumerateServicesAsync(ServiceQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="EnumerateMediaAsync"]/*'/>*/
    IAsyncEnumerable<KusDepot.Data.Models.Media> EnumerateMediaAsync(MediaQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="EnumerateNoteIDsAsync"]/*'/>*/
    IAsyncEnumerable<Guid> EnumerateNoteIDsAsync(NoteQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="EnumerateTagIDsAsync"]/*'/>*/
    IAsyncEnumerable<Guid> EnumerateTagIDsAsync(TagQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchElements"]/*'/>*/
    Task<CatalogResponse<ElementResponse>> SearchElements(ElementQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchCommands"]/*'/>*/
    Task<CatalogResponse<CommandResponse>> SearchCommands(CommandQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchServices"]/*'/>*/
    Task<CatalogResponse<ServiceResponse>> SearchServices(ServiceQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchMedia"]/*'/>*/
    Task<CatalogResponse<MediaResponse>> SearchMedia(MediaQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchNotes"]/*'/>*/
    Task<CatalogResponse<NoteResponse>> SearchNotes(NoteQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchTags"]/*'/>*/
    Task<CatalogResponse<TagResponse>> SearchTags(TagQuery query , String? token = null , CancellationToken cancel = default);
}