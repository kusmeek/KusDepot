namespace KusDepot.Data.Clients;

/**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/main/*'/>*/
public interface ICatalogClient : ICatalogClientBase , IDisposable
{
    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchElements"]/*'/>*/
    Task<RestResponse<ElementResponse?>> SearchElements(ElementQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchCommands"]/*'/>*/
    Task<RestResponse<CommandResponse?>> SearchCommands(CommandQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchServices"]/*'/>*/
    Task<RestResponse<ServiceResponse?>> SearchServices(ServiceQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchMedia"]/*'/>*/
    Task<RestResponse<MediaResponse?>> SearchMedia(MediaQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchNotes"]/*'/>*/
    Task<RestResponse<NoteResponse?>> SearchNotes(NoteQuery query , String? token = null , CancellationToken cancel = default);

    /**<include file='ICatalogClient.xml' path='ICatalogClient/interface[@name="ICatalogClient"]/method[@name="SearchTags"]/*'/>*/
    Task<RestResponse<TagResponse?>> SearchTags(TagQuery query , String? token = null , CancellationToken cancel = default);
}