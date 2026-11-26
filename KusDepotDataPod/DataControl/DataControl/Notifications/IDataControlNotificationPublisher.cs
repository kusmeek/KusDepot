namespace DataPodServices.DataControl;

public interface IDataControlNotificationPublisher
{
    Task PublishAsync(DataControlNotification notification , CancellationToken cancel = default);
}
