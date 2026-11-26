namespace DataPodServices.DataControl;

internal sealed class DataControlNotificationTopicResolver : IDataControlNotificationTopicResolver
{
    public IReadOnlyList<String> GetTopics(DataControlNotification notification)
    {
        HashSet<String> topics = new(StringComparer.OrdinalIgnoreCase);

        if(notification.ItemID is Guid itemId)
        {
            topics.Add(DataControlNotificationTopics.ForItem(itemId));
        }

        if(notification.SessionID is Guid sessionId)
        {
            topics.Add(DataControlNotificationTopics.ForSession(sessionId));
        }

        if(notification.SourceSessionID is Guid sourceSessionId)
        {
            topics.Add(DataControlNotificationTopics.ForSourceSession(sourceSessionId));
        }

        return topics.Count == 0 ? Array.Empty<String>() : topics.OrderBy(_ => _,StringComparer.OrdinalIgnoreCase).ToArray();
    }
}
