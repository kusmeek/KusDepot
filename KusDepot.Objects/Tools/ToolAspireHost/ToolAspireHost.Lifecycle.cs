namespace KusDepot;

/**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/main/*'/>*/
public partial class ToolAspireHost : ToolHost , IToolAspireHost
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

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/method[@name="Starting_NoSync"]/*'/>*/
    protected override async Task<Boolean> Starting_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolAspireHostStarting,MyTypeName,MyID);

        return await base.Starting_NoSync(cancel,key).ConfigureAwait(false);
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

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/method[@name="Start_NoSync"]/*'/>*/
    protected override async Task<Boolean> Start_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolAspireHostStartEnter,MyTypeName,MyID);

        if(await base.Start_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        await (AspireApplication?.StartAsync(cancel.Value) ?? Task.CompletedTask).ConfigureAwait(false);

        Logger?.Verbose(ToolAspireHostStartExit,MyTypeName,MyID);

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

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/method[@name="Started_NoSync"]/*'/>*/
    protected override async Task<Boolean> Started_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        if(await base.Started_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        Logger?.Information(ToolAspireHostStarted,MyTypeName,MyID);

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

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/method[@name="Stopping_NoSync"]/*'/>*/
    protected override async Task<Boolean> Stopping_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolAspireHostStopping,MyTypeName,MyID);

        if(await base.Stopping_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

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

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/method[@name="Stop_NoSync"]/*'/>*/
    protected override async Task<Boolean> Stop_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolAspireHostStopEnter,MyTypeName,MyID);

        if(await base.Stop_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        Logger?.Verbose(ToolAspireHostStopExit,MyTypeName,MyID);

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

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/method[@name="Stopped_NoSync"]/*'/>*/
    protected override async Task<Boolean> Stopped_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        if(await base.Stopped_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        Logger?.Information(ToolAspireHostStopped,MyTypeName,MyID); await (AspireApplication?.StopAsync(cancel.Value) ?? Task.CompletedTask).ConfigureAwait(false); return true;
    }
}