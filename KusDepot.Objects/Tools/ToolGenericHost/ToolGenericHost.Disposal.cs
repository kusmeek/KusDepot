namespace KusDepot;

/**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/main/*'/>*/
public partial class ToolGenericHost : ToolHost , IToolGenericHost
{
    ///<inheritdoc/>
    protected override void DisposeHostResources(List<Exception> errors)
    {
        try { this.Logger?.Verbose(ToolGenericHostDisposing,MyTypeName,MyID); } catch {}

        try
        {
            if(this.Host is IToolHost toolhost) { toolhost.Dispose(this.HostKey); }

            else { this.Host?.Dispose(); }
        }
        catch( Exception e ) { errors.Add(e); try { this.Logger?.Error(e,DisposeFail,MyTypeName,MyID); } catch {} }

        finally { this.Host = null; }
    }

    ///<inheritdoc/>
    protected override async ValueTask DisposeHostResourcesAsync(List<Exception> errors)
    {
        try { this.Logger?.Verbose(ToolGenericHostDisposing,MyTypeName,MyID); } catch {}

        try
        {
            if(this.Host is IAsyncDisposable disposable)
            {
                if(this.Host is IToolHost toolhost) { await toolhost.DisposeAsync(this.HostKey).ConfigureAwait(false); }

                else { await disposable.DisposeAsync().ConfigureAwait(false); }
            }
            else { this.Host?.Dispose(); }
        }
        catch( Exception e ) { errors.Add(e); try { this.Logger?.Error(e,DisposeAsyncFail,MyTypeName,MyID); } catch {} }

        finally { this.Host = null; }
    }
}
