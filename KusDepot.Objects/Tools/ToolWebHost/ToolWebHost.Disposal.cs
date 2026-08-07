namespace KusDepot;

/**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/main/*'/>*/
public partial class ToolWebHost : ToolHost , IToolWebHost
{
    ///<inheritdoc/>
    public override void Dispose(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolWebHostDisposing,MyTypeName,MyID);

            WebApplication?.Dispose();

            Lifetime!.Dispose(); base.Dispose_NoSync(key);

            Urls = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    public override void Dispose()
    {
        Boolean ds = false;
        try
        {
            if(this.Disposed || this.Locked) { return; } 

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolWebHostDisposing,MyTypeName,MyID);

            WebApplication?.Dispose();

            Lifetime!.Dispose(); base.Dispose_NoSync();

            Urls = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if(this.Disposed || await this.AccessCheckAsync(key).ConfigureAwait(false) is false) { return; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolWebHostDisposing,MyTypeName,MyID);

            if(WebApplication is IAsyncDisposable d) { await d.DisposeAsync().ConfigureAwait(false); } else { WebApplication?.Dispose(); }

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync(key).ConfigureAwait(false);

            Urls = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        Boolean ds = false;
        try
        {
            if(this.Disposed || this.Locked) { return; }

            if(!await this.Sync.Disposed.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolWebHostDisposing,MyTypeName,MyID);

            if(WebApplication is IAsyncDisposable d) { await d.DisposeAsync().ConfigureAwait(false); } else { WebApplication?.Dispose(); }

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync().ConfigureAwait(false);

            Urls = null;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }
}