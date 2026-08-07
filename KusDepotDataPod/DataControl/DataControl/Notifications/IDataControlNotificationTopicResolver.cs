namespace DataPodServices.DataControl;

public interface IDataControlNotificationTopicResolver
{
    IReadOnlyList<String> GetTopics(DataControlNotification notification);
}
