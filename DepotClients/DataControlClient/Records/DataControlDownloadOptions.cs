namespace KusDepot.Data.Clients;

/**<include file='DataControlDownloadOptions.xml' path='DataControlDownloadOptions/record[@name="DataControlDownloadOptions"]/main/*'/>*/
public sealed record DataControlDownloadOptions
{
    /**<include file='DataControlDownloadOptions.xml' path='DataControlDownloadOptions/record[@name="DataControlDownloadOptions"]/property[@name="BindLiveStreamItem"]/*'/>*/
    public Boolean BindLiveStreamItem { get; init; }

    /**<include file='DataControlDownloadOptions.xml' path='DataControlDownloadOptions/record[@name="DataControlDownloadOptions"]/property[@name="DownloadRequestSizeBytes"]/*'/>*/
    public Int64 DownloadRequestSizeBytes { get; init; } = 8 * MiB;

    /**<include file='DataControlDownloadOptions.xml' path='DataControlDownloadOptions/record[@name="DataControlDownloadOptions"]/property[@name="Mode"]/*'/>*/
    public DataControlDownloadMode Mode { get; init; } = DataControlDownloadMode.PublishedRead;

    /**<include file='DataControlDownloadOptions.xml' path='DataControlDownloadOptions/record[@name="DataControlDownloadOptions"]/property[@name="SourceSessionID"]/*'/>*/
    public Guid? SourceSessionID { get; init; }
}
