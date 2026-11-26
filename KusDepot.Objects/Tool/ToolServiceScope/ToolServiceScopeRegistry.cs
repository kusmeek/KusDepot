namespace KusDepot;

/**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/main/*'/>*/
public sealed class ToolServiceScopeRegistry : IDisposable , IAsyncDisposable
{
    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/field[@name="disposing"]/*'/>*/
    private Boolean disposing;

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/field[@name="sync"]/*'/>*/
    private readonly Lock sync = new();

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/field[@name="services"]/*'/>*/
    private readonly IServiceProvider services;

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/field[@name="DefaultScopeName"]/*'/>*/
    private const String DefaultScopeName = "default";

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/field[@name="scopes"]/*'/>*/
    private readonly Dictionary<String,ToolNamedServiceScope> scopes = new(StringComparer.Ordinal);

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/constructor[@name="Constructor"]/*'/>*/
    internal ToolServiceScopeRegistry(IServiceProvider services)
    {
        this.services = services ?? throw new ArgumentNullException(nameof(services));

        var scope = services.CreateAsyncScope();

        try
        {
            var defaultscope = new ToolNamedServiceScope(DefaultScopeName,scope,this.RemoveEntry,isdefault:true);

            this.scopes.Add(DefaultScopeName,defaultscope);
        }
        catch { scope.Dispose(); throw; }
    }

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/method[@name="Create"]/*'/>*/
    internal ToolServiceScope? Create(String name)
    {
        if(String.IsNullOrWhiteSpace(name)) { return null; }

        lock(this.sync)
        {
            if(this.disposing) { return null; }

            if(this.scopes.ContainsKey(name)) { return null; }

            var scope = new ToolNamedServiceScope(name,this.services.CreateAsyncScope(),this.RemoveEntry);

            this.scopes.Add(name,scope);

            return scope.Acquire();
        }
    }

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/method[@name="Get"]/*'/>*/
    internal ToolServiceScope? Get(String name)
    {
        if(String.IsNullOrWhiteSpace(name)) { return null; }

        lock(this.sync)
        {
            if(this.disposing) { return null; }

            return this.scopes.TryGetValue(name,out ToolNamedServiceScope? scope) ? scope.Acquire() : null;
        }
    }

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/method[@name="GetDefault"]/*'/>*/
    internal ToolServiceScope? GetDefault() => this.Get(DefaultScopeName);

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/method[@name="GetNames"]/*'/>*/
    internal IEnumerable<String> GetNames()
    {
        lock(this.sync)
        {
            if(this.disposing) { return []; }

            return [ .. this.scopes.Keys.Order(StringComparer.Ordinal) ];
        }
    }

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/method[@name="Dispose"]/*'/>*/
    public void Dispose()
    {
        ToolNamedServiceScope[] values;

        lock(this.sync)
        {
            if(this.disposing) { return; } this.disposing = true;

            values = [ .. this.scopes.Values ]; this.scopes.Clear();
        }

        List<Exception>? errors = null;

        foreach(ToolNamedServiceScope value in values)
        {
            try { value.Dispose(); }

            catch(Exception error) { (errors ??= []).Add(error); }
        }

        if(errors is not null) { throw new AggregateException(errors); }
    }

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/method[@name="DisposeAsync"]/*'/>*/
    public async ValueTask DisposeAsync()
    {
        ToolNamedServiceScope[] values;

        lock(this.sync)
        {
            if(this.disposing) { return; } this.disposing = true;

            values = [ .. this.scopes.Values ]; this.scopes.Clear();
        }

        List<Exception>? errors = null;

        foreach(ToolNamedServiceScope value in values)
        {
            try { await value.DisposeAsync().ConfigureAwait(false); }

            catch(Exception error) { (errors ??= []).Add(error); }
        }

        if(errors is not null) { throw new AggregateException(errors); }
    }

    /**<include file='ToolServiceScopeRegistry.xml' path='ToolServiceScopeRegistry/class[@name="ToolServiceScopeRegistry"]/method[@name="RemoveEntry"]/*'/>*/
    private Boolean RemoveEntry(String name , ToolNamedServiceScope scope)
    {
        lock(this.sync)
        {
            return this.scopes.TryGetValue(name,out ToolNamedServiceScope? entry) && ReferenceEquals(entry,scope) && this.scopes.Remove(name);
        }
    }
}
