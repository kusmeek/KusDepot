namespace KusDepot;

public partial class Tool : Common , IHostedLifecycleService , ITool
{
    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DisposedCheck"]/*'/>*/
    protected void DC() => ObjectDisposedException.ThrowIf(this.Disposed,this);

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.Dispose)]
    public virtual void Dispose(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            this.DisposeCore(true); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds) { this.Sync?.Disposed.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Dispose_NoSync_AccessKey"]/*'/>*/
    [AccessCheck(ProtectedOperation.Dispose)]
    protected virtual void Dispose_NoSync(AccessKey? key = null)
    {
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            this.DisposeCore(true); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public virtual void Dispose()
    {
        Boolean ds = false;
        try
        {
            if(this.Locked || this.Disposed) { return; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            this.DisposeCore(true); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds) { this.Sync?.Disposed.Release(); } }
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

        Logger?.Trace(ToolDisposing,MyTypeName,MyID);

        RemoveInstance(this);

        try
        {
            if(disposing)
            {
                this.StopHostAsync(key:SelfKey).GetAwaiter().GetResult();

                this.HostingKeys?.ToList().ForEach(_ => { _.Key.Dispose(_.Value); });

                if(this.ToolServiceScope.HasValue) { this.ToolServiceScope.Value.Dispose(); this.ToolServiceScope = null; }

                this.DestroySecrets(this.SelfManagerKey); this.ClearReferences();
            }
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { this.Sync = null!; this.Disposed = true; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.Dispose)]
    public virtual async ValueTask DisposeAsync(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            await this.DisposeAsyncCore(true).ConfigureAwait(false); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds) { this.Sync?.Disposed.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DisposeAsync_NoSync_AccessKey"]/*'/>*/
    [AccessCheck(ProtectedOperation.Dispose)]
    protected virtual async ValueTask DisposeAsync_NoSync(AccessKey? key = null)
    {
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            await this.DisposeAsyncCore(true).ConfigureAwait(false); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public virtual async ValueTask DisposeAsync()
    {
        Boolean ds = false;
        try
        {
            if(this.Locked || this.Disposed) { return; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            await this.DisposeAsyncCore(true).ConfigureAwait(false); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds) { this.Sync?.Disposed.Release(); } }
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

        Logger?.Trace(ToolDisposing,MyTypeName,MyID);

        RemoveInstance(this);

        try
        {
            if(disposing)
            {
                await this.StopHostAsync(timeout:HostingOptions?.ShutdownTimeout,key:SelfKey).ConfigureAwait(false);

                if(this.HostingKeys is not null)
                {
                    var t = this.HostingKeys.Select(_ => _.Key.DisposeAsync(_.Value).AsTask());

                    await Task.WhenAll(t).ConfigureAwait(false);
                }

                if(this.ToolServiceScope.HasValue) { await this.ToolServiceScope.Value.DisposeAsync().ConfigureAwait(false); this.ToolServiceScope = null; }

                this.DestroySecrets(this.SelfManagerKey); this.ClearReferences();
            }
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncCoreFail,MyTypeName,MyID); throw; }

        finally { this.Sync = null!; this.Disposed = true; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="ClearReferences"]/*'/>*/
    protected virtual void ClearReferences()
    {
        this.AccessManager          = null;
        this.Activities             = null;
        this.AttachedCommands       = null;
        this.CommandHandles         = null;
        this.CommandKeys            = null;
        this.CommandManagerKeys     = null;
        this.Commands               = null;
        this.Configuration          = null;
        this.Data                   = null;
        this.HostedServiceLockState = null;
        this.HostedServices         = null;
        this.HostingKeys            = null;
        this.HostingMyHostKeys      = null;
        this.HostingManagerKeys     = null;
        this.HostingOptions         = null;
        this.Inputs                 = null;
        this.LifeState              = null!;
        this.Logger                 = null;
        this.MyHostKey              = null;
        this.Outputs                = null;
        this.OwnerSecret            = null;
        this.SelfKey                = null;
        this.SelfManagerKey         = null;
        this.Status                 = null;
        this.ToolServiceProvider    = null;
        this.ToolServices           = null;
        this.ToolServiceScope       = null;
        this.WorkingSet             = null;
        this.Secrets                = null;
    }
}