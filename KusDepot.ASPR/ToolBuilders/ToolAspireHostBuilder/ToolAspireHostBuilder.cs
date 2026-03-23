namespace KusDepot.Builders;

/**<include file='ToolAspireHostBuilder.xml' path='ToolAspireHostBuilder/class[@name="ToolAspireHostBuilder"]/main/*'/>*/
public partial class ToolAspireHostBuilder : ToolBuilder , IToolAspireHostBuilder
{
    private Boolean Link;

    private Boolean Connect;

    private Boolean Console;

    private IDistributedApplicationBuilder? builder;

    private readonly List<Action<ToolBuilderContext,IToolAspireHost>> ConfigureToolAspireHostActions = new();

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

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l!));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); }

            InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); } else { a = app; }

            var _ = new ToolAspireHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration,aspireapplication:a,lifeid:i,lifetime:l,logger:a.Services.GetService<ILoggerFactory>());

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolAspireHost = _; }

            if(Start) { ConfigureToolAspireHost_NoSync(StartAction); } if(seal) { ConfigureToolAspireHost_NoSync(SealAction); } configuretool(_); configuretoolaspirehost(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolAspireHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolAspireHost BuildAspireHost<TToolAspireHost>(DistributedApplication? app = null) where TToolAspireHost : notnull , IToolAspireHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l!));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); }

            InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); } else { a = app; }

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolAspireHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,i).WithArgument(7,l).WithArgument(8,a.Services.GetService<ILoggerFactory>()).WithArgument(9,a);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolAspireHost)ob.Value;

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolAspireHost = _; }

            if(Start) { ConfigureToolAspireHost_NoSync(StartAction); } if(seal) { ConfigureToolAspireHost_NoSync(SealAction); } configuretool(_); configuretoolaspirehost(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolAspireHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolAspireHost> BuildAspireHostAsync(DistributedApplication? app = null , CancellationToken cancel = default)
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l!));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); }

            InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); } else { a = app; }

            var _ = new ToolAspireHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration,aspireapplication:a,lifeid:i,lifetime:l,logger:a.Services.GetService<ILoggerFactory>());

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolAspireHost = _; }

            configuretool(_); configuretoolaspirehost(_); if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolAspireHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolAspireHost> BuildAspireHostAsync<TToolAspireHost>(DistributedApplication? app = null , CancellationToken cancel = default) where TToolAspireHost : notnull , IToolAspireHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            Guid i = Guid.NewGuid(); IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l!));

            ToolConnect? c = default; if(Connect) { c = new(); Builder.Services.AddSingleton<IToolConnect>(c); } if(Link) { Builder.Services.AddSingleton(l); }

            InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true; DistributedApplication? a; if(app is null) { a = Builder.Build(); } else { a = app; }

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolAspireHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,i).WithArgument(7,l).WithArgument(8,a.Services.GetService<ILoggerFactory>()).WithArgument(9,a);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolAspireHost)ob.Value;

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolAspireHost = _; }

            configuretool(_!); configuretoolaspirehost(_!); if(Start) { await StartTool(_!,cancel).ConfigureAwait(false); } if(seal) { SealTool(_!); } return _!;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolAspireHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolAspireHostBuilder ConfigureToolAspireHost(Action<ToolBuilderContext,IToolAspireHost> action)
    {
        Boolean sb = false;
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
        if(action is not null) { this.ConfigureToolAspireHostActions.Add(action); }
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

    ///<inheritdoc/>
    public IToolAspireHostBuilder ConnectWithApplication() { Connect = true; return this; }

    ///<inheritdoc/>
    public IToolAspireHostBuilder LinkLifetime() { Link = true; return this; }

    ///<inheritdoc/>
    public IToolAspireHostBuilder UseBuilderArguments(String[]? args = null)
    {
        Boolean sb = false;
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
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            builder = options is null ? DistributedApplication.CreateBuilder(new DistributedApplicationOptions()) : DistributedApplication.CreateBuilder(options); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseBuilderOptionsFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolAspireHostBuilder UseConsoleLifetime() { Console = true; return this; }
}