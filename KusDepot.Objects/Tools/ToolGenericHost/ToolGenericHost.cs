namespace KusDepot;

/**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/main/*'/>*/
public partial class ToolGenericHost : ToolHost , IToolGenericHost
{
    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/property[@name="Host"]/*'/>*/
    protected IHost? Host { get; set; }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/property[@name="HostKey"]/*'/>*/
    protected HostKey? HostKey { get; set; }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/property[@name="HostManagerKey"]/*'/>*/
    protected ManagerKey? HostManagerKey { get; set; }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/property[@name="LifeSync"]/*'/>*/
    protected new SemaphoreSlim LifeSync { get; set; } = new(1,1);

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/constructor[@name="Constructor"]/*'/>*/
    public ToolGenericHost(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null,
           IHost? host = null , Guid? lifeid = null , IToolHostLifetime? lifetime = null , ILoggerFactory? logger = null ) : base(accessmanager,data,id,services,commands,configuration,lifeid,lifetime,logger)
    {
        try
        {
            this.Host = host; this.Services = this.Host?.Services ?? this.ToolServiceScope?.ServiceProvider;

            this.ResolveLogger(logger); if(this.RequestHostKey() is true) { this.LockHost(); }
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ToolGenericHost() : this(null,null,null,null,null,null,null,null,null,null){}

    ///<inheritdoc/>
    protected override Boolean DestroySecrets(AccessKey? accesskey)
    {
        if(accesskey is null || Equals(this.SelfKey,accesskey) is false) { return false; }

        HostKey?.ClearKey(); HostManagerKey?.ClearKey();

        return base.DestroySecrets(accesskey);
    }

    ///<inheritdoc/>
    public virtual IHost? GetManagedApplication(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.Host; }

        catch ( Exception _ ) { Logger?.Error(_,GetManagedApplicationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/method[@name="LockHost"]/*'/>*/
    protected virtual Boolean LockHost()
    {
        DC();
        try
        {
            if( Host is null || Host as IToolHost is not IToolHost t ) { return true; }

            var c = CreateCertificate(t,"HostManagerKey"); if(c is null) { return false; } HostManagerKey = new ManagerKey(c);

            return t.RegisterManager(HostManagerKey) && t.Lock(HostManagerKey) ? true : throw new SecurityException(LockHostFail);
        }
        catch ( Exception _ ) { Logger?.Error(_,LockHostFail,MyTypeName,MyID); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='ToolGenericHost.xml' path='ToolGenericHost/class[@name="ToolGenericHost"]/method[@name="RequestHostKey"]/*'/>*/
    protected virtual Boolean RequestHostKey()
    {
        DC();
        try
        {
            if( Host is null || Host as IToolHost is not IToolHost t ) { return true; }

            HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey;

            return HostKey is not null ? true : throw new SecurityException(RequestHostKeyFail);
        }
        catch ( Exception _ ) { Logger?.Error(_,RequestHostKeyFail,MyTypeName,MyID); if(NoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    protected override Boolean ResolveLogger(ILoggerFactory? logger = null)
    {
        DC();
        try
        {
            if(this.Logger is not null) { return false; }

            this.Logger = (logger ?? this.Host?.Services.GetService<ILoggerFactory>() ?? this.ToolServices?.GetService<ILoggerFactory>())?
                .CreateLogger(String.Format(ToolLoggerNameFormat,this.GetType().FullName,this.GetID()));

            if(this.Logger is not null) { return true; } return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,ResolveLoggerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }
}