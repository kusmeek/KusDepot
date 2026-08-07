namespace KusDepot.Data.Clients;

public sealed partial class DataControlClient
{
    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="RegisterFollowSubscriptionAsync"]/*'/>*/
    internal async Task<IDataControlNotificationSubscriptionHandle?> RegisterFollowSubscriptionAsync(DataControlTransferSessionInfo session , String? token = null , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(session);

        DataControlNotificationSubscription? subscription = DataControlDownloadNotificationStrategy.Create(session);

        if(subscription is not null)
        {
            return await this.NotificationClient.RegisterSubscriptionAsync(subscription,this.ResolveToken(token),cancel).ConfigureAwait(false);
        }

        return null;
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="CreateDownloadSessionAsync"]/*'/>*/
    internal async Task<DataControlDownloadSession> CreateDownloadSessionAsync(DataControlClientWorkingDirectoryStorage storage , String? token , DataControlTransferSessionInfo session , StreamCompletionMode completionmode = StreamCompletionMode.UntilSourceCompletes , Int64? maxbytes = null , BufferedFollowStream? followstream = null , DataStreamItem? liveitem = null , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(storage); ArgumentNullException.ThrowIfNull(session);

        IDataControlNotificationSubscriptionHandle? followsubscription = await this.RegisterFollowSubscriptionAsync(session,token,cancel).ConfigureAwait(false);

        return new DataControlDownloadSession(this,storage,token,session,completionmode,maxbytes,followstream,liveitem,followsubscription);
    }
}
