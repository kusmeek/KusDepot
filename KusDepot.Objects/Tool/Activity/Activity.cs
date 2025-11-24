namespace KusDepot;

/**<include file='Activity.xml' path='Activity/class[@name="Activity"]/main/*'/>*/
public sealed class Activity : IDisposable
{
    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Age"]/*'/>*/
    public TimeSpan                 Age      { get { return DateTimeOffset.Now - this.started; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Cancel"]/*'/>*/
    public CancellationTokenSource? Cancel   { get { return this.cancel;   } set { this.cancel   ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Details"]/*'/>*/
    public CommandDetails?          Details  { get { return this.details;  } set { this.details  ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Handle"]/*'/>*/
    public String?                  Handle   { get { return this.handle;   } set { this.handle   ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="ID"]/*'/>*/
    public Guid?                    ID       { get { return this.id;       } set { this.id       ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Logger"]/*'/>*/
    public ILogger?                 Logger   { get { return this.logger;   } set { this.logger   ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Progress"]/*'/>*/
    public IProgress<Object>?       Progress { get { return this.progress; } set { this.progress ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Started"]/*'/>*/
    public DateTimeOffset           Started  { get { return this.started;  } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Task"]/*'/>*/
    public Task<Object?>?           Task     { get { return this.task;     } set { this.task     ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="cancel"]/*'/>*/
    private CancellationTokenSource? cancel;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="details"]/*'/>*/
    private CommandDetails?          details;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="handle"]/*'/>*/
    private String?                  handle;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="id"]/*'/>*/
    private Guid?                    id;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="logger"]/*'/>*/
    private ILogger?                 logger;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="progress"]/*'/>*/
    private IProgress<Object>?       progress;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="started"]/*'/>*/
    private readonly DateTimeOffset  started;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="task"]/*'/>*/
    private Task<Object?>?           task;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/constructor[@name="Constructor"]/*'/>*/
    public Activity(Task<Object?>? task = null , String? handle = null , Guid? id = null , ILogger? logger = null , DateTimeOffset? started = null , IProgress<Object>? progress = null , CancellationTokenSource? cancel = null)
    {
         this.cancel = cancel; this.handle = handle; this.id = id; this.logger = logger; this.progress = progress; this.started = started ?? DateTimeOffset.Now; this.task = task;
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="CreateActivity"]/*'/>*/
    public static Activity? CreateActivity(CommandDetails? details)
    {
        try
        {
            if(details is null) { return null; }

            Activity a = new()
            {
                ID = details.ID,

                Details = details,

                Logger = details.GetArgument<ILogger?>("Logger")
            };

            TimeSpan? Timeout = details.GetArgument<TimeSpan?>("Timeout") ?? DefaultCommandTimeout;

            CancellationToken? Cancel = details.GetArgument<CancellationToken?>("Cancel") ?? CancellationToken.None;

            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(Cancel.Value); cts.CancelAfter(Timeout.Value); a.Cancel = cts; return a;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CreateActivityFail); return null; }
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="LogWorkflowEvent"]/*'/>*/
    public Boolean LogWorkflowEvent(String? action , String? data)
    {
        try
        {
            if(this.details is null) { return false; } if(String.IsNullOrEmpty(action)) { return false; }

            if(this.details.Workflow is null) { this.details.Workflow = new CommandWorkflow(this.details); }

            this.details.Workflow.Details ??= this.details;

            return this.details.Workflow.LogEvent(action,data);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,LogWorkflowEventFail); return false; }
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="StartWorkflowAction"]/*'/>*/
    public Boolean StartWorkflowAction(String? action = null , String? data = null)
    {
        try
        {
            if(String.IsNullOrEmpty(action)) { return false; }

            String _ = String.Concat("Start-",action);

            return this.LogWorkflowEvent(_,data);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,StartWorkflowActionFail); return false; }
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="StopWorkflowAction"]/*'/>*/
    public Boolean StopWorkflowAction(String? action = null , String? data = null)
    {
        try
        {
            if(String.IsNullOrEmpty(action)) { return false; }

            String _ = String.Concat("Stop-",action);

            return this.LogWorkflowEvent(_,data);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,StopWorkflowActionFail); return false; }
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="Dispose"]/*'/>*/
    public void Dispose() { this.cancel?.Dispose(); }
}