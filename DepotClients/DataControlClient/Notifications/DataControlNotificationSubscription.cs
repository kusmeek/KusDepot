namespace KusDepot.Data.Clients;

/**<include file='DataControlNotificationSubscription.xml' path='DataControlNotificationSubscription/record[@name="DataControlNotificationSubscription"]/main/*'/>*/
internal sealed record DataControlNotificationSubscription
{
    /**<include file='DataControlNotificationSubscription.xml' path='DataControlNotificationSubscription/record[@name="DataControlNotificationSubscription"]/property[@name="ItemID"]/*'/>*/
    public Guid? ItemID { get; init; }

    /**<include file='DataControlNotificationSubscription.xml' path='DataControlNotificationSubscription/record[@name="DataControlNotificationSubscription"]/property[@name="SessionID"]/*'/>*/
    public Guid? SessionID { get; init; }

    /**<include file='DataControlNotificationSubscription.xml' path='DataControlNotificationSubscription/record[@name="DataControlNotificationSubscription"]/property[@name="SourceSessionID"]/*'/>*/
    public Guid? SourceSessionID { get; init; }

    /**<include file='DataControlNotificationSubscription.xml' path='DataControlNotificationSubscription/record[@name="DataControlNotificationSubscription"]/property[@name="TransferFamily"]/*'/>*/
    public DataControlTransferFamily? TransferFamily { get; init; }
}
