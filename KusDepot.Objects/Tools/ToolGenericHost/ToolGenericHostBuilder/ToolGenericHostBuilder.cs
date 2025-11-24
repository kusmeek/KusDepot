namespace KusDepot;

/**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/main/*'/>*/
public class ToolGenericHostBuilder : ToolBuilder , IToolGenericHostBuilder
{
    private Boolean Link;

    private Boolean Connect;

    private Boolean Console;

    private IHostBuilder? hostbuilder;

    private HostApplicationBuilder? builder;

    ///<inheritdoc/>
    public IToolGenericHost BuildGenericHost(IHost? host = null)
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            ToolConnect? c = default; if(Connect) { c = new ToolConnect(); this.builder?.Services.AddSingleton<IToolConnect>(c); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton<IToolConnect>(c)); } Guid i = Guid.NewGuid(); IToolHostLifetime? l = default;

            if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } if(Link) { this.builder?.Services.AddSingleton(l); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton(l)); } IHost? h = default;

            if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); } InitializeToolBuilderContext(); InitializeConfiguration(); InitializeServiceProvider(); ContainerBuilder!.RegisterInstance(l).AsImplementedInterfaces(); ToolBuilt = true;

            var _ = new ToolGenericHost(accessmanager:AccessManager,commands:Commands.ToArgument(),container:ContainerBuilder!.Build(),configuration:Context!.Configuration,host:host ?? h,lifeid:i,lifetime:l);

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolGenericHost = _; }

            if(Start) { ConfigureToolGenericHost(StartAction); } if(seal) { ConfigureToolGenericHost(SealAction); } configuretool(_); configuretoolgenerichost(_); return _;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderFail); return new ToolGenericHost(); }
    }

    ///<inheritdoc/>
    public async Task<IToolGenericHost> BuildGenericHostAsync(IHost? host = null , CancellationToken cancel = default)
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            ToolConnect? c = default; if(Connect) { c = new ToolConnect(); this.builder?.Services.AddSingleton<IToolConnect>(c); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton<IToolConnect>(c)); } Guid i = Guid.NewGuid(); IToolHostLifetime? l = default;

            if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } if(Link) { this.builder?.Services.AddSingleton(l); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton(l)); } IHost? h = default;

            if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); } InitializeToolBuilderContext(); InitializeConfiguration(); InitializeServiceProvider(); ContainerBuilder!.RegisterInstance(l).AsImplementedInterfaces(); ToolBuilt = true;

            var _ = new ToolGenericHost(accessmanager:AccessManager,commands:Commands.ToArgument(),container:ContainerBuilder!.Build(),configuration:Context!.Configuration,host:host ?? h,lifeid:i,lifetime:l);

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolGenericHost = _; }

            configuretool(_); configuretoolgenerichost(_); if(Start) { await StartTool(_,cancel); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderFail); return new ToolGenericHost(); }
    }

    ///<inheritdoc/>
    public IToolGenericHost BuildGenericHost<TToolGenericHost>(IHost? host = null) where TToolGenericHost : notnull , IToolGenericHost , new()
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            ToolConnect? c = default; if(Connect) { c = new ToolConnect(); this.builder?.Services.AddSingleton<IToolConnect>(c); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton<IToolConnect>(c));  } Guid i = Guid.NewGuid(); IHost? h = default;

            IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } if(Link) { this.builder?.Services.AddSingleton(l); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton(l));  } if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); }

            InitializeToolBuilderContext(); InitializeConfiguration(); InitializeServiceProvider(); ContainerBuilder!.RegisterInstance(l).AsImplementedInterfaces(); ToolBuilt = true; IToolGenericHost? _ = default;

            try { _ = (IToolGenericHost?)Activator.CreateInstance(typeof(TToolGenericHost),AccessManager,null,null,Commands.ToArgument(),Context!.Configuration,ContainerBuilder!.Build(),host ?? h,null,i,l,null); }

            catch { _ = new ToolGenericHost(accessmanager:AccessManager,commands:Commands.ToArgument(),container:ContainerBuilder!.Build(),configuration:Context!.Configuration,host:host ?? h,lifeid:i,lifetime:l); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolGenericHost = _; }

            if(Start) { ConfigureToolGenericHost(StartAction); } if(seal) { ConfigureToolGenericHost(SealAction); } configuretool(_!); configuretoolgenerichost(_!); return _!;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderFail); return new TToolGenericHost(); }
    }

    ///<inheritdoc/>
    public async Task<IToolGenericHost> BuildGenericHostAsync<TToolGenericHost>(IHost? host = null , CancellationToken cancel = default) where TToolGenericHost : notnull , IToolGenericHost , new()
    {
        try
        {
            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            ToolConnect? c = default; if(Connect) { c = new ToolConnect(); this.builder?.Services.AddSingleton<IToolConnect>(c); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton<IToolConnect>(c));  } Guid i = Guid.NewGuid(); IHost? h = default;

            IToolHostLifetime? l; if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); } if(Link) { this.builder?.Services.AddSingleton(l); this.hostbuilder?.ConfigureServices(s=> s.AddSingleton(l));  } if(host is null) { h = this.builder?.Build() ?? this.hostbuilder?.Build(); }

            InitializeToolBuilderContext(); InitializeConfiguration(); InitializeServiceProvider(); ContainerBuilder!.RegisterInstance(l).AsImplementedInterfaces(); ToolBuilt = true; IToolGenericHost? _ = default;

            try { _ = (IToolGenericHost?)Activator.CreateInstance(typeof(TToolGenericHost),AccessManager,null,null,Commands.ToArgument(),Context!.Configuration,ContainerBuilder!.Build(),host ?? h,null,i,l,null); }

            catch { _ = new ToolGenericHost(accessmanager:AccessManager,commands:Commands.ToArgument(),container:ContainerBuilder!.Build(),configuration:Context!.Configuration,host:host ?? h,lifeid:i,lifetime:l); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; } if(Connect) { c!.ToolGenericHost = _; }

            configuretool(_!); configuretoolgenerichost(_!); if(Start) { await StartTool(_!,cancel); } if(seal) { SealTool(_!); } return _!;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderFail); return new TToolGenericHost(); }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseFullBuilder(HostApplicationBuilderSettings? settings = null)
    {
        try
        {
            this.hostbuilder = null; builder = settings is null ? Host.CreateApplicationBuilder(new HostApplicationBuilderSettings()) : Host.CreateApplicationBuilder(settings); return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderUseFullBuilderFail); return this; }
    }

    /**<include file='ToolGenericHostBuilder.xml' path='ToolGenericHostBuilder/class[@name="ToolGenericHostBuilder"]/method[@name="configuretoolgenerichost"]/*'/>*/
    protected void configuretoolgenerichost(IToolGenericHost toolgenerichost)
    {
        try
        {
            foreach(Action<ToolBuilderContext,IToolGenericHost> a in this.ConfigureToolGenericHostActions) { a(this.Context!,toolgenerichost); }
        }
        catch ( Exception _ ) { Log.Error(_,BuilderConfigureToolGenericHostFail); }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseBuilderSettings(HostApplicationBuilderSettings? settings = null)
    {
        try
        {
            builder = Host.CreateEmptyApplicationBuilder(settings ?? new()); return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderUseBuilderSettingsFail); return this; }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseMcpServerDefaults(Assembly? toolassembly = null)
    {
        try
        {
            if(toolassembly is null)

            { this.Builder.Services.AddMcpServer().WithToolsFromAssembly().WithStdioServerTransport(); } else

            { this.Builder.Services.AddMcpServer().WithToolsFromAssembly(toolassembly).WithStdioServerTransport(); }

            return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderUseMcpServerDefaultsFail); return this; }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseHostBuilder(String[]? args = null)
    {
        try
        {
            this.builder = null; this.hostbuilder = Host.CreateDefaultBuilder(args); return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderUseHostBuilderFail); return this; }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseOrleans(Action<ISiloBuilder> configureorleans)
    {
        try
        {
            this.Builder.UseOrleans(configureorleans); return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderUseOrleansFail); return this; }
    }

    ///<inheritdoc/>
    public ToolGenericHostBuilder()
    {
        try
        {
            builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());
        }
        catch ( Exception _ ) { Log.Error(_,BuilderConstructorFail); }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseFullBuilder(String[]? args = null)
    {
        try
        {
            builder = Host.CreateApplicationBuilder(args); return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderUseFullBuilderFail); return this; }
    }

    ///<inheritdoc/>
    public override IToolBuilder UseSemanticKernel(Kernel? kernel = null)
    {
        try
        {
            if(kernel is not null)
            {
                this.builder?.Services.AddSingleton(kernel);

                this.hostbuilder?.ConfigureServices((s) => s.AddSingleton(kernel));

                return this;
            }

            this.hostbuilder?.ConfigureServices((s) => s.AddTransient<Kernel>());

            this.builder?.Services.AddTransient<Kernel>();

            return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderUseSemanticKernelFail); return this; }
    }

    ///<inheritdoc/>
    public IToolGenericHostBuilder ConfigureToolGenericHost(Action<ToolBuilderContext,IToolGenericHost> action)
    {
        try
        {
            if(action is not null) { this.ConfigureToolGenericHostActions.Add(action); } return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderConfigureToolGenericHostFail); return this; }
    }

    private readonly List<Action<ToolBuilderContext,IToolGenericHost>> ConfigureToolGenericHostActions = new();


    ///<inheritdoc/>
    public IToolGenericHostBuilder ConnectWithApplication() { Connect = true; return this; }

    ///<inheritdoc/>
    public IToolGenericHostBuilder UseConsoleLifetime() { Console = true; return this; }

    ///<inheritdoc/>
    public IToolGenericHostBuilder LinkLifetime() { Link = true; return this; }

    ///<inheritdoc/>
    public HostApplicationBuilder Builder { get { return builder!; } }

    ///<inheritdoc/>
    public IHostBuilder HostBuilder { get { return hostbuilder!; } }
}