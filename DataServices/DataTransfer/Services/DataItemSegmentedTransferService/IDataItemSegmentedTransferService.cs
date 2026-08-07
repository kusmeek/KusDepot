namespace KusDepot.Data.Services.DataTransfer;

/**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/main/*'/>*/
public interface IDataItemSegmentedTransferService
{
    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="AbortTransfer"]/*'/>*/
    Task<AbortTransferResponse> AbortTransfer(AbortTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="BeginCommitUploadTransfer"]/*'/>*/
    Task<CommitUploadTransferResponse> BeginCommitUploadTransfer(CommitUploadTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="CancelCommitUploadTransfer"]/*'/>*/
    Task<GetTransferStateResponse> CancelCommitUploadTransfer(DataItemTransferIdentity identity , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="CompleteCommitUploadTransfer"]/*'/>*/
    Task<CommitUploadTransferResponse> CompleteCommitUploadTransfer(DataItemTransferIdentity identity , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="GetSessions"]/*'/>*/
    Task<DataControlServerSessionInfo[]> GetSessions(GetTransferSessionsRequest? request = null , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="GetTransferSegment"]/*'/>*/
    Task<SegmentedTransferReadResult> GetTransferSegment(GetTransferSegmentRequest request , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="GetTransferState"]/*'/>*/
    Task<GetTransferStateResponse> GetTransferState(DataItemTransferIdentity identity , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="OpenGetTransfer"]/*'/>*/
    Task<OpenGetTransferResponse> OpenGetTransfer(OpenGetTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="OpenUploadTransfer"]/*'/>*/
    Task<OpenUploadTransferResponse> OpenUploadTransfer(OpenUploadTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="PutTransferSegment"]/*'/>*/
    Task<PutTransferSegmentResponse> PutTransferSegment(PutTransferSegmentRequest request , Stream payload , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="RemoveTransfer"]/*'/>*/
    Task<RemoveTransferResponse> RemoveTransfer(RemoveTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="ReOpenGetTransfer"]/*'/>*/
    Task<ReOpenGetTransferResponse> ReOpenGetTransfer(ReOpenGetTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="ReOpenUploadTransfer"]/*'/>*/
    Task<ReOpenUploadTransferResponse> ReOpenUploadTransfer(ReOpenUploadTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataItemSegmentedTransferService.xml' path='IDataItemSegmentedTransferService/interface[@name="IDataItemSegmentedTransferService"]/method[@name="SessionExists"]/*'/>*/
    Task<Boolean> SessionExists(DataItemTransferIdentity identity , CancellationToken cancel = default);
}