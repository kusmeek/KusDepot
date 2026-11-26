namespace KusDepot.Data.Clients;

/**<include file='IDataControlNotificationSubscriptionHandle.xml' path='IDataControlNotificationSubscriptionHandle/interface[@name="IDataControlNotificationSubscriptionHandle"]/main/*'/>*/
internal interface IDataControlNotificationSubscriptionHandle : IAsyncDisposable
{
    /**<include file='IDataControlNotificationSubscriptionHandle.xml' path='IDataControlNotificationSubscriptionHandle/interface[@name="IDataControlNotificationSubscriptionHandle"]/method[@name="WaitForWakeAsync"]/*'/>*/
    Task<Boolean> WaitForWakeAsync(TimeSpan wait = default , CancellationToken cancel = default);
}
