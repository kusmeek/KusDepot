namespace KusDepot.Builders;

/**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/main/*'/>*/
public partial class ToolGenericHostBuilder : ToolBuilder , IToolGenericHostBuilder
{
    private Boolean Link;

    private Boolean Connect;

    private Boolean Console;

    private IHostBuilder? hostbuilder;

    private HostApplicationBuilder? builder;

    private readonly List<Action<ToolBuilderContext,IToolGenericHost>> ConfigureToolGenericHostActions = new();

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

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            ToolConnect? c = default; if(Connect) { c = new ToolConnect(); this.builder?.Services.AddSingleton<IToolConnect>(c); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton<IToolConnect>(c)); } Guid i = Guid.NewGuid(); IToolHostLifetime? l = default;

            if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l!));

            if(Link) { this.builder?.Services.AddSingleton(l); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton(l)); } IHost? h = default;

            if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); } InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            var _ = new ToolGenericHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration,host:(host ?? h),lifeid:i,lifetime:l,logger:(host ?? h)?.Services.GetService<ILoggerFactory>());

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolGenericHost = _; }

            if(Start) { ConfigureToolGenericHost_NoSync(StartAction); } if(seal) { ConfigureToolGenericHost_NoSync(SealAction); } configuretool(_); configuretoolgenerichost(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolGenericHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolGenericHost BuildGenericHost<TToolGenericHost>(IHost? host = null) where TToolGenericHost : notnull , IToolGenericHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            ToolConnect? c = default; if(Connect) { c = new ToolConnect(); this.builder?.Services.AddSingleton<IToolConnect>(c); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton<IToolConnect>(c));  } Guid i = Guid.NewGuid(); IHost? h = default;

            IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l!));

            if(Link) { this.builder?.Services.AddSingleton(l); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton(l)); } if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); }

            InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolGenericHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,host ?? h).WithArgument(7,i).WithArgument(8,l).WithArgument(9,(host ?? h)?.Services.GetService<ILoggerFactory>());

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolGenericHost)ob.Value;

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolGenericHost = _; }

            if(Start) { ConfigureToolGenericHost_NoSync(StartAction); } if(seal) { ConfigureToolGenericHost_NoSync(SealAction); } configuretool(_); configuretoolgenerichost(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolGenericHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolGenericHost> BuildGenericHostAsync(IHost? host = null , CancellationToken cancel = default)
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            ToolConnect? c = default; if(Connect) { c = new ToolConnect(); this.builder?.Services.AddSingleton<IToolConnect>(c); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton<IToolConnect>(c)); } Guid i = Guid.NewGuid(); IToolHostLifetime? l = default;

            if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l!));

            if(Link) { this.builder?.Services.AddSingleton(l); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton(l)); } IHost? h = default;

            if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); } InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            var _ = new ToolGenericHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration,host:host ?? h,lifeid:i,lifetime:l,logger:(host ?? h)?.Services.GetService<ILoggerFactory>());

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolGenericHost = _; }

            configuretool(_); configuretoolgenerichost(_); if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolGenericHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolGenericHost> BuildGenericHostAsync<TToolGenericHost>(IHost? host = null , CancellationToken cancel = default) where TToolGenericHost : notnull , IToolGenericHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            ToolConnect? c = default; if(Connect) { c = new ToolConnect(); this.builder?.Services.AddSingleton<IToolConnect>(c); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton<IToolConnect>(c));  } Guid i = Guid.NewGuid(); IHost? h = default;

            IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l!));

            if(Link) { this.builder?.Services.AddSingleton(l); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton(l)); } if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); }

            InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolGenericHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,host ?? h).WithArgument(7,i).WithArgument(8,l).WithArgument(9,(host ?? h)?.Services.GetService<ILoggerFactory>());

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolGenericHost)ob.Value;

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolGenericHost = _; }

            configuretool(_!); configuretoolgenerichost(_!); if(Start) { await StartTool(_!,cancel).ConfigureAwait(false); } if(seal) { SealTool(_!); } return _!;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolGenericHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder ConfigureToolGenericHost(Action<ToolBuilderContext,IToolGenericHost> action)
    {
        Boolean sb = false;
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
        if(action is not null) { this.ConfigureToolGenericHostActions.Add(action); }
    }

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/method[@name="configuretoolgenerichost"]/*'/>*/
    protected void configuretoolgenerichost(IToolGenericHost toolgenerichost)
    {
        try
        {
            foreach(Action<ToolBuilderContext,IToolGenericHost> a in this.ConfigureToolGenericHostActions) { a(this.Context!,toolgenerichost); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolGenericHostFail); }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder ConnectWithApplication() { Connect = true; return this; }

    ///<inheritdoc/>
    public IToolGenericHostBuilder LinkLifetime() { Link = true; return this; }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseBuilderSettings(HostApplicationBuilderSettings? settings = null)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder = Host.CreateEmptyApplicationBuilder(settings ?? new()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseBuilderSettingsFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseConsoleLifetime() { Console = true; return this; }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseFullBuilder(HostApplicationBuilderSettings? settings = null)
    {
        Boolean sb = false;
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
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            this.builder = null; this.hostbuilder = Host.CreateDefaultBuilder(args); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseHostBuilderFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }
}