namespace KusDepot;

/**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/main/*'/>*/
public partial class ToolWebHost : ToolHost , IToolWebHost
{
    ///<inheritdoc/>
    protected override void DisposeHostResources(List<Exception> errors)
    {
        try { this.Logger?.Verbose(ToolWebHostDisposing,MyTypeName,MyID); } catch {}

        try { this.WebApplication?.Dispose(); }

        catch( Exception e ) { errors.Add(e); try { this.Logger?.Error(e,DisposeFail,MyTypeName,MyID); } catch {} }

        finally { this.Urls = null; this.WebApplication = null; }
    }

    ///<inheritdoc/>
    protected override async ValueTask DisposeHostResourcesAsync(List<Exception> errors)
    {
        try { this.Logger?.Verbose(ToolWebHostDisposing,MyTypeName,MyID); } catch {}

        try
        {
            if(this.WebApplication is IAsyncDisposable disposable) { await disposable.DisposeAsync().ConfigureAwait(false); }

            else { this.WebApplication?.Dispose(); }
        }
        catch( Exception e ) { errors.Add(e); try { this.Logger?.Error(e,DisposeAsyncFail,MyTypeName,MyID); } catch {} }

        finally { this.Urls = null; this.WebApplication = null; }
    }
}
