namespace KusDepot;

/**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/main/*'/>*/
public partial class ToolHost : Tool , IToolHost
{
    ///<inheritdoc/>
    public override void Dispose(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); base.Dispose_NoSync(key);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    protected override void Dispose_NoSync(AccessKey? key = null)
    {
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); base.Dispose_NoSync(key);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public override void Dispose()
    {
        Boolean ds = false;
        try
        {
            if(this.Disposed || this.Locked) { return; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); base.Dispose_NoSync();

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    protected override void Dispose_NoSync()
    {
        try
        {
            if(this.Disposed || this.Locked) { return; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); base.Dispose_NoSync();

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if(this.Disposed || await this.AccessCheckAsync(key).ConfigureAwait(false) is false) { return; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync(key).ConfigureAwait(false);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    protected override async ValueTask DisposeAsync_NoSync(AccessKey? key = null)
    {
        try
        {
            if(this.Disposed || await this.AccessCheckAsync(key).ConfigureAwait(false) is false) { return; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync(key).ConfigureAwait(false);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        Boolean ds = false;
        try
        {
            if(this.Disposed || this.Locked) { return; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync().ConfigureAwait(false);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    protected override async ValueTask DisposeAsync_NoSync()
    {
        try
        {
            if(this.Disposed || this.Locked) { return; }

            Logger?.Verbose(ToolHostDisposing,MyTypeName,MyID);

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync().ConfigureAwait(false);

            Lifetime = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }
    }
}