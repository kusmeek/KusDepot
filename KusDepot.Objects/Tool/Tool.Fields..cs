namespace KusDepot;

public partial class Tool : Common , IHostedLifecycleService , ITool
{
    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="AccessManager"]/*'/>*/
    protected IAccessManager? AccessManager;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Activities"]/*'/>*/
    protected List<Activity>? Activities;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="AttachedCommands"]/*'/>*/
    protected HashSet<ICommand>? AttachedCommands;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="CommandHandles"]/*'/>*/
    protected Dictionary<ICommand,HashSet<String>>? CommandHandles;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="CommandKeys"]/*'/>*/
    protected Dictionary<ICommand,CommandKey>? CommandKeys;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="CommandManagerKeys"]/*'/>*/
    protected Dictionary<ICommand,ManagerKey>? CommandManagerKeys;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Commands"]/*'/>*/
    protected Dictionary<String,ICommand>? Commands;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="CommandTypesMasked"]/*'/>*/
    protected Boolean CommandTypesMasked = true;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Configuration"]/*'/>*/
    protected IConfiguration? Configuration;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Data"]/*'/>*/
    protected HashSet<DataItem>? Data;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="DataIndex"]/*'/>*/
    protected Dictionary<Guid,DataItem>? DataIndex;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Disposed"]/*'/>*/
    protected Boolean Disposed;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostedServiceLockState"]/*'/>*/
    protected Dictionary<ITool,Boolean>? HostedServiceLockState;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostedServices"]/*'/>*/
    protected Dictionary<String,IHostedService>? HostedServices;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostedServiceNamesByInstance"]/*'/>*/
    protected Dictionary<IHostedService,String>? HostedServiceNamesByInstance;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostedServicesMasked"]/*'/>*/
    protected Boolean HostedServicesMasked = true;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostingKeys"]/*'/>*/
    protected Dictionary<ITool,HostKey>? HostingKeys;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostingMyHostKeys"]/*'/>*/
    protected Dictionary<ITool,MyHostKey>? HostingMyHostKeys;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostingManagerKeys"]/*'/>*/
    protected Dictionary<ITool,ManagerKey>? HostingManagerKeys;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostingOptions"]/*'/>*/
    protected ToolHostOptions? HostingOptions;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Inputs"]/*'/>*/
    protected Queue<Object>? Inputs;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/property[@name="Instances"]/*'/>*/
    protected static ConcurrentDictionary<Guid,WeakReference<ITool>> Instances { get; }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="LifeState"]/*'/>*/
    [NotNull]
    protected LifeCycleStateMachine? LifeState;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Logger"]/*'/>*/
    protected ILogger? Logger;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="MyHostKey"]/*'/>*/
    protected MyHostKey? MyHostKey;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Outputs"]/*'/>*/
    protected Dictionary<Guid,Object?>? Outputs;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="OutputWaiters"]/*'/>*/
    protected Dictionary<Guid,List<TaskCompletionSource<Object?>>>? OutputWaiters;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="OwnerSecret"]/*'/>*/
    protected Byte[]? OwnerSecret;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="SelfKey"]/*'/>*/
    protected ExecutiveKey? SelfKey;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Status"]/*'/>*/
    protected Dictionary<String,Object?>? Status;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Sync"]/*'/>*/
    protected new ToolSync Sync;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="ToolServiceProvider"]/*'/>*/
    protected ToolServiceProvider? ToolServiceProvider;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="ToolServices"]/*'/>*/
    protected IServiceProvider? ToolServices;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="ToolServiceScope"]/*'/>*/
    protected AsyncServiceScope? ToolServiceScope;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="WorkingSet"]/*'/>*/
    protected ConcurrentDictionary<String,Object?>? WorkingSet;
}