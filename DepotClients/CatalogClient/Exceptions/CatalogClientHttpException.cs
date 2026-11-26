namespace KusDepot.Data.Clients;

/**<include file='CatalogClientHttpException.xml' path='CatalogClientHttpException/class[@name="CatalogClientHttpException"]/main/*'/>*/
public sealed class CatalogClientHttpException : HttpRequestException
{
    /**<include file='CatalogClientHttpException.xml' path='CatalogClientHttpException/class[@name="CatalogClientHttpException"]/property[@name="ResponseBody"]/*'/>*/
    public String ResponseBody { get; }

    /**<include file='CatalogClientHttpException.xml' path='CatalogClientHttpException/class[@name="CatalogClientHttpException"]/property[@name="StatusCode"]/*'/>*/
    public HttpStatusCode ResponseStatusCode { get; }

    /**<include file='CatalogClientHttpException.xml' path='CatalogClientHttpException/class[@name="CatalogClientHttpException"]/constructor[@name="Constructor"]/*'/>*/
    public CatalogClientHttpException(HttpStatusCode statusCode , String responseBody)
        : base($"Catalog request failed with HTTP {(Int32)statusCode} ({statusCode}).",null,statusCode)
    {
        this.ResponseBody = responseBody; this.ResponseStatusCode = statusCode;
    }
}