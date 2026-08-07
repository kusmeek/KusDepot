namespace KusDepot.Data.Clients;

/**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/main/*'/>*/
internal sealed class SignalRDataControlNotificationClient : IDataControlNotificationClient , IAsyncDisposable
{
    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/field[@name="endpoint"]/*'/>*/
    private readonly String endpoint;

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/field[@name="connection"]/*'/>*/
    private HubConnection? connection;

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/field[@name="currentToken"]/*'/>*/
    private String? currentToken;

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/field[@name="gate"]/*'/>*/
    private readonly SemaphoreSlim gate = new(1,1);

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/field[@name="certificate"]/*'/>*/
    private readonly X509Certificate2 certificate;

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/field[@name="registrations"]/*'/>*/
    private readonly Dictionary<String,RegistrationState> registrations = new(StringComparer.Ordinal);

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/field[@name="ConnectionClosedWhileWaiting"]/*'/>*/
    private static readonly OperationFailedException ConnectionClosedWhileWaiting = new("The DataControl notification connection closed while waiting for follow data.");

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/constructor[@name="Constructor"]/*'/>*/
    internal SignalRDataControlNotificationClient(String endpoint , X509Certificate2 certificate)
    {
        this.endpoint = endpoint.TrimEnd('/'); this.certificate = certificate;
    }

    ///<inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.gate.WaitAsync().ConfigureAwait(false);

