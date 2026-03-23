namespace KusDepot;

public partial class Tool : Common , IHostedLifecycleService , ITool
{
    ///<inheritdoc/>
    public virtual Task StartingAsync(CancellationToken cancel = default) { return this.StartingAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartingAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StartingAsync)]
    public virtual async Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Logger?.Trace(ToolStarting,MyTypeName,MyID); cancel ??= CancellationToken.None; Boolean ls = false; Stopwatch? w = default;

        try
        {
            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            if((LifeState.StartingOK() is false) || (this.AccessCheck(key) is false)) { return; }

            TimeSpan r; this.LifeState.ToStarting();

            IList<IHostedService>? h = this.GetHostedServices(SelfKey);

            if(h is not null && h.Count > 0)
            {
                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(async s =>
                    {
                        try
                        {
                            if(s is ITool t)
                            {
                                await t.StartingAsync(cancel.Value,HostingKeys![t]).ConfigureAwait(false);
                            }
                            else { await s.StartingAsync(cancel.Value).ConfigureAwait(false); }
                        }
                        catch ( ObjectDisposedException ) { }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h.OfType<IHostedLifecycleService>())
                    {
                        r = (HostingOptions?.StartupTimeout ?? InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t)
                        {
                            try { await t.StartingAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { } 

                            s = _; continue;
                        }

                        try { await _.StartingAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { }

                        s = _;
                    }
                }
            }
        }
        catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StartAsync(CancellationToken cancel = default) { return this.StartAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StartAsync)]
    public virtual async Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Logger?.Trace(ToolStartEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; Boolean ls = false; Stopwatch? w = default;

        try
        {
            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            if((LifeState.StartOK() is false) || (this.AccessCheck(key) is false)) { return; }

            TimeSpan r; this.LifeState.ToStarting();

            IList<IHostedService>? h = this.GetHostedServices(SelfKey);

            if(h is not null && h.Count > 0)
            {
                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(async s =>
                    {
                        try
                        {
                            if(s is ITool t)
                            {
                                await t.StartAsync(cancel.Value,HostingKeys![t]).ConfigureAwait(false);
                            }
                            else { await s.StartAsync(cancel.Value).ConfigureAwait(false); }
                        }
                        catch ( ObjectDisposedException ) { }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedService? s = default;

                    foreach(IHostedService _ in h)
                    {
                        r = (HostingOptions?.StartupTimeout ?? InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t)
                        {
                            try { await t.StartAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { }

                            s = _; continue;
                        }

                        try { await _.StartAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { }

                        s = _;
                    }
                }
            }
            await this.Activate_NoSync(cancel.Value,this.SelfKey).ConfigureAwait(false); Logger?.Trace(ToolStartExit,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StartedAsync(CancellationToken cancel = default) { return this.StartedAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartedAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StartedAsync)]
    public virtual async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; Stopwatch? w = default;

        try
        {
            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            if((LifeState.StartedOK() is false) || (this.AccessCheck(key) is false)) { return; }

            IList<IHostedService>? h = this.GetHostedServices(SelfKey);

            if(h is not null && h.Count > 0)
            {
                TimeSpan r;

                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(async s =>
                    {
                        try
                        {
                            if(s is ITool t)
                            {
                                await t.StartedAsync(cancel.Value,HostingKeys![t]).ConfigureAwait(false);
                            }
                            else { await s.StartedAsync(cancel.Value).ConfigureAwait(false); }
                        }
                        catch ( ObjectDisposedException ) { }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h.OfType<IHostedLifecycleService>())
                    {
                        r = (HostingOptions?.StartupTimeout ?? InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t)
                        {
                            try { await t.StartedAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { }

                            s = _; continue;
                        }

                        try { await _.StartedAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { }

                        s = _;
                    }
                }
            }
            Logger?.Trace(ToolStarted,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } w?.Stop(); }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.StartHostAsync)]
    public virtual async Task<Boolean> StartHostAsync(CancellationToken? cancel = null , TimeSpan? timeout = null , AccessKey? key = null)
    {
        DC();
        try
        {
            if((LifeState.StartHostOK() is false) || (this.AccessCheck(key) is false)) { return false; }

            cancel ??= new(); CancellationTokenSource _ = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value); if(timeout.HasValue) { _.CancelAfter(timeout.Value); }

            await this.StartingAsync(_.Token,key).ConfigureAwait(false); await this.StartAsync(_.Token,key).ConfigureAwait(false); await this.StartedAsync(_.Token,key).ConfigureAwait(false);

            return Equals(this.LifeState.State,Active);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartHostingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { this.LifeState.ToError(); return false; } throw; }
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
        DC();
        try
        {
            if((LifeState.StopHostOK() is false) || (this.AccessCheck(key) is false)) { return false; }

            cancel ??= new(); CancellationTokenSource _ = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value); if(timeout.HasValue) { _.CancelAfter(timeout.Value); }

            await this.StoppingAsync(_.Token,key).ConfigureAwait(false); await this.StopAsync(_.Token,key).ConfigureAwait(false); await this.StoppedAsync(_.Token,key).ConfigureAwait(false);

            return Equals(this.LifeState.State,InActive);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopHostingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { this.LifeState.ToError(); return false; } throw; }
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
        DC(); Logger?.Trace(ToolStopping,MyTypeName,MyID); cancel ??= CancellationToken.None; Boolean ls = false; Stopwatch? w = default;

        try
        {
            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            if((LifeState.StoppingOK() is false) || (this.AccessCheck(key) is false)) { return; }

            TimeSpan r; this.LifeState.ToStopping();

            IList<IHostedService>? h = this.GetHostedServices(SelfKey);

            if(h is not null && h.Count > 0)
            {
                if(HostingOptions?.ServicesStopConcurrently?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(async s =>
                    {
                        try
                        {
                            if(s is ITool t)
                            {
                                await t.StoppingAsync(cancel.Value,HostingKeys![t]).ConfigureAwait(false);
                            }
                            else { await s.StoppingAsync(cancel.Value).ConfigureAwait(false); }
                        }
                        catch ( ObjectDisposedException ) { }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h.OfType<IHostedLifecycleService>())
                    {
                        r = (HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t)
                        {
                            try { await t.StoppingAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { }

                            s = _; continue;
                        }

                        try { await _.StoppingAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { }

                        s = _;
                    }
                }
            }
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancel = default) { return this.StopAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StopAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StopAsync)]
    public virtual async Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Logger?.Trace(ToolStopEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; Boolean ls = false; Stopwatch? w = default;

        try
        {
            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            if((LifeState.StopOK() is false) || (this.AccessCheck(key) is false)) { return; }

            TimeSpan r; this.LifeState.ToStopping();

            IList<IHostedService>? h = this.GetHostedServices(SelfKey);

            if(h is not null && h.Count > 0)
            {
                if(HostingOptions?.ServicesStopConcurrently ?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(async s =>
                    {
                        try
                        {
                            if(s is ITool t)
                            {
                                await t.StopAsync(cancel.Value,HostingKeys![t]).ConfigureAwait(false);
                            }
                            else { await s.StopAsync(cancel.Value).ConfigureAwait(false); }
                        }
                        catch ( ObjectDisposedException ) { }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedService? s = default;

                    foreach(IHostedService _ in h)
                    {
                        r = (HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t)
                        {
                            try { await t.StopAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { }

                            s = _; continue;
                        }

                        try { await _.StopAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { }

                        s = _;
                    }
                }
            }
            await this.Deactivate_NoSync(cancel.Value,this.SelfKey).ConfigureAwait(false); Logger?.Trace(ToolStopExit,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } w?.Stop(); }
    }

    ///<inheritdoc/>
    public virtual Task StoppedAsync(CancellationToken cancel = default) { return this.StoppedAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StoppedAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StoppedAsync)]
    public virtual async Task StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; Stopwatch? w = default;

        try
        {
            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { throw SyncException; } ls = true;

            if((LifeState.StoppedOK() is false) || (this.AccessCheck(key) is false)) { return; }

            IList<IHostedService>? h = this.GetHostedServices(SelfKey);

            if(h is not null && h.Count > 0)
            {
                TimeSpan r;

                if(HostingOptions?.ServicesStopConcurrently ?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(async s =>
                    {
                        try
                        {
                            if(s is ITool t)
                            {
                                await t.StoppedAsync(cancel.Value,HostingKeys![t]).ConfigureAwait(false);
                            }
                            else { await s.StoppedAsync(cancel.Value).ConfigureAwait(false); }
                        }
                        catch ( ObjectDisposedException ) { }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h.OfType<IHostedLifecycleService>())
                    {
                        r = (HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t)
                        {
                            try { await t.StoppedAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { }

                            s = _; continue;
                        }

                        try { await _.StoppedAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); } catch ( ObjectDisposedException ) { }

                        s = _;
                    }
                }
            }
            Logger?.Trace(ToolStopped,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } w?.Stop(); }
    }
}