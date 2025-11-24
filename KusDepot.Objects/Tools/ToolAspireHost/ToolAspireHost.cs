namespace KusDepot;

/**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/main/*'/>*/
public class ToolAspireHost : ToolHost , IToolAspireHost
{
    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/property[@name="AspireApplication"]/*'/>*/
    protected IHost? AspireApplication { get; set; }

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/property[@name="LifeSync"]/*'/>*/
    protected new SemaphoreSlim LifeSync { get; set; } = new(1,1);

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/constructor[@name="Constructor"]/*'/>*/
    public ToolAspireHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null,
           Guid? lifeid = null , IToolHostLifetime? lifetime = null , ILoggerFactory? logger = null , IHost? aspireapplication = null) : base(accessmanager,data,id,services,commands,configuration,lifeid,lifetime,logger)
    {
        try
        {
            this.AspireApplication = aspireapplication;

            this.Services = this.AspireApplication?.Services ?? this.ToolServiceScope?.ServiceProvider; this.ResolveLogger(logger);
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolAspireHost() : this(null,null,null,null,null,null,null,null,null,null){}

    ///<inheritdoc/>
    public IHost? GetManagedApplication(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.AspireApplication; }

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

            this.Logger = (logger ?? this.AspireApplication?.Services.GetService<ILoggerFactory>() ?? this.ToolServices?.GetService<ILoggerFactory>())?
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

            Logger?.Verbose(ToolAspireHostStarting,MyTypeName,MyID); await base.StartingAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStarting(LifeID);
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
            Logger?.Verbose(ToolAspireHostStartEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; try { await LifeSync!.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StartAsync(cancel,key).ConfigureAwait(false); await (AspireApplication?.StartAsync(cancel.Value) ?? Task.CompletedTask).ConfigureAwait(false); Logger?.Verbose(ToolAspireHostStartExit,MyTypeName,MyID);
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

            await base.StartedAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStarted(LifeID); Logger?.Information(ToolAspireHostStarted,MyTypeName,MyID);
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

            Logger?.Verbose(ToolAspireHostStopping,MyTypeName,MyID); await base.StoppingAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStopping(LifeID);
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
            Logger?.Verbose(ToolAspireHostStopEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; try { await LifeSync!.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StopOK() is false) || (this.AccessCheck(key) is false)) { return; }

            await base.StopAsync(cancel,key).ConfigureAwait(false); Logger?.Verbose(ToolAspireHostStopExit,MyTypeName,MyID);
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

            await base.StoppedAsync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStopped(LifeID); Logger?.Information(ToolAspireHostStopped,MyTypeName,MyID); await (AspireApplication?.StopAsync(cancel.Value) ?? Task.CompletedTask).ConfigureAwait(false);
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

            Logger?.Verbose(ToolAspireHostDisposing,MyTypeName,MyID);

            AspireApplication?.Dispose();

            Lifetime?.Dispose(); base.Dispose_NoSync(key);
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

            Logger?.Verbose(ToolAspireHostDisposing,MyTypeName,MyID);

            AspireApplication?.Dispose();

            Lifetime?.Dispose(); base.Dispose_NoSync();
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

            Logger?.Verbose(ToolAspireHostDisposing,MyTypeName,MyID);

            if(AspireApplication is IAsyncDisposable d) { await d.DisposeAsync().ConfigureAwait(false); } else { AspireApplication?.Dispose(); }

            Lifetime?.Dispose(); await base.DisposeAsync_NoSync(key).ConfigureAwait(false);
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

            Logger?.Verbose(ToolAspireHostDisposing,MyTypeName,MyID);

            if(AspireApplication is IAsyncDisposable d) { await d.DisposeAsync().ConfigureAwait(false); } else { AspireApplication?.Dispose(); }

            Lifetime?.Dispose(); await base.DisposeAsync_NoSync().ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }
}