namespace KusDepot.Data.Clients;

/**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/main/*'/>*/
public sealed class CatalogClient : ICatalogClient , IDisposable
{
    /**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/field[@name="Client"]/*'/>*/
    private readonly RestClient Client;

    /**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/field[@name="Auth"]/*'/>*/
    private readonly JwtAuthenticator Auth;

    /**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/constructor[@name="Constructor"]/*'/>*/
    public CatalogClient(String endpoint , X509Certificate2 certificate , String? token = null)
    {
        this.Auth = new JwtAuthenticator(token ?? "null"); this.Client = new(new RestClientOptions(endpoint){ Authenticator = this.Auth , ClientCertificates = new(){certificate} });
    }

    ///<inheritdoc/>
    public Task<RestResponse<ElementResponse?>> SearchElements(ElementQuery query , String? token = null , CancellationToken cancel = default)
    {
        RestRequest _ = new RestRequest("/Catalog/Elements",Method.Post).AddJsonBody(query); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); } return this.Client.ExecuteAsync<ElementResponse?>(_,cancel);
    }

    ///<inheritdoc/>
    public Task<RestResponse<CommandResponse?>> SearchCommands(CommandQuery query , String? token = null , CancellationToken cancel = default)
    {
        RestRequest _ = new RestRequest("/Catalog/Commands",Method.Post).AddJsonBody(query); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); } return this.Client.ExecuteAsync<CommandResponse?>(_,cancel);
    }

    ///<inheritdoc/>
    public Task<RestResponse<ServiceResponse?>> SearchServices(ServiceQuery query , String? token = null , CancellationToken cancel = default)
    {
        RestRequest _ = new RestRequest("/Catalog/Services",Method.Post).AddJsonBody(query); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); } return this.Client.ExecuteAsync<ServiceResponse?>(_,cancel);
    }

    ///<inheritdoc/>
    public Task<RestResponse<MediaResponse?>> SearchMedia(MediaQuery query , String? token = null , CancellationToken cancel = default)
    {
        RestRequest _ = new RestRequest("/Catalog/Media",Method.Post).AddJsonBody(query); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); } return this.Client.ExecuteAsync<MediaResponse?>(_,cancel);
    }

    ///<inheritdoc/>
    public Task<RestResponse<NoteResponse?>> SearchNotes(NoteQuery query , String? token = null , CancellationToken cancel = default)
    {
        RestRequest _ = new RestRequest("/Catalog/Notes",Method.Post).AddJsonBody(query); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); } return this.Client.ExecuteAsync<NoteResponse?>(_,cancel);
    }

    ///<inheritdoc/>
    public Task<RestResponse<TagResponse?>> SearchTags(TagQuery query , String? token = null , CancellationToken cancel = default)
    {
        RestRequest _ = new RestRequest("/Catalog/Tags",Method.Post).AddJsonBody(query); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); } return this.Client.ExecuteAsync<TagResponse?>(_,cancel);
    }

    ///<inheritdoc/>
    public void Dispose() { this.Auth.SetBearerToken("null"); this.Client.Dispose(); }
}