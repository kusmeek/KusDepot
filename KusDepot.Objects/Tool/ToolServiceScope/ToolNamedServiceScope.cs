namespace KusDepot;

/**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/main/*'/>*/
internal sealed class ToolNamedServiceScope : IDisposable , IAsyncDisposable
{
    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/field[@name="refcount"]/*'/>*/
    private Int32 refcount;

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/field[@name="disposing"]/*'/>*/
    private Boolean disposing;

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/field[@name="sync"]/*'/>*/
    private readonly Lock sync = new();

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/field[@name="scope"]/*'/>*/
    private readonly AsyncServiceScope scope;

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/field[@name="remove"]/*'/>*/
    private readonly Func<String,ToolNamedServiceScope,Boolean> remove;

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/constructor[@name="Constructor"]/*'/>*/
    internal ToolNamedServiceScope(String name , AsyncServiceScope scope , Func<String,ToolNamedServiceScope,Boolean> remove , Boolean isdefault = false)
    {
        Name = String.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;

        this.scope = scope; this.remove = remove ?? throw new ArgumentNullException(nameof(remove));

        IsDefault = isdefault;
    }

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/property[@name="IsDefault"]/*'/>*/
    internal Boolean IsDefault { get; }

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/property[@name="Name"]/*'/>*/
    internal String Name { get; }

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/property[@name="ServiceProvider"]/*'/>*/
    internal IServiceProvider ServiceProvider
    {
        get
        {
            lock(this.sync)
            {
                ObjectDisposedException.ThrowIf(this.disposing,this); return this.scope.ServiceProvider;
            }
        }
    }

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/method[@name="Acquire"]/*'/>*/
    internal ToolServiceScope? Acquire()
    {
        lock(this.sync)
        {
            if(this.disposing) { return null; }

            checked { this.refcount++; }

            return new ToolServiceScope(this);
        }
    }

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/method[@name="Release"]/*'/>*/
    internal void Release()
    {
        Boolean dispose = false;

        lock(this.sync)
        {
            if(this.refcount > 0) { this.refcount--; }

            if(this.IsDefault || this.disposing || this.refcount != 0) { return; }

            this.disposing = true; dispose = true;
        }

        if(!dispose) { return; }

        try { this.remove(this.Name,this); } finally { this.DisposeCore(); }
    }

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/method[@name="ReleaseAsync"]/*'/>*/
    internal async ValueTask ReleaseAsync()
    {
        Boolean dispose = false;

        lock(this.sync)
        {
            if(this.refcount > 0) { this.refcount--; }

            if(this.IsDefault || this.disposing || this.refcount != 0) { return; }

            this.disposing = true; dispose = true;
        }

        if(!dispose) { return; }

        try { this.remove(this.Name,this); } finally { await this.DisposeAsyncCore().ConfigureAwait(false); }
    }

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/method[@name="Dispose"]/*'/>*/
    public void Dispose()
    {
        lock(this.sync)
        {
            if(this.disposing) { return; }

            this.disposing = true;
        }

        this.DisposeCore();
    }

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/method[@name="DisposeAsync"]/*'/>*/
    public async ValueTask DisposeAsync()
    {
        lock(this.sync)
        {
            if(this.disposing) { return; }

            this.disposing = true;
        }

        await this.DisposeAsyncCore().ConfigureAwait(false);
    }

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/method[@name="DisposeCore"]/*'/>*/
    private void DisposeCore() { this.scope.Dispose(); }

    /**<include file='ToolNamedServiceScope.xml' path='ToolNamedServiceScope/class[@name="ToolNamedServiceScope"]/method[@name="DisposeAsyncCore"]/*'/>*/
    private async ValueTask DisposeAsyncCore() { await this.scope.DisposeAsync().ConfigureAwait(false); }
}
