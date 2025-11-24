namespace KusDepot;

/**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/main/*'/>*/
public class ToolGenericHost : ToolHost , IToolGenericHost
{
    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/property[@name="Host"]/*'/>*/
    protected IHost? Host { get; set; }
    
    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/property[@name="HostKey"]/*'/>*/
    protected HostKey? HostKey { get; set; }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/property[@name="HostManagerKey"]/*'/>*/
    protected ManagerKey? HostManagerKey { get; set; }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/property[@name="LifeSync"]/*'/>*/
    protected new SemaphoreSlim LifeSync { get; set; } = new(1,1);

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/constructor[@name="Constructor"]/*'/>*/
    public ToolGenericHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null,
           IHost? host = null , Guid? lifeid = null , IToolHostLifetime? lifetime = null , ILoggerFactory? logger = null ) : base(accessmanager,data,id,services,commands,configuration,lifeid,lifetime,logger)
    {
        try
        {
            this.Host = host; this.Services = this.Host?.Services ?? this.ToolServiceScope?.ServiceProvider;

            this.ResolveLogger(logger); if(this.RequestHostKey() is true) { this.LockHost(); }
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolGenericHost() : this(null,null,null,null,null,null,null,null,null,null){}

    ///<inheritdoc/>
    public virtual IHost? GetManagedApplication(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.Host; }

        catch ( Exception _ ) { Logger?.Error(_,GetManagedApplicationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/method[@name="LockHost"]/*'/>*/
    protected virtual Boolean LockHost()
    {
        DC();
        try
        {
            if( Host is null || Host as IToolHost is not IToolHost t ) { return true; }

            HostManagerKey = t.CreateManagementKey("HostManagerKey") as ManagerKey; if(HostManagerKey is null) { return false; }

            return t.Lock(HostManagerKey) ? true : throw new SecurityException(LockHostFail);
        }
        catch ( Exception _ ) { Logger?.Error(_,LockHostFail,MyTypeName,MyID); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/method[@name="RequestHostKey"]/*'/>*/
    protected virtual Boolean RequestHostKey()
    {
        DC();
        try
        {
            if( Host is null || Host as IToolHost is not IToolHost t ) { return true; }

            HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey;

            return HostKey is not null ? true : throw new SecurityException(RequestHostKeyFail);
        }
        catch ( Exception _ ) { Logger?.Error(_,RequestHostKeyFail,MyTypeName,MyID); if(NoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    protected override Boolean ResolveLogger(ILoggerFactory? logger = null)
    {
        DC();
        try
        {
            if(this.Logger is not null) { return false; }

            if(!TryEnter(this.Sync.Logger,SyncTime)) { throw SyncException; }

            this.Logger = (logger ?? this.Host?.Services.GetService<ILoggerFactory>() ?? this.ToolServices?.GetService<ILoggerFactory>())?
                .CreateLogger(String.Format(ToolLoggerNameFormat,this.GetType().FullName,this.GetID()));

            if(this.Logger is not null) { return true; } return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,ResolveLoggerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Logger)) { Exit(this.Sync.Logger); } }
    }

    ///<inheritdoc/>
    public override async Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync!.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartingOK() is false) || (this.AccessCheck(key) is false)) { return; }

            Logger?.Verbose(ToolGenericHostStarting,MyTypeName,MyID); await base.StartingAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStarting(LifeID); if(Host is IToolHost t) { await t.StartingAsync(cancel,HostKey).ConfigureAwait(false); }
        }
        catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync!.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            Logger?.Verbose(ToolGenericHostStartEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; try { await LifeSync!.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StartAsync(cancel,key).ConfigureAwait(false);

            if(Host is IToolHost t) { await t.StartAsync(cancel,HostKey).ConfigureAwait(false); } else { await (Host?.StartAsync(cancel.Value) ?? Task.CompletedTask).ConfigureAwait(false); }

            Logger?.Verbose(ToolGenericHostStartExit,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync!.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync!.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartedOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StartedAsync(cancel,key).ConfigureAwait(false); if(Host is IToolHost t) { await t.StartedAsync(cancel,HostKey).ConfigureAwait(false); } Lifetime!.NotifyStarted(LifeID); Logger?.Information(ToolGenericHostStarted,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync!.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync!.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StoppingOK() is false) || (this.AccessCheck(key) is false)) { return; }

            Logger?.Verbose(ToolGenericHostStopping,MyTypeName,MyID); await base.StoppingAsync(cancel,key).ConfigureAwait(false); if(Host is IToolHost t) { await t.StoppingAsync(cancel,HostKey).ConfigureAwait(false); } Lifetime!.NotifyStopping(LifeID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync!.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            Logger?.Verbose(ToolGenericHostStopEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; try { await LifeSync!.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StopOK() is false) || (this.AccessCheck(key) is false)) { return; }

            if(Host is IToolHost t) { await t.StopAsync(cancel,HostKey).ConfigureAwait(false); } else { await (Host?.StopAsync(cancel.Value) ?? Task.CompletedTask).ConfigureAwait(false); }

            await base.StopAsync(cancel,key).ConfigureAwait(false); Logger?.Verbose(ToolGenericHostStopExit,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync!.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync!.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StoppedOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StoppedAsync(cancel,key).ConfigureAwait(false); if(Host is IToolHost t) { await t.StoppedAsync(cancel,HostKey).ConfigureAwait(false); } Lifetime!.NotifyStopped(LifeID); Logger?.Information(ToolGenericHostStopped,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync!.Release(); } }
    }

    ///<inheritdoc/>
    public override void Dispose(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolGenericHostDisposing,MyTypeName,MyID);

            if(Host is IToolHost t) { t.Dispose(HostKey); } else { this.Host?.Dispose(); }

            Lifetime!.Dispose(); base.Dispose_NoSync(key);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    public override void Dispose()
    {
        Boolean ds = false;
        try
        {
            if(this.Disposed || this.Locked) { return; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolGenericHostDisposing,MyTypeName,MyID);

            if(Host is IToolHost t) { t.Dispose(HostKey); } else { this.Host?.Dispose(); }

            Lifetime!.Dispose(); base.Dispose_NoSync();
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolGenericHostDisposing,MyTypeName,MyID);

            if(Host is IAsyncDisposable d)
            {
                if(Host is IToolHost t) { await t.DisposeAsync(HostKey).ConfigureAwait(false); }

                else { await d.DisposeAsync().ConfigureAwait(false); }
            }
            else { Host?.Dispose(); }

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync(key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        Boolean ds = false;
        try
        {
            if(this.Disposed || this.Locked) { return; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolGenericHostDisposing,MyTypeName,MyID);

            if(Host is IAsyncDisposable d)
            {
                if(Host is IToolHost t) { await t.DisposeAsync(HostKey).ConfigureAwait(false); }

                else { await d.DisposeAsync().ConfigureAwait(false); }
            }
            else { Host?.Dispose(); }

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync().ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }
}