namespace KusDepot.Data.Clients;

/**<include file='DataControlUploadOptions.xml' path='DataControlUploadOptions/record[@name="DataControlUploadOptions"]/main/*'/>*/
public sealed record DataControlUploadOptions
{
    /**<include file='DataControlUploadOptions.xml' path='DataControlUploadOptions/record[@name="DataControlUploadOptions"]/property[@name="CompletionMode"]/*'/>*/
    public StreamCompletionMode CompletionMode { get; init; } = StreamCompletionMode.UntilSourceCompletes;

    /**<include file='DataControlUploadOptions.xml' path='DataControlUploadOptions/record[@name="DataControlUploadOptions"]/property[@name="MaxBytes"]/*'/>*/
    public Int64? MaxBytes { get; init; }

    /**<include file='DataControlUploadOptions.xml' path='DataControlUploadOptions/record[@name="DataControlUploadOptions"]/property[@name="PublishFinalStreamHash"]/*'/>*/
    public Boolean PublishFinalStreamHash { get; init; } = true;

    /**<include file='DataControlUploadOptions.xml' path='DataControlUploadOptions/record[@name="DataControlUploadOptions"]/property[@name="RequestedSegmentSizePolicy"]/*'/>*/
    public DataItemTransferSegmentSizePolicy? RequestedSegmentSizePolicy { get; init; }

    /**<include file='DataControlUploadOptions.xml' path='DataControlUploadOptions/record[@name="DataControlUploadOptions"]/property[@name="TemporaryStagingPath"]/*'/>*/
    public String? TemporaryStagingPath { get; init; }
}
