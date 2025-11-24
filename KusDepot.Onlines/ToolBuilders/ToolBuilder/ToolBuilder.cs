namespace KusDepot;

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

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="Commands"]/*'/>*/
    protected Dictionary<String,ICommand>? Commands = new();

    ///<inheritdoc/>
    public Dictionary<Object,Object> ToolProperties { get; } = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ConfigureToolActions"]/*'/>*/
    protected readonly List<Action<ToolBuilderContext,ITool>> ConfigureToolActions = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ConfigureServicesActions"]/*'/>*/
    protected readonly List<Action<ToolBuilderContext,IServiceCollection>> ConfigureServicesActions = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ConfigureToolConfigActions"]/*'/>*/
    protected readonly List<Action<ToolBuilderContext,IConfigurationBuilder>> ConfigureToolConfigActions = new();

    ///<inheritdoc/>
    public ITool Build()
    {
        try
        {
            if(this.ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration();

            this.ToolBuilt = true; if(Start) { ConfigureTool(StartAction); } if(seal) { ConfigureTool(SealAction); }

            var _ = new Tool(accessmanager:AccessManager,services:BuildServiceProvider(),commands:Commands,configuration:this.Context!.Configuration);

            configuretool(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new Tool(); }
    }

    ///<inheritdoc/>
    public async Task<ITool> BuildAsync(CancellationToken cancel = default)
    {
        try
        {
            if(this.ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration();

            this.ToolBuilt = true;

            var _ = new Tool(accessmanager:AccessManager,services:BuildServiceProvider(),commands:Commands,configuration:this.Context!.Configuration);

            configuretool(_); if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new Tool(); }
    }

    ///<inheritdoc/>
    public TTool Build<TTool>() where TTool : notnull , ITool , new()
    {
        try
        {
            if(this.ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration();

            this.ToolBuilt = true; TTool? _ = default; if(Start) { ConfigureTool(StartAction); } if(seal) { ConfigureTool(SealAction); }

            _ = (TTool)Activator.CreateInstance(typeof(TTool),AccessManager,null,null,BuildServiceProvider(),Commands,Context!.Configuration,null)!; configuretool(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TTool(); }
    }

    ///<inheritdoc/>
    public async Task<TTool> BuildAsync<TTool>(CancellationToken cancel = default) where TTool : notnull , ITool , new()
    {
        try
        {
            if(this.ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration();

            this.ToolBuilt = true; TTool? _ = default;

            _ = (TTool)Activator.CreateInstance(typeof(TTool),AccessManager,null,null,BuildServiceProvider(),Commands,Context!.Configuration,null)!; configuretool(_);

            if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TTool(); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolBuilder() { try { this.ToolBuilt = false; } catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConstructorFail); } }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="InitializeToolBuilderContext"]/*'/>*/
    protected void InitializeToolBuilderContext()
    {
        try { this.Context = new ToolBuilderContext(ToolProperties); }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderInitializeToolBuilderContextFail); }
    }

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

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="BuildServiceProvider"]/*'/>*/
    protected ToolServiceProvider BuildServiceProvider()
    {
        try
        {
            if(this.ExternalServiceProvider is not null) { return new ToolServiceProvider(this.ExternalServiceProvider); }

            ServiceCollection s = new(); if(this.Context!.Configuration is not null) { s.AddSingletonWithInterfaces(this.Context.Configuration); }

            foreach(Action<ToolBuilderContext,IServiceCollection> a in this.ConfigureServicesActions) { a(this.Context!,s); }

            return new ToolServiceProvider(s.BuildServiceProvider());
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderBuildServiceProviderFail); throw; }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureServices(Action<ToolBuilderContext,IServiceCollection> action)
    {
        try { if(action is not null) { this.ConfigureServicesActions.Add(action); } return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureServicesFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureTool(Action<ToolBuilderContext,ITool> action)
    {
        try { if(action is not null) { this.ConfigureToolActions.Add(action); } return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolFail); return this; }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="configuretool"]/*'/>*/
    protected void configuretool(ITool tool)
    {
        try { foreach(Action<ToolBuilderContext,ITool> a in this.ConfigureToolActions) { a(this.Context!,tool); } }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolFail); }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureToolConfiguration(Action<ToolBuilderContext,IConfigurationBuilder> action)
    {
        try { if(action is not null) { this.ConfigureToolConfigActions.Add(action); } return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolConfigurationFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterTool(ITool tool)
    {
        return tool is not null ? this.ConfigureServices((x,s) => s.AddSingletonWithInterfaces(tool)) : this;
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterTool<TTool>() where TTool : class , ITool , new()
    {
        return this.ConfigureServices((x,s) => s.AddSingletonWithInterfaces<TTool>());
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterCommand(String handle , ICommand command)
    {
        try
        {
            if( String.IsNullOrEmpty(handle) || command is null || this.Commands!.ContainsKey(handle) ) { return this; }

            this.Commands.Add(handle,command); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterCommandFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterCommand<TCommand>(String handle) where TCommand : notnull , ICommand , new()
    {
        try
        {
            if( String.IsNullOrEmpty(handle) || this.Commands!.ContainsKey(handle) ) { return this; }

            this.Commands.Add(handle,Activator.CreateInstance<TCommand>()!); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterCommandFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterCommand(String handle , Type commandtype)
    {
        try
        {
            if( String.IsNullOrEmpty(handle) || commandtype is null || this.Commands!.ContainsKey(handle)) { return this; }

            if(commandtype.IsAssignableTo(typeof(ICommand)) is false) { return this; }

            this.Commands.Add(handle,(ICommand)Activator.CreateInstance(commandtype)!); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderRegisterCommandFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder UseAccessManager(IAccessManager accessmanager)
    {
        try { this.AccessManager = accessmanager; return this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseAccessManagerFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder UseAccessManager<TAccessManager>() where TAccessManager : notnull , IAccessManager , new()
    {
        try { return this.UseAccessManager(Activator.CreateInstance<TAccessManager>()); }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseAccessManagerFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder UseLogger(ILoggerFactory logger)
    {
        try { return logger is not null ? this.ConfigureServices((x,s) => s.AddSingletonWithInterfaces(logger)) : this; }

        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseLoggerFail); return this; }
    }

    ///<inheritdoc/>
    public virtual IToolBuilder UseSemanticKernel(Kernel? kernel = null)
    {
        try
        {
            if(kernel is not null)
            {
                this.ConfigureServices((x,s) => s.AddSingleton(kernel)); return this;
            }
            this.ConfigureServices((x,s) => s.AddTransient<Kernel>()); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseSemanticKernelFail); return this; }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="UseServiceProvider"]/*'/>*/
    public IToolBuilder UseServiceProvider(IServiceProvider? serviceprovider)
    {
        try
        {
            if(serviceprovider is null) { return this; }

            this.ExternalServiceProvider = serviceprovider; return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseServiceProviderFail); return this; }
    }

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
    public IToolBuilder AutoStart() { Start = true; return this; }

    ///<inheritdoc/>
    public IToolBuilder Seal() { seal = true; return this; }
}