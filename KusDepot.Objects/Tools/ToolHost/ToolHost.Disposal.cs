namespace KusDepot;

/**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/main/*'/>*/
public partial class ToolHost : Tool , IToolHost
{
    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="DisposeCore"]/*'/>*/
    protected override void DisposeCore(Boolean disposing)
    {
        if(this.GetDisposed()) { return; }

        if(this.TryBeginDisposal() is false && this.GetDisposalState() != ToolDisposalState.Disposing) { return; }

        this.ClearOutputReferences();

        List<Exception> errors = new(); IToolHostLifetime? lifetime = this.Lifetime; this.Lifetime = null;

        try { this.DisposeHostResources(errors); } catch ( Exception e ) { errors.Add(e); }

        try { lifetime?.Dispose(this.LifeKey); }   catch ( Exception e ) { errors.Add(e); }

        try { base.DisposeCore(disposing); }       catch ( Exception e ) { errors.Add(e); }

        if(errors.Count != 0) { throw new AggregateException(errors); }
    }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="DisposeAsyncCore"]/*'/>*/
    protected override async ValueTask DisposeAsyncCore(Boolean disposing)
    {
        if(this.GetDisposed()) { return; }

        if(this.TryBeginDisposal() is false && this.GetDisposalState() != ToolDisposalState.Disposing) { return; }

        this.ClearOutputReferences();

        List<Exception> errors = new(); IToolHostLifetime? lifetime = this.Lifetime; this.Lifetime = null;

        try { await this.DisposeHostResourcesAsync(errors).ConfigureAwait(false); } catch ( Exception e ) { errors.Add(e); }

        try { lifetime?.Dispose(this.LifeKey); }                                    catch ( Exception e ) { errors.Add(e); }

        try { await base.DisposeAsyncCore(disposing).ConfigureAwait(false); }       catch ( Exception e ) { errors.Add(e); }

        if(errors.Count != 0) { throw new AggregateException(errors); }
    }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="DisposeHostResources"]/*'/>*/
    protected virtual void DisposeHostResources(List<Exception> errors) {}

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="DisposeHostResourcesAsync"]/*'/>*/
    protected virtual ValueTask DisposeHostResourcesAsync(List<Exception> errors)
    {
        this.DisposeHostResources(errors); return ValueTask.CompletedTask;
    }
}
