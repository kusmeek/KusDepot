namespace KusDepot;

/**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/main/*'/>*/
public partial class ToolHost : Tool , IToolHost
{
    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/field[@name="LifeKey"]/*'/>*/
    private Guid LifeKey;

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/property[@name="Lifetime"]/*'/>*/
    protected IToolHostLifetime? Lifetime {get; set;}

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/property[@name="LifeSync"]/*'/>*/
    protected SemaphoreSlim LifeSync { get; set; } = new(1,1);

    ///<inheritdoc/>
    [NotNull]
    public IServiceProvider? Services { get { return this.GetLocked() is true ? null! : field!; } protected set; }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/constructor[@name="Constructor"]/*'/>*/
    public ToolHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null,
           Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null , ILoggerFactory? logger = null)
           : base(accessmanager,data,id,services,commands,configuration,logger)
    {
        try
        {
            this.Services = this.ToolServices;

            this.Lifetime = this.ToolServiceProvider?.Ownership is ToolServiceProviderOwnership.Tool
                ? this.ToolServices?.GetService<IToolHostLifetime>() ?? new ToolHostLifetime()
                : new ToolHostLifetime();

            this.LifeKey = this.Lifetime.Bind(this); this.AssignLifetimeLogger();
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolHost() : this(null,null,null,null,null,null,null){}

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/method[@name="AssignLifetimeLogger"]/*'/>*/
    protected void AssignLifetimeLogger()
    {
        if(this.Lifetime is ToolHostLifetime l) { l.Logger = this.Logger; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetServices)]
    public virtual IServiceProvider? GetServices(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.Services; }

        catch ( Exception _ ) { Logger?.Error(_,GetServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }
}