namespace KusDepot;

/**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/main/*'/>*/
public class ToolHost : Tool , IToolHost
{
    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/property[@name="LifeID"]/*'/>*/
    protected Guid LifeID { get; set; }

    ///<inheritdoc/>
    public IToolHostLifetime? Lifetime {get; protected set;}

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/property[@name="LifeSync"]/*'/>*/
    protected SemaphoreSlim LifeSync { get; set; } = new(1,1);

    ///<inheritdoc/>
    [NotNull]
    public IServiceProvider? Services { get { return this.GetLocked() is true ? null! : field!; } protected set => field = value; }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/constructor[@name="Constructor"]/*'/>*/
    public ToolHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null,
           IConfiguration? configuration = null , Guid? lifeid = null , IToolHostLifetime? lifetime = null , ILoggerFactory? logger = null) : base(accessmanager,data,id,services,commands,configuration,logger)
    {
        try
        {
            this.LifeID = lifeid ?? Guid.NewGuid();

            this.Lifetime = lifetime ?? new ToolHostLifetime(LifeID);

            this.Services = this.ToolServiceScope?.ServiceProvider;
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolHost() : this(null,null,null,null,null,null,null,null,null){}

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetServices)]
    public virtual IServiceProvider? GetServices(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.Services; }

        catch ( Exception _ ) { Logger?.Error(_,GetServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override async Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartingOK() is false) || (this.AccessCheck(key) is false)) { return; }

            Logger?.Verbose(ToolHostStarting,MyTypeName,MyID); await base.StartingAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStarting(LifeID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            Logger?.Verbose(ToolHostStartEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StartAsync(cancel,key).ConfigureAwait(false); Logger?.Verbose(ToolHostStartExit,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartedOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StartedAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStarted(LifeID); Logger?.Verbose(ToolHostStarted,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StoppingOK() is false) || (this.AccessCheck(key) is false)) { return; }

            Logger?.Verbose(ToolHostStopping,MyTypeName,MyID); await base.StoppingAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStopping(LifeID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            Logger?.Verbose(ToolHostStopEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StopOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StopAsync(cancel,key).ConfigureAwait(false); Logger?.Verbose(ToolHostStopExit,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StoppedOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StoppedAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStopped(LifeID); Logger?.Verbose(ToolHostStopped,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override void Dispose(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); base.Dispose_NoSync(key);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    protected override void Dispose_NoSync(AccessKey? key = null)
    {
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); base.Dispose_NoSync(key);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public override void Dispose()
    {
        Boolean ds = false;
        try
        {
            if(this.Disposed || this.Locked) { return; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); base.Dispose_NoSync();

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    protected override void Dispose_NoSync()
    {
        try
        {
            if(this.Disposed || this.Locked) { return; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); base.Dispose_NoSync();

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync(key).ConfigureAwait(false);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    protected override async ValueTask DisposeAsync_NoSync(AccessKey? key = null)
    {
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync(key).ConfigureAwait(false);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        Boolean ds = false;
        try
        {
            if(this.Disposed || this.Locked) { return; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync().ConfigureAwait(false);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    protected override async ValueTask DisposeAsync_NoSync()
    {
        try
        {
            if(this.Disposed || this.Locked) { return; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync().ConfigureAwait(false);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public virtual async Task RunAsync(CancellationToken cancel = default)
    {
        await this.StartHostAsync(cancel,key:SelfAccessKey).ConfigureAwait(false);

        try { await Task.Delay(Timeout.Infinite,cancel).ConfigureAwait(false); } catch {}

        await this.StopHostAsync(timeout:DefaultHostStopTimeout,key:SelfAccessKey).ConfigureAwait(false);
    }

    ///<inheritdoc/>
    public virtual void Run() { this.RunAsync().GetAwaiter().GetResult(); }
}