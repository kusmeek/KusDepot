namespace DataPodServices.DataControl;

internal sealed class DataControlNotificationHub : Hub
{
    internal const String HubPath = "/DataControl/Notifications";

    internal const String NotificationMethod = "NotificationReceived";

    private readonly IDataControlNotificationAuthorization authorization;

    public DataControlNotificationHub(IDataControlNotificationAuthorization authorization)
    {
        this.authorization = authorization ?? throw new ArgumentNullException(nameof(authorization));
    }

    public async Task SubscribeItem(Guid itemid)
    {
        if(await this.authorization.CanSubscribeItemAsync(itemid,GetToken(),this.Context.ConnectionAborted) is false) { throw new HubException("Unauthorized"); }

        await this.Groups.AddToGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForItem(itemid));
    }

    public async Task SubscribeSession(Guid sessionid)
    {
        if(await this.authorization.CanSubscribeSessionAsync(sessionid,GetToken(),this.Context.ConnectionAborted) is false) { throw new HubException("Unauthorized"); }

        await this.Groups.AddToGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForSession(sessionid));
    }

    public async Task SubscribeSourceSession(Guid sourcesessionid)
    {
        if(await this.authorization.CanSubscribeSourceSessionAsync(sourcesessionid,GetToken(),this.Context.ConnectionAborted) is false) { throw new HubException("Unauthorized"); }

        await this.Groups.AddToGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForSourceSession(sourcesessionid));
    }

    private String GetToken() => DataControlNotificationToken.Get(this.Context.User,this.Context.GetHttpContext());

    public async Task UnsubscribeItem(Guid itemid)
    {
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForItem(itemid));
    }

    public async Task UnsubscribeSession(Guid sessionid)
    {
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForSession(sessionid));
    }

    public async Task UnsubscribeSourceSession(Guid sourcesessionid)
    {
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId,DataControlNotificationTopics.ForSourceSession(sourcesessionid));
    }
}
