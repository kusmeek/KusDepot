namespace KusDepot;

/**<include file='Activity.xml' path='Activity/class[@name="Activity"]/main/*'/>*/
public sealed class Activity : IDisposable
{
    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Age"]/*'/>*/
    public TimeSpan                 Age => DateTimeOffset.Now - this.started!.Value;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Cancel"]/*'/>*/
    public CancellationTokenSource? Cancel   { get => this.cancel;   set  { this.cancel   ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Details"]/*'/>*/
    [NotNull]
    public CommandDetails?          Details  { get => this.details!; set  { this.details  ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="FreeMode"]/*'/>*/
    public Boolean                  FreeMode { get => this.freemode; init { this.freemode   = value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Handle"]/*'/>*/
    public String?                  Handle   { get => this.handle;   set  { this.handle   ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="ID"]/*'/>*/
    public Guid?                    ID       { get => this.id;       set  { this.id       ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Logger"]/*'/>*/
    public ILogger?                 Logger   { get => this.logger;   set  { this.logger   ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Progress"]/*'/>*/
    public IProgress<Object>?       Progress { get => this.progress; set  { this.progress ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Record"]/*'/>*/
    public ActivityRecord?          Record   { get => Volatile.Read(ref this.record); }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Started"]/*'/>*/
    public DateTimeOffset           Started  { get => this.started!.Value; set { this.started  ??= value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="State"]/*'/>*/
    public ActivityState            State    { get => GetState(); }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Task"]/*'/>*/
    public Task<Object?>?           Task
    {
        get => Volatile.Read(ref this.task);

        set { if(value is not null) { _ = Interlocked.CompareExchange(ref this.task,value,null); } }
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/property[@name="Timeout"]/*'/>*/
    public TimeSpan                 Timeout  { get => this.timeout; private set { this.timeout = value; } }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="cancel"]/*'/>*/
    private CancellationTokenSource? cancel;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="commandlifecycle"]/*'/>*/
    private Int32                    commandlifecycle;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="details"]/*'/>*/
    private CommandDetails?          details;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="freemode"]/*'/>*/
    private Boolean                  freemode;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="handle"]/*'/>*/
    private String?                  handle;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="id"]/*'/>*/
    private Guid?                    id;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="logger"]/*'/>*/
    private ILogger?                 logger;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="progress"]/*'/>*/
    private IProgress<Object>?       progress;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="record"]/*'/>*/
    private ActivityRecord?          record;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="started"]/*'/>*/
    private DateTimeOffset?          started;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="task"]/*'/>*/
    private Task<Object?>?           task;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/field[@name="timeout"]/*'/>*/
    private TimeSpan                 timeout;

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/constructor[@name="Constructor"]/*'/>*/
    public Activity() {}

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="CreateActivity"]/*'/>*/
    public static Activity? CreateActivity(CommandDetails? details , ICommand? command , Boolean asynchronous)
    {
        try
        {
            if(details is null || command is null || command is not Command cmd) { return null; }

            Boolean freemode = details.GetArgument<Boolean?>("FreeMode") ?? false;

            if(asynchronous)
            {
                if(cmd.ExecutionMode.AllowAsynchronous is false) { return null; }

                if(freemode && cmd.ExecutionMode.AllowFreeMode is false) { return null; }
            }
            else { if(freemode || cmd.ExecutionMode.AllowSynchronous is false) { return null; } }

            Activity a = new()
            {
                ID = details.ID , Details = details , Logger = details.GetLogger() , FreeMode = freemode
            };

            CancellationToken cancel = details.GetCancel(); CancellationToken toolcancel = details.GetToolCancel();
            
            TimeSpan timeout = details.GetTimeout() ?? ExecuteCommandTimeout;

            CancellationTokenSource cts = cancel.CanBeCanceled || toolcancel.CanBeCanceled
                ? CancellationTokenSource.CreateLinkedTokenSource(cancel,toolcancel)
                : new();

            a.Timeout = timeout; a.Cancel = cts; cts.CancelAfter(timeout); details.SetCancel(cts.Token);

            a.Started = DateTimeOffset.Now;

            return a;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CreateActivityFail); return null; }
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="CancelActivity"]/*'/>*/
    public Boolean CancelActivity()
    {
        try
        {
            if(this.cancel is null || this.cancel.IsCancellationRequested) { return false; }

            this.cancel.Cancel(); return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CancelFail); return false; }
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="Dispose"]/*'/>*/
    public void Dispose() { this.cancel?.Dispose(); }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="IsTimedOut"]/*'/>*/
    public Boolean IsTimedOut()
    {
        try
        {
            if(this.Details is null) { return false; }

            Boolean AC = this.Details.GetCancel().IsCancellationRequested;

            Boolean MC = this.Details.GetToolCancel().IsCancellationRequested;

            return AC && (MC is false) && (this.Age >= this.Timeout);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,IsTimedOutActivityFail); return false; }
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="TryClaimCommandLifecycle"]/*'/>*/
    internal Boolean TryClaimCommandLifecycle()
    {
        return Interlocked.CompareExchange(ref this.commandlifecycle,1,0) is 0;
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="TrySetTaskByCommand"]/*'/>*/
    internal Boolean TrySetTaskByCommand(Task<Object?> task)
    {
        if(Volatile.Read(ref this.commandlifecycle) is 0) { return false; }

        return Interlocked.CompareExchange(ref this.task,task,null) is null;
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="LogWorkflowEvent"]/*'/>*/
    public Boolean LogWorkflowEvent(String? action , String? data)
    {
        if(this.details is null) { throw new InvalidOperationException("Activity has no command details."); }

        if(this.details.Workflow is null) { this.details.Workflow = new CommandWorkflow(this.details); }

        this.details.Workflow.Details ??= this.details;

        return this.details.Workflow.LogEvent(action,data);
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="SetRecordByCommand"]/*'/>*/
    public Boolean SetRecordByCommand(ActivityRecordCode code , String? message = null) => this.SetRecordByCommand(code,message,null);

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="SetRecordByCommandWithException"]/*'/>*/
    public Boolean SetRecordByCommand(ActivityRecordCode code , String? message , Exception? exception) => this.SetRecordByCommand(code,message,exception,false);

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="SetRecordByCommandWithOutput"]/*'/>*/
    public Boolean SetRecordByCommand(ActivityRecordCode code , String? message , Exception? exception , Boolean hasoutput)
    {
        try
        {
            ActivityRecord record = new(code,message,exception,hasoutput);

            return Interlocked.CompareExchange(ref this.record,record,null) is null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetActivityRecordFail); return false; }
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="SetRecordByTool"]/*'/>*/
    public Boolean SetRecordByTool(ActivityRecordCode code , String? message = null) => this.SetRecordByTool(code,message,null);

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="SetRecordByToolWithException"]/*'/>*/
    public Boolean SetRecordByTool(ActivityRecordCode code , String? message , Exception? exception) => this.SetRecordByTool(code,message,exception,false);

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="SetRecordByToolWithOutput"]/*'/>*/
    public Boolean SetRecordByTool(ActivityRecordCode code , String? message , Exception? exception , Boolean hasoutput)
    {
        try
        {
            if(this.FreeMode || Volatile.Read(ref this.commandlifecycle) is not 0) { return false; }

            ActivityRecord record = new(code,message,exception,hasoutput);

            return Interlocked.CompareExchange(ref this.record,record,null) is null;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetActivityRecordFail); return false; }
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="StartWorkflowAction"]/*'/>*/
    public Boolean StartWorkflowAction(String? action = null , String? data = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(action);

        String _ = String.Concat("Start-",action);

        return this.LogWorkflowEvent(_,data);
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="StopWorkflowAction"]/*'/>*/
    public Boolean StopWorkflowAction(String? action = null , String? data = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(action);

        String _ = String.Concat("Stop-",action);

        return this.LogWorkflowEvent(_,data);
    }

    /**<include file='Activity.xml' path='Activity/class[@name="Activity"]/method[@name="GetState"]/*'/>*/
    private ActivityState GetState()
    {
        try
        {
            ActivityRecord? record = Volatile.Read(ref this.record); Task<Object?>? task = Volatile.Read(ref this.task);

            if(record is not null)
            {
                ActivityState __ = record.Code switch
                {
                    ActivityRecordCode.Success  => ActivityState.Success,
                    ActivityRecordCode.Failed   => ActivityState.Failed,
                    ActivityRecordCode.Faulted  => ActivityState.Faulted,
                    ActivityRecordCode.Canceled => ActivityState.Canceled,
                    ActivityRecordCode.TimedOut => ActivityState.TimedOut,
                    _                           => ActivityState.None
                };

                if(task is not null && (task.Status is
                    TaskStatus.Created or TaskStatus.WaitingForActivation or TaskStatus.WaitingToRun or
                    TaskStatus.Running or TaskStatus.WaitingForChildrenToComplete))
                {
                    if( __ is ActivityState.Canceled) { return ActivityState.CanceledRunning; }

                    if( __ is ActivityState.TimedOut) { return ActivityState.TimedOutRunning; }
                }

                return __;
            }

            if(task is null) { return ActivityState.None; }

            switch(task.Status)
            {
                case TaskStatus.Created:
                case TaskStatus.WaitingForActivation:
                case TaskStatus.WaitingToRun:
                case TaskStatus.Running:
                case TaskStatus.WaitingForChildrenToComplete:
                {
                    return this.FreeMode ? ActivityState.RunningFreeMode : ActivityState.Running;
                }

                case TaskStatus.RanToCompletion:
                {
                    return this.FreeMode ? ActivityState.CompletedFreeMode : ActivityState.Completed;
                }

                case TaskStatus.Canceled: { return ActivityState.Canceled; }

                case TaskStatus.Faulted: { return ActivityState.Faulted; }

                default: { return ActivityState.None; }
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetActivityStateFail); return ActivityState.None; }
    }
}