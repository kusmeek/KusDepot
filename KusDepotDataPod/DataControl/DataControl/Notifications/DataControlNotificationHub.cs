namespace DataPodServices.DataControl;

internal sealed class DataControlNotificationHub : Hub
{
    internal const String HubPath = "/DataControl/Notifications";

    internal const String NotificationMethod = "NotificationReceived";

    public async Task SubscribeItem(Guid itemid)
    {
        await this.Groups.AddToGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForItem(itemid)).ConfigureAwait(false);
    }

    public async Task SubscribeSession(Guid sessionid)
    {
        await this.Groups.AddToGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForSession(sessionid)).ConfigureAwait(false);
    }

    public async Task SubscribeSourceSession(Guid sourcesessionid)
    {
        await this.Groups.AddToGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForSourceSession(sourcesessionid)).ConfigureAwait(false);
    }

    public async Task UnsubscribeItem(Guid itemid)
    {
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForItem(itemid)).ConfigureAwait(false);
    }

    public async Task UnsubscribeSession(Guid sessionid)
    {
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForSession(sessionid)).ConfigureAwait(false);
    }

    public async Task UnsubscribeSourceSession(Guid sourcesessionid)
    {
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForSourceSession(sourcesessionid)).ConfigureAwait(false);
    }
}
