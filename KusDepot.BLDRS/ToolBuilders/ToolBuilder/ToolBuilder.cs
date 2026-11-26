namespace KusDepot.Builders;

/**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/main/*'/>*/
public partial class ToolBuilder : IToolBuilder
{
    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="seal"]/*'/>*/
    protected Boolean seal;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="Start"]/*'/>*/
    protected Boolean Start;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="BuildStarted"]/*'/>*/
    protected Boolean BuildStarted;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="Context"]/*'/>*/
    protected ToolBuilderContext? Context;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="AccessManager"]/*'/>*/
    protected IAccessManager? AccessManager;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ExternalServiceProvider"]/*'/>*/
    protected IServiceProvider? ExternalServiceProvider;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ExternalServiceProviderOwnership"]/*'/>*/
    protected ToolServiceProviderOwnership ExternalServiceProviderOwnership = default;

    ///<inheritdoc/>
    public Dictionary<Object,Object> ToolProperties { get; } = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="RegisteredCommands"]/*'/>*/
    protected readonly List<RegisteredCommand> RegisteredCommands = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ConfigureToolActions"]/*'/>*/
    protected readonly List<ConfigureToolAction> ConfigureToolActions = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ConfigureToolServicesActions"]/*'/>*/
    protected readonly List<ConfigureToolServicesAction> ConfigureToolServicesActions = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ConfigureToolConfigurationActions"]/*'/>*/
    protected readonly List<Action<ToolBuilderContext,IConfigurationBuilder>> ConfigureToolConfigurationActions = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="RegisteredServiceInstances"]/*'/>*/
    protected readonly Dictionary<String,IHostedService> RegisteredServiceInstances = new(StringComparer.Ordinal);

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="RegisteredHostedTools"]/*'/>*/
    protected readonly List<RegisteredHostedTool> RegisteredHostedTools = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="Sync"]/*'/>*/
    protected readonly SemaphoreSlim Sync = new(1,1);

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolBuilder() { try { this.BuildStarted = false; } catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConstructorFail); } }

    ///<inheritdoc/>
    public IToolBuilder AutoStart() { DC(); Start = true; return this; }

    ///<inheritdoc/>
    public ITool Build()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(this.BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration(); this.BuildStarted = true;

            var _ = new Tool(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:this.Context!.Configuration); SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            configuretool(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public TTool Build<TTool>() where TTool : notnull , ITool , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(this.BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration(); this.BuildStarted = true;

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TTool)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (TTool)ob.Value; SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            configuretool(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public TTool Build<TTool>(ToolFactory<TTool> factory) where TTool : notnull , ITool , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(this.BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration(); this.BuildStarted = true;

            var _ = factory(AccessManager,BuildServiceProvider(),Context!.Configuration);

            if(_ is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            configuretool(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<ITool> BuildAsync(CancellationToken cancel = default)
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(this.BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration(); this.BuildStarted = true;

            var _ = new Tool(accessmanager:AccessManager,services:await BuildServiceProviderAsync(cancel).ConfigureAwait(false),commands:null,configuration:this.Context!.Configuration); SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            await configuretoolasync(_,cancel).ConfigureAwait(false); if(Start) { await StartToolAsync(_,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_,cancel).ConfigureAwait(false); }

            ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<TTool> BuildAsync<TTool>(CancellationToken cancel = default) where TTool : notnull , ITool , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(this.BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration(); this.BuildStarted = true;

            var sp = await BuildServiceProviderAsync(cancel).ConfigureAwait(false); var ob = ObjectBuilder.Create(typeof(TTool)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (TTool)ob.Value; SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            await configuretoolasync(_,cancel).ConfigureAwait(false); if(Start) { await StartToolAsync(_,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_,cancel).ConfigureAwait(false); }

            ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<TTool> BuildAsync<TTool>(ToolFactory<TTool> factory , CancellationToken cancel = default) where TTool : notnull , ITool , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(this.BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration(); this.BuildStarted = true;

            var _ = factory(AccessManager,await BuildServiceProviderAsync(cancel).ConfigureAwait(false),Context!.Configuration);

            if(_ is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            await configuretoolasync(_,cancel).ConfigureAwait(false); if(Start) { await StartToolAsync(_,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_,cancel).ConfigureAwait(false); }

            ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="BuildServiceProvider"]/*'/>*/
    protected ToolServiceProvider BuildServiceProvider()
    {
        try
        {
            ToolServiceProvider? result;

            if(this.ExternalServiceProvider is not null)
            {
                result = new(this.ExternalServiceProvider,this.ExternalServiceProviderOwnership);

                this.BuildServiceProviderInstance = result; return result;
            }

            ServiceCollection s = new(); if(this.Context!.Configuration is not null) { s.AddSingletonWithInterfaces(this.Context.Configuration); }

            foreach(ConfigureToolServicesAction a in this.ConfigureToolServicesActions) { a.Execute(this.Context!,s); }

            IServiceProvider p = s.BuildServiceProvider(); result = new(p,ToolServiceProviderOwnership.Tool);

            this.BuildServiceProviderInstance = result; return result;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderBuildServiceProviderFail); throw; }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="BuildServiceProviderAsync"]/*'/>*/
    protected async Task<ToolServiceProvider> BuildServiceProviderAsync(CancellationToken cancel = default)
    {
        try
        {
            ToolServiceProvider? result;

            if(this.ExternalServiceProvider is not null)
            {
                result = new(this.ExternalServiceProvider,this.ExternalServiceProviderOwnership);

                this.BuildServiceProviderInstance = result; return result;
            }

            ServiceCollection s = new(); if(this.Context!.Configuration is not null) { s.AddSingletonWithInterfaces(this.Context.Configuration); }

            foreach(ConfigureToolServicesAction a in this.ConfigureToolServicesActions) { await a.ExecuteAsync(this.Context!,s,cancel).ConfigureAwait(false); }

            IServiceProvider p = s.BuildServiceProvider(); result = new(p,ToolServiceProviderOwnership.Tool);

            this.BuildServiceProviderInstance = result; return result;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderBuildServiceProviderFail); throw; }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureToolServices(Action<ToolBuilderContext,IServiceCollection> action)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureToolServices_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolServicesFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureToolServices(Func<ToolBuilderContext,IServiceCollection,CancellationToken,Task> action)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureToolServices_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolServicesFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="ConfigureToolServices_NoSync"]/*'/>*/
    protected void ConfigureToolServices_NoSync(Action<ToolBuilderContext,IServiceCollection> action)
    {
        if(action is not null) { this.ConfigureToolServicesActions.Add(new(action)); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="ConfigureToolServicesAsync_NoSync"]/*'/>*/
    protected void ConfigureToolServices_NoSync(Func<ToolBuilderContext,IServiceCollection,CancellationToken,Task> action)
    {
        if(action is not null) { this.ConfigureToolServicesActions.Add(new(action)); }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureTool(Action<ToolBuilderContext,ITool> action)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureTool_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureTool(Func<ToolBuilderContext,ITool,CancellationToken,Task> action)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureTool_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="ConfigureTool_NoSync"]/*'/>*/
    protected void ConfigureTool_NoSync(Action<ToolBuilderContext,ITool> action)
    {
        if(action is not null) { this.ConfigureToolActions.Add(new(action)); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="ConfigureToolAsync_NoSync"]/*'/>*/
    protected void ConfigureTool_NoSync(Func<ToolBuilderContext,ITool,CancellationToken,Task> action)
    {
        if(action is not null) { this.ConfigureToolActions.Add(new(action)); }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureToolConfiguration(Action<ToolBuilderContext,IConfigurationBuilder> action)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureToolConfiguration_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolConfigurationFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="ConfigureToolConfiguration_NoSync"]/*'/>*/
    protected void ConfigureToolConfiguration_NoSync(Action<ToolBuilderContext,IConfigurationBuilder> action)
    {
        if(action is not null) { this.ConfigureToolConfigurationActions.Add(action); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="configuretool"]/*'/>*/
    protected void configuretool(ITool tool)
    {
        foreach(ConfigureToolAction a in this.ConfigureToolActions) { a.Execute(this.Context!,tool); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="configuretoolasync"]/*'/>*/
    protected async Task configuretoolasync(ITool tool , CancellationToken cancel = default)
    {
        foreach(ConfigureToolAction a in this.ConfigureToolActions) { await a.ExecuteAsync(this.Context!,tool,cancel).ConfigureAwait(false); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="CreateHostLifetime"]/*'/>*/
    protected IToolHostLifetime? CreateHostLifetime(Boolean console)
    {
        if(this.ExternalServiceProvider is not null) { return null; }

        IToolHostLifetime lifetime = console ? new ToolHostConsoleLifetime() : new ToolHostLifetime();

        this.ConfigureToolServices_NoSync((x,s) => s.AddSingletonWithInterfaces(lifetime)); return lifetime;
    }

    ///<inheritdoc/>
    public IToolBuilder EnableAllCommands() { return this.ConfigureTool(EnableAllCommandsAction); }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="EnableAllCommandsAction"]/*'/>*/
    protected readonly Action<ToolBuilderContext,ITool> EnableAllCommandsAction = (x,t) =>
    {
        try { t.EnableAllCommands().GetAwaiter().GetResult(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderEnableAllCommandsActionFail); throw; }
    };

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="InitializeConfiguration"]/*'/>*/
    protected void InitializeConfiguration()
    {
        try
        {
            if(Equals(this.ConfigureToolConfigurationActions.Count,0)) { return; }

            IConfigurationBuilder _ = new ConfigurationBuilder().AddInMemoryCollection();

            foreach(Action<ToolBuilderContext,IConfigurationBuilder> a in this.ConfigureToolConfigurationActions) { a(this.Context!,_); }

            this.Context!.Configuration = _.Build();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderInitializeConfigurationFail); throw; }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="InitializeToolBuilderContext"]/*'/>*/
    protected void InitializeToolBuilderContext()
    {
        try { this.Context = new ToolBuilderContext(ToolProperties); }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderInitializeToolBuilderContextFail); throw; }
    }

    ///<inheritdoc/>
    public IToolBuilder Seal() { DC(); seal = true; return this; }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="SealTool"]/*'/>*/
    protected virtual void SealTool(ITool tool)
    {
        OwnerKey? s = tool.CreateOwnerKey("Seal")!; tool.Lock(s); s.ClearKey();

        if(tool.GetLocked() is not true) { throw new SecurityException(BuilderSealFail); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="SealToolAsync"]/*'/>*/
    protected virtual Task SealToolAsync(ITool tool , CancellationToken cancel = default)
    {
        SealTool(tool); return Task.CompletedTask;
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="StartTool"]/*'/>*/
    protected virtual void StartTool(ITool tool) => tool.StartHostAsync().GetAwaiter().GetResult();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="StartToolAsync"]/*'/>*/
    protected virtual async Task StartToolAsync(ITool tool , CancellationToken cancel = default) => await tool.StartHostAsync(cancel).ConfigureAwait(false);

    ///<inheritdoc/>
    public IToolBuilder UseAccessManager(IAccessManager accessmanager)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            UseAccessManager_NoSync(accessmanager); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseAccessManagerFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder UseAccessManager<TAccessManager>() where TAccessManager : notnull , IAccessManager , new()
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            UseAccessManager_NoSync(new TAccessManager()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseAccessManagerFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="UseAccessManager_NoSync"]/*'/>*/
    protected void UseAccessManager_NoSync(IAccessManager accessmanager) => this.AccessManager = accessmanager;

    ///<inheritdoc/>
    public IToolBuilder UseLogger(ILoggerFactory logger)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(logger is not null) { ConfigureToolServices_NoSync((x,s) => s.AddSingletonWithInterfaces(logger)); }

            return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseLoggerFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder UseServiceProvider(IServiceProvider? serviceprovider , ToolServiceProviderOwnership ownership = default)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(serviceprovider is null) { return this; }

            UseServiceProvider_NoSync(serviceprovider,ownership); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseServiceProviderFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="UseServiceProvider_NoSync"]/*'/>*/
    protected void UseServiceProvider_NoSync(IServiceProvider serviceprovider , ToolServiceProviderOwnership ownership = default)
    {
        this.ExternalServiceProvider = serviceprovider; this.ExternalServiceProviderOwnership = ownership;
    }
}