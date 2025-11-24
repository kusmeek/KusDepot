namespace KusDepot;

/**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/main/*'/>*/
public class ToolWebHost : ToolHost , IToolWebHost
{
    /**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/property[@name="WebApplication"]/*'/>*/
    protected IHost? WebApplication { get; set; }

    ///<inheritdoc/>
    public ICollection<String>? Urls { get; protected set; }

    /**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/property[@name="LifeSync"]/*'/>*/
    protected new SemaphoreSlim LifeSync { get; set; } = new(1,1);

    /**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/constructor[@name="Constructor"]/*'/>*/
    public ToolWebHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null,
           Guid? lifeid = null , IToolHostLifetime? lifetime = null , ILoggerFactory? logger = null , ICollection<String>? urls = null , IHost? webapplication = null) : base(accessmanager,data,id,services,commands,configuration,lifeid,lifetime,logger)
    {
        try
        {
            this.Urls = urls?.ToArray(); this.WebApplication = webapplication;

            this.Services = this.WebApplication?.Services ?? this.ToolServiceScope?.ServiceProvider; this.ResolveLogger(logger);
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolWebHost() : this(null,null,null,null,null,null,null,null,null,null,null){}

    ///<inheritdoc/>
    public IHost? GetManagedApplication(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.WebApplication; }

        catch ( Exception _ ) { Logger?.Error(_,GetManagedApplicationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    protected override Boolean ResolveLogger(ILoggerFactory? logger = null)
    {
        DC();
        try
        {
            if(this.Logger is not null) { return false; }

            if(!TryEnter(this.Sync.Logger,SyncTime)) { throw SyncException; }

            this.Logger = (logger ?? this.WebApplication?.Services.GetService<ILoggerFactory>() ?? this.ToolServices?.GetService<ILoggerFactory>())?
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
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartingOK() is false) || (this.AccessCheck(key) is false)) { return; }

            Logger?.Verbose(ToolWebHostStarting,MyTypeName,MyID); await base.StartingAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStarting(LifeID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            Logger?.Verbose(ToolWebHostStartEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StartAsync(cancel,key).ConfigureAwait(false); await (WebApplication?.StartAsync(cancel.Value) ?? Task.CompletedTask).ConfigureAwait(false); Logger?.Verbose(ToolWebHostStartExit,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartedOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StartedAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStarted(LifeID); Logger?.Information(ToolWebHostStarted,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StoppingOK() is false) || (this.AccessCheck(key) is false)) { return; }

            Logger?.Verbose(ToolWebHostStopping,MyTypeName,MyID); await base.StoppingAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStopping(LifeID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            Logger?.Verbose(ToolWebHostStopEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StopOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StopAsync(cancel,key).ConfigureAwait(false); Logger?.Verbose(ToolWebHostStopExit,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override async Task StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StoppedOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StoppedAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStopped(LifeID); Logger?.Information(ToolWebHostStopped,MyTypeName,MyID); await (WebApplication?.StopAsync(cancel.Value) ?? Task.CompletedTask).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    ///<inheritdoc/>
    public override void Dispose(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolWebHostDisposing,MyTypeName,MyID);

            WebApplication?.Dispose();

            Lifetime!.Dispose(); base.Dispose_NoSync(key);

            Urls = null;
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

            Logger?.Verbose(ToolWebHostDisposing,MyTypeName,MyID);

            WebApplication?.Dispose();

            Lifetime!.Dispose(); base.Dispose_NoSync();

            Urls = null;
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

            Logger?.Verbose(ToolWebHostDisposing,MyTypeName,MyID);

            if(WebApplication is IAsyncDisposable d) { await d.DisposeAsync().ConfigureAwait(false); } else { WebApplication?.Dispose(); }

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync(key).ConfigureAwait(false);

            Urls = null;
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

            Logger?.Verbose(ToolWebHostDisposing,MyTypeName,MyID);

            if(WebApplication is IAsyncDisposable d) { await d.DisposeAsync().ConfigureAwait(false); } else { WebApplication?.Dispose(); }

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync().ConfigureAwait(false);

            Urls = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }
}