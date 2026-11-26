namespace KusDepot;

/**<include file='ToolServiceProvider.xml' path='ToolServiceProvider/class[@name="ToolServiceProvider"]/main/*'/>*/
public sealed class ToolServiceProvider : IDisposable , IAsyncDisposable
{
    /**<include file='ToolServiceProvider.xml' path='ToolServiceProvider/class[@name="ToolServiceProvider"]/field[@name="disposed"]/*'/>*/
    private Int32 disposed;

    /**<include file='ToolServiceProvider.xml' path='ToolServiceProvider/class[@name="ToolServiceProvider"]/property[@name="ServiceProvider"]/*'/>*/
    public IServiceProvider ServiceProvider { get; }

    /**<include file='ToolServiceProvider.xml' path='ToolServiceProvider/class[@name="ToolServiceProvider"]/property[@name="Ownership"]/*'/>*/
    public ToolServiceProviderOwnership Ownership { get; }

    /**<include file='ToolServiceProvider.xml' path='ToolServiceProvider/class[@name="ToolServiceProvider"]/constructor[@name="Constructor"]/*'/>*/
    public ToolServiceProvider(IServiceProvider serviceprovider , ToolServiceProviderOwnership ownership = ToolServiceProviderOwnership.Tool)
    {
        ServiceProvider = serviceprovider ?? throw new ArgumentNullException(nameof(serviceprovider)); Ownership = ownership;
    }

    /**<include file='ToolServiceProvider.xml' path='ToolServiceProvider/class[@name="ToolServiceProvider"]/method[@name="Dispose"]/*'/>*/
    public void Dispose()
    {
        if(Interlocked.Exchange(ref this.disposed,1) is not 0 || this.Ownership is not ToolServiceProviderOwnership.Tool) { return; }

        if(this.ServiceProvider is IDisposable d) { d.Dispose(); }

        GC.SuppressFinalize(this);
    }

    /**<include file='ToolServiceProvider.xml' path='ToolServiceProvider/class[@name="ToolServiceProvider"]/method[@name="DisposeAsync"]/*'/>*/
    public async ValueTask DisposeAsync()
    {
        if(Interlocked.Exchange(ref this.disposed,1) is not 0 || this.Ownership is not ToolServiceProviderOwnership.Tool) { return; }

        if(this.ServiceProvider is IAsyncDisposable d) { await d.DisposeAsync().ConfigureAwait(false); }

        else if(this.ServiceProvider is IDisposable p) { p.Dispose(); }

        GC.SuppressFinalize(this);
    }
}