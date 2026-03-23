namespace KusDepot.Builders;

/**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/main/*'/>*/
public partial class ToolWebHostBuilder : ToolBuilder , IToolWebHostBuilder
{
    private Boolean Link;

    private Boolean Connect;

    private Boolean Console;

    private WebApplicationBuilder? builder;

    private readonly List<Action<WebApplication>> ConfigureWebApplicationActions = new();

    ///<inheritdoc/>
    public WebApplicationBuilder Builder { get { return builder!; } }

    private readonly List<Action<ToolBuilderContext,IToolWebHost>> ConfigureToolWebHostActions = new();

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
    public IToolWebHost BuildWebHost(WebApplication? app = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); } InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); configurewebapplication(a); } else { a = app; }

            var _ = new ToolWebHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration,urls:a.Urls,webapplication:a,lifeid:i,lifetime:l,logger:a.Services.GetService<ILoggerFactory>());

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolWebHost = _; }

            if(Start) { ConfigureToolWebHost_NoSync(StartAction); } if(seal) { ConfigureToolWebHost_NoSync(SealAction); } configuretool(_); configuretoolwebhost(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolWebHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHost BuildWebHost<TToolWebHost>(WebApplication? app = null) where TToolWebHost : notnull , IToolWebHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); } InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); configurewebapplication(a); } else { a = app; }

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolWebHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,i).WithArgument(7,l).WithArgument(8,a.Services.GetService<ILoggerFactory>()).WithArgument(9,a.Urls).WithArgument(10,a);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolWebHost)ob.Value;

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolWebHost = _; }

            if(Start) { ConfigureToolWebHost_NoSync(StartAction); } if(seal) { ConfigureToolWebHost_NoSync(SealAction); } configuretool(_); configuretoolwebhost(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolWebHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolWebHost> BuildWebHostAsync(WebApplication? app = null , CancellationToken cancel = default)
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); } InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); configurewebapplication(a); } else { a = app; }

            var _ = new ToolWebHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration,urls:a.Urls,webapplication:a,lifeid:i,lifetime:l,logger:a.Services.GetService<ILoggerFactory>());

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolWebHost = _; }

            configuretool(_); configuretoolwebhost(_); if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolWebHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolWebHost> BuildWebHostAsync<TToolWebHost>(WebApplication? app = null , CancellationToken cancel = default) where TToolWebHost : notnull , IToolWebHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); } InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); configurewebapplication(a); } else { a = app; }

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolWebHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,i).WithArgument(7,l).WithArgument(8,a.Services.GetService<ILoggerFactory>()).WithArgument(9,a.Urls).WithArgument(10,a);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolWebHost)ob.Value;

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolWebHost = _; }

            configuretool(_!); configuretoolwebhost(_!); if(Start) { await StartTool(_!,cancel).ConfigureAwait(false); } if(seal) { SealTool(_!); } return _!;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolWebHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder ConfigureToolWebHost(Action<ToolBuilderContext,IToolWebHost> action)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureToolWebHost_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolWebHostFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/method[@name="ConfigureToolWebHost_NoSync"]/*'/>*/
    protected void ConfigureToolWebHost_NoSync(Action<ToolBuilderContext,IToolWebHost> action)
    {
        if(action is not null) { this.ConfigureToolWebHostActions.Add(action); }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder ConfigureWebApplication(Action<WebApplication> action)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(action is not null) { this.ConfigureWebApplicationActions.Add(action); } return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureWebApplicationFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/method[@name="configurewebapplication"]/*'/>*/
    protected void configurewebapplication(WebApplication webapplication)
    {
        try { foreach(Action<WebApplication> a in this.ConfigureWebApplicationActions) { a(webapplication); } }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureWebApplicationFail); }
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
    public IToolWebHostBuilder ConnectWithApplication() { Connect = true; return this; }

    ///<inheritdoc/>
    public IToolWebHostBuilder LinkLifetime() { Link = true; return this; }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseBuilderOptions(WebApplicationOptions? options = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder = WebApplication.CreateEmptyBuilder(options ?? new()); builder.WebHost.UseKestrel(); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseBuilderOptionsFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseFullBuilder(WebApplicationOptions? options = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder = options is null ? WebApplication.CreateBuilder(new WebApplicationOptions()) : WebApplication.CreateBuilder(options); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseFullBuilderFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseFullBuilder(String[]? args = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder = WebApplication.CreateBuilder(args ?? Array.Empty<String>()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseFullBuilderFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseRandomLocalPorts()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder?.WebHost.UseUrls(["http://127.0.0.1:0","http://[::1]:0"]); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseRandomLocalPortsFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseSlimBuilder(WebApplicationOptions? options = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder = options is null ? WebApplication.CreateSlimBuilder(new WebApplicationOptions()) : WebApplication.CreateSlimBuilder(options); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseSlimBuilderFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseSlimBuilder(String[]? args = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder =  WebApplication.CreateSlimBuilder(args ?? Array.Empty<String>()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseSlimBuilderFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseConsoleLifetime() { Console = true; return this; }
}