namespace KusDepot;

public partial class Tool : Common , IHostedLifecycleService , ITool
{
    ///<inheritdoc/>
    public virtual Task StartingAsync(CancellationToken cancel = default) { return this.StartingAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartingAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StartingAsync)]
    public virtual async Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Logger?.Verbose(ToolStarting,MyTypeName,MyID); cancel ??= CancellationToken.None; Boolean ls = false; try { await this.Sync.Life.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

        catch ( Exception _ ) { Logger?.Error(_,StartingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; } Stopwatch? w = default;

        if((LifeState.StartingOK() is false) || (this.AccessCheck(key) is false)) { if(ls) { this.Sync.Life.Release(); } return; }

        try
        {
            TimeSpan r; this.LifeState.ToStarting();

            IList<IHostedService>? h = this.GetHostedServices(SelfAccessKey);

            if(h is not null && h.Count > 0)
            {
                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(s =>
                    {
                        if(s is ITool t)
                        {
                            return t.StartingAsync(cancel.Value,HostingKeys![t]);
                        }
                        else { return s.StartingAsync(cancel.Value); }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? Timeout.InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h.OfType<IHostedLifecycleService>())
                    {
                        r = (HostingOptions?.StartupTimeout ?? Timeout.InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t) { await t.StartingAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _; continue; }

                        await _.StartingAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _;
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
        DC(); Logger?.Verbose(ToolStartEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; Boolean ls = false; try { await this.Sync.Life.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

        catch ( Exception _ ) { Logger?.Error(_,StartAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; } Stopwatch? w = default;

        if((LifeState.StartOK() is false) || (this.AccessCheck(key) is false)) { if(ls) { this.Sync.Life.Release(); } return; }

        try
        {
            TimeSpan r; this.LifeState.ToStarting();

            IList<IHostedService>? h = this.GetHostedServices(SelfAccessKey);

            if(h is not null && h.Count > 0)
            {
                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(s =>
                    {
                        if(s is ITool t)
                        {
                            return t.StartAsync(cancel.Value,HostingKeys![t]);
                        }
                        else { return s.StartAsync(cancel.Value); }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? Timeout.InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedService? s = default;

                    foreach(IHostedService _ in h)
                    {
                        r = (HostingOptions?.StartupTimeout ?? Timeout.InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t) { await t.StartAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _; continue; }

                        await _.StartAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _;
                    }
                }
            }
            await this.Activate(cancel.Value,this.SelfAccessKey).ConfigureAwait(false); Logger?.Verbose(ToolStartExit,MyTypeName,MyID);
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
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; try { await this.Sync.Life.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

        catch ( Exception _ ) { Logger?.Error(_,StartedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; } Stopwatch? w = default;

        if((LifeState.StartedOK() is false) || (this.AccessCheck(key) is false)) { if(ls) { this.Sync.Life.Release(); } return; }

        try
        {
            IList<IHostedService>? h = this.GetHostedServices(SelfAccessKey);

            if(h is not null && h.Count > 0)
            {
                TimeSpan r;

                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(s =>
                    {
                        if(s is ITool t)
                        {
                            return t.StartedAsync(cancel.Value,HostingKeys![t]);
                        }
                        else { return s.StartedAsync(cancel.Value); }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? Timeout.InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h.OfType<IHostedLifecycleService>())
                    {
                        r = (HostingOptions?.StartupTimeout ?? Timeout.InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t) { await t.StartedAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _; continue; }

                        await _.StartedAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _;
                    }
                }
            }
            Logger?.Verbose(ToolStarted,MyTypeName,MyID);
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

    ///<inheritdoc/>
    public virtual Task StoppingAsync(CancellationToken cancel = default) { return this.StoppingAsync(cancel,null); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StoppingAsync"]/*'/>*/
    [AccessCheck(ProtectedOperation.StoppingAsync)]
    public virtual async Task StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Logger?.Verbose(ToolStopping,MyTypeName,MyID); cancel ??= CancellationToken.None; Boolean ls = false; try { await this.Sync.Life.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

        catch ( Exception _ ) { Logger?.Error(_,StoppingAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; } Stopwatch? w = default;

        if((LifeState.StoppingOK() is false) || (this.AccessCheck(key) is false)) { if(ls) { this.Sync.Life.Release(); } return; }

        try
        {
            TimeSpan r; this.LifeState.ToStopping();

            IList<IHostedService>? h = this.GetHostedServices(SelfAccessKey);

            if(h is not null && h.Count > 0)
            {
                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(s =>
                    {
                        if(s is ITool t)
                        {
                            return t.StoppingAsync(cancel.Value,HostingKeys![t]);
                        }
                        else { return s.StoppingAsync(cancel.Value); }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? Timeout.InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h.OfType<IHostedLifecycleService>())
                    {
                        r = (HostingOptions?.ShutdownTimeout ?? Timeout.InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t) { await t.StoppingAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _; continue; }

                        await _.StoppingAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _;
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
        DC(); Logger?.Verbose(ToolStopEnter,MyTypeName,MyID); cancel ??= CancellationToken.None; Boolean ls = false; try { await this.Sync.Life.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

        catch ( Exception _ ) { Logger?.Error(_,StopAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; } Stopwatch? w = default;

        if((LifeState.StopOK() is false) || (this.AccessCheck(key) is false)) { if(ls) { this.Sync.Life.Release(); } return; }

        try
        {
            TimeSpan r; this.LifeState.ToStopping();

            IList<IHostedService>? h = this.GetHostedServices(SelfAccessKey);

            if(h is not null && h.Count > 0)
            {
                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(s =>
                    {
                        if(s is ITool t)
                        {
                            return t.StopAsync(cancel.Value,HostingKeys![t]);
                        }
                        else { return s.StopAsync(cancel.Value); }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? Timeout.InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedService? s = default;

                    foreach(IHostedService _ in h)
                    {
                        r = (HostingOptions?.ShutdownTimeout ?? Timeout.InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t) { await t.StopAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _; continue; }

                        await _.StopAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _;
                    }
                }
            }
            await this.Deactivate(cancel.Value,this.SelfAccessKey).ConfigureAwait(false); Logger?.Verbose(ToolStopExit,MyTypeName,MyID);
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
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; try { await this.Sync.Life.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

        catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return; } throw; } Stopwatch? w = default;

        if((LifeState.StoppedOK() is false) || (this.AccessCheck(key) is false)) { if(ls) { this.Sync.Life.Release(); } return; }

        try
        {
            IList<IHostedService>? h = this.GetHostedServices(SelfAccessKey);

            if(h is not null && h.Count > 0)
            {
                TimeSpan r;

                if(HostingOptions?.ServicesStartConcurrently ?? true)
                {
                    Task[] l = h!.OfType<IHostedLifecycleService>().Select(s =>
                    {
                        if(s is ITool t)
                        {
                            return t.StoppedAsync(cancel.Value,HostingKeys![t]);
                        }
                        else { return s.StoppedAsync(cancel.Value); }

                    }).ToArray();

                    await Task.WhenAll(l).WaitAsync(HostingOptions?.StartupTimeout ?? Timeout.InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
                }
                else
                {
                    if(HostingOptions is not null) { w = new(); w.Start(); } IHostedLifecycleService? s = default;

                    foreach(IHostedLifecycleService _ in h.OfType<IHostedLifecycleService>())
                    {
                        r = (HostingOptions?.ShutdownTimeout ?? Timeout.InfiniteTimeSpan) - (w?.Elapsed ?? TimeSpan.Zero);

                        if(r <= TimeSpan.Zero)
                        {
                            switch(s)
                            {
                                case not null: throw new TimeoutException(s.GetType().ToString());

                                default: throw new TimeoutException();
                            }
                        }

                        if(_ is ITool t) { await t.StoppedAsync(cancel.Value,HostingKeys![t]).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _; continue; }

                        await _.StoppedAsync(cancel.Value).WaitAsync(r,cancel.Value).ConfigureAwait(false); s = _;
                    }
                }
            }
            Logger?.Verbose(ToolStopped,MyTypeName,MyID);
        }
        catch ( Exception _ ) { Logger?.Error(_,StoppedAsyncFail,MyTypeName,MyID); this.LifeState.ToError(); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } w?.Stop(); }
    }    
}
