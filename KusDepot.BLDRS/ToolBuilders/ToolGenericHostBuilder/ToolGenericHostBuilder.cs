namespace KusDepot.Builders;

/**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/main/*'/>*/
public partial class ToolGenericHostBuilder : ToolBuilder , IToolGenericHostBuilder
{
    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/field[@name="Link"]/*'/>*/
    private Boolean Link;

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/field[@name="Connect"]/*'/>*/
    private Boolean Connect;

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/field[@name="Console"]/*'/>*/
    private Boolean Console;

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/field[@name="hostbuilder"]/*'/>*/
    private IHostBuilder? hostbuilder;

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/field[@name="builder"]/*'/>*/
    private HostApplicationBuilder? builder;

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/field[@name="ConfigureToolGenericHostActions"]/*'/>*/
    private readonly List<ConfigureToolGenericHostAction> ConfigureToolGenericHostActions = new();

    ///<inheritdoc/>
    public HostApplicationBuilder Builder { get { return builder!; } }

    ///<inheritdoc/>
    public IHostBuilder HostBuilder { get { return hostbuilder!; } }

    ///<inheritdoc/>
    public ToolGenericHostBuilder()
    {
        try
        {
            builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConstructorFail); }
    }

    ///<inheritdoc/>
    public IToolGenericHost BuildGenericHost(IHost? host = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(host);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            IHost? h = default; if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); } SetBuildHost(host ?? h);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var _ = new ToolGenericHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration,host:(host ?? h),logger:(host ?? h)?.Services.GetService<ILoggerFactory>()); SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolGenericHost = _;

            configuretool(_); configuretoolgenerichost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolGenericHost BuildGenericHost<TToolGenericHost>(IHost? host = null) where TToolGenericHost : notnull , IToolGenericHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(host);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            IHost? h = default; if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); } SetBuildHost(host ?? h);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolGenericHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,host ?? h).WithArgument(7,(host ?? h)?.Services.GetService<ILoggerFactory>());

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolGenericHost)ob.Value; SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolGenericHost = _;

            configuretool(_); configuretoolgenerichost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolGenericHost BuildGenericHost<TToolGenericHost>(ToolGenericHostFactory<TToolGenericHost> factory , IHost? host = null) where TToolGenericHost : notnull , IToolGenericHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(host);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            IHost? h = default; if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); } SetBuildHost(host ?? h);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var _ = (IToolGenericHost)factory(AccessManager,BuildServiceProvider(),Context!.Configuration,host ?? h,(host ?? h)?.Services.GetService<ILoggerFactory>());

            if(_ is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolGenericHost = _;

            configuretool(_); configuretoolgenerichost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolGenericHost> BuildGenericHostAsync(IHost? host = null , CancellationToken cancel = default)
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(host);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            IHost? h = default; if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); } SetBuildHost(host ?? h);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var _ = new ToolGenericHost(accessmanager:AccessManager,services:await BuildServiceProviderAsync(cancel).ConfigureAwait(false),commands:null,configuration:Context!.Configuration,host:host ?? h,logger:(host ?? h)?.Services.GetService<ILoggerFactory>()); SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolGenericHost = _;

            await configuretoolasync(_,cancel).ConfigureAwait(false); await configuretoolgenerichostasync(_,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_,cancel).ConfigureAwait(false); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolGenericHost> BuildGenericHostAsync<TToolGenericHost>(IHost? host = null , CancellationToken cancel = default) where TToolGenericHost : notnull , IToolGenericHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(host);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            IHost? h = default; if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); } SetBuildHost(host ?? h);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var sp = await BuildServiceProviderAsync(cancel).ConfigureAwait(false); var ob = ObjectBuilder.Create(typeof(TToolGenericHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,host ?? h).WithArgument(7,(host ?? h)?.Services.GetService<ILoggerFactory>());

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolGenericHost)ob.Value; SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolGenericHost = _;

            await configuretoolasync(_!,cancel).ConfigureAwait(false); await configuretoolgenerichostasync(_!,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_!,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_!,cancel).ConfigureAwait(false); } ClearBuildResources(); return _!;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolGenericHost> BuildGenericHostAsync<TToolGenericHost>(ToolGenericHostFactory<TToolGenericHost> factory , IHost? host = null , CancellationToken cancel = default) where TToolGenericHost : notnull , IToolGenericHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); } SetBuildHost(host);

            IToolHostLifetime? l = CreateHostLifetime(Console); ToolConnect? c = CreateApplicationConnection(l); LinkApplicationLifetime(l);

            IHost? h = default; if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); } SetBuildHost(host ?? h);

            InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var _ = (IToolGenericHost)factory(AccessManager,await BuildServiceProviderAsync(cancel).ConfigureAwait(false),Context!.Configuration,host ?? h,(host ?? h)?.Services.GetService<ILoggerFactory>());

            if(_ is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            c?.ToolGenericHost = _;

            await configuretoolasync(_!,cancel).ConfigureAwait(false); await configuretoolgenerichostasync(_!,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_!,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_!,cancel).ConfigureAwait(false); } ClearBuildResources(); return _!;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder ConfigureToolGenericHost(Action<ToolBuilderContext,IToolGenericHost> action)
    {
        Boolean sb = false;
        DC();
        try 
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureToolGenericHost_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolGenericHostFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder ConfigureToolGenericHost(Func<ToolBuilderContext,IToolGenericHost,CancellationToken,Task> action)
    {
        Boolean sb = false;
        DC();
        try 
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureToolGenericHost_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolGenericHostFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/method[@name="ConfigureToolGenericHost_NoSync"]/*'/>*/
    protected void ConfigureToolGenericHost_NoSync(Action<ToolBuilderContext,IToolGenericHost> action)
    {
        if(action is not null) { this.ConfigureToolGenericHostActions.Add(new(action)); }
    }

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/method[@name="ConfigureToolGenericHostAsync_NoSync"]/*'/>*/
    protected void ConfigureToolGenericHost_NoSync(Func<ToolBuilderContext,IToolGenericHost,CancellationToken,Task> action)
    {
        if(action is not null) { this.ConfigureToolGenericHostActions.Add(new(action)); }
    }

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/method[@name="configuretoolgenerichost"]/*'/>*/
    protected void configuretoolgenerichost(IToolGenericHost toolgenerichost)
    {
        try
        {
            foreach(ConfigureToolGenericHostAction a in this.ConfigureToolGenericHostActions) { a.Execute(this.Context!,toolgenerichost); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolGenericHostFail); throw; }
    }

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/method[@name="configuretoolgenerichostasync"]/*'/>*/
    protected async Task configuretoolgenerichostasync(IToolGenericHost toolgenerichost , CancellationToken cancel = default)
    {
        try
        {
            foreach(ConfigureToolGenericHostAction a in this.ConfigureToolGenericHostActions) { await a.ExecuteAsync(this.Context!,toolgenerichost,cancel).ConfigureAwait(false); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolGenericHostFail); throw; }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder ConnectWithApplication() { DC(); Connect = true; return this; }

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/method[@name="CreateApplicationConnection"]/*'/>*/
    private ToolConnect? CreateApplicationConnection(IToolHostLifetime? lifetime)
    {
        if(!Connect || lifetime is null) { return null; }

        ToolConnect connection = new();

        this.builder?.Services.AddSingleton<IToolConnect>(connection);

        this.hostbuilder?.ConfigureServices(services => services.AddSingleton<IToolConnect>(connection));

        return connection;
    }

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/method[@name="LinkApplicationLifetime"]/*'/>*/
    private void LinkApplicationLifetime(IToolHostLifetime? lifetime)
    {
        if(!Link || lifetime is null) { return; }

        this.builder?.Services.AddSingleton(lifetime);

        this.hostbuilder?.ConfigureServices(s => s.AddSingleton(lifetime));
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder LinkLifetime() { DC(); Link = true; return this; }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseBuilderSettings(HostApplicationBuilderSettings? settings = null)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder = Host.CreateEmptyApplicationBuilder(settings ?? new()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseBuilderSettingsFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseConsoleLifetime() { DC(); Console = true; return this; }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseFullBuilder(HostApplicationBuilderSettings? settings = null)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            this.hostbuilder = null; builder = settings is null ? Host.CreateApplicationBuilder(new HostApplicationBuilderSettings()) : Host.CreateApplicationBuilder(settings); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseFullBuilderFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseFullBuilder(String[]? args = null)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            this.hostbuilder = null; builder = Host.CreateApplicationBuilder(args); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseFullBuilderFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseHostBuilder(String[]? args = null)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            this.builder = null; this.hostbuilder = Host.CreateDefaultBuilder(args); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseHostBuilderFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }
}