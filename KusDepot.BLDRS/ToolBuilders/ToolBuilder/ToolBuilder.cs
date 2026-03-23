namespace KusDepot.Builders;

/**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/main/*'/>*/
public partial class ToolBuilder : IToolBuilder
{
    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="seal"]/*'/>*/
    protected Boolean seal;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="Start"]/*'/>*/
    protected Boolean Start;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ToolBuilt"]/*'/>*/
    protected Boolean ToolBuilt;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="Context"]/*'/>*/
    protected ToolBuilderContext? Context;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="AccessManager"]/*'/>*/
    protected IAccessManager? AccessManager;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ExternalServiceProvider"]/*'/>*/
    protected IServiceProvider? ExternalServiceProvider;

    ///<inheritdoc/>
    public Dictionary<Object,Object> ToolProperties { get; } = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="RegisteredCommands"]/*'/>*/
    protected readonly List<RegisteredCommand> RegisteredCommands = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ConfigureToolActions"]/*'/>*/
    protected readonly List<Action<ToolBuilderContext,ITool>> ConfigureToolActions = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ConfigureServicesActions"]/*'/>*/
    protected readonly List<Action<ToolBuilderContext,IServiceCollection>> ConfigureServicesActions = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ConfigureToolConfigActions"]/*'/>*/
    protected readonly List<Action<ToolBuilderContext,IConfigurationBuilder>> ConfigureToolConfigActions = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="RegisteredServiceInstances"]/*'/>*/
    protected readonly Dictionary<String,IHostedService> RegisteredServiceInstances = new(StringComparer.Ordinal);

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="RegisteredHostedTools"]/*'/>*/
    protected readonly List<RegisteredHostedTool> RegisteredHostedTools = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="Sync"]/*'/>*/
    protected readonly SemaphoreSlim Sync = new(1,1);

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolBuilder() { try { this.ToolBuilt = false; } catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConstructorFail); } }

    ///<inheritdoc/>
    public IToolBuilder AutoStart() { Start = true; return this; }

    ///<inheritdoc/>
    public ITool Build()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            if(this.ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration();

            this.ToolBuilt = true; if(Start) { ConfigureTool_NoSync(StartAction); } if(seal) { ConfigureTool_NoSync(SealAction); }

            var _ = new Tool(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:this.Context!.Configuration);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            configuretool(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new Tool(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public TTool Build<TTool>() where TTool : notnull , ITool , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            if(this.ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration();

            this.ToolBuilt = true; if(Start) { ConfigureTool_NoSync(StartAction); } if(seal) { ConfigureTool_NoSync(SealAction); }

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TTool)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (TTool)ob.Value;

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            configuretool(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TTool(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<ITool> BuildAsync(CancellationToken cancel = default)
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            if(this.ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration();

            this.ToolBuilt = true;

            var _ = new Tool(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:this.Context!.Configuration);

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            configuretool(_); if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new Tool(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<TTool> BuildAsync<TTool>(CancellationToken cancel = default) where TTool : notnull , ITool , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            if(this.ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration();

            this.ToolBuilt = true;

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TTool)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (TTool)ob.Value;

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            configuretool(_); if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TTool(); }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="BuildServiceProvider"]/*'/>*/
    protected ToolServiceProvider BuildServiceProvider()
    {
        try
        {
            if(this.ExternalServiceProvider is not null) { return new(this.ExternalServiceProvider); }

            ServiceCollection s = new(); if(this.Context!.Configuration is not null) { s.AddSingletonWithInterfaces(this.Context.Configuration); }

            foreach(Action<ToolBuilderContext,IServiceCollection> a in this.ConfigureServicesActions) { a(this.Context!,s); }

            return new(s.BuildServiceProvider());
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderBuildServiceProviderFail); throw; }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureServices(Action<ToolBuilderContext,IServiceCollection> action)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureServices_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureServicesFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="ConfigureServices_NoSync"]/*'/>*/
    protected void ConfigureServices_NoSync(Action<ToolBuilderContext,IServiceCollection> action)
    {
        if(action is not null) { this.ConfigureServicesActions.Add(action); }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureTool(Action<ToolBuilderContext,ITool> action)
    {
        Boolean sb = false;
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
        if(action is not null) { this.ConfigureToolActions.Add(action); }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureToolConfiguration(Action<ToolBuilderContext,IConfigurationBuilder> action)
    {
        Boolean sb = false;
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
        if(action is not null) { this.ConfigureToolConfigActions.Add(action); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="configuretool"]/*'/>*/
    protected void configuretool(ITool tool)
    {
        try { foreach(Action<ToolBuilderContext,ITool> a in this.ConfigureToolActions) { a(this.Context!,tool); } }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolFail); }
    }

    ///<inheritdoc/>
    public IToolBuilder EnableAllCommands() { return this.ConfigureTool(EnableAllCommandsAction); }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="EnableAllCommandsAction"]/*'/>*/
    protected readonly Action<ToolBuilderContext,ITool> EnableAllCommandsAction = (x,t) =>
    {
        try { t.EnableAllCommands().GetAwaiter().GetResult(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderEnableAllCommandsActionFail); }
    };

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="InitializeConfiguration"]/*'/>*/
    protected void InitializeConfiguration()
    {
        try
        {
            if(Equals(this.ConfigureToolConfigActions.Count,0)) { return; }

            IConfigurationBuilder _ = new ConfigurationBuilder().AddInMemoryCollection();

            foreach(Action<ToolBuilderContext,IConfigurationBuilder> a in this.ConfigureToolConfigActions) { a(this.Context!,_); }

            this.Context!.Configuration = _.Build();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderInitializeConfigurationFail); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="InitializeToolBuilderContext"]/*'/>*/
    protected void InitializeToolBuilderContext()
    {
        try { this.Context = new ToolBuilderContext(ToolProperties); }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderInitializeToolBuilderContextFail); }
    }

    ///<inheritdoc/>
    public IToolBuilder Seal() { seal = true; return this; }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="SealTool"]/*'/>*/
    protected virtual Boolean SealTool(ITool tool) { try { SealAction(Context!,tool); return true; } catch { return false; } }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="SealAction"]/*'/>*/
    protected readonly Action<ToolBuilderContext,ITool> SealAction = (x,t) =>
    {
        try
        {
            OwnerKey? s = t.CreateOwnerKey("Seal")!; t.Lock(s); s.ClearKey();

            if(t.GetLocked() is not true) { throw new SecurityException(BuilderSealFail); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderSealActionFail); }
    };

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="StartTool"]/*'/>*/
    protected virtual async Task<Boolean> StartTool(ITool tool , CancellationToken cancel = default)
    {
        return await tool.StartHostAsync(cancel).ConfigureAwait(false);
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="StartAction"]/*'/>*/
    protected readonly Action<ToolBuilderContext,ITool> StartAction = (x,t) =>
    {
        try { t.StartHostAsync().GetAwaiter().GetResult(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderStartActionFail); }
    };

    ///<inheritdoc/>
    public IToolBuilder UseAccessManager(IAccessManager accessmanager)
    {
        Boolean sb = false;
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
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            UseAccessManager_NoSync(Activator.CreateInstance<TAccessManager>()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseAccessManagerFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="UseAccessManager_NoSync"]/*'/>*/
    protected void UseAccessManager_NoSync(IAccessManager accessmanager)
    {
        this.AccessManager = accessmanager;
    }

    ///<inheritdoc/>
    public IToolBuilder UseLogger(ILoggerFactory logger)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(logger is not null) { ConfigureServices_NoSync((x,s) => s.AddSingletonWithInterfaces(logger)); }

            return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseLoggerFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolBuilder UseServiceProvider(IServiceProvider? serviceprovider)
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            if(serviceprovider is null) { return this; }

            UseServiceProvider_NoSync(serviceprovider); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseServiceProviderFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="UseServiceProvider_NoSync"]/*'/>*/
    protected void UseServiceProvider_NoSync(IServiceProvider serviceprovider)
    {
        this.ExternalServiceProvider = serviceprovider;
    }
}