namespace KusDepot.Data.Clients;

/**<include file='CatalogClientOptions.xml' path='CatalogClientOptions/record[@name="CatalogClientOptions"]/main/*'/>*/
public sealed record CatalogClientOptions
{
    /**<include file='CatalogClientOptions.xml' path='CatalogClientOptions/record[@name="CatalogClientOptions"]/property[@name="HttpClient"]/*'/>*/
    public HttpClient? HttpClient { get; init; }

    /**<include file='CatalogClientOptions.xml' path='CatalogClientOptions/record[@name="CatalogClientOptions"]/property[@name="HttpClientOwned"]/*'/>*/
    public Boolean HttpClientOwned { get; init; }
}
