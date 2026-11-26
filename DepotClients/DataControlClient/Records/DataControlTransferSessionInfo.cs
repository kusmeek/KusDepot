namespace KusDepot.Data.Clients;

/**<include file='DataControlTransferSessionInfo.xml' path='DataControlTransferSessionInfo/record[@name="DataControlTransferSessionInfo"]/main/*'/>*/
public sealed record DataControlTransferSessionInfo
{
    /**<include file='DataControlTransferSessionInfo.xml' path='DataControlTransferSessionInfo/record[@name="DataControlTransferSessionInfo"]/property[@name="ItemId"]/*'/>*/
    public Guid ItemId { get; init; }

    /**<include file='DataControlTransferSessionInfo.xml' path='DataControlTransferSessionInfo/record[@name="DataControlTransferSessionInfo"]/property[@name="SessionId"]/*'/>*/
    public Guid SessionId { get; init; }

    /**<include file='DataControlTransferSessionInfo.xml' path='DataControlTransferSessionInfo/record[@name="DataControlTransferSessionInfo"]/property[@name="SegmentedState"]/*'/>*/
    public DataItemTransferState? SegmentedState { get; init; }

    /**<include file='DataControlTransferSessionInfo.xml' path='DataControlTransferSessionInfo/record[@name="DataControlTransferSessionInfo"]/property[@name="State"]/*'/>*/
    public DataItemTransferState State { get; init; } = new();

    /**<include file='DataControlTransferSessionInfo.xml' path='DataControlTransferSessionInfo/record[@name="DataControlTransferSessionInfo"]/property[@name="StreamState"]/*'/>*/
    public DataStreamTransferState? StreamState { get; init; }

    /**<include file='DataControlTransferSessionInfo.xml' path='DataControlTransferSessionInfo/record[@name="DataControlTransferSessionInfo"]/property[@name="SupportsLiveFollowBinding"]/*'/>*/
    public Boolean SupportsLiveFollowBinding { get; init; }

    /**<include file='DataControlTransferSessionInfo.xml' path='DataControlTransferSessionInfo/record[@name="DataControlTransferSessionInfo"]/property[@name="TransferFamily"]/*'/>*/
    public DataControlTransferFamily TransferFamily { get; init; } = DataControlTransferFamily.Segmented;

    /**<include file='DataControlTransferSessionInfo.xml' path='DataControlTransferSessionInfo/record[@name="DataControlTransferSessionInfo"]/property[@name="UploadSource"]/*'/>*/
    public DataControlTransferSourceDescriptor? UploadSource { get; init; }

    /**<include file='DataControlTransferSessionInfo.xml' path='DataControlTransferSessionInfo/record[@name="DataControlTransferSessionInfo"]/property[@name="WorkingDirectory"]/*'/>*/
    public String WorkingDirectory { get; init; } = String.Empty;
}
