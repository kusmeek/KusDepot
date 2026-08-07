namespace DataPodServices.DataControl;

internal sealed class SignalRDataControlNotificationPublisher : IDataControlNotificationPublisher
{
    private readonly IHubContext<DataControlNotificationHub> hubContext;

    private readonly IDataControlNotificationTopicResolver topicResolver;

    public SignalRDataControlNotificationPublisher(IHubContext<DataControlNotificationHub> hubContext , IDataControlNotificationTopicResolver topicResolver)
    {
        this.hubContext = hubContext;

        this.topicResolver = topicResolver;
    }

    public async Task PublishAsync(DataControlNotification notification , CancellationToken cancel = default)
    {
        IReadOnlyList<String> topics = this.topicResolver.GetTopics(notification);

        if(topics.Count == 0) { return; }

        foreach(String topic in topics)
        {
            await this.hubContext.Clients.Group(topic)
                .SendAsync(DataControlNotificationHub.NotificationMethod,notification,cancel)
                .ConfigureAwait(false);
        }
    }
}
