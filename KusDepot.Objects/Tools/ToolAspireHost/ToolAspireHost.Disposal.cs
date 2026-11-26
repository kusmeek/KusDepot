namespace KusDepot;

/**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/main/*'/>*/
public partial class ToolAspireHost : ToolHost , IToolAspireHost
{
    ///<inheritdoc/>
    protected override void DisposeHostResources(List<Exception> errors)
    {
        try { this.Logger?.Verbose(ToolAspireHostDisposing,MyTypeName,MyID); } catch {}

        try { this.AspireApplication?.Dispose(); }

        catch( Exception e ) { errors.Add(e); try { this.Logger?.Error(e,DisposeFail,MyTypeName,MyID); } catch {} }

        finally { this.AspireApplication = null; }
    }

    ///<inheritdoc/>
    protected override async ValueTask DisposeHostResourcesAsync(List<Exception> errors)
    {
        try { this.Logger?.Verbose(ToolAspireHostDisposing,MyTypeName,MyID); } catch {}

        try
        {
            if(this.AspireApplication is IAsyncDisposable disposable) { await disposable.DisposeAsync().ConfigureAwait(false); }

            else { this.AspireApplication?.Dispose(); }
        }
        catch( Exception e ) { errors.Add(e); try { this.Logger?.Error(e,DisposeAsyncFail,MyTypeName,MyID); } catch {} }

        finally { this.AspireApplication = null; }
    }
}
