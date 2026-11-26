namespace KusDepot.Data.Transfer;

/**<include file='DataControlServerSessionInfo.xml' path='DataControlServerSessionInfo/record[@name="DataControlServerSessionInfo"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record DataControlServerSessionInfo
{
    /**<include file='DataControlServerSessionInfo.xml' path='DataControlServerSessionInfo/record[@name="DataControlServerSessionInfo"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid ItemID { get; init; }

    /**<include file='DataControlServerSessionInfo.xml' path='DataControlServerSessionInfo/record[@name="DataControlServerSessionInfo"]/property[@name="SegmentedState"]/*'/>*/
    [JsonPropertyName("SegmentedState")] [JsonInclude]
    public DataItemTransferState? SegmentedState { get; init; }

    /**<include file='DataControlServerSessionInfo.xml' path='DataControlServerSessionInfo/record[@name="DataControlServerSessionInfo"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid SessionID { get; init; }

    /**<include file='DataControlServerSessionInfo.xml' path='DataControlServerSessionInfo/record[@name="DataControlServerSessionInfo"]/property[@name="StreamState"]/*'/>*/
    [JsonPropertyName("StreamState")] [JsonInclude]
    public DataStreamTransferState? StreamState { get; init; }

    /**<include file='DataControlServerSessionInfo.xml' path='DataControlServerSessionInfo/record[@name="DataControlServerSessionInfo"]/property[@name="TransferFamily"]/*'/>*/
    [JsonPropertyName("TransferFamily")] [JsonInclude]
    public DataControlTransferFamily TransferFamily { get; init; } = DataControlTransferFamily.Segmented;
}