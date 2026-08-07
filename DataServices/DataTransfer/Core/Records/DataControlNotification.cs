namespace KusDepot.Data.Transfer;

/**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record DataControlNotification
{
    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="AppendedLength"]/*'/>*/
    [JsonPropertyName("AppendedLength")] [JsonInclude]
    public Int64? AppendedLength { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="EventType"]/*'/>*/
    [JsonPropertyName("EventType")] [JsonInclude]
    public DataControlNotificationEventType EventType { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="ItemID"]/*'/>*/
    [JsonPropertyName("ItemID")] [JsonInclude]
    public Guid? ItemID { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="LatestRange"]/*'/>*/
    [JsonPropertyName("LatestRange")] [JsonInclude]
    public DataItemTransferRange? LatestRange { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="Mode"]/*'/>*/
    [JsonPropertyName("Mode")] [JsonInclude]
    public DataItemTransferMode? Mode { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="RealizedBytes"]/*'/>*/
    [JsonPropertyName("RealizedBytes")] [JsonInclude]
    public Int64? RealizedBytes { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="Sequence"]/*'/>*/
    [JsonPropertyName("Sequence")] [JsonInclude]
    public Int64 Sequence { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonInclude]
    public Guid? SessionID { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="SourceSessionID"]/*'/>*/
    [JsonPropertyName("SourceSessionID")] [JsonInclude]
    public Guid? SourceSessionID { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="Status"]/*'/>*/
    [JsonPropertyName("Status")] [JsonInclude]
    public DataItemTransferStatus? Status { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="StateVersion"]/*'/>*/
    [JsonPropertyName("StateVersion")] [JsonInclude]
    public Int64? StateVersion { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="Timestamp"]/*'/>*/
    [JsonPropertyName("Timestamp")] [JsonInclude]
    public DateTimeOffset Timestamp { get; init; }

    /**<include file='DataControlNotification.xml' path='DataControlNotification/record[@name="DataControlNotification"]/property[@name="TransferFamily"]/*'/>*/
    [JsonPropertyName("TransferFamily")] [JsonInclude]
    public DataControlTransferFamily? TransferFamily { get; init; }
}
