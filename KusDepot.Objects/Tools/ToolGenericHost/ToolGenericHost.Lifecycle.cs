namespace KusDepot;

/**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/main/*'/>*/
public partial class ToolGenericHost : ToolHost , IToolGenericHost
{
    ///<inheritdoc/>
    public override async Task<Boolean> StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            return await base.StartingAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( OperationCanceledException ) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/method[@name="Starting_NoSync"]/*'/>*/
    protected override async Task<Boolean> Starting_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolGenericHostStarting,MyTypeName,MyID);

        if(await base.Starting_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        if(Host is IToolHost t && await t.StartingAsync(cancel,HostKey).ConfigureAwait(false) is false) { return false; }

        return true;
    }

    ///<inheritdoc/>
    public override async Task<Boolean> StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            return await base.StartAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( OperationCanceledException ) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/method[@name="Start_NoSync"]/*'/>*/
    protected override async Task<Boolean> Start_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolGenericHostStartEnter,MyTypeName,MyID);

        if(await base.Start_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        if(Host is IToolHost t) { if(await t.StartAsync(cancel,HostKey).ConfigureAwait(false) is false) { return false; } } else { await (Host?.StartAsync(cancel.Value) ?? Task.CompletedTask).ConfigureAwait(false); }

        Logger?.Verbose(ToolGenericHostStartExit,MyTypeName,MyID);

        return true;
    }

    ///<inheritdoc/>
    public override async Task<Boolean> StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            return await base.StartedAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( OperationCanceledException ) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/method[@name="Started_NoSync"]/*'/>*/
    protected override async Task<Boolean> Started_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        if(await base.Started_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        if(Host is IToolHost t && await t.StartedAsync(cancel,HostKey).ConfigureAwait(false) is false) { return false; }

        Logger?.Information(ToolGenericHostStarted,MyTypeName,MyID);

        return true;
    }

    ///<inheritdoc/>
    public override async Task<Boolean> StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            return await base.StoppingAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( OperationCanceledException ) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/method[@name="Stopping_NoSync"]/*'/>*/
    protected override async Task<Boolean> Stopping_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolGenericHostStopping,MyTypeName,MyID);

        if(await base.Stopping_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        if(Host is IToolHost t && await t.StoppingAsync(cancel,HostKey).ConfigureAwait(false) is false) { return false; }

        return true;
    }

    ///<inheritdoc/>
    public override async Task<Boolean> StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            return await base.StopAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( OperationCanceledException ) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/method[@name="Stop_NoSync"]/*'/>*/
    protected override async Task<Boolean> Stop_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolGenericHostStopEnter,MyTypeName,MyID);

        if(Host is IToolHost t) { if(await t.StopAsync(cancel,HostKey).ConfigureAwait(false) is false) { return false; } } else { await (Host?.StopAsync(cancel.Value) ?? Task.CompletedTask).ConfigureAwait(false); }

        if(await base.Stop_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        Logger?.Verbose(ToolGenericHostStopExit,MyTypeName,MyID);

        return true;
    }

    ///<inheritdoc/>
    public override async Task<Boolean> StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            return await base.StoppedAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( OperationCanceledException ) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/method[@name="Stopped_NoSync"]/*'/>*/
    protected override async Task<Boolean> Stopped_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        if(await base.Stopped_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        if(Host is IToolHost t && await t.StoppedAsync(cancel,HostKey).ConfigureAwait(false) is false) { return false; }

        Logger?.Information(ToolGenericHostStopped,MyTypeName,MyID);

        return true;
    }}
