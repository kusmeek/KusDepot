namespace KusDepot.Builders;

/**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/main/*'/>*/
public partial class ToolAspireHostBuilder : ToolBuilder , IToolAspireHostBuilder
{
    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/field[@name="Link"]/*'/>*/
    private Boolean Link;

    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/field[@name="Connect"]/*'/>*/
    private Boolean Connect;

    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/field[@name="Console"]/*'/>*/
    private Boolean Console;

    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/field[@name="builder"]/*'/>*/
    private IDistributedApplicationBuilder? builder;

    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/field[@name="ConfigureToolAspireHostActions"]/*'/>*/
    private readonly List<ConfigureToolAspireHostAction> ConfigureToolAspireHostActions = new();

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

    ///<inheritdoc/>
    public IToolAspireHost BuildAspireHost(DistributedApplication? app = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); } else { a = app; }

            var _ = new ToolAspireHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration,aspireapplication:a,logger:a.Services.GetService<ILoggerFactory>()); SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolAspireHost = _;

            configuretool(_); configuretoolaspirehost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolAspireHost BuildAspireHost<TToolAspireHost>(DistributedApplication? app = null) where TToolAspireHost : notnull , IToolAspireHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); } else { a = app; }

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolAspireHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,a.Services.GetService<ILoggerFactory>()).WithArgument(7,a);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolAspireHost)ob.Value; SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolAspireHost = _;

            configuretool(_); configuretoolaspirehost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolAspireHost BuildAspireHost<TToolAspireHost>(ToolAspireHostFactory<TToolAspireHost> factory , DistributedApplication? app = null) where TToolAspireHost : notnull , IToolAspireHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); } else { a = app; }

            var _ = (IToolAspireHost)factory(AccessManager,BuildServiceProvider(),Context!.Configuration,a.Services.GetService<ILoggerFactory>(),a);

            if(_ is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolAspireHost = _;

            configuretool(_); configuretoolaspirehost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolAspireHost> BuildAspireHostAsync(DistributedApplication? app = null , CancellationToken cancel = default)
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); } else { a = app; }

            var _ = new ToolAspireHost(accessmanager:AccessManager,services:await BuildServiceProviderAsync(cancel).ConfigureAwait(false),commands:null,configuration:Context!.Configuration,aspireapplication:a,logger:a.Services.GetService<ILoggerFactory>()); SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolAspireHost = _;

            await configuretoolasync(_,cancel).ConfigureAwait(false); await configuretoolaspirehostasync(_,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_,cancel).ConfigureAwait(false); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolAspireHost> BuildAspireHostAsync<TToolAspireHost>(DistributedApplication? app = null , CancellationToken cancel = default) where TToolAspireHost : notnull , IToolAspireHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); } else { a = app; }

            var sp = await BuildServiceProviderAsync(cancel).ConfigureAwait(false); var ob = ObjectBuilder.Create(typeof(TToolAspireHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,a.Services.GetService<ILoggerFactory>()).WithArgument(7,a);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolAspireHost)ob.Value; SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolAspireHost = _;

            await configuretoolasync(_!,cancel).ConfigureAwait(false); await configuretoolaspirehostasync(_!,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_!,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_!,cancel).ConfigureAwait(false); } ClearBuildResources(); return _!;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolAspireHost> BuildAspireHostAsync<TToolAspireHost>(ToolAspireHostFactory<TToolAspireHost> factory , DistributedApplication? app = null , CancellationToken cancel = default) where TToolAspireHost : notnull , IToolAspireHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(app);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); SetBuildHost(a); } else { a = app; }

            var _ = (IToolAspireHost)factory(AccessManager,await BuildServiceProviderAsync(cancel).ConfigureAwait(false),Context!.Configuration,a.Services.GetService<ILoggerFactory>(),a);

            if(_ is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolAspireHost = _;

            await configuretoolasync(_!,cancel).ConfigureAwait(false); await configuretoolaspirehostasync(_!,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_!,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_!,cancel).ConfigureAwait(false); } ClearBuildResources(); return _!;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolAspireHostBuilder ConfigureToolAspireHost(Action<ToolBuilderContext,IToolAspireHost> action)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureToolAspireHost_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolAspireHostFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolAspireHostBuilder ConfigureToolAspireHost(Func<ToolBuilderContext,IToolAspireHost,CancellationToken,Task> action)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureToolAspireHost_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolAspireHostFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/method[@name="ConfigureToolAspireHost_NoSync"]/*'/>*/
    protected void ConfigureToolAspireHost_NoSync(Action<ToolBuilderContext,IToolAspireHost> action)
    {
        if(action is not null) { this.ConfigureToolAspireHostActions.Add(new(action)); }
    }

    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/method[@name="ConfigureToolAspireHostAsync_NoSync"]/*'/>*/
    protected void ConfigureToolAspireHost_NoSync(Func<ToolBuilderContext,IToolAspireHost,CancellationToken,Task> action)
    {
        if(action is not null) { this.ConfigureToolAspireHostActions.Add(new(action)); }
    }

    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/method[@name="configuretoolaspirehost"]/*'/>*/
    protected void configuretoolaspirehost(IToolAspireHost toolaspirehost)
    {
        try
        {
            foreach(ConfigureToolAspireHostAction a in this.ConfigureToolAspireHostActions) { a.Execute(this.Context!,toolaspirehost); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolAspireHostFail); throw; }
    }

    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/method[@name="configuretoolaspirehostasync"]/*'/>*/
    protected async Task configuretoolaspirehostasync(IToolAspireHost toolaspirehost , CancellationToken cancel = default)
    {
        try
        {
            foreach(ConfigureToolAspireHostAction a in this.ConfigureToolAspireHostActions) { await a.ExecuteAsync(this.Context!,toolaspirehost,cancel).ConfigureAwait(false); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolAspireHostFail); throw; }
    }

    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/method[@name="CreateApplicationConnection"]/*'/>*/
    private ToolConnect? CreateApplicationConnection(IToolHostLifetime? lifetime)
    {
        if(!Connect || lifetime is null) { return null; }

        ToolConnect connection = new(); Builder.Services.AddSingleton<IToolConnect>(connection); return connection;
    }

    /**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/method[@name="LinkApplicationLifetime"]/*'/>*/
    private void LinkApplicationLifetime(IToolHostLifetime? lifetime)
    {
        if(Link && lifetime is not null) { Builder.Services.AddSingleton(lifetime); }
    }

    ///<inheritdoc/>
    public IToolAspireHostBuilder ConnectWithApplication() { DC(); Connect = true; return this; }

    ///<inheritdoc/>
    public IToolAspireHostBuilder LinkLifetime() { DC(); Link = true; return this; }

    ///<inheritdoc/>
    public IToolAspireHostBuilder UseBuilderArguments(String[]? args = null)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder = DistributedApplication.CreateBuilder(args ?? Array.Empty<String>()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseBuilderArgumentsFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolAspireHostBuilder UseBuilderOptions(DistributedApplicationOptions? options = null)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder = options is null ? DistributedApplication.CreateBuilder(new DistributedApplicationOptions()) : DistributedApplication.CreateBuilder(options); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseBuilderOptionsFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolAspireHostBuilder UseConsoleLifetime() { DC(); Console = true; return this; }
}