namespace KusDepot;

/**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/main/*'/>*/
public partial class ToolHost : Tool , IToolHost
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

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Starting_NoSync"]/*'/>*/
    protected override async Task<Boolean> Starting_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolHostStarting,MyTypeName,MyID);

        if(await base.Starting_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        Lifetime!.NotifyStarting(this.LifeKey);

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

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Start_NoSync"]/*'/>*/
    protected override async Task<Boolean> Start_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolHostStartEnter,MyTypeName,MyID);

        if(await base.Start_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        Logger?.Verbose(ToolHostStartExit,MyTypeName,MyID);

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

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Started_NoSync"]/*'/>*/
    protected override async Task<Boolean> Started_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        if(await base.Started_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        Lifetime!.NotifyStarted(this.LifeKey);

        Logger?.Verbose(ToolHostStarted,MyTypeName,MyID);

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

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Stopping_NoSync"]/*'/>*/
    protected override async Task<Boolean> Stopping_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolHostStopping,MyTypeName,MyID);

        if(await base.Stopping_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        Lifetime!.NotifyStopping(this.LifeKey);

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

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Stop_NoSync"]/*'/>*/
    protected override async Task<Boolean> Stop_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolHostStopEnter,MyTypeName,MyID);

        if(await base.Stop_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        Logger?.Verbose(ToolHostStopExit,MyTypeName,MyID);

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

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Stopped_NoSync"]/*'/>*/
    protected override async Task<Boolean> Stopped_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        if(await base.Stopped_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

        Lifetime!.NotifyStopped(this.LifeKey);

        Logger?.Verbose(ToolHostStopped,MyTypeName,MyID);

        return true;
    }

    ///<inheritdoc/>
    public virtual Boolean Run(AccessKey? key = null , TimeSpan? timeout = null)
    {
        return this.RunAsync(key,timeout).GetAwaiter().GetResult();
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> RunAsync(AccessKey? key = null , TimeSpan? timeout = null , CancellationToken cancel = default)
    {
        if(await this.StartHostAsync(cancel,timeout,key).ConfigureAwait(false) is false) { return false; }

        using CancellationTokenSource wait = CancellationTokenSource.CreateLinkedTokenSource(cancel,this.Lifetime!.ApplicationStopped);

        try { await Task.Delay(Infinite,wait.Token).ConfigureAwait(false); }

        catch ( OperationCanceledException ) when ( wait.IsCancellationRequested ) {}

        if(this.Lifetime.ApplicationStopped.IsCancellationRequested) { return true; }

        return await this.StopHostAsync(timeout:timeout,key:key).ConfigureAwait(false);
    }
}