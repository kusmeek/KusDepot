namespace KusDepot.Builders;

/**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/main/*'/>*/
public partial class ToolWebHostBuilder : ToolBuilder , IToolWebHostBuilder
{
    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/field[@name="Link"]/*'/>*/
    private Boolean Link;

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/field[@name="Connect"]/*'/>*/
    private Boolean Connect;

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/field[@name="Console"]/*'/>*/
    private Boolean Console;

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/field[@name="builder"]/*'/>*/
    private WebApplicationBuilder? builder;

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/field[@name="ConfigureWebApplicationActions"]/*'/>*/
    private readonly List<Action<WebApplication>> ConfigureWebApplicationActions = new();

    ///<inheritdoc/>
    public WebApplicationBuilder Builder { get { return builder!; } }

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/field[@name="ConfigureToolWebHostActions"]/*'/>*/
    private readonly List<ConfigureToolWebHostAction> ConfigureToolWebHostActions = new();

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

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); configurewebapplication(a); } else { a = app; }

            var _ = new ToolWebHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration,urls:a.Urls,webapplication:a,logger:a.Services.GetService<ILoggerFactory>()); SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolWebHost = _;

            configuretool(_); configuretoolwebhost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHost BuildWebHost<TToolWebHost>(WebApplication? app = null) where TToolWebHost : notnull , IToolWebHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); configurewebapplication(a); } else { a = app; }

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolWebHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,a.Services.GetService<ILoggerFactory>()).WithArgument(7,a.Urls).WithArgument(8,a);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolWebHost)ob.Value; SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolWebHost = _;

            configuretool(_); configuretoolwebhost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHost BuildWebHost<TToolWebHost>(ToolWebHostFactory<TToolWebHost> factory , WebApplication? app = null) where TToolWebHost : notnull , IToolWebHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); configurewebapplication(a); } else { a = app; }

            var _ = (IToolWebHost)factory(AccessManager,BuildServiceProvider(),Context!.Configuration,a.Services.GetService<ILoggerFactory>(),a.Urls,a);

            if(_ is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolWebHost = _;

            configuretool(_); configuretoolwebhost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolWebHost> BuildWebHostAsync(WebApplication? app = null , CancellationToken cancel = default)
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); configurewebapplication(a); } else { a = app; }

            var _ = new ToolWebHost(accessmanager:AccessManager,services:await BuildServiceProviderAsync(cancel).ConfigureAwait(false),commands:null,configuration:Context!.Configuration,urls:a.Urls,webapplication:a,logger:a.Services.GetService<ILoggerFactory>()); SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolWebHost = _;

            await configuretoolasync(_,cancel).ConfigureAwait(false); await configuretoolwebhostasync(_,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_,cancel).ConfigureAwait(false); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolWebHost> BuildWebHostAsync<TToolWebHost>(WebApplication? app = null , CancellationToken cancel = default) where TToolWebHost : notnull , IToolWebHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); configurewebapplication(a); } else { a = app; }

            var sp = await BuildServiceProviderAsync(cancel).ConfigureAwait(false); var ob = ObjectBuilder.Create(typeof(TToolWebHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,a.Services.GetService<ILoggerFactory>()).WithArgument(7,a.Urls).WithArgument(8,a);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolWebHost)ob.Value; SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolWebHost = _;

            await configuretoolasync(_!,cancel).ConfigureAwait(false); await configuretoolwebhostasync(_!,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_!,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_!,cancel).ConfigureAwait(false); } ClearBuildResources(); return _!;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolWebHost> BuildWebHostAsync<TToolWebHost>(ToolWebHostFactory<TToolWebHost> factory , WebApplication? app = null , CancellationToken cancel = default) where TToolWebHost : notnull , IToolWebHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            WebApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); configurewebapplication(a); } else { a = app; }

            var _ = (IToolWebHost)factory(AccessManager,await BuildServiceProviderAsync(cancel).ConfigureAwait(false),Context!.Configuration,a.Services.GetService<ILoggerFactory>(),a.Urls,a);

            if(_ is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolWebHost = _;

            await configuretoolasync(_!,cancel).ConfigureAwait(false); await configuretoolwebhostasync(_!,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_!,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_!,cancel).ConfigureAwait(false); } ClearBuildResources(); return _!;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder ConfigureToolWebHost(Action<ToolBuilderContext,IToolWebHost> action)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureToolWebHost_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolWebHostFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder ConfigureToolWebHost(Func<ToolBuilderContext,IToolWebHost,CancellationToken,Task> action)
    {
        Boolean sb = false;
        DC();
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
        if(action is not null) { this.ConfigureToolWebHostActions.Add(new(action)); }
    }

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/method[@name="ConfigureToolWebHostAsync_NoSync"]/*'/>*/
    protected void ConfigureToolWebHost_NoSync(Func<ToolBuilderContext,IToolWebHost,CancellationToken,Task> action)
    {
        if(action is not null) { this.ConfigureToolWebHostActions.Add(new(action)); }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder ConfigureWebApplication(Action<WebApplication> action)
    {
        Boolean sb = false;
        DC();
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

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureWebApplicationFail); throw; }
    }

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/method[@name="configuretoolwebhost"]/*'/>*/
    protected void configuretoolwebhost(IToolWebHost toolwebhost)
    {
        try
        {
            foreach(ConfigureToolWebHostAction a in this.ConfigureToolWebHostActions) { a.Execute(this.Context!,toolwebhost); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolWebHostFail); throw; }
    }

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/method[@name="configuretoolwebhostasync"]/*'/>*/
    protected async Task configuretoolwebhostasync(IToolWebHost toolwebhost , CancellationToken cancel = default)
    {
        try
        {
            foreach(ConfigureToolWebHostAction a in this.ConfigureToolWebHostActions) { await a.ExecuteAsync(this.Context!,toolwebhost,cancel).ConfigureAwait(false); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolWebHostFail); throw; }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder ConnectWithApplication() { DC(); Connect = true; return this; }

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/method[@name="CreateApplicationConnection"]/*'/>*/
    private ToolConnect? CreateApplicationConnection(IToolHostLifetime? lifetime)
    {
        if(!Connect || lifetime is null) { return null; }

        ToolConnect connection = new(); Builder.Services.AddSingleton<IToolConnect>(connection); return connection;
    }

    /**<include file='ToolWebHostBuilder.xml' path='ToolWebHostBuilder/class[@name="ToolWebHostBuilder"]/method[@name="LinkApplicationLifetime"]/*'/>*/
    private void LinkApplicationLifetime(IToolHostLifetime? lifetime)
    {
        if(Link && lifetime is not null) { Builder.Services.AddSingleton(lifetime); }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder LinkLifetime() { DC(); Link = true; return this; }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseBuilderOptions(WebApplicationOptions? options = null)
    {
        Boolean sb = false;
        DC();
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
        DC();
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
        DC();
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
        DC();
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
        DC();
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
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder =  WebApplication.CreateSlimBuilder(args ?? Array.Empty<String>()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseSlimBuilderFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolWebHostBuilder UseConsoleLifetime() { DC(); Console = true; return this; }
}