namespace KusDepot;

/**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/main/*'/>*/
public class ToolBuilder : IToolBuilder
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

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ContainerBuilder"]/*'/>*/
    protected ContainerBuilder? ContainerBuilder;

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="Commands"]/*'/>*/
    protected Dictionary<String,ICommand>? Commands = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ServiceProviderFactory"]/*'/>*/
    protected AutofacServiceProviderFactory? ServiceProviderFactory;

    ///<inheritdoc/>
    public Dictionary<Object,Object> ToolProperties { get; } = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ConfigureToolActions"]/*'/>*/
    protected readonly List<Action<ToolBuilderContext,ITool>> ConfigureToolActions = new();

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="ConfigureContainerActions"]/*'/>*/
    protected readonly List<Action<ToolBuilderContext,ContainerBuilder>> ConfigureContainerActions = new();

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

            InitializeToolBuilderContext(); InitializeConfiguration(); InitializeServiceProvider();

            this.ToolBuilt = true; if(Start) { ConfigureTool(StartAction); } if(seal) { ConfigureTool(SealAction); }

            var _ = new Tool(accessmanager:this.AccessManager,commands:this.Commands.ToArgument(),configuration:this.Context!.Configuration,container:this.ContainerBuilder!.Build());

            configuretool(_); return _;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderFail); return new Tool(); }
    }

    ///<inheritdoc/>
    public async Task<ITool> BuildAsync(CancellationToken cancel = default)
    {
        try
        {
            if(this.ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration(); InitializeServiceProvider();

            this.ToolBuilt = true;

            var _ = new Tool(accessmanager:this.AccessManager,commands:this.Commands.ToArgument(),configuration:this.Context!.Configuration,container:this.ContainerBuilder!.Build());

            configuretool(_); if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderFail); return new Tool(); }
    }

    ///<inheritdoc/>
    public TTool Build<TTool>() where TTool : notnull , ITool , new()
    {
        try
        {
            if(this.ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration(); InitializeServiceProvider(); this.ToolBuilt = true; TTool? _ = default; if(Start) { ConfigureTool(StartAction); } if(seal) { ConfigureTool(SealAction); }

            try { _ = (TTool)Activator.CreateInstance(typeof(TTool),this.AccessManager,null,null,this.Commands.ToArgument(),this.Context!.Configuration,this.ContainerBuilder!.Build(),null,null)!; configuretool(_); return _; }

            catch { _ = Activator.CreateInstance<TTool>()!; configuretool(_); return _; }
        }
        catch ( Exception _ ) { Log.Error(_,BuilderFail); return new TTool(); }
    }

    ///<inheritdoc/>
    public async Task<TTool> BuildAsync<TTool>(CancellationToken cancel = default) where TTool : notnull , ITool , new()
    {
        try
        {
            if(this.ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            InitializeToolBuilderContext(); InitializeConfiguration(); InitializeServiceProvider(); this.ToolBuilt = true; TTool? _ = default;

            try { _ = (TTool)Activator.CreateInstance(typeof(TTool),this.AccessManager,null,null,this.Commands.ToArgument(),this.Context!.Configuration,this.ContainerBuilder!.Build(),null,null)!; configuretool(_); }

            catch { _ = Activator.CreateInstance<TTool>()!; configuretool(_); }

            if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderFail); return new TTool(); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolBuilder()
    {
        try
        {
            this.ServiceProviderFactory = new AutofacServiceProviderFactory(); this.ToolBuilt = false;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderConstructorFail); }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureServices(Action<ToolBuilderContext,IServiceCollection> action)
    {
        try
        {
            if(action is not null) { this.ConfigureServicesActions.Add(action); } return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderConfigureServicesFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureTool(Action<ToolBuilderContext,ITool> action)
    {
        try
        {
            if(action is not null) { this.ConfigureToolActions.Add(action); } return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderConfigureToolFail); return this; }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="configuretool"]/*'/>*/
    protected void configuretool(ITool tool)
    {
        try
        {
            foreach(Action<ToolBuilderContext,ITool> a in this.ConfigureToolActions) { a(this.Context!,tool); }
        }
        catch ( Exception _ ) { Log.Error(_,BuilderConfigureToolFail); }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureToolConfiguration(Action<ToolBuilderContext,IConfigurationBuilder> action)
    {
        try
        {
            if(action is not null) { this.ConfigureToolConfigActions.Add(action); } return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderConfigureToolConfigurationFail); return this; }
    }

    ///<inheritdoc/>
    public virtual IToolBuilder UseSemanticKernel(Kernel? kernel = null)
    {
        try
        {
            if(kernel is not null)
            {
                this.ConfigureContainer((x,b) => b.RegisterInstance(kernel).AsSelf().AsImplementedInterfaces()); return this;
            }
            this.ConfigureServices((x,s) => s.AddTransient<Kernel>()); return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderUseSemanticKernelFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder ConfigureContainer(Action<ToolBuilderContext,Autofac.ContainerBuilder> action)
    {
        try
        {
            if(action is not null) { this.ConfigureContainerActions.Add(action); } return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderConfigureContainerFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterCommand(String handle , ICommand command)
    {
        try
        {
            if( String.IsNullOrEmpty(handle) || command is null || this.Commands!.ContainsKey(handle) ) { return this; }

            this.Commands.Add(handle,command); return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderRegisterCommandFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder RegisterCommand<TCommand>(String handle) where TCommand : notnull , ICommand , new()
    {
        try
        {
            if( String.IsNullOrEmpty(handle) || this.Commands!.ContainsKey(handle) ) { return this; }

            this.Commands.Add(handle,Activator.CreateInstance<TCommand>()!); return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderRegisterCommandFail); return this; }
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
        catch ( Exception _ ) { Log.Error(_,BuilderRegisterCommandFail); return this; }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="InitializeToolBuilderContext"]/*'/>*/
    protected void InitializeToolBuilderContext()
    {
        try
        {
            this.Context = new ToolBuilderContext(ToolProperties);
        }
        catch ( Exception _ ) { Log.Error(_,BuilderInitializeToolBuilderContextFail); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="InitializeConfiguration"]/*'/>*/
    protected void InitializeConfiguration()
    {
        try
        {
            IConfigurationBuilder _ = new ConfigurationBuilder().AddInMemoryCollection();

            foreach(Action<ToolBuilderContext,IConfigurationBuilder> a in this.ConfigureToolConfigActions) { a(this.Context!,_); }

            this.Context!.Configuration = _.Build();
        }
        catch ( Exception _ ) { Log.Error(_,BuilderInitializeConfigurationFail); }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="InitializeServiceProvider"]/*'/>*/
    protected void InitializeServiceProvider()
    {
        try
        {
            ServiceCollection _ = new ServiceCollection();

            foreach(Action<ToolBuilderContext,IServiceCollection> a in this.ConfigureServicesActions) { a(this.Context!,_); }

            ContainerBuilder b = this.ServiceProviderFactory!.CreateBuilder(_); b.RegisterInstance<IConfiguration>(this.Context!.Configuration!);

            foreach(Action<ToolBuilderContext,ContainerBuilder> a in this.ConfigureContainerActions) { a(this.Context!,b); } this.ContainerBuilder = b;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderInitializeServiceProviderFail); }
    }

    ///<inheritdoc/>
    public IToolBuilder UseLogger(ILogger logger)
    {
        try
        {
            return logger is not null ? this.ConfigureContainer((x,b) => b.RegisterInstance(logger)) : this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderUseLoggerFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder UseAccessManager(IAccessManager accessmanager)
    {
        try
        {
            this.AccessManager = accessmanager; return this;
        }
        catch ( Exception _ ) { Log.Error(_,BuilderUseAccessManagerFail); return this; }
    }

    ///<inheritdoc/>
    public IToolBuilder UseAccessManager<TAccessManager>() where TAccessManager : notnull , IAccessManager , new()
    {
        try
        {
            return this.UseAccessManager(Activator.CreateInstance<TAccessManager>());
        }
        catch ( Exception _ ) { Log.Error(_,BuilderUseAccessManagerFail); return this; }
    }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="SealTool"]/*'/>*/
    protected virtual Boolean SealTool(ITool tool) { try { SealAction(Context!,tool); return true; } catch { return false; } }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="SealAction"]/*'/>*/
    protected readonly Action<ToolBuilderContext,ITool> SealAction = (x,t) =>
    {
        try
        {
            OwnerKey? s = t.CreateOwnerKey("Seal")!; t.Lock(s); s.ClearKey(); if(t.GetLocked() is not true) { throw new SecurityException(BuilderSealFail); }
        }
        catch ( Exception _ ) { Log.Error(_,BuilderSealActionFail); }
    };

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/method[@name="StartTool"]/*'/>*/
    protected virtual async Task<Boolean> StartTool(ITool tool , CancellationToken cancel = default) { return await tool.StartHostAsync(cancel).ConfigureAwait(false); }

    /**<include file='ToolBuilder.xml' path='ToolBuilder/class[@name="ToolBuilder"]/field[@name="StartAction"]/*'/>*/
    protected readonly Action<ToolBuilderContext,ITool> StartAction = (x,t) =>
    {
        try
        {
            t.StartHostAsync().GetAwaiter().GetResult();
        }
        catch ( Exception _ ) { Log.Error(_,BuilderStartActionFail); }
    };

    ///<inheritdoc/>
    public IToolBuilder AutoStart() { Start = true; return this; }

    ///<inheritdoc/>
    public IToolBuilder Seal() { seal = true; return this; }
}