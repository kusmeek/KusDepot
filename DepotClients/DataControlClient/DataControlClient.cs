namespace KusDepot.Data.Clients;

/**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/main/*'/>*/
public sealed partial class DataControlClient : IDataControlClient , IDisposable , IAsyncDisposable
{
    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="disposed"]/*'/>*/
    private Int32 disposed;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="Client"]/*'/>*/
    private readonly RestClient Client;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="DefaultToken"]/*'/>*/
    private readonly String? DefaultToken;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="Auth"]/*'/>*/
    private readonly JwtAuthenticator Auth;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/property[@name="EndPoint"]/*'/>*/
    public String EndPoint { private get; init;}

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/property[@name="Certificate"]/*'/>*/
    public X509Certificate2 Certificate { private get; init;}

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="NotificationClient"]/*'/>*/
    private readonly SignalRDataControlNotificationClient NotificationClient;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/constructor[@name="Constructor"]/*'/>*/
    public DataControlClient(String endpoint , X509Certificate2 certificate , String? token = null)
    {
        this.DefaultToken = token; this.Auth = new JwtAuthenticator(token ?? "null");

        this.Client = new(new RestClientOptions(endpoint){ Authenticator = this.Auth,
            ClientCertificates = new(){certificate} }); this.EndPoint = endpoint; this.Certificate = certificate;

        this.NotificationClient = new SignalRDataControlNotificationClient(endpoint,certificate);
    }

    ///<inheritdoc/>
    public async Task<RestResponse<Guid>> Delete(Guid? id , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest($"/Delete/{id}",Method.Delete); if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<Guid>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DeleteFail); if(NoExceptions) { return null!; } throw; }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="ResolveToken"]/*'/>*/
    private String? ResolveToken(String? token) => String.IsNullOrWhiteSpace(token) ? this.DefaultToken : token;
}
