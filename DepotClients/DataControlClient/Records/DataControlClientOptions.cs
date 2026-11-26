namespace KusDepot.Data.Clients;

/**<include file='DataControlClientOptions.xml' path='DataControlClientOptions/record[@name="DataControlClientOptions"]/main/*'/>*/
public sealed record DataControlClientOptions
{
    /**<include file='DataControlClientOptions.xml' path='DataControlClientOptions/record[@name="DataControlClientOptions"]/property[@name="HttpClient"]/*'/>*/
    public HttpClient? HttpClient { get; init; }

    /**<include file='DataControlClientOptions.xml' path='DataControlClientOptions/record[@name="DataControlClientOptions"]/property[@name="HttpClientOwned"]/*'/>*/
    public Boolean HttpClientOwned { get; init; }

    /**<include file='DataControlClientOptions.xml' path='DataControlClientOptions/record[@name="DataControlClientOptions"]/property[@name="UploadPayloadRateLimit"]/*'/>*/
    public Int64? UploadPayloadRateLimit { get; init; }

    /**<include file='DataControlClientOptions.xml' path='DataControlClientOptions/record[@name="DataControlClientOptions"]/property[@name="DownloadPayloadRateLimit"]/*'/>*/
    public Int64? DownloadPayloadRateLimit { get; init; }

    /**<include file='DataControlClientOptions.xml' path='DataControlClientOptions/record[@name="DataControlClientOptions"]/property[@name="DownloadStateRefreshInterval"]/*'/>*/
    public TimeSpan DownloadStateRefreshInterval { get; init; } = TimeSpan.FromHours(1);

    /**<include file='DataControlClientOptions.xml' path='DataControlClientOptions/record[@name="DataControlClientOptions"]/property[@name="WorkingDirectoryOptions"]/*'/>*/
    public DataControlClientWorkingDirectoryOptions WorkingDirectoryOptions { get; init; } = new();
}