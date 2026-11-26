namespace KusDepot.Data.Clients;

/**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/main/*'/>*/
public interface IDataControlClient : IDisposable , IAsyncDisposable
{
    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="AbortTransfer"]/*'/>*/
    Task<DataControlResponse<AbortTransferResponse>> AbortTransfer(AbortTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="CloseTransferSession"]/*'/>*/
    Task<DataControlResponse<CloseTransferSessionResponse>> CloseTransferSession(CloseTransferSessionRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="Delete"]/*'/>*/
    Task<DataControlResponse<Guid>> Delete(Guid? id , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="DeleteLocalSession"]/*'/>*/
    Task<Boolean> DeleteLocalSession(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="Download"]/*'/>*/
    Task<DataControlDownloadCompletionResult?> Download(Guid itemid , String? token = null , DataControlDownloadOptions? options = null , IProgress<DataItemTransferProgress>? progress = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="GetLocalSession"]/*'/>*/
    Task<DataControlTransferSessionInfo?> GetLocalSession(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="GetLocalSessions"]/*'/>*/
    Task<IReadOnlyList<DataControlTransferSessionInfo>> GetLocalSessions(CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="GetServerSessions"]/*'/>*/
    Task<DataControlResponse<DataControlServerSessionInfo[]>> GetServerSessions(GetTransferSessionsRequest? request = null , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="GetStreamTransferSegment"]/*'/>*/
    Task<StreamSegmentDownloadInfo?> GetStreamTransferSegment(GetStreamTransferSegmentRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="GetStreamTransferState"]/*'/>*/
    Task<DataControlResponse<GetStreamTransferStateResponse>> GetStreamTransferState(DataItemTransferIdentity identity , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="GetTransferIntegrityEvidence"]/*'/>*/
    Task<DataControlResponse<GetTransferIntegrityEvidenceResponse>> GetTransferIntegrityEvidence(GetTransferIntegrityEvidenceRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="GetTransferSegment"]/*'/>*/
    Task<SegmentDownloadInfo?> GetTransferSegment(GetTransferSegmentRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="GetTransferState"]/*'/>*/
    Task<DataControlResponse<GetTransferStateResponse>> GetTransferState(DataItemTransferIdentity identity , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="InspectPublication"]/*'/>*/
    Task<DataControlResponse<PublicationRecoveryFootprint>> InspectPublication(InspectPublicationRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="OpenDownload"]/*'/>*/
    Task<DataControlDownloadSession?> OpenDownload(Guid itemid , String? token = null , DataControlDownloadOptions? options = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="OpenFollowStreamTransfer"]/*'/>*/
    Task<DataControlResponse<OpenFollowStreamTransferResponse>> OpenFollowStreamTransfer(OpenFollowStreamTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="OpenGetTransfer"]/*'/>*/
    Task<DataControlResponse<OpenGetTransferResponse>> OpenGetTransfer(OpenGetTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="OpenStreamTransfer"]/*'/>*/
    Task<DataControlResponse<OpenStreamTransferResponse>> OpenStreamTransfer(OpenStreamTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="OpenUpload"]/*'/>*/
    Task<DataControlUploadSession?> OpenUpload(DataItem item , String? token = null , DataControlUploadOptions? options = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="OpenUploadTransfer"]/*'/>*/
    Task<DataControlResponse<OpenUploadTransferResponse>> OpenUploadTransfer(OpenUploadTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="PublishSegmentedTransfer"]/*'/>*/
    Task<DataControlResponse<PublishSegmentedTransferResponse>> PublishSegmentedTransfer(PublishSegmentedTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="PublishStreamTransfer"]/*'/>*/
    Task<DataControlResponse<PublishStreamTransferResponse>> PublishStreamTransfer(PublishStreamTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="PurgePublication"]/*'/>*/
    Task<DataControlResponse<PurgePublicationResponse>> PurgePublication(PurgePublicationRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="PutStreamTransferSegment"]/*'/>*/
    Task<PutStreamTransferSegmentClientResponse?> PutStreamTransferSegment(PutStreamTransferSegmentRequest request , Stream payload , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="PutTransferSegment"]/*'/>*/
    Task<PutTransferSegmentClientResponse?> PutTransferSegment(PutTransferSegmentRequest request , Stream payload , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="RemoveTransfer"]/*'/>*/
    Task<DataControlResponse<RemoveTransferResponse>> RemoveTransfer(RemoveTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="ReOpenUploadTransfer"]/*'/>*/
    Task<DataControlResponse<ReOpenUploadTransferResponse>> ReOpenUploadTransfer(ReOpenUploadTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="ReOpenFollowStreamTransfer"]/*'/>*/
    Task<DataControlResponse<ReOpenFollowStreamTransferResponse>> ReOpenFollowStreamTransfer(ReOpenFollowStreamTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="ReOpenGetTransfer"]/*'/>*/
    Task<DataControlResponse<ReOpenGetTransferResponse>> ReOpenGetTransfer(ReOpenGetTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="ReOpenStreamTransfer"]/*'/>*/
    Task<DataControlResponse<ReOpenStreamTransferResponse>> ReOpenStreamTransfer(ReOpenStreamTransferRequest request , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="ResumeDownload"]/*'/>*/
    Task<DataControlDownloadSession?> ResumeDownload(Guid sessionid , String? token = null , DataControlDownloadOptions? options = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="ResumeUpload"]/*'/>*/
    Task<DataControlUploadSession?> ResumeUpload(Guid sessionid , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="Upload"]/*'/>*/
    Task<DataControlUploadCompletionResult?> Upload(DataItem item , String? token = null , DataControlUploadOptions? options = null , IProgress<DataItemTransferProgress>? progress = null , CancellationToken cancel = default);
}