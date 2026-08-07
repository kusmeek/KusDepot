namespace KusDepot.Data.Clients;

/**<include file='IDataControlNotificationClient.xml' path='IDataControlNotificationClient/interface[@name="IDataControlNotificationClient"]/main/*'/>*/
internal interface IDataControlNotificationClient
{
    /**<include file='IDataControlNotificationClient.xml' path='IDataControlNotificationClient/interface[@name="IDataControlNotificationClient"]/method[@name="RegisterSubscriptionAsync"]/*'/>*/
    Task<IDataControlNotificationSubscriptionHandle> RegisterSubscriptionAsync(DataControlNotificationSubscription subscription , String? token = null , CancellationToken cancel = default);
}