namespace KusDepot;

/**<include file='Tool.xml' path='Tool/class[@name="Tool"]/main/*'/>*/
public partial class Tool : Common , IHostedLifecycleService , ITool
{
    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/property[@name="Disposed"]/*'/>*/
    protected Boolean Disposed => this.GetDisposed();

    ///<inheritdoc/>
    public ToolDisposalState GetDisposalState()
    {
        try { return Interlocked.CompareExchange(ref this.DisposalState,ToolDisposalState.Active,ToolDisposalState.Active); }

        catch ( Exception _ ) { Logger?.Error(_,GetDisposalStateFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public Boolean GetDisposed()
    {
        try { return Interlocked.CompareExchange(ref this.DisposalState,ToolDisposalState.Active,ToolDisposalState.Active) == ToolDisposalState.Disposed; }

        catch ( Exception _ ) { Logger?.Error(_,GetDisposedFail,MyTypeName,MyID); throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="TryBeginDisposal"]/*'/>*/
    protected Boolean TryBeginDisposal() => Interlocked.CompareExchange(ref this.DisposalState,ToolDisposalState.Disposing,ToolDisposalState.Active) == ToolDisposalState.Active;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DisposedCheck"]/*'/>*/
    protected void DC() => ObjectDisposedException.ThrowIf(Interlocked.CompareExchange(ref this.DisposalState,ToolDisposalState.Active,ToolDisposalState.Active) is not ToolDisposalState.Active,this);

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="IsDisposalStateActive_NoSync"]/*'/>*/
    protected Boolean IsDisposalStateActive_NoSync() => Interlocked.CompareExchange(ref this.DisposalState,ToolDisposalState.Active,ToolDisposalState.Active) is ToolDisposalState.Active;

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.Dispose)]
    public virtual void Dispose(AccessKey? key = null)
    {
        Boolean ls = false; Boolean ds = false;
        try
        {
            if( this.Disposed || this.AccessCheck(key) is false ) { return; }

            if(!this.Sync.Life.Wait(SyncTime)) { throw SyncException; } else { ls = true; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            this.DisposeCore(true); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds) { this.Sync.Disposed.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Dispose_NoSync_AccessKey"]/*'/>*/
    [AccessCheck(ProtectedOperation.Dispose)]
    protected virtual void Dispose_NoSync(AccessKey? key = null)
    {
        try
        {
            if( this.Disposed || this.AccessCheck(key) is false ) { return; }

            this.DisposeCore(true); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public virtual void Dispose()
    {
        Boolean ls = false; Boolean ds = false;
        try
        {
            if(this.Locked || this.Disposed) { return; }

            if(!this.Sync.Life.Wait(SyncTime)) { throw SyncException; } else { ls = true; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            this.DisposeCore(true); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds) { this.Sync.Disposed.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Dispose_NoSync"]/*'/>*/
    protected virtual void Dispose_NoSync()
    {
        try
        {
            if(this.Locked || this.Disposed) { return; }

            this.DisposeCore(true); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DisposeCore"]/*'/>*/
    protected virtual void DisposeCore(Boolean disposing)
    {
        if(this.Disposed) { return; }

        if(this.TryBeginDisposal() is false && Interlocked.CompareExchange(ref this.DisposalState,ToolDisposalState.Active,ToolDisposalState.Active) != ToolDisposalState.Disposing) { return; }

        ILogger? logger = this.Logger;

        try
        {
            this.ClearOutputReferences();

            logger?.Trace(ToolDisposing,MyTypeName,MyID); RemoveInstance(this);

            List<Exception> errors = new();

            if(disposing)
            {
                try
                {
                    if(this.HostingKeys is not null)
                    {
                        foreach(var _ in this.HostingKeys.ToList())
                        {
                            try { _.Key.Dispose(_.Value); }  catch ( Exception e ) { errors.Add(e); }
                        }
                    }
                }
                catch ( Exception e ) { errors.Add(e); }

                try { this.ToolServiceScopes?.Dispose(); }   catch ( Exception e ) { errors.Add(e); }

                try { this.ToolServiceProvider?.Dispose(); } catch ( Exception e ) { errors.Add(e); }

                try { this.DestroySecrets(); }               catch ( Exception e ) { errors.Add(e); }

                try { this.ClearReferences(); }              catch ( Exception e ) { errors.Add(e); }

                if(errors.Count != 0)
                {
                    AggregateException aggregate = new(errors);

                    try { logger?.Error(aggregate,DisposeFail,MyTypeName,MyID); } catch {}

                    throw aggregate;
                }
            }
        }
        finally { Interlocked.Exchange(ref this.DisposalState,ToolDisposalState.Disposed); }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.Dispose)]
    public virtual async ValueTask DisposeAsync(AccessKey? key = null)
    {
        Boolean ls = false; Boolean ds = false;
        try
        {
            if( this.Disposed || await this.AccessCheckAsync(key).ConfigureAwait(false) is false ) { return; }

            if(!await this.Sync.Life.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ls = true; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            await this.DisposeAsyncCore(true).ConfigureAwait(false); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds) { this.Sync.Disposed.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DisposeAsync_NoSync_AccessKey"]/*'/>*/
    [AccessCheck(ProtectedOperation.Dispose)]
    protected virtual async ValueTask DisposeAsync_NoSync(AccessKey? key = null)
    {
        try
        {
            if( this.Disposed || await this.AccessCheckAsync(key).ConfigureAwait(false) is false ) { return; }

            await this.DisposeAsyncCore(true).ConfigureAwait(false); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public virtual async ValueTask DisposeAsync()
    {
        Boolean ls = false; Boolean ds = false;
        try
        {
            if(this.Locked || this.Disposed) { return; }

            if(!await this.Sync.Life.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ls = true; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            await this.DisposeAsyncCore(true).ConfigureAwait(false); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds) { this.Sync.Disposed.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DisposeAsync_NoSync"]/*'/>*/
    protected virtual async ValueTask DisposeAsync_NoSync()
    {
        try
        {
            if(this.Locked || this.Disposed) { return; }

            await this.DisposeAsyncCore(true).ConfigureAwait(false); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DisposeAsyncCore"]/*'/>*/
    protected virtual async ValueTask DisposeAsyncCore(Boolean disposing)
    {
        if(this.Disposed) { return; }

        if(this.TryBeginDisposal() is false && Interlocked.CompareExchange(ref this.DisposalState,ToolDisposalState.Active,ToolDisposalState.Active) != ToolDisposalState.Disposing) { return; }

        ILogger? logger = this.Logger;

        try
        {
            this.ClearOutputReferences();

            logger?.Trace(ToolDisposing,MyTypeName,MyID); RemoveInstance(this);

            List<Exception> errors = new();

            if(disposing)
            {
                try
                {
                    if(this.HostingKeys is not null)
                    {
                        foreach(var _ in this.HostingKeys.ToList())
                        {
                            try { await _.Key.DisposeAsync(_.Value).ConfigureAwait(false); } catch ( Exception e ) { errors.Add(e); }
                        }
                    }
                }
                catch ( Exception e ) { errors.Add(e); }

                try { if(this.ToolServiceScopes is not null) { await this.ToolServiceScopes.DisposeAsync().ConfigureAwait(false); } } catch ( Exception e ) { errors.Add(e); }

                try { if(this.ToolServiceProvider is not null) { await this.ToolServiceProvider.DisposeAsync().ConfigureAwait(false); } } catch ( Exception e ) { errors.Add(e); }

                try { this.DestroySecrets(); }  catch ( Exception e ) { errors.Add(e); }

                try { this.ClearReferences(); } catch ( Exception e ) { errors.Add(e); }

                if(errors.Count != 0)
                {
                    AggregateException aggregate = new(errors);

                    try { logger?.Error(aggregate,DisposeAsyncCoreFail,MyTypeName,MyID); } catch {}

                    throw aggregate;
                }
            }
        }
        finally { Interlocked.Exchange(ref this.DisposalState,ToolDisposalState.Disposed); }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DisposeToolServices"]/*'/>*/
    private void DisposeToolServices()
    {
        try { this.ToolServiceScopes?.Dispose(); } catch(Exception _) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); }

        try { this.ToolServiceProvider?.Dispose(); } catch(Exception _) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="ClearReferences"]/*'/>*/
    protected virtual void ClearReferences()
    {
        this.AccessManager                = null;
        this.Activities                   = null;
        this.AttachedCommands             = null;
        this.CommandHandles               = null;
        this.CommandKeys                  = null;
        this.CommandManagerKeys           = null;
        this.Commands                     = null;
        this.Configuration                = null;
        this.Data                         = null;
        this.DataIndex                    = null;
        this.HostedServiceLockState       = null;
        this.HostedServiceNamesByInstance = null;
        this.HostedServices               = null;
        this.HostingKeys                  = null;
        this.HostingMyHostKeys            = null;
        this.HostingManagerKeys           = null;
        this.HostingOptions               = null;
        this.Inputs                       = null;
        this.LifeState                    = null!;
        this.Logger                       = null;
        this.MyHostKey                    = null;
        this.OwnerSecret                  = null;
        this.Secrets                      = null;
        this.SelfKey                      = null;
        this.Status                       = null;
        this.ToolServiceProvider          = null;
        this.ToolServices                 = null;
        this.ToolServiceScopes            = null;
        this.WorkingSet                   = null;
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="ClearOutputReferences"]/*'/>*/
    protected void ClearOutputReferences()
    {
        this.Sync.Outputs.Wait();

        List<TaskCompletionSource<Object?>>? waiters = null;

        try
        {
            this.Outputs = null;

            if(this.OutputWaiters is not null) { waiters = this.OutputWaiters.Values.SelectMany(static pending => pending).ToList(); }

            this.OutputWaiters = null;
        }
        finally { this.Sync.Outputs.Release(); }

        if(waiters is not null)
        {
            foreach(TaskCompletionSource<Object?> waiter in waiters) { waiter.TrySetResult(null); }
        }
    }
}