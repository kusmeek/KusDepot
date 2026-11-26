namespace KusDepot.Data.Clients;

/**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/main/*'/>*/
public sealed partial class DataControlClient : IDataControlClient , IDisposable , IAsyncDisposable
{
    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="disposed"]/*'/>*/
    private Int32 disposed;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="Endpoint"]/*'/>*/
    private readonly Uri Endpoint;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="Client"]/*'/>*/
    private readonly HttpClient Client;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="HttpClientOwned"]/*'/>*/
    private readonly Boolean HttpClientOwned;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="DefaultToken"]/*'/>*/
    private readonly String? DefaultToken;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="Options"]/*'/>*/
    private readonly DataControlClientOptions Options;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="UploadPayloadRateLimiter"]/*'/>*/
    private readonly AsyncByteRateLimiter? UploadPayloadRateLimiter;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="DownloadPayloadRateLimiter"]/*'/>*/
    private readonly AsyncByteRateLimiter? DownloadPayloadRateLimiter;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/field[@name="NotificationClient"]/*'/>*/
    private readonly SignalRDataControlNotificationClient NotificationClient;

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/constructor[@name="Constructor"]/*'/>*/
    public DataControlClient(String endpoint , String? token = null , DataControlClientOptions? options = null)
    {
        this.Endpoint = new Uri(endpoint,UriKind.Absolute); this.DefaultToken = token;

        options ??= new DataControlClientOptions(); this.Options = options;

        if(options.HttpClient is HttpClient c && (c.BaseAddress is not null || c.DefaultRequestHeaders.Authorization is not null))
        {
            throw new ArgumentException(HttpClientConfigurationInvalid,nameof(options));
        }

        this.HttpClientOwned = options.HttpClient is null || options.HttpClientOwned;

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.UploadPayloadRateLimit ?? 1,nameof(options.UploadPayloadRateLimit));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.DownloadPayloadRateLimit ?? 1,nameof(options.DownloadPayloadRateLimit));

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(options.DownloadStateRefreshInterval,TimeSpan.Zero,nameof(options.DownloadStateRefreshInterval));

        ArgumentOutOfRangeException.ThrowIfGreaterThan(options.DownloadStateRefreshInterval,TimeSpan.FromMilliseconds(Int32.MaxValue),nameof(options.DownloadStateRefreshInterval));

        this.UploadPayloadRateLimiter = options.UploadPayloadRateLimit is Int64 uploadpayloadratelimit ? new(uploadpayloadratelimit) : null;

        this.DownloadPayloadRateLimiter = options.DownloadPayloadRateLimit is Int64 downloadpayloadratelimit ? new(downloadpayloadratelimit) : null;

        this.NotificationClient = new SignalRDataControlNotificationClient(endpoint);

        this.Client = options.HttpClient ?? new HttpClient();
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<Guid>> Delete(Guid? id , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<Object,Guid>(HttpMethod.Delete,$"/Delete/{id}",token:ResolveToken(token),cancel:cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DeleteFail); throw; }
    }
}
