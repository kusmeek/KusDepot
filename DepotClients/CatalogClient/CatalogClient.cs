namespace KusDepot.Data.Clients;

/**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/main/*'/>*/
public sealed partial class CatalogClient : ICatalogClient , IDisposable
{
    /**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/field[@name="Endpoint"]/*'/>*/
    private readonly Uri Endpoint;

    /**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/field[@name="Client"]/*'/>*/
    private readonly HttpClient Client;

    /**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/field[@name="HttpClientOwned"]/*'/>*/
    private readonly Boolean HttpClientOwned;

    /**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/field[@name="DefaultToken"]/*'/>*/
    private readonly String? DefaultToken;

    /**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/constructor[@name="Constructor"]/*'/>*/
    public CatalogClient(String endpoint , String? token = null , CatalogClientOptions? options = null)
    {
        this.Endpoint = new Uri(endpoint,UriKind.Absolute);

        this.DefaultToken = token;

        options ??= new CatalogClientOptions();

        if(options.HttpClient is HttpClient c && (c.BaseAddress is not null || c.DefaultRequestHeaders.Authorization is not null))
        {
            throw new ArgumentException(HttpClientConfigurationInvalid,nameof(options));
        }

        this.HttpClientOwned = options.HttpClient is null || options.HttpClientOwned;

        this.Client = options.HttpClient ?? new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
    }

    ///<inheritdoc/>
    public async Task<CatalogResponse<ElementResponse>> SearchElements(ElementQuery query , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<ElementQuery,ElementResponse>(HttpMethod.Post,"/Catalog/Elements",query,this.ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SearchElementsFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<CatalogResponse<CommandResponse>> SearchCommands(CommandQuery query , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<CommandQuery,CommandResponse>(HttpMethod.Post,"/Catalog/Commands",query,this.ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SearchCommandsFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<CatalogResponse<ServiceResponse>> SearchServices(ServiceQuery query , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<ServiceQuery,ServiceResponse>(HttpMethod.Post,"/Catalog/Services",query,this.ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SearchServicesFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<CatalogResponse<MediaResponse>> SearchMedia(MediaQuery query , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<MediaQuery,MediaResponse>(HttpMethod.Post,"/Catalog/Media",query,this.ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SearchMediaFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<CatalogResponse<NoteResponse>> SearchNotes(NoteQuery query , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<NoteQuery,NoteResponse>(HttpMethod.Post,"/Catalog/Notes",query,this.ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SearchNotesFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<CatalogResponse<TagResponse>> SearchTags(TagQuery query , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<TagQuery,TagResponse>(HttpMethod.Post,"/Catalog/Tags",query,this.ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SearchTagsFail); throw; }
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        if(this.HttpClientOwned) { this.Client.Dispose(); }
    }
}