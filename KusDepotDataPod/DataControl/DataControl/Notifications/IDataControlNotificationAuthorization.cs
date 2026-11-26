namespace DataPodServices.DataControl;

internal interface IDataControlNotificationAuthorization
{
    Task<Boolean> CanSubscribeItemAsync(Guid itemid , String token , CancellationToken cancel = default);

    Task<Boolean> CanSubscribeSessionAsync(Guid sessionid , String token , CancellationToken cancel = default);

    Task<Boolean> CanSubscribeSourceSessionAsync(Guid sourcesessionid , String token , CancellationToken cancel = default);
}