namespace KusDepot.Data.Clients;

/**<include file='CatalogResponse.xml' path='CatalogResponse/record[@name="CatalogResponse"]/main/*'/>*/
public sealed record CatalogResponse<T>
{
    /**<include file='CatalogResponse.xml' path='CatalogResponse/record[@name="CatalogResponse"]/property[@name="Content"]/*'/>*/
    public String? Content { get; init; }

    /**<include file='CatalogResponse.xml' path='CatalogResponse/record[@name="CatalogResponse"]/property[@name="Data"]/*'/>*/
    public T? Data { get; init; }

    /**<include file='CatalogResponse.xml' path='CatalogResponse/record[@name="CatalogResponse"]/property[@name="StatusCode"]/*'/>*/
    public HttpStatusCode StatusCode { get; init; }
}
