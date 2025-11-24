namespace KusDepot;

/**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/main/*'/>*/
public class ToolAspireHostBuilder : ToolBuilder , IToolAspireHostBuilder
{
    private Boolean Link;

    private Boolean Connect;

    private Boolean Console;

    private IDistributedApplicationBuilder? builder;

    ///<inheritdoc/>
    public IToolAspireHost BuildAspireHost(DistributedApplication? app = null)
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l!));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); }

            InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); } else { a = app; }

            var _ = new ToolAspireHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:Commands,configuration:Context!.Configuration,aspireapplication:a,lifeid:i,lifetime:l,logger:a.Services.GetService<ILoggerFactory>());

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolAspireHost = _; }

            if(Start) { ConfigureToolAspireHost(StartAction); } if(seal) { ConfigureToolAspireHost(SealAction); } configuretool(_); configuretoolaspirehost(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolAspireHost(); }
    }

    ///<inheritdoc/>
    public async Task<IToolAspireHost> BuildAspireHostAsync(DistributedApplication? app = null , CancellationToken cancel = default)
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l!));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); }

            InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); } else { a = app; }

            var _ = new ToolAspireHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:Commands,configuration:Context!.Configuration,aspireapplication:a,lifeid:i,lifetime:l,logger:a.Services.GetService<ILoggerFactory>());

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolAspireHost = _; }

            configuretool(_); configuretoolaspirehost(_); if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolAspireHost(); }
    }

    ///<inheritdoc/>
    public IToolAspireHost BuildAspireHost<TToolAspireHost>(DistributedApplication? app = null) where TToolAspireHost : notnull , IToolAspireHost , new()
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l!));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); }

            InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); } else { a = app; } IToolAspireHost? _ = default;

            _ = (IToolAspireHost?)Activator.CreateInstance(typeof(TToolAspireHost),AccessManager,null,null,BuildServiceProvider(),Commands,Context!.Configuration,i,l,a.Services.GetService<ILoggerFactory>(),a);

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolAspireHost = _; }

            if(Start) { ConfigureToolAspireHost(StartAction); } if(seal) { ConfigureToolAspireHost(SealAction); } configuretool(_!); configuretoolaspirehost(_!); return _!;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolAspireHost(); }
    }

    ///<inheritdoc/>
    public async Task<IToolAspireHost> BuildAspireHostAsync<TToolAspireHost>(DistributedApplication? app = null , CancellationToken cancel = default) where TToolAspireHost : notnull , IToolAspireHost , new()
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l!));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); }

            InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); } else { a = app; } IToolAspireHost? _ = default;

            _ = (IToolAspireHost?)Activator.CreateInstance(typeof(TToolAspireHost),AccessManager,null,null,BuildServiceProvider(),Commands,Context!.Configuration,i,l,a.Services.GetService<ILoggerFactory>(),a);

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolAspireHost = _; }

            configuretool(_!); configuretoolaspirehost(_!); if(Start) { await StartTool(_!,cancel).ConfigureAwait(false); } if(seal) { SealTool(_!); } return _!;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolAspireHost(); }
    }

    ///<inheritdoc/>
    public IToolAspireHostBuilder UseBuilderOptions(DistributedApplicationOptions? options = null)
    {
        try
        {
            builder = options is null ? DistributedApplication.CreateBuilder(new DistributedApplicationOptions()) : DistributedApplication.CreateBuilder(options); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseBuilderOptionsFail); return this; }
    }

    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/method[@name="configuretoolaspirehost"]/*'/>*/
    protected void configuretoolaspirehost(IToolAspireHost toolaspirehost)
    {
        try
        {
            foreach(Action<ToolBuilderContext,IToolAspireHost> a in this.ConfigureToolAspireHostActions) { a(this.Context!,toolaspirehost); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolAspireHostFail); }
    }

    private readonly List<Action<ToolBuilderContext,IToolAspireHost>> ConfigureToolAspireHostActions = new();

    ///<inheritdoc/>
    public IToolAspireHostBuilder ConfigureToolAspireHost(Action<ToolBuilderContext,IToolAspireHost> action)
    {
        try
        {
            if(action is not null) { this.ConfigureToolAspireHostActions.Add(action); } return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolAspireHostFail); return this; }
    }

    ///<inheritdoc/>
    public IToolAspireHostBuilder ConnectWithApplication() { Connect = true; return this; }

    ///<inheritdoc/>
    public IToolAspireHostBuilder UseBuilderArguments(String[]? args = null)
    {
        try
        {
            builder = DistributedApplication.CreateBuilder(args ?? Array.Empty<String>()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseBuilderArgumentsFail); return this; }
    }

    ///<inheritdoc/>
    public override IToolBuilder UseSemanticKernel(Kernel? kernel = null)
    {
        try
        {
            if(kernel is not null) { this.builder?.Services.AddSingleton(kernel); return this; }

            this.builder?.Services.AddTransient<Kernel>();

            return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseSemanticKernelFail); return this; }
    }

    ///<inheritdoc/>
    public IToolAspireHostBuilder UseConsoleLifetime() { Console = true; return this; }

    ///<inheritdoc/>
    public IToolAspireHostBuilder LinkLifetime() { Link = true; return this; }

    ///<inheritdoc/>
    public IDistributedApplicationBuilder Builder { get { return builder!; } }

    ///<inheritdoc/>
    public ToolAspireHostBuilder()
    {
        try
        {
            builder = DistributedApplication.CreateBuilder();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConstructorFail); }
    }
}