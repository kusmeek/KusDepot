namespace KusDepot.Data.Services.DataTransfer;

/**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/main/*'/>*/
public interface IDataStreamTransferService
{
    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="AbortTransfer"]/*'/>*/
    Task<AbortTransferResponse> AbortTransfer(AbortTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="BeginCompleteStreamTransfer"]/*'/>*/
    Task<CompleteStreamTransferResponse> BeginCompleteStreamTransfer(CompleteStreamTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="CancelCompleteStreamTransfer"]/*'/>*/
    Task<GetStreamTransferStateResponse> CancelCompleteStreamTransfer(DataItemTransferIdentity identity , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="FinishCompleteStreamTransfer"]/*'/>*/
    Task<CompleteStreamTransferResponse> FinishCompleteStreamTransfer(DataItemTransferIdentity identity , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="GetSessions"]/*'/>*/
    Task<DataControlServerSessionInfo[]> GetSessions(GetTransferSessionsRequest? request = null , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="GetStreamTransferSegment"]/*'/>*/
    Task<StreamTransferReadResult> GetStreamTransferSegment(GetStreamTransferSegmentRequest request , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="GetStreamTransferState"]/*'/>*/
    Task<GetStreamTransferStateResponse> GetStreamTransferState(DataItemTransferIdentity identity , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="OpenFollowStreamTransfer"]/*'/>*/
    Task<OpenFollowStreamTransferResponse> OpenFollowStreamTransfer(OpenFollowStreamTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="OpenStreamTransfer"]/*'/>*/
    Task<OpenStreamTransferResponse> OpenStreamTransfer(OpenStreamTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="PutStreamTransferSegment"]/*'/>*/
    Task<PutStreamTransferSegmentResponse> PutStreamTransferSegment(PutStreamTransferSegmentRequest request , Stream payload , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="RemoveTransfer"]/*'/>*/
    Task<RemoveTransferResponse> RemoveTransfer(RemoveTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="ReOpenFollowStreamTransfer"]/*'/>*/
    Task<ReOpenFollowStreamTransferResponse> ReOpenFollowStreamTransfer(ReOpenFollowStreamTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="ReOpenStreamTransfer"]/*'/>*/
    Task<ReOpenStreamTransferResponse> ReOpenStreamTransfer(ReOpenStreamTransferRequest request , CancellationToken cancel = default);

    /**<include file='IDataStreamTransferService.xml' path='IDataStreamTransferService/interface[@name="IDataStreamTransferService"]/method[@name="SessionExists"]/*'/>*/
    Task<Boolean> SessionExists(DataItemTransferIdentity identity , CancellationToken cancel = default);
}