namespace KusDepot;

/**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/main/*'/>*/
public partial class ToolGenericHost : ToolHost , IToolGenericHost
{
    ///<inheritdoc/>
    public override void Dispose(AccessKey? key = null)
    {
        Boolean ds = false;
        try
        {
            if( this.Disposed || (this.AccessCheck(key) is false) ) { return; }

            if(!this.Sync.Disposed.Wait(SyncTime)) { throw SyncException; } else { ds = true; }

            Logger?.Verbose(ToolGenericHostDisposing,MyTypeName,MyID);

            if(Host is IToolHost t) { t.Dispose(HostKey); } else { this.Host?.Dispose(); }

            Lifetime!.Dispose(); base.Dispose_NoSync(key);
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

            Logger?.Verbose(ToolGenericHostDisposing,MyTypeName,MyID);

            if(Host is IToolHost t) { t.Dispose(HostKey); } else { this.Host?.Dispose(); }

            Lifetime!.Dispose(); base.Dispose_NoSync();
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

            Logger?.Verbose(ToolGenericHostDisposing,MyTypeName,MyID);

            if(Host is IAsyncDisposable d)
            {
                if(Host is IToolHost t) { await t.DisposeAsync(HostKey).ConfigureAwait(false); }

                else { await d.DisposeAsync().ConfigureAwait(false); }
            }
            else { Host?.Dispose(); }

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync(key).ConfigureAwait(false);
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

            Logger?.Verbose(ToolGenericHostDisposing,MyTypeName,MyID);

            if(Host is IAsyncDisposable d)
            {
                if(Host is IToolHost t) { await t.DisposeAsync(HostKey).ConfigureAwait(false); }

                else { await d.DisposeAsync().ConfigureAwait(false); }
            }
            else { Host?.Dispose(); }

            Lifetime!.Dispose(); await base.DisposeAsync_NoSync().ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeAsyncFail,MyTypeName,MyID); throw; }

        finally { if(ds && this.Sync is not null) { this.Sync.Disposed.Release(); } }
    }
}