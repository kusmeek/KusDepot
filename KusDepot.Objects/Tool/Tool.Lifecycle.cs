namespace KusDepot;

/**<include file='Tool.xml' path='Tool/class[@name="Tool"]/main/*'/>*/
public partial class Tool : Common , IHostedLifecycleService , ITool
{
    ///<inheritdoc/>
    public virtual Task StartingAsync(CancellationToken cancel = default) { return this.StartingAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartingAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StartingAsync)]
    public virtual async Task<Boolean> StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(await this.Starting_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            return true;
        }
        catch ( OperationCanceledException ) when(!ls) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Starting_NoSync"]/*'/>*/
    protected virtual async Task<Boolean> Starting_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Logger?.Trace(ToolStarting,MyTypeName,MyID); cancel ??= CancellationToken.None;

        Stopwatch? w = default; CancellationTokenSource? timeoutcancel = default;

        try
        {
            LifeCycleOperationResult operation = LifeState.TryAdmitOperation(LifeCycleOperation.Starting,out _);

            if(operation is LifeCycleOperationResult.Rejected) { return false; }

            if(operation is LifeCycleOperationResult.AlreadySatisfied) { return true; }

            TimeSpan timeout = HostingOptions?.StartupTimeout ?? InfiniteTimeSpan;

            if(timeout != InfiniteTimeSpan)
            {
                timeoutcancel = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value);

                timeoutcancel.CancelAfter(timeout);
            }

            CancellationToken phasecancel = timeoutcancel?.Token ?? cancel.Value;

            TimeSpan r;

            IHostedLifecycleService[] h = GetHostedLifecycleServicesSnapshot(this.GetHostedServicesSnapshot());

            if(h.Length > 0)
            {
                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task<Boolean>[] l = new Task<Boolean>[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStartingAsync(h[i],phasecancel); }

                    if((await Task.WhenAll(l).WaitAsync(timeout,cancel.Value).ConfigureAwait(false)).Any(_ => _ is false)) { return false; }
                }
                else
                {
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

                        if(await this.InvokeHostedServiceStartingAsync(_,phasecancel).WaitAsync(r,cancel.Value).ConfigureAwait(false) is false) { return false; }

                        s = _;
                    }
                }

            }

            return true;
        }
        catch ( OperationCanceledException )
        {
            if(timeoutcancel?.IsCancellationRequested is true && cancel.Value.IsCancellationRequested is false) { timeoutcancel.Cancel(); throw new TimeoutException(); }

            timeoutcancel?.Cancel(); throw;
        }
        catch ( TimeoutException ) { timeoutcancel?.Cancel(); throw; }

        finally { timeoutcancel?.Dispose(); w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StartAsync(CancellationToken cancel = default) { return this.StartAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StartAsync)]
    public virtual async Task<Boolean> StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(await this.Start_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            return true;
        }
        catch ( OperationCanceledException ) when(!ls) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Start_NoSync"]/*'/>*/
    protected virtual async Task<Boolean> Start_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Logger?.Trace(ToolStartEnter,MyTypeName,MyID); cancel ??= CancellationToken.None;

        Stopwatch? w = default; CancellationTokenSource? timeoutcancel = default;

        try
        {
            LifeCycleOperationResult operation = LifeState.TryAdmitOperation(LifeCycleOperation.Start,out _);

            if(operation is LifeCycleOperationResult.Rejected) { return false; }

            if(operation is LifeCycleOperationResult.AlreadySatisfied) { return true; }

            TimeSpan timeout = HostingOptions?.StartupTimeout ?? InfiniteTimeSpan;

            if(timeout != InfiniteTimeSpan)
            {
                timeoutcancel = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value);

                timeoutcancel.CancelAfter(timeout);
            }

            CancellationToken phasecancel = timeoutcancel?.Token ?? cancel.Value;

            TimeSpan r;

            IHostedService[] h = this.GetHostedServicesSnapshot();

            if(h.Length > 0)
            {
                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task<Boolean>[] l = new Task<Boolean>[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStartAsync(h[i],phasecancel); }

                    if((await Task.WhenAll(l).WaitAsync(timeout,cancel.Value).ConfigureAwait(false)).Any(_ => _ is false)) { return false; }
                }
                else
                {
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

                        if(await this.InvokeHostedServiceStartAsync(_,phasecancel).WaitAsync(r,cancel.Value).ConfigureAwait(false) is false) { return false; }

                        s = _;
                    }
                }
            }
            Boolean activated = await this.Activate_NoSync(cancel.Value).ConfigureAwait(false);

            Logger?.Trace(ToolStartExit,MyTypeName,MyID); return activated;
        }
        catch ( OperationCanceledException )
        {
            if(timeoutcancel?.IsCancellationRequested is true && cancel.Value.IsCancellationRequested is false) { timeoutcancel.Cancel(); throw new TimeoutException(); }

            timeoutcancel?.Cancel(); throw;
        }
        catch ( TimeoutException ) { timeoutcancel?.Cancel(); throw; }

        finally { timeoutcancel?.Dispose(); w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StartedAsync(CancellationToken cancel = default) { return this.StartedAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartedAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StartedAsync)]
    public virtual async Task<Boolean> StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(await this.Started_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            return true;
        }
        catch ( OperationCanceledException ) when(!ls) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Started_NoSync"]/*'/>*/
    protected virtual async Task<Boolean> Started_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Stopwatch? w = default; CancellationTokenSource? timeoutcancel = default;

        try
        {
            LifeCycleOperationResult operation = LifeState.TryAdmitOperation(LifeCycleOperation.Started,out _);

            if(operation is LifeCycleOperationResult.Rejected) { return false; }

            if(operation is LifeCycleOperationResult.AlreadySatisfied) { return true; }

            TimeSpan timeout = HostingOptions?.StartupTimeout ?? InfiniteTimeSpan;

            if(timeout != InfiniteTimeSpan)
            {
                timeoutcancel = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value);

                timeoutcancel.CancelAfter(timeout);
            }

            CancellationToken phasecancel = timeoutcancel?.Token ?? cancel.Value;

            IHostedLifecycleService[] h = GetHostedLifecycleServicesSnapshot(this.GetHostedServicesSnapshot());

            if(h.Length > 0)
            {
                TimeSpan r;

                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task<Boolean>[] l = new Task<Boolean>[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStartedAsync(h[i],phasecancel); }

                    if((await Task.WhenAll(l).WaitAsync(timeout,cancel.Value).ConfigureAwait(false)).Any(_ => _ is false)) { return false; }
                }
                else
                {
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

                        if(await this.InvokeHostedServiceStartedAsync(_,phasecancel).WaitAsync(r,cancel.Value).ConfigureAwait(false) is false) { return false; }

                        s = _;
                    }
                }
            }
            Logger?.Trace(ToolStarted,MyTypeName,MyID); return true;
        }
        catch ( OperationCanceledException )
        {
            if(timeoutcancel?.IsCancellationRequested is true && cancel.Value.IsCancellationRequested is false) { timeoutcancel.Cancel(); throw new TimeoutException(); }

            timeoutcancel?.Cancel(); throw;
        }
        catch ( TimeoutException ) { timeoutcancel?.Cancel(); throw; }

        finally { timeoutcancel?.Dispose(); w?.Stop(); }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.StartHostAsync)]
    public virtual async Task<Boolean> StartHostAsync(CancellationToken? cancel = null , TimeSpan? timeout = null , AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel ?? CancellationToken.None).ConfigureAwait(false) is false) { return false; }

            cancel ??= CancellationToken.None; using CancellationTokenSource? opcancel = timeout.HasValue ? CancellationTokenSource.CreateLinkedTokenSource(cancel.Value) : null;

            if(opcancel is not null && timeout.HasValue) { opcancel.CancelAfter(timeout.Value); } cancel = opcancel?.Token ?? cancel.Value;

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            if(await this.Starting_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            if(await this.Start_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            if(await this.Started_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            return Equals(this.LifeState.State,Active);
        }
        catch ( OperationCanceledException ) when(!ls) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StartHostingAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="StartHostedServiceCore"]/*'/>*/
    protected virtual async Task<Boolean> StartHostedServiceCore(IHostedService service , CancellationToken cancel = default)
    {
        try
        {
            ITool? t = service as ITool; IHostedLifecycleService? s = service as IHostedLifecycleService;

            if(t is not null) { if(await t.StartingAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false) is false) { return false; } }

            else { if(s is not null) { await s.StartingAsync(cancel).ConfigureAwait(false); } }

            if(t is not null) { if(await t.StartAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false) is false) { return false; } }

            else { await service.StartAsync(cancel).ConfigureAwait(false); }

            if(t is not null) { if(await t.StartedAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false) is false) { return false; } }

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
            if(await this.AccessCheckAsync(key,cancel:cancel ?? CancellationToken.None).ConfigureAwait(false) is false) { return false; }

            cancel ??= CancellationToken.None; using CancellationTokenSource? opcancel = timeout.HasValue ? CancellationTokenSource.CreateLinkedTokenSource(cancel.Value) : null;

            if(opcancel is not null && timeout.HasValue) { opcancel.CancelAfter(timeout.Value); } cancel = opcancel?.Token ?? cancel.Value;

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            if(await this.Stopping_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            if(await this.Stop_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            if(await this.Stopped_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            return Equals(this.LifeState.State,InActive);
        }
        catch ( OperationCanceledException ) when(!ls) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StopHostingAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="StopHostedServiceCore"]/*'/>*/
    protected virtual async Task<Boolean> StopHostedServiceCore(IHostedService service , CancellationToken cancel = default)
    {
        try
        {
            ITool? t = service as ITool; IHostedLifecycleService? s = service as IHostedLifecycleService;

            if(t is not null) { if(await t.StoppingAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false) is false) { return false; } }

            else { if(s is not null) { await s.StoppingAsync(cancel).ConfigureAwait(false); } }

            if(t is not null) { if(await t.StopAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false) is false) { return false; } }

            else { await service.StopAsync(cancel).ConfigureAwait(false); }

            if(t is not null) { if(await t.StoppedAsync(cancel,this.HostingKeys!.GetValueOrDefault(t)).ConfigureAwait(false) is false) { return false; } }

            else { if(s is not null) { await s.StoppedAsync(cancel).ConfigureAwait(false); } }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,StopHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Task StoppingAsync(CancellationToken cancel = default) { return this.StoppingAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StoppingAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StoppingAsync)]
    public virtual async Task<Boolean> StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(await this.Stopping_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            return true;
        }
        catch ( OperationCanceledException ) when(!ls) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Stopping_NoSync"]/*'/>*/
    protected virtual async Task<Boolean> Stopping_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Logger?.Trace(ToolStopping,MyTypeName,MyID); cancel ??= CancellationToken.None;

        Stopwatch? w = default; CancellationTokenSource? timeoutcancel = default;

        try
        {
            LifeCycleOperationResult operation = LifeState.TryAdmitOperation(LifeCycleOperation.Stopping,out _);

            if(operation is LifeCycleOperationResult.Rejected) { return false; }

            if(operation is LifeCycleOperationResult.AlreadySatisfied) { return true; }

            TimeSpan timeout = HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan;

            if(timeout != InfiniteTimeSpan)
            {
                timeoutcancel = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value);

                timeoutcancel.CancelAfter(timeout);
            }

            CancellationToken phasecancel = timeoutcancel?.Token ?? cancel.Value;

            TimeSpan r;

            IHostedLifecycleService[] h = GetHostedLifecycleServicesSnapshot(this.GetHostedServicesSnapshot());

            if(h.Length > 0)
            {
                if(HostingOptions?.ServicesStopConcurrently?? true)
                {
                    Task<Boolean>[] l = new Task<Boolean>[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStoppingAsync(h[i],phasecancel); }

                    if((await Task.WhenAll(l).WaitAsync(timeout,cancel.Value).ConfigureAwait(false)).Any(_ => _ is false)) { return false; }
                }
                else
                {
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

                        if(await this.InvokeHostedServiceStoppingAsync(_,phasecancel).WaitAsync(r,cancel.Value).ConfigureAwait(false) is false) { return false; }

                        s = _;
                    }
                }
            }

            return true;
        }
        catch ( OperationCanceledException )
        {
            if(timeoutcancel?.IsCancellationRequested is true && cancel.Value.IsCancellationRequested is false) { timeoutcancel.Cancel(); throw new TimeoutException(); }

            timeoutcancel?.Cancel(); throw;
        }
        catch ( TimeoutException ) { timeoutcancel?.Cancel(); throw; }

        finally { timeoutcancel?.Dispose(); w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancel = default) { return this.StopAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StopAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StopAsync)]
    public virtual async Task<Boolean> StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(await this.Stop_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            return true;
        }
        catch ( OperationCanceledException ) when(!ls) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Stop_NoSync"]/*'/>*/
    protected virtual async Task<Boolean> Stop_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Logger?.Trace(ToolStopEnter,MyTypeName,MyID); cancel ??= CancellationToken.None;

        Stopwatch? w = default; CancellationTokenSource? timeoutcancel = default;

        try
        {
            LifeCycleOperationResult operation = LifeState.TryAdmitOperation(LifeCycleOperation.Stop,out _);

            if(operation is LifeCycleOperationResult.Rejected) { return false; }

            if(operation is LifeCycleOperationResult.AlreadySatisfied) { return true; }

            TimeSpan timeout = HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan;

            if(timeout != InfiniteTimeSpan)
            {
                timeoutcancel = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value);

                timeoutcancel.CancelAfter(timeout);
            }

            CancellationToken phasecancel = timeoutcancel?.Token ?? cancel.Value;

            TimeSpan r;

            IHostedService[] h = this.GetHostedServicesSnapshot();

            if(h.Length > 0)
            {
                if(HostingOptions?.ServicesStopConcurrently ?? true)
                {
                    Task<Boolean>[] l = new Task<Boolean>[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStopAsync(h[i],phasecancel); }

                    if((await Task.WhenAll(l).WaitAsync(timeout,cancel.Value).ConfigureAwait(false)).Any(_ => _ is false)) { return false; }
                }
                else
                {
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

                        if(await this.InvokeHostedServiceStopAsync(_,phasecancel).WaitAsync(r,cancel.Value).ConfigureAwait(false) is false) { return false; }

                        s = _;
                    }
                }
            }
            Boolean deactivated = await this.Deactivate_NoSync(cancel.Value).ConfigureAwait(false);

            Logger?.Trace(ToolStopExit,MyTypeName,MyID); return deactivated;
        }
        catch ( OperationCanceledException )
        {
            if(timeoutcancel?.IsCancellationRequested is true && cancel.Value.IsCancellationRequested is false) { timeoutcancel.Cancel(); throw new TimeoutException(); }

            timeoutcancel?.Cancel(); throw;
        }
        catch ( TimeoutException ) { timeoutcancel?.Cancel(); throw; }

        finally { timeoutcancel?.Dispose(); w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StoppedAsync(CancellationToken cancel = default) { return this.StoppedAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StoppedAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StoppedAsync)]
    public virtual async Task<Boolean> StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;

        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(await this.Stopped_NoSync(cancel,key).ConfigureAwait(false) is false) { return false; }

            return true;
        }
        catch ( OperationCanceledException ) when(!ls) { if(NoExceptions||MyNoExceptions) { return false; } throw; }

        catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Stopped_NoSync"]/*'/>*/
    protected virtual async Task<Boolean> Stopped_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        cancel ??= CancellationToken.None;

        Stopwatch? w = default; CancellationTokenSource? timeoutcancel = default;

        try
        {
            LifeCycleOperationResult operation = LifeState.TryAdmitOperation(LifeCycleOperation.Stopped,out _);

            if(operation is LifeCycleOperationResult.Rejected) { return false; }

            if(operation is LifeCycleOperationResult.AlreadySatisfied) { return true; }

            TimeSpan timeout = HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan;

            if(timeout != InfiniteTimeSpan)
            {
                timeoutcancel = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value);

                timeoutcancel.CancelAfter(timeout);
            }

            CancellationToken phasecancel = timeoutcancel?.Token ?? cancel.Value;

            IHostedLifecycleService[] h = GetHostedLifecycleServicesSnapshot(this.GetHostedServicesSnapshot());

            if(h.Length > 0)
            {
                TimeSpan r;

                if(HostingOptions?.ServicesStopConcurrently ?? true)
                {
                    Task<Boolean>[] l = new Task<Boolean>[h.Length];

                    for(Int32 i = 0; i < h.Length; i++) { l[i] = this.InvokeHostedServiceStoppedAsync(h[i],phasecancel); }

                    if((await Task.WhenAll(l).WaitAsync(timeout,cancel.Value).ConfigureAwait(false)).Any(_ => _ is false)) { return false; }
                }
                else
                {
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

                        if(await this.InvokeHostedServiceStoppedAsync(_,phasecancel).WaitAsync(r,cancel.Value).ConfigureAwait(false) is false) { return false; }

                        s = _;
                    }
                }
            }
            Logger?.Trace(ToolStopped,MyTypeName,MyID); return true;
        }
        catch ( OperationCanceledException )
        {
            if(timeoutcancel?.IsCancellationRequested is true && cancel.Value.IsCancellationRequested is false) { timeoutcancel.Cancel(); throw new TimeoutException(); }

            timeoutcancel?.Cancel(); throw;
        }
        catch ( TimeoutException ) { timeoutcancel?.Cancel(); throw; }

        finally { timeoutcancel?.Dispose(); w?.Stop(); }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStartingAsync"]/*'/>*/
    private async Task<Boolean> InvokeHostedServiceStartingAsync(IHostedLifecycleService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                return await t.StartingAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StartingAsync(cancel).ConfigureAwait(false); }

            return true;
        }
        catch ( ObjectDisposedException ) { return true; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStartAsync"]/*'/>*/
    private async Task<Boolean> InvokeHostedServiceStartAsync(IHostedService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                return await t.StartAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StartAsync(cancel).ConfigureAwait(false); }

            return true;
        }
        catch ( ObjectDisposedException ) { return true; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStartedAsync"]/*'/>*/
    private async Task<Boolean> InvokeHostedServiceStartedAsync(IHostedLifecycleService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                return await t.StartedAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StartedAsync(cancel).ConfigureAwait(false); }

            return true;
        }
        catch ( ObjectDisposedException ) { return true; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStoppingAsync"]/*'/>*/
    private async Task<Boolean> InvokeHostedServiceStoppingAsync(IHostedLifecycleService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                return await t.StoppingAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StoppingAsync(cancel).ConfigureAwait(false); }

            return true;
        }
        catch ( ObjectDisposedException ) { return true; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStopAsync"]/*'/>*/
    private async Task<Boolean> InvokeHostedServiceStopAsync(IHostedService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                return await t.StopAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StopAsync(cancel).ConfigureAwait(false); }

            return true;
        }
        catch ( ObjectDisposedException ) { return true; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InvokeHostedServiceStoppedAsync"]/*'/>*/
    private async Task<Boolean> InvokeHostedServiceStoppedAsync(IHostedLifecycleService service , CancellationToken cancel)
    {
        try
        {
            if(service is ITool t)
            {
                return await t.StoppedAsync(cancel,HostingKeys![t]).ConfigureAwait(false);
            }
            else { await service.StoppedAsync(cancel).ConfigureAwait(false); }

            return true;
        }
        catch ( ObjectDisposedException ) { return true; }
    }
}