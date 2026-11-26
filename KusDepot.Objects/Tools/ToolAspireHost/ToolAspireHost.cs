namespace KusDepot;

/**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/main/*'/>*/
public partial class ToolAspireHost : ToolHost , IToolAspireHost
{
    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/property[@name="AspireApplication"]/*'/>*/
    protected IHost? AspireApplication { get; set; }

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/property[@name="LifeSync"]/*'/>*/
    protected new SemaphoreSlim LifeSync { get; set; } = new(1,1);

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/constructor[@name="Constructor"]/*'/>*/
    public ToolAspireHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null,
           Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null , ILoggerFactory? logger = null , IHost? aspireapplication = null)
           : base(accessmanager,data,id,services,commands,configuration,logger)
    {
        try
        {
            this.AspireApplication = aspireapplication;

            this.Services = this.AspireApplication?.Services ?? this.ToolServices; this.ResolveLogger(logger);
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='ToolAspireHost.xml' path='ToolAspireHost/class[@name="ToolAspireHost"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolAspireHost() : this(null,null,null,null,null,null,null,null){}

    ///<inheritdoc/>
    public IHost? GetManagedApplication(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.AspireApplication; }

        catch ( Exception _ ) { Logger?.Error(_,GetManagedApplicationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    protected override Boolean ResolveLogger(ILoggerFactory? logger = null)
    {
        DC();
        try
        {
            ILoggerFactory? al = this.AspireApplication?.Services.GetService<ILoggerFactory>();

            ILoggerFactory? sl = logger ?? al ?? this.ToolServices?.GetService<ILoggerFactory>();

            if(this.Logger is not null && (logger is not null || al is null)) { return false; }

            if(sl is not null) { this.Logger = sl.CreateLogger(String.Format(ToolLoggerNameFormat,this.GetType().FullName,this.GetID())); }

            this.AssignLifetimeLogger();

            if(this.Logger is not null) { return true; } return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,ResolveLoggerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }
}