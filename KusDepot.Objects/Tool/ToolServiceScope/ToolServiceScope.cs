namespace KusDepot;

/**<include file='ToolServiceScope.xml' path='ToolServiceScope/class[@name="ToolServiceScope"]/main/*'/>*/
public sealed class ToolServiceScope : IDisposable , IAsyncDisposable
{
    /**<include file='ToolServiceScope.xml' path='ToolServiceScope/class[@name="ToolServiceScope"]/field[@name="disposed"]/*'/>*/
    private Int32 disposed;

    /**<include file='ToolServiceScope.xml' path='ToolServiceScope/class[@name="ToolServiceScope"]/field[@name="owner"]/*'/>*/
    private readonly ToolNamedServiceScope owner;

    /**<include file='ToolServiceScope.xml' path='ToolServiceScope/class[@name="ToolServiceScope"]/constructor[@name="Constructor"]/*'/>*/
    internal ToolServiceScope(ToolNamedServiceScope owner)
    {
        this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }

    /**<include file='ToolServiceScope.xml' path='ToolServiceScope/class[@name="ToolServiceScope"]/property[@name="IsDefault"]/*'/>*/
    public Boolean IsDefault { get { return this.owner.IsDefault; } }

    /**<include file='ToolServiceScope.xml' path='ToolServiceScope/class[@name="ToolServiceScope"]/property[@name="Name"]/*'/>*/
    public String Name { get { return this.owner.Name; } }

    /**<include file='ToolServiceScope.xml' path='ToolServiceScope/class[@name="ToolServiceScope"]/property[@name="ServiceProvider"]/*'/>*/
    public IServiceProvider ServiceProvider
    {
        get
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return this.owner.ServiceProvider;
        }
    }

    /**<include file='ToolServiceScope.xml' path='ToolServiceScope/class[@name="ToolServiceScope"]/method[@name="Dispose"]/*'/>*/
    public void Dispose()
    {
        if(Interlocked.Exchange(ref this.disposed,1) != 0) { return; }

        this.owner.Release();
    }

    /**<include file='ToolServiceScope.xml' path='ToolServiceScope/class[@name="ToolServiceScope"]/method[@name="DisposeAsync"]/*'/>*/
    public async ValueTask DisposeAsync()
    {
        if(Interlocked.Exchange(ref this.disposed,1) != 0) { return; }

        await this.owner.ReleaseAsync().ConfigureAwait(false);
    }
}
