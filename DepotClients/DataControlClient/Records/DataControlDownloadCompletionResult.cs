namespace KusDepot.Data.Clients;

/**<include file='DataControlDownloadCompletionResult.xml' path='DataControlDownloadCompletionResult/record[@name="DataControlDownloadCompletionResult"]/main/*'/>*/
public sealed record DataControlDownloadCompletionResult
{
    /**<include file='DataControlDownloadCompletionResult.xml' path='DataControlDownloadCompletionResult/record[@name="DataControlDownloadCompletionResult"]/property[@name="Item"]/*'/>*/
    public DataItem? Item { get; init; }

    /**<include file='DataControlDownloadCompletionResult.xml' path='DataControlDownloadCompletionResult/record[@name="DataControlDownloadCompletionResult"]/property[@name="LiveItem"]/*'/>*/
    public DataStreamItem? LiveItem { get; init; }

    /**<include file='DataControlDownloadCompletionResult.xml' path='DataControlDownloadCompletionResult/record[@name="DataControlDownloadCompletionResult"]/property[@name="Session"]/*'/>*/
    public DataControlTransferSessionInfo Session { get; init; } = new();

    /**<include file='DataControlDownloadCompletionResult.xml' path='DataControlDownloadCompletionResult/record[@name="DataControlDownloadCompletionResult"]/property[@name="StreamPath"]/*'/>*/
    public String StreamPath { get; init; } = String.Empty;

    /**<include file='DataControlDownloadCompletionResult.xml' path='DataControlDownloadCompletionResult/record[@name="DataControlDownloadCompletionResult"]/property[@name="TransferFamily"]/*'/>*/
    public DataControlTransferFamily TransferFamily { get; init; } = DataControlTransferFamily.Segmented;
}
