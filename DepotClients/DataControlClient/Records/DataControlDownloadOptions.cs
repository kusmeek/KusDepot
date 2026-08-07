namespace KusDepot.Data.Clients;

/**<include file='DataControlDownloadOptions.xml' path='DataControlDownloadOptions/record[@name="DataControlDownloadOptions"]/main/*'/>*/
public sealed record DataControlDownloadOptions
{
    /**<include file='DataControlDownloadOptions.xml' path='DataControlDownloadOptions/record[@name="DataControlDownloadOptions"]/property[@name="BindLiveStreamItem"]/*'/>*/
    public Boolean BindLiveStreamItem { get; init; }

    /**<include file='DataControlDownloadOptions.xml' path='DataControlDownloadOptions/record[@name="DataControlDownloadOptions"]/property[@name="CompletionMode"]/*'/>*/
    public StreamCompletionMode CompletionMode { get; init; } = StreamCompletionMode.UntilSourceCompletes;

    /**<include file='DataControlDownloadOptions.xml' path='DataControlDownloadOptions/record[@name="DataControlDownloadOptions"]/property[@name="MaxBytes"]/*'/>*/
    public Int64? MaxBytes { get; init; }

    /**<include file='DataControlDownloadOptions.xml' path='DataControlDownloadOptions/record[@name="DataControlDownloadOptions"]/property[@name="Mode"]/*'/>*/
    public DataControlDownloadMode Mode { get; init; } = DataControlDownloadMode.Committed;

    /**<include file='DataControlDownloadOptions.xml' path='DataControlDownloadOptions/record[@name="DataControlDownloadOptions"]/property[@name="SourceSessionID"]/*'/>*/
    public Guid? SourceSessionID { get; init; }
}
