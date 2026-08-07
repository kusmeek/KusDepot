namespace KusDepot;

/**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/main/*'/>*/
public partial class ToolHost : Tool , IToolHost
{
    ///<inheritdoc/>
    public override async Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartingOK() is false) || await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            await base.StartingAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Starting_NoSync"]/*'/>*/
    protected override async Task Starting_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolHostStarting,MyTypeName,MyID);

        await base.Starting_NoSync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStarting(LifeID);
    }

    ///<inheritdoc/>
    public override async Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartOK() is false) || await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            await base.StartAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Start_NoSync"]/*'/>*/
    protected override async Task Start_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolHostStartEnter,MyTypeName,MyID);

        await base.Start_NoSync(cancel,key).ConfigureAwait(false);

        Logger?.Verbose(ToolHostStartExit,MyTypeName,MyID);
    }

    ///<inheritdoc/>
    public override async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartedOK() is false) || await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            await base.StartedAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Started_NoSync"]/*'/>*/
    protected override async Task Started_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        await base.Started_NoSync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStarted(LifeID);

        Logger?.Verbose(ToolHostStarted,MyTypeName,MyID);
    }

    ///<inheritdoc/>
    public override async Task StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StoppingOK() is false) || await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            await base.StoppingAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Stopping_NoSync"]/*'/>*/
    protected override async Task Stopping_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolHostStopping,MyTypeName,MyID);

        await base.Stopping_NoSync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStopping(LifeID);
    }

    ///<inheritdoc/>
    public override async Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StopOK() is false) || await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            await base.StopAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Stop_NoSync"]/*'/>*/
    protected override async Task Stop_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Logger?.Verbose(ToolHostStopEnter,MyTypeName,MyID);

        await base.Stop_NoSync(cancel,key).ConfigureAwait(false);

        Logger?.Verbose(ToolHostStopExit,MyTypeName,MyID);
    }

    ///<inheritdoc/>
    public override async Task StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None; try { await LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StoppedOK() is false) || await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            await base.StoppedAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { LifeSync.Release(); } }
    }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="Stopped_NoSync"]/*'/>*/
    protected override async Task Stopped_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        await base.Stopped_NoSync(cancel,key).ConfigureAwait(false); Lifetime!.NotifyStopped(LifeID);

        Logger?.Verbose(ToolHostStopped,MyTypeName,MyID);
    }

    ///<inheritdoc/>
    public virtual async Task RunAsync(CancellationToken cancel = default)
    {
        await this.StartHostAsync(cancel,key:SelfKey).ConfigureAwait(false);

        try { await Task.Delay(Timeout.Infinite,cancel).ConfigureAwait(false); } catch {}

        await this.StopHostAsync(timeout:HostStopTimeout,key:SelfKey).ConfigureAwait(false);
    }

    ///<inheritdoc/>
    public virtual void Run() { this.RunAsync().GetAwaiter().GetResult(); }
}