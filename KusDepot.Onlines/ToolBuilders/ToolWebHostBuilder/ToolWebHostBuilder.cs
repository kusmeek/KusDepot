namespace KusDepot;

/**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/main/*'/>*/
public class ToolWebHostBuilder : ToolBuilder , IToolWebHostBuilder
{
    private Boolean Link;

    private Boolean Connect;

    private Boolean Console;

    private WebApplicationBuilder? builder;

    ///<inheritdoc/>
    public IToolWebHost BuildWebHost(WebApplication? app = null)
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); } InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); if(configure is not null) { configure(a); } } else { a = app; }

            var _ = new ToolWebHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:Commands,configuration:Context!.Configuration,urls:a.Urls,webapplication:a,lifeid:i,lifetime:l,logger:a.Services.GetService<ILoggerFactory>());

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolWebHost = _; }

            if(Start) { ConfigureToolWebHost(StartAction); } if(seal) { ConfigureToolWebHost(SealAction); } configuretool(_); configuretoolwebhost(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolWebHost(); }
    }

    ///<inheritdoc/>
    public async Task<IToolWebHost> BuildWebHostAsync(WebApplication? app = null , CancellationToken cancel = default)
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); } InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); if(configure is not null) { configure(a); } } else { a = app; }

            var _ = new ToolWebHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:Commands,configuration:Context!.Configuration,urls:a.Urls,webapplication:a,lifeid:i,lifetime:l,logger:a.Services.GetService<ILoggerFactory>());

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolWebHost = _; }

            configuretool(_); configuretoolwebhost(_); if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolWebHost(); }
    }

    ///<inheritdoc/>
    public IToolWebHost BuildWebHost<TToolWebHost>(WebApplication? app = null) where TToolWebHost : notnull , IToolWebHost , new()
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); } InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); if(configure is not null) { configure(a); } } else { a = app; } IToolWebHost? _ = default;

            _ = (IToolWebHost?)Activator.CreateInstance(typeof(TToolWebHost),AccessManager,null,null,BuildServiceProvider(),Commands,Context!.Configuration,i,l,a.Services.GetService<ILoggerFactory>(),a.Urls,a);

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolWebHost = _; }

            if(Start) { ConfigureToolWebHost(StartAction); } if(seal) { ConfigureToolWebHost(SealAction); } configuretool(_!); configuretoolwebhost(_!); return _!;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolWebHost(); }
    }

    ///<inheritdoc/>
    public async Task<IToolWebHost> BuildWebHostAsync<TToolWebHost>(WebApplication? app = null , CancellationToken cancel = default) where TToolWebHost : notnull , IToolWebHost , new()
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices((c,s) => s.AddSingletonWithInterfaces(l));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); } InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); if(configure is not null) { configure(a); } } else { a = app; } IToolWebHost? _ = default;

            _ = (IToolWebHost?)Activator.CreateInstance(typeof(TToolWebHost),AccessManager,null,null,BuildServiceProvider(),Commands,Context!.Configuration,i,l,a.Services.GetService<ILoggerFactory>(),a.Urls,a);

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolWebHost = _; }

            configuretool(_!); configuretoolwebhost(_!); if(Start) { await StartTool(_!,cancel).ConfigureAwait(false); } if(seal) { SealTool(_!); } return _!;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolWebHost(); }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseSlimBuilder(WebApplicationOptions? options = null)
    {
        try
        {
            builder = options is null ? WebApplication.CreateSlimBuilder(new WebApplicationOptions()) : WebApplication.CreateSlimBuilder(options); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseSlimBuilderFail); return this; }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseSlimBuilder(String[]? args = null)
    {
        try
        {
            builder =  WebApplication.CreateSlimBuilder(args ?? Array.Empty<String>()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseSlimBuilderFail); return this; }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseFullBuilder(WebApplicationOptions? options = null)
    {
        try
        {
            builder = options is null ? WebApplication.CreateBuilder(new WebApplicationOptions()) : WebApplication.CreateBuilder(options); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseFullBuilderFail); return this; }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseFullBuilder(String[]? args = null)
    {
        try
        {
            builder = WebApplication.CreateBuilder(args ?? Array.Empty<String>()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseFullBuilderFail); return this; }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseBuilderOptions(WebApplicationOptions? options = null)
    {
        try
        {
            builder = WebApplication.CreateEmptyBuilder(options ?? new()); builder.WebHost.UseKestrel(); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseBuilderOptionsFail); return this; }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseMcpServer(ServerCapabilities? capabilities = null , String? instructions = null)
    {
        try
        {
            this.Builder.Services.AddMcpServer( (o) => { o.Capabilities = capabilities; o.ServerInstructions = instructions ?? String.Empty; } ).WithHttpTransport();

            this.Builder.Services.AddRouting(); this.ConfigureWebApplication( (a) => { a.MapMcp(); });

            return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseMcpServerFail); return this; }
    }

    ///<inheritdoc/>
    public ToolWebHostBuilder()
    {
        try
        {
            builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions()); builder.WebHost.UseKestrel();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConstructorFail); }
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
    public IToolWebHostBuilder UseRandomLocalPorts()
    {
        try
        {
            builder?.WebHost.UseUrls(["http://127.0.0.1:0","http://[::1]:0"]); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseSlimBuilderFail); return this; }
    }

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/method[@name="configuretoolwebhost"]/*'/>*/
    protected void configuretoolwebhost(IToolWebHost toolwebhost)
    {
        try
        {
            foreach(Action<ToolBuilderContext,IToolWebHost> a in this.ConfigureToolWebHostActions) { a(this.Context!,toolwebhost); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolWebHostFail); }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder ConfigureWebApplication(Action<WebApplication> action) { configure = action; return this; }

    private readonly List<Action<ToolBuilderContext,IToolWebHost>> ConfigureToolWebHostActions = new();

    ///<inheritdoc/>
    public IToolWebHostBuilder ConfigureToolWebHost(Action<ToolBuilderContext,IToolWebHost> action)
    {
        try
        {
            if(action is not null) { this.ConfigureToolWebHostActions.Add(action); } return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolWebHostFail); return this; }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder ConnectWithApplication() { Connect = true; return this; }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseConsoleLifetime() { Console = true; return this; }

    ///<inheritdoc/>
    public IToolWebHostBuilder LinkLifetime() { Link = true; return this; }

    ///<inheritdoc/>
    public WebApplicationBuilder Builder { get { return builder!; } }

    private Action<WebApplication>? configure;
}