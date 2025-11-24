namespace KusDepot;

/**<include file='ToolHostBuilder.xml' path='ToolHostBuilder/class[@name="ToolHostBuilder"]/main/*'/>*/
public class ToolHostBuilder : ToolBuilder , IToolHostBuilder
{
    private Boolean Console;

    ///<inheritdoc/>
    public IToolHost BuildHost()
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            IToolHostLifetime? l; Guid i = Guid.NewGuid(); if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); }

            this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l)); InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            var _ = new ToolHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:Commands,configuration:Context!.Configuration,lifeid:i,lifetime:l);

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; }

            if(Start) { ConfigureToolHost(StartAction); } if(seal) { ConfigureToolHost(SealAction); } configuretool(_); configuretoolhost(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolHost(); }
    }

    ///<inheritdoc/>
    public async Task<IToolHost> BuildHostAsync(CancellationToken cancel = default)
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            IToolHostLifetime? l; Guid i = Guid.NewGuid(); if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); }

            this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l)); InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            var _ = new ToolHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:Commands,configuration:Context!.Configuration,lifeid:i,lifetime:l);

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; }

            configuretool(_); configuretoolhost(_); if(Start) { await StartTool(_,cancel); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolHost(); }
    }

    ///<inheritdoc/>
    public IToolHost BuildHost<TToolHost>() where TToolHost : notnull , IToolHost , new()
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            IToolHostLifetime? l; Guid i = Guid.NewGuid(); if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); }

            this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l)); InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true; IToolHost? _ = default;

            _ = (IToolHost?)Activator.CreateInstance(typeof(TToolHost),AccessManager,null,null,BuildServiceProvider(),Commands,Context!.Configuration,i,l,null);

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; }

            if(Start) { ConfigureToolHost(StartAction); } if(seal) { ConfigureToolHost(SealAction); } configuretool(_!); configuretoolhost(_!); return _!;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolHost(); }
    }

    ///<inheritdoc/>
    public async Task<IToolHost> BuildHostAsync<TToolHost>(CancellationToken cancel = default) where TToolHost : notnull , IToolHost , new()
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            IToolHostLifetime? l; Guid i = Guid.NewGuid(); if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); }

            this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l)); InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true; IToolHost? _ = default;

            _ = (IToolHost?)Activator.CreateInstance(typeof(TToolHost),AccessManager,null,null,BuildServiceProvider(),Commands,Context!.Configuration,i,l,null);

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; }

            configuretool(_!); configuretoolhost(_!); if(Start) { await StartTool(_!,cancel); } if(seal) { SealTool(_!); } return _!;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolHost(); }
    }

    /**<include file='ToolHostBuilder.xml' path='ToolHostBuilder/class[@name="ToolHostBuilder"]/method[@name="configuretoolhost"]/*'/>*/
    protected void configuretoolhost(IToolHost toolhost)
    {
        try
        {
            foreach(Action<ToolBuilderContext,IToolHost> a in this.ConfigureToolHostActions) { a(this.Context!,toolhost); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolHostFail); }
    }

    private readonly List<Action<ToolBuilderContext,IToolHost>> ConfigureToolHostActions = new();

    ///<inheritdoc/>
    public IToolHostBuilder ConfigureToolHost(Action<ToolBuilderContext,IToolHost> action)
    {
        try
        {
            if(action is not null) { this.ConfigureToolHostActions.Add(action); } return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolHostFail); return this; }
    }

    ///<inheritdoc/>
    public IToolHostBuilder UseConsoleLifetime() { Console = true; return this; }
}