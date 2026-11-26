namespace KusDepot.Data.Clients;

/**<include file='DataControlUploadCompletionResult.xml' path='DataControlUploadCompletionResult/record[@name="DataControlUploadCompletionResult"]/main/*'/>*/
public sealed record DataControlUploadCompletionResult
{
    /**<include file='DataControlUploadCompletionResult.xml' path='DataControlUploadCompletionResult/record[@name="DataControlUploadCompletionResult"]/property[@name="Segmented"]/*'/>*/
    public PublishSegmentedTransferResponse? Segmented { get; init; }

    /**<include file='DataControlUploadCompletionResult.xml' path='DataControlUploadCompletionResult/record[@name="DataControlUploadCompletionResult"]/property[@name="Session"]/*'/>*/
    public DataControlTransferSessionInfo Session { get; init; } = new();

    /**<include file='DataControlUploadCompletionResult.xml' path='DataControlUploadCompletionResult/record[@name="DataControlUploadCompletionResult"]/property[@name="Stream"]/*'/>*/
    public PublishStreamTransferResponse? Stream { get; init; }

    /**<include file='DataControlUploadCompletionResult.xml' path='DataControlUploadCompletionResult/record[@name="DataControlUploadCompletionResult"]/property[@name="TransferFamily"]/*'/>*/
    public DataControlTransferFamily TransferFamily { get; init; } = DataControlTransferFamily.Segmented;
}
