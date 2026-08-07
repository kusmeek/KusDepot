namespace KusDepot;

public partial class Tool : Common , IHostedLifecycleService , ITool
{
    ///<inheritdoc/>
    public virtual Task StartingAsync(CancellationToken cancel = default) { return this.StartingAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartingAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StartingAsync)]
    public virtual async Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            await this.Starting_NoSync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Starting_NoSync"]/*'/>*/
    protected virtual async Task Starting_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Logger?.Trace(ToolStarting,MyTypeName,MyID); cancel ??= CancellationToken.None; Stopwatch? w = default;

        try
        {
            if(LifeState.StartingOK() is false) { return; }

            TimeSpan r; this.LifeState.ToStarting();

            IHostedLifecycleService[] h = GetHostedLifecycleServicesSnapshot(this.GetHostedServicesSnapshot());

            if(h.Length > 0)
            {
                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = new Task[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStartingAsync(h[i],cancel.Value); }

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    TimeSpan timeout = HostingOptions?.StartupTimeout ?? InfiniteTimeSpan;

                    if(timeout != InfiniteTimeSpan) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h)
                    {
                        r = timeout == InfiniteTimeSpan ? InfiniteTimeSpan : timeout - (w?.Elapsed ?? TimeSpan.Zero);

                        if(timeout != InfiniteTimeSpan && r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        await this.InvokeHostedServiceStartingAsync(_,cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false);

                        s = _;
                    }
                }
            }
        }
        finally { w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StartAsync(CancellationToken cancel = default) { return this.StartAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StartAsync)]
    public virtual async Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            await this.Start_NoSync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Start_NoSync"]/*'/>*/
    protected virtual async Task Start_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Logger?.Trace(ToolStartEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; Stopwatch? w = default;

        try
        {
            if(LifeState.StartOK() is false) { return; }

            TimeSpan r; this.LifeState.ToStarting();

            IHostedService[] h = this.GetHostedServicesSnapshot();

            if(h.Length > 0)
            {
                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = new Task[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStartAsync(h[i],cancel.Value); }

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    TimeSpan timeout = HostingOptions?.StartupTimeout ?? InfiniteTimeSpan;

                    if(timeout != InfiniteTimeSpan) { w = new(); w.Start(); } IHostedService? s = default;

                    foreach(IHostedService _ in h)
                    {
                        r = timeout == InfiniteTimeSpan ? InfiniteTimeSpan : timeout - (w?.Elapsed ?? TimeSpan.Zero);

                        if(timeout != InfiniteTimeSpan && r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        await this.InvokeHostedServiceStartAsync(_,cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false);

                        s = _;
                    }
                }
            }
            await this.Activate_NoSync(cancel.Value,this.SelfKey).ConfigureAwait(false); Logger?.Trace(ToolStartExit,MyTypeName,MyID);
        }
        finally { w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StartedAsync(CancellationToken cancel = default) { return this.StartedAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartedAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StartedAsync)]
    public virtual async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            await this.Started_NoSync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Started_NoSync"]/*'/>*/
    protected virtual async Task Started_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None; Stopwatch? w = default;

        try
        {
            if(LifeState.StartedOK() is false) { return; }

            IHostedLifecycleService[] h = GetHostedLifecycleServicesSnapshot(this.GetHostedServicesSnapshot());

            if(h.Length > 0)
            {
                TimeSpan r;

                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = new Task[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStartedAsync(h[i],cancel.Value); }

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    TimeSpan timeout = HostingOptions?.StartupTimeout ?? InfiniteTimeSpan;

                    if(timeout != InfiniteTimeSpan) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h)
                    {
                        r = timeout == InfiniteTimeSpan ? InfiniteTimeSpan : timeout - (w?.Elapsed ?? TimeSpan.Zero);

                        if(timeout != InfiniteTimeSpan && r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        await this.InvokeHostedServiceStartedAsync(_,cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false);

                        s = _;
                    }
                }
            }
            Logger?.Trace(ToolStarted,MyTypeName,MyID);
        }
        finally { w?.Stop(); }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.StartHostAsync)]
    public virtual async Task<Boolean> StartHostAsync(CancellationToken? cancel = null , TimeSpan? timeout = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel ?? CancellationToken.None).ConfigureAwait(false) is false || LifeState.StartHostOK() is false) { return false; }

            cancel ??= new(); CancellationTokenSource _ = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value); if(timeout.HasValue) { _.CancelAfter(timeout.Value); }

            if(!await this.Sync.Life.WaitAsync(SyncTime,_.Token).ConfigureAwait(false)) { throw SyncException; } ls = true;

            await this.Starting_NoSync(_.Token,key).ConfigureAwait(false); await this.Start_NoSync(_.Token,key).ConfigureAwait(false); await this.Started_NoSync(_.Token,key).ConfigureAwait(false);

            return Equals(this.LifeState.State,Active);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartHostingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { this.LifeState.ToError(); return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="StartHostedServiceCore"]/*'/>*/
    protected virtual async Task<Boolean> StartHostedServiceCore(IHostedService service , CancellationToken cancel = default)
    {
        try
        {
            ITool? t = service as ITool; IHostedLifecycleService? s = service as IHostedLifecycleService;

            if(t is not null) { await t.StartingAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false); }

            else { if(s is not null) { await s.StartingAsync(cancel).ConfigureAwait(false); } }

            if(t is not null) { await t.StartAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false); }

            else { await service.StartAsync(cancel).ConfigureAwait(false); }

            if(t is not null) { await t.StartedAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false); }

            else { if(s is not null) { await s.StartedAsync(cancel).ConfigureAwait(false); } }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,StartHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.StopHostAsync)]
    public virtual async Task<Boolean> StopHostAsync(CancellationToken? cancel = null , TimeSpan? timeout = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel ?? CancellationToken.None).ConfigureAwait(false) is false || LifeState.StopHostOK() is false) { return false; }

            cancel ??= new(); CancellationTokenSource _ = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value); if(timeout.HasValue) { _.CancelAfter(timeout.Value); }

            if(!await this.Sync.Life.WaitAsync(SyncTime,_.Token).ConfigureAwait(false)) { throw SyncException; } ls = true;

            await this.Stopping_NoSync(_.Token,key).ConfigureAwait(false); await this.Stop_NoSync(_.Token,key).ConfigureAwait(false); await this.Stopped_NoSync(_.Token,key).ConfigureAwait(false);

            return Equals(this.LifeState.State,InActive);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopHostingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { this.LifeState.ToError(); return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="StopHostedServiceCore"]/*'/>*/
    protected virtual async Task<Boolean> StopHostedServiceCore(IHostedService service , CancellationToken cancel = default)
    {
        try
        {
            ITool? t = service as ITool; IHostedLifecycleService? s = service as IHostedLifecycleService;

            if(t is not null) { await t.StoppingAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false); }

            else { if(s is not null) { await s.StoppingAsync(cancel).ConfigureAwait(false); } }

            if(t is not null) { await t.StopAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false); }

            else { await service.StopAsync(cancel).ConfigureAwait(false); }

            if(t is not null) { await t.StoppedAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false); }

            else { if(s is not null) { await s.StoppedAsync(cancel).ConfigureAwait(false); } }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,StopHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Task StoppingAsync(CancellationToken cancel = default) { return this.StoppingAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StoppingAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StoppingAsync)]
    public virtual async Task StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            await this.Stopping_NoSync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Stopping_NoSync"]/*'/>*/
    protected virtual async Task Stopping_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Logger?.Trace(ToolStopping,MyTypeName,MyID); cancel ??= CancellationToken.None; Stopwatch? w = default;

        try
        {
            if(LifeState.StoppingOK() is false) { return; }

            TimeSpan r; this.LifeState.ToStopping();

            IHostedLifecycleService[] h = GetHostedLifecycleServicesSnapshot(this.GetHostedServicesSnapshot());

            if(h.Length > 0)
            {
                if(HostingOptions?.ServicesStopConcurrently?? true)
                {
                    Task[] l = new Task[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStoppingAsync(h[i],cancel.Value); }

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    TimeSpan timeout = HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan;

                    if(timeout != InfiniteTimeSpan) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h)
                    {
                        r = timeout == InfiniteTimeSpan ? InfiniteTimeSpan : timeout - (w?.Elapsed ?? TimeSpan.Zero);

                        if(timeout != InfiniteTimeSpan && r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        await this.InvokeHostedServiceStoppingAsync(_,cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false);

                        s = _;
                    }
                }
            }
        }
        finally { w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancel = default) { return this.StopAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StopAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StopAsync)]
    public virtual async Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            await this.Stop_NoSync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Stop_NoSync"]/*'/>*/
    protected virtual async Task Stop_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Logger?.Trace(ToolStopEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; Stopwatch? w = default;

        try
        {
            if(LifeState.StopOK() is false) { return; }

            TimeSpan r; this.LifeState.ToStopping();

            IHostedService[] h = this.GetHostedServicesSnapshot();

            if(h.Length > 0)
            {
                if(HostingOptions?.ServicesStopConcurrently ?? true)
                {
                    Task[] l = new Task[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStopAsync(h[i],cancel.Value); }

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    TimeSpan timeout = HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan;

                    if(timeout != InfiniteTimeSpan) { w = new(); w.Start(); } IHostedService? s = default;

                    foreach(IHostedService _ in h)
                    {
                        r = timeout == InfiniteTimeSpan ? InfiniteTimeSpan : timeout - (w?.Elapsed ?? TimeSpan.Zero);

                        if(timeout != InfiniteTimeSpan && r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        await this.InvokeHostedServiceStopAsync(_,cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false);

                        s = _;
                    }
                }
            }
            await this.Deactivate_NoSync(cancel.Value,this.SelfKey).ConfigureAwait(false); Logger?.Trace(ToolStopExit,MyTypeName,MyID);
        }
        finally { w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StoppedAsync(CancellationToken cancel = default) { return this.StoppedAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StoppedAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StoppedAsync)]
    public virtual async Task StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            await this.Stopped_NoSync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Stopped_NoSync"]/*'/>*/
    protected virtual async Task Stopped_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None; Stopwatch? w = default;

        try
        {
            if(LifeState.StoppedOK() is false) { return; }

            IHostedLifecycleService[] h = GetHostedLifecycleServicesSnapshot(this.GetHostedServicesSnapshot());

            if(h.Length > 0)
            {
                TimeSpan r;

                if(HostingOptions?.ServicesStopConcurrently ?? true)
                {
                    Task[] l = new Task[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStoppedAsync(h[i],cancel.Value); }

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    TimeSpan timeout = HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan;

                    if(timeout != InfiniteTimeSpan) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h)
                    {
                        r = timeout == InfiniteTimeSpan ? InfiniteTimeSpan : timeout - (w?.Elapsed ?? TimeSpan.Zero);

                        if(timeout != InfiniteTimeSpan && r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        await this.InvokeHostedServiceStoppedAsync(_,cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false);

                        s = _;
                    }
                }
            }
            Logger?.Trace(ToolStopped,MyTypeName,MyID);
        }
        finally { w?.Stop(); }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStartingAsync"]/*'/>*/
    private async Task InvokeHostedServiceStartingAsync(IHostedLifecycleService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                await t.StartingAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StartingAsync(cancel).ConfigureAwait(false); }
        }
        catch ( ObjectDisposedException ) { }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStartAsync"]/*'/>*/
    private async Task InvokeHostedServiceStartAsync(IHostedService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                await t.StartAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StartAsync(cancel).ConfigureAwait(false); }
        }
        catch ( ObjectDisposedException ) { }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStartedAsync"]/*'/>*/
    private async Task InvokeHostedServiceStartedAsync(IHostedLifecycleService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                await t.StartedAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StartedAsync(cancel).ConfigureAwait(false); }
        }
        catch ( ObjectDisposedException ) { }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStoppingAsync"]/*'/>*/
    private async Task InvokeHostedServiceStoppingAsync(IHostedLifecycleService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                await t.StoppingAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StoppingAsync(cancel).ConfigureAwait(false); }
        }
        catch ( ObjectDisposedException ) { }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStopAsync"]/*'/>*/
    private async Task InvokeHostedServiceStopAsync(IHostedService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                await t.StopAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StopAsync(cancel).ConfigureAwait(false); }
        }
        catch ( ObjectDisposedException ) { }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStoppedAsync"]/*'/>*/
    private async Task InvokeHostedServiceStoppedAsync(IHostedLifecycleService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                await t.StoppedAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StoppedAsync(cancel).ConfigureAwait(false); }
        }
        catch ( ObjectDisposedException ) { }
    }
}