        try
        {
            this.FailActiveWaiters(ConnectionClosedWhileWaiting);

            if(this.connection is not null)
            {
                await this.connection.DisposeAsync().ConfigureAwait(false);

                this.connection = null;
            }
        }
        finally
        {
            this.registrations.Clear();
            this.currentToken = null;
            this.gate.Release();
            this.gate.Dispose();
        }
    }

    ///<inheritdoc/>
    public async Task<IDataControlNotificationSubscriptionHandle> RegisterSubscriptionAsync(DataControlNotificationSubscription subscription , String? token = null , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(subscription); ValidateSubscription(subscription);

        await this.gate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            this.SetToken(token);

            await this.EnsureConnectedAsync(cancel).ConfigureAwait(false);

            String key = CreateKey(subscription);

            if(this.registrations.TryGetValue(key,out RegistrationState? registration) is false)
            {
                registration = new(subscription);

                this.registrations.Add(key,registration);

                await this.SubscribeAsync(subscription,cancel).ConfigureAwait(false);
            }

            registration.ReferenceCount++;

            return new PersistentSubscriptionHandle(this,key,registration.SignalVersion);
        }
        catch ( Exception _ )
        {
            KusDepotLog.Error(_,RegisterSubscriptionFail);

            throw;
        }
        finally
        {
            this.gate.Release();
        }
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="WaitForRegistrationWakeAsync"]/*'/>*/
    private async Task<Boolean> WaitForRegistrationWakeAsync(PersistentSubscriptionHandle handle , TimeSpan wait , CancellationToken cancel)
    {
        if(wait == TimeSpan.Zero || wait < TimeSpan.Zero && wait != Timeout.InfiniteTimeSpan)
        {
            throw new ArgumentOutOfRangeException(nameof(wait),wait,"Wait must be a positive duration or Timeout.InfiniteTimeSpan.");
        }

        ArgumentNullException.ThrowIfNull(handle); ObjectDisposedException.ThrowIf(handle.IsDisposed,handle);

        Task<Boolean> pending;

        await this.gate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            Boolean found = this.registrations.TryGetValue(handle.Key,out RegistrationState? registration);

            ObjectDisposedException.ThrowIf(!found,typeof(IDataControlNotificationSubscriptionHandle));

            if(registration!.SignalVersion > handle.ObservedSignalVersion)
            {
                handle.ObservedSignalVersion = registration.SignalVersion;

                return true;
            }

            pending = registration.Waiter.Task;
        }
        finally
        {
            this.gate.Release();
        }

        try
        {
            Boolean woke = wait == Timeout.InfiniteTimeSpan
                ? await pending.WaitAsync(cancel).ConfigureAwait(false)
                : await pending.WaitAsync(wait,cancel).ConfigureAwait(false);

            return woke;
        }
        catch ( TimeoutException )
        {
            return false;
        }
        finally
        {
            await this.gate.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                if(this.registrations.TryGetValue(handle.Key,out RegistrationState? registration))
                {
                    handle.ObservedSignalVersion = Math.Max(handle.ObservedSignalVersion,registration.SignalVersion);
                }
            }
            finally
            {
                this.gate.Release();
            }
        }
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="UnregisterAsync"]/*'/>*/
    private async Task UnregisterAsync(String key , CancellationToken cancel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        DataControlNotificationSubscription? subscription = null;

        await this.gate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            if(this.registrations.TryGetValue(key,out RegistrationState? registration) is false) { return; }

            registration.ReferenceCount--;

            if(registration.ReferenceCount > 0) { return; }

            registration.Waiter.TrySetException(ConnectionClosedWhileWaiting);

            subscription = registration.Subscription;

            this.registrations.Remove(key);

            if(subscription is not null)
            {
                await this.UnsubscribeAsync(subscription,cancel).ConfigureAwait(false);
            }
        }
        finally
        {
            this.gate.Release();
        }
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="OnNotificationReceived"]/*'/>*/
    private async Task OnNotificationReceived(DataControlNotification notification)
    {
        await this.gate.WaitAsync().ConfigureAwait(false);

        try
        {
            foreach(RegistrationState registration in this.registrations.Values)
            {
                if(Matches(registration.Subscription,notification) is false) { continue; }

                registration.SignalVersion++;
                registration.LastNotification = notification;
                registration.Waiter.TrySetResult(true);
                registration.Waiter = CreateWaiter();
            }
        }
        finally
        {
            this.gate.Release();
        }
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="FailActiveWaiters"]/*'/>*/
    private void FailActiveWaiters(Exception error)
    {
        foreach(RegistrationState registration in this.registrations.Values)
        {
            registration.Waiter.TrySetException(error);
            registration.Waiter = CreateWaiter();
        }
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="Matches"]/*'/>*/
    private static Boolean Matches(DataControlNotificationSubscription subscription , DataControlNotification notification)
    {
        if(subscription.TransferFamily.HasValue && notification.TransferFamily.HasValue && subscription.TransferFamily.Value != notification.TransferFamily.Value)
        {
            return false;
        }

        if(subscription.ItemID.HasValue && notification.ItemID.HasValue && subscription.ItemID.Value != notification.ItemID.Value)
        {
            return false;
        }

        if(subscription.SessionID.HasValue && notification.SessionID.HasValue && subscription.SessionID.Value != notification.SessionID.Value)
        {
            return false;
        }

        if(subscription.SourceSessionID.HasValue && notification.SourceSessionID.HasValue && subscription.SourceSessionID.Value != notification.SourceSessionID.Value)
        {
            return false;
        }

        return true;
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="EnsureConnectedAsync"]/*'/>*/
    private async Task EnsureConnectedAsync(CancellationToken cancel)
    {
        if(this.connection is null)
        {
            this.connection = this.CreateConnection();

            this.connection.On<DataControlNotification>("NotificationReceived",notification => this.OnNotificationReceived(notification));

            this.connection.Closed += error =>
            {
                this.FailActiveWaiters(error ?? ConnectionClosedWhileWaiting);

                return Task.CompletedTask;
            };
        }

        if(this.connection.State is HubConnectionState.Disconnected)
        {
            await this.connection.StartAsync(cancel).ConfigureAwait(false);
        }
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="CreateConnection"]/*'/>*/
    private HubConnection CreateConnection()
    {
        Uri hubUri = new(new Uri(this.endpoint,UriKind.Absolute),"/DataControl/Notifications");

        return new HubConnectionBuilder()
            .WithUrl(hubUri,_ =>
            {
                _.ClientCertificates ??= new X509CertificateCollection();
                _.ClientCertificates.Add(this.certificate);
                _.AccessTokenProvider = () => Task.FromResult(this.currentToken);
            })
            .WithAutomaticReconnect()
            .WithStatefulReconnect()
            .Build();
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="SubscribeAsync"]/*'/>*/
    private async Task SubscribeAsync(DataControlNotificationSubscription subscription , CancellationToken cancel)
    {
        if(this.connection is null) { return; }

        if(subscription.ItemID is Guid itemId && itemId != Guid.Empty)
        {
            await this.connection.InvokeAsync("SubscribeItem",itemId,cancel).ConfigureAwait(false);
        }

        if(subscription.SessionID is Guid sessionId && sessionId != Guid.Empty)
        {
            await this.connection.InvokeAsync("SubscribeSession",sessionId,cancel).ConfigureAwait(false);
        }

        if(subscription.SourceSessionID is Guid sourceSessionId && sourceSessionId != Guid.Empty)
        {
            await this.connection.InvokeAsync("SubscribeSourceSession",sourceSessionId,cancel).ConfigureAwait(false);
        }
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="UnsubscribeAsync"]/*'/>*/
    private async Task UnsubscribeAsync(DataControlNotificationSubscription subscription , CancellationToken cancel)
    {
        if(this.connection is null || this.connection.State is not HubConnectionState.Connected) { return; }

        if(subscription.ItemID is Guid itemId && itemId != Guid.Empty)
        {
            await this.connection.InvokeAsync("UnsubscribeItem",itemId,cancel).ConfigureAwait(false);
        }

        if(subscription.SessionID is Guid sessionId && sessionId != Guid.Empty)
        {
            await this.connection.InvokeAsync("UnsubscribeSession",sessionId,cancel).ConfigureAwait(false);
        }

        if(subscription.SourceSessionID is Guid sourceSessionId && sourceSessionId != Guid.Empty)
        {
            await this.connection.InvokeAsync("UnsubscribeSourceSession",sourceSessionId,cancel).ConfigureAwait(false);
        }
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="ValidateSubscription"]/*'/>*/
    private static void ValidateSubscription(DataControlNotificationSubscription subscription)
    {
        if(subscription.ItemID.HasValue is false && subscription.SessionID.HasValue is false && subscription.SourceSessionID.HasValue is false)
        {
            throw new ArgumentException("At least one notification subscription identifier must be supplied.",nameof(subscription));
        }
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="CreateKey"]/*'/>*/
    private static String CreateKey(DataControlNotificationSubscription subscription)
    {
        return $"{subscription.TransferFamily?.ToString() ?? "null"}|{subscription.ItemID?.ToString("D") ?? "null"}|{subscription.SessionID?.ToString("D") ?? "null"}|{subscription.SourceSessionID?.ToString("D") ?? "null"}";
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="CreateWaiter"]/*'/>*/
    private static TaskCompletionSource<Boolean> CreateWaiter() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="SignalRDataControlNotificationClient"]/method[@name="SetToken"]/*'/>*/
    internal void SetToken(String? token)
    {
        this.currentToken = token;
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="RegistrationState"]/main/*'/>*/
    private sealed class RegistrationState
    {
        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="RegistrationState"]/constructor[@name="Constructor"]/*'/>*/
        public RegistrationState(DataControlNotificationSubscription subscription)
        {
            this.Subscription = subscription;
        }

        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="RegistrationState"]/property[@name="LastNotification"]/*'/>*/
        public DataControlNotification? LastNotification { get; set; }

        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="RegistrationState"]/property[@name="ReferenceCount"]/*'/>*/
        public Int32 ReferenceCount { get; set; }

        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="RegistrationState"]/property[@name="SignalVersion"]/*'/>*/
        public Int64 SignalVersion { get; set; }

        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="RegistrationState"]/property[@name="Subscription"]/*'/>*/
        public DataControlNotificationSubscription Subscription { get; }

        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="RegistrationState"]/property[@name="Waiter"]/*'/>*/
        public TaskCompletionSource<Boolean> Waiter { get; set; } = CreateWaiter();
    }

    /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="PersistentSubscriptionHandle"]/main/*'/>*/
    private sealed class PersistentSubscriptionHandle : IDataControlNotificationSubscriptionHandle
    {
        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="PersistentSubscriptionHandle"]/field[@name="owner"]/*'/>*/
        private readonly SignalRDataControlNotificationClient owner;

        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="PersistentSubscriptionHandle"]/field[@name="disposed"]/*'/>*/
        private Boolean disposed;

        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="PersistentSubscriptionHandle"]/constructor[@name="Constructor"]/*'/>*/
        public PersistentSubscriptionHandle(SignalRDataControlNotificationClient owner , String key , Int64 observedSignalVersion)
        {
            this.owner = owner;
            this.Key = key;
            this.ObservedSignalVersion = observedSignalVersion;
        }

        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="PersistentSubscriptionHandle"]/property[@name="IsDisposed"]/*'/>*/
        public Boolean IsDisposed => this.disposed;

        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="PersistentSubscriptionHandle"]/property[@name="Key"]/*'/>*/
        public String Key { get; }

        /**<include file='SignalRDataControlNotificationClient.xml' path='SignalRDataControlNotificationClient/class[@name="PersistentSubscriptionHandle"]/property[@name="ObservedSignalVersion"]/*'/>*/
        public Int64 ObservedSignalVersion { get; set; }

        ///<inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            if(this.disposed) { return; }

            this.disposed = true;

            await this.owner.UnregisterAsync(this.Key,CancellationToken.None).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public Task<Boolean> WaitForWakeAsync(TimeSpan wait = default , CancellationToken cancel = default)
        {
            ObjectDisposedException.ThrowIf(this.disposed,this);

            return this.owner.WaitForRegistrationWakeAsync(this,wait,cancel);
        }
    }
}