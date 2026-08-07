namespace KusDepot;

/**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/main/*'/>*/
public partial class ToolWebHost : ToolHost , IToolWebHost
{
    /**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/property[@name="WebApplication"]/*'/>*/
    protected IHost? WebApplication { get; set; }

    ///<inheritdoc/>
    public ICollection<String>? Urls { get; protected set; }

    /**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/property[@name="LifeSync"]/*'/>*/
    protected new SemaphoreSlim LifeSync { get; set; } = new(1,1);

    /**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/constructor[@name="Constructor"]/*'/>*/
    public ToolWebHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null,
           Guid? lifeid = null , IToolHostLifetime? lifetime = null , ILoggerFactory? logger = null , ICollection<String>? urls = null , IHost? webapplication = null) : base(accessmanager,data,id,services,commands,configuration,lifeid,lifetime,logger)
    {
        try
        {
            this.Urls = urls?.ToArray(); this.WebApplication = webapplication;

            this.Services = this.WebApplication?.Services ?? this.ToolServiceScope?.ServiceProvider; this.ResolveLogger(logger);
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='ToolWebHost.xml' path='ToolWebHost/class[@name="ToolWebHost"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolWebHost() : this(null,null,null,null,null,null,null,null,null,null,null){}

    ///<inheritdoc/>
    public IHost? GetManagedApplication(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.WebApplication; }

        catch ( Exception _ ) { Logger?.Error(_,GetManagedApplicationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    protected override Boolean ResolveLogger(ILoggerFactory? logger = null)
    {
        DC();
        try
        {
            if(this.Logger is not null) { return false; }

            this.Logger = (logger ?? this.WebApplication?.Services.GetService<ILoggerFactory>() ?? this.ToolServices?.GetService<ILoggerFactory>())?
                .CreateLogger(String.Format(ToolLoggerNameFormat,this.GetType().FullName,this.GetID()));

            if(this.Logger is not null) { return true; } return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,ResolveLoggerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }
}