namespace KusDepot.Builders;

/**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/main/*'/>*/
public partial class ToolBuilder : IToolBuilder
{
    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="BuilderDisposed"]/*'/>*/
    private Int32 BuilderDisposed;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="BuildToolInstance"]/*'/>*/
    private ITool? BuildToolInstance;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="BuildHostInstance"]/*'/>*/
    protected IHost? BuildHostInstance;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="BuildServiceProviderInstance"]/*'/>*/
    private ToolServiceProvider? BuildServiceProviderInstance;

    ///<inheritdoc/>
    public virtual void Dispose()
    {
        Boolean sb = false;
        try
        {
            if(!this.Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            if(Interlocked.Exchange(ref this.BuilderDisposed,1) is not 0) { return; }

            DisposeBuildResources(); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderDisposeFail); throw; }

        finally { if(sb) { this.Sync.Release(); } }
    }

    ///<inheritdoc/>
    public virtual async ValueTask DisposeAsync()
    {
        Boolean sb = false;
        try
        {
            if(!await this.Sync.WaitAsync(SyncTime).ConfigureAwait(false)) { throw SyncException; } sb = true;

            if(Interlocked.Exchange(ref this.BuilderDisposed,1) is not 0) { return; }

            await DisposeBuildResourcesAsync().ConfigureAwait(false); GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderDisposeAsyncFail); throw; }

        finally { if(sb) { this.Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="ClearBuildResources"]/*'/>*/
    protected void ClearBuildResources()
    {
        this.BuildToolInstance = null; this.BuildHostInstance = null; this.BuildServiceProviderInstance = null;
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="DisposeBuildResources"]/*'/>*/
    protected void DisposeBuildResources()
    {
        try
        {
            this.BuildToolInstance?.Dispose();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); }

        try
        {
            if(this.BuildToolInstance is null) { this.BuildHostInstance?.Dispose(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); }

        try
        {
            this.BuildServiceProviderInstance?.Dispose();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); }

        ClearBuildResources();
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="DisposeBuildResourcesAsync"]/*'/>*/
    protected async ValueTask DisposeBuildResourcesAsync()
    {
        try
        {
            if(this.BuildToolInstance is IAsyncDisposable d) { await d.DisposeAsync().ConfigureAwait(false); }

            else { this.BuildToolInstance?.Dispose(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); }

        try
        {
            if(this.BuildToolInstance is null)
            {
                if(this.BuildHostInstance is IAsyncDisposable d) { await d.DisposeAsync().ConfigureAwait(false); }

                else { this.BuildHostInstance?.Dispose(); }
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); }

        try
        {
            if(this.BuildServiceProviderInstance is not null) { await this.BuildServiceProviderInstance.DisposeAsync().ConfigureAwait(false); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); }

        ClearBuildResources();
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="SetBuildTool"]/*'/>*/
    protected void SetBuildTool(ITool tool)
    {
        this.BuildToolInstance = tool; this.BuildServiceProviderInstance = null;
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="SetBuildHost"]/*'/>*/
    protected void SetBuildHost(IHost? host) => this.BuildHostInstance = host;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="DC"]/*'/>*/
    protected void DC()
    {
        ObjectDisposedException.ThrowIf(Interlocked.CompareExchange(ref this.BuilderDisposed,0,0) is not 0,this);
    }

}