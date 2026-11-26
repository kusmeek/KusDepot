namespace KusDepot.Data.Clients;

public sealed partial class CatalogClient
{
    /**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/method[@name="ResolveToken"]/*'/>*/
    private String? ResolveToken(String? token) => String.IsNullOrWhiteSpace(token) ? this.DefaultToken : token;

    /**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/method[@name="CreateRequestUri"]/*'/>*/
    private Uri CreateRequestUri(String path) => new(this.Endpoint,path);

    /**<include file='CatalogClient.xml' path='CatalogClient/class[@name="CatalogClient"]/method[@name="SendAsync"]/*'/>*/
    private async Task<CatalogResponse<TResponse>> SendAsync<TRequest,TResponse>(HttpMethod method , String path , TRequest? request = default , String? token = null , CancellationToken cancel = default)
    {
        using HttpRequestMessage message = new(method,this.CreateRequestUri(path));

        if(String.IsNullOrWhiteSpace(token) is false)
        {
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token);
        }

        if(request is not null)
        {
            message.Content = new StringContent(JsonUtility.ToJsonString(request),Encoding.UTF8,DataTypes.JSON);
        }

        using HttpResponseMessage response = await this.Client.SendAsync(message,HttpCompletionOption.ResponseContentRead,cancel).ConfigureAwait(false);

        String content = await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);

        TResponse? data = default;

        if(String.IsNullOrWhiteSpace(content) is false)
        {
            try { data = JsonUtility.Parse<TResponse>(content); }

            catch ( Exception _ ) { KusDepotLog.Error(_,CatalogResponseDeserializationFail,typeof(TResponse).FullName); }
        }

        return new CatalogResponse<TResponse> { StatusCode = response.StatusCode , Content = content , Data = data };
    }
}
