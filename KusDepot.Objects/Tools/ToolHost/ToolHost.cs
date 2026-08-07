namespace KusDepot;

/**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/main/*'/>*/
public partial class ToolHost : Tool , IToolHost
{
    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/property[@name="LifeID"]/*'/>*/
    protected Guid LifeID { get; set; }

    ///<inheritdoc/>
    public IToolHostLifetime? Lifetime {get; protected set;}

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/property[@name="LifeSync"]/*'/>*/
    protected SemaphoreSlim LifeSync { get; set; } = new(1,1);

    ///<inheritdoc/>
    [NotNull]
    public IServiceProvider? Services { get { return this.GetLocked() is true ? null! : field!; } protected set => field = value; }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/constructor[@name="Constructor"]/*'/>*/
    public ToolHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null,
           IConfiguration? configuration = null , Guid? lifeid = null , IToolHostLifetime? lifetime = null , ILoggerFactory? logger = null) : base(accessmanager,data,id,services,commands,configuration,logger)
    {
        try
        {
            this.LifeID = lifeid ?? Guid.NewGuid();

            this.Lifetime = lifetime ?? new ToolHostLifetime(LifeID);

            this.Services = this.ToolServiceScope?.ServiceProvider;
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='ToolHost.xml' path='ToolHost/class[@name="ToolHost"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolHost() : this(null,null,null,null,null,null,null,null,null){}

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetServices)]
    public virtual IServiceProvider? GetServices(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.Services; }

        catch ( Exception _ ) { Logger?.Error(_,GetServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }
}