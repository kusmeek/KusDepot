namespace KusDepot;

/**<include file='Command.xml' path='Command/class[@name="Command"]/main/*'/>*/
public abstract class Command : Common , ICommand , IComparable<Command> , IEquatable<Command>
{
    /**<include file='Command.xml' path='Command/class[@name="Command"]/field[@name="enabled"]/*'/>*/
    protected Boolean enabled;

    /**<include file='Command.xml' path='Command/class[@name="Command"]/field[@name="Key"]/*'/>*/
    protected CommandKey? Key;

    /**<include file='Command.xml' path='Command/class[@name="Command"]/field[@name="AttachedTool"]/*'/>*/
    protected ITool? AttachedTool;

    /**<include file='Command.xml' path='Command/class[@name="Command"]/field[@name="Activities"]/*'/>*/
    protected List<Activity>? Activities = new();

    ///<inheritdoc/>
    public Boolean Enabled { get => this.enabled; }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/property[@name="ExecutionMode"]/*'/>*/
    public CommandExecutionMode ExecutionMode { get; }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public Command() { try { this.Initialize(); this.ExecutionMode = new(); } catch { throw; } }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="AccessCheck"]/*'/>*/
    protected Boolean AccessCheck(CommandKey? key) { return Key is null ? true : Key.Equals(key); }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="AddActivity"]/*'/>*/
    protected Boolean AddActivity(Activity a)
    {
        try { Activities?.Add(a); return AttachedTool?.AddActivity(a,Key) ?? false; }

        catch { return false; }
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="AddOutput"]/*'/>*/
    protected Boolean AddOutput(Guid? id = null , Object? output = null)
    {
        try { return AttachedTool?.AddOutput(id,output,Key) ?? false; }

        catch { return false; }
    }

    ///<inheritdoc/>
    public virtual Boolean Attach(ITool tool , CommandKey key)
    {
        if( (Locked && AccessCheck(key) is false) || (AttachedTool is not null && Equals(AttachedTool,tool) is false ) || this.Enabled is true ) { return false; }

        Key = key?.Copy<CommandKey>(); AttachedTool = tool; return Key is not null && AttachedTool is not null;
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="CleanUp"]/*'/>*/
    protected virtual void CleanUp(Activity? activity = null)
    {
        if(activity is null || activity.FreeMode) { return; }

        if((activity.Details?.GetArgument<Boolean?>("!PreserveActivity") ?? false) is true) { return; }

        this.RemoveActivity(activity); activity.Dispose();
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="CleanUpFreeMode"]/*'/>*/
    protected virtual void CleanUpFreeMode(Activity? activity = null)
    {
        if(activity is null) { return; }

        if((activity.Details?.GetArgument<Boolean?>("!PreserveActivity") ?? false) is true) { return; }

        this.RemoveActivity(activity); activity.Dispose();
    }

    ///<inheritdoc/>
    public virtual Boolean Detach(CommandKey? key = null)
    {
        if( (Locked && AccessCheck(key) is false) || this.Enabled is true ) { return false; }

        Key!.ClearKey(); Key = null; AttachedTool = null; return true;
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> Disable(CancellationToken cancel = default , CommandKey? key = null)
    {
        if(AccessCheck(key) is false) { return Task.FromResult(false); }

        enabled = false; return Task.FromResult(true);
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> Enable(CancellationToken cancel = default , CommandKey? key = null)
    {
        if(AccessCheck(key) is false) { return Task.FromResult(false); }

        enabled = true; this.ExecutionMode.Lock(); return Task.FromResult(true);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="EnabledAllowed"]/*'/>*/
    protected Boolean EnabledAllowed(CommandKey? key) => Enabled is true && AccessCheck(key) is true; 

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is Command c ? this.CompareTo(c) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is Command c ? this.CompareTo(c) : base.CompareTo(other); }

    ///<inheritdoc/>
    public virtual Int32 CompareTo(ICommand? other) { return this.CompareTo(other as Command); }

    ///<inheritdoc/>
    public virtual Int32 CompareTo(Command? other)
    {
        try
        {
            if(ReferenceEquals(this,other))          { return 0; }

            Guid? _ = other?.GetID();

            if(this.ID is null     && _ is null)     { return 0; }
            if(this.ID is not null && _ is null)     { return 1; }
            if(this.ID is null     && _ is not null) { return -1; }

            return this.ID!.Value.CompareTo(_!.Value);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CompareToFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return Int32.MinValue; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean Equals(ICommand? other) { return this.Equals(other as Command); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as Command); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as Command); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as Command); }

    ///<inheritdoc/>
    public virtual Boolean Equals(Command? other)
    {
        try
        {
            if(other is null) { return false; }

            if(ReferenceEquals(this,other)) { return true; }

            if(Equals(this.GetType(),other.GetType()) is false) { return false; }

            return Guid.Equals(this.ID,other.GetID());
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EqualsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Guid? Execute(Activity? activity = null , CommandKey? key = null) { return null; }

    ///<inheritdoc/>
    public virtual Task<Guid?> ExecuteAsync(Activity? activity , CommandKey? key = null) { return Task.FromResult<Guid?>(null); }

    ///<inheritdoc/>
    public override Int32 GetHashCode()
    {
        try { return HashCode.Combine(this.ID); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public virtual CommandDescriptor? GetCommandDescriptor(CommandKey? key = null)
    {
        if(Locked && AccessCheck(key) is false) { return null; }

        return CommandDescriptor.Create(this);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="RemoveActivity"]/*'/>*/
    protected Boolean RemoveActivity(Activity a)
    {
        try
        {
            if(AttachedTool?.MyExceptionsEnabled() ?? false) { return false; }

            Activities?.Remove(a); return AttachedTool?.RemoveActivity(a,Key) ?? false;
        }

        catch { return false; }
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="LogCanceled"]/*'/>*/
    protected virtual void LogCanceled(Activity? activity , String? message = null)
    {
        activity?.Logger?.Error(CommandExecuteCancel,MyTypeName,activity?.Details?.Handle,message);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="LogCanceledException"]/*'/>*/
    protected virtual void LogCanceled(Activity? activity , Exception exception)
    {
        activity?.Logger?.Error(exception,CommandExecuteCancel,MyTypeName,activity?.Details?.Handle);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="LogFailed"]/*'/>*/
    protected virtual void LogFailed(Activity? activity , String? message = null)
    {
        activity?.Logger?.Error(CommandExecuteFail,MyTypeName,activity?.Details?.Handle,message);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="LogFaulted"]/*'/>*/
    protected virtual void LogFaulted(Activity? activity , Exception exception)
    {
        activity?.Logger?.Error(exception,CommandExecuteFault,MyTypeName,activity?.Details?.Handle);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="LogExecuting"]/*'/>*/
    protected virtual void LogExecuting(Activity? activity , String message , params Object?[] args)
    {
        activity?.Logger?.Information(message,args);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="LogSuccess"]/*'/>*/
    protected virtual void LogSuccess(Activity? activity , String? message = null)
    {
        activity?.Logger?.Information(CommandExecuteSuccess,MyTypeName,activity?.Details?.Handle,message);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="SetCanceled"]/*'/>*/
    protected virtual void SetCanceled(Activity? activity , String? message = null)
    {
        if(activity?.IsTimedOut() ?? false) { this.SetTimedOut(activity,message); }

        else { activity?.SetRecordByCommand(ActivityRecordCode.Canceled,message); }
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="SetFailed"]/*'/>*/
    protected virtual void SetFailed(Activity? activity , String? message = null)
    {
        activity?.SetRecordByCommand(ActivityRecordCode.Failed,message);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="SetFaulted"]/*'/>*/
    protected virtual void SetFaulted(Activity? activity , String? message = null)
    {
        activity?.SetRecordByCommand(ActivityRecordCode.Faulted,message);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="SetFaultedException"]/*'/>*/
    protected virtual void SetFaulted(Activity? activity , Exception exception)
    {
        activity?.SetRecordByCommand(ActivityRecordCode.Faulted,exception.Message);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="SetSuccess"]/*'/>*/
    protected virtual void SetSuccess(Activity? activity , String? message = null)
    {
        activity?.SetRecordByCommand(ActivityRecordCode.Success,message);
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="SetTimedOut"]/*'/>*/
    protected virtual void SetTimedOut(Activity? activity , String? message = null)
    {
        activity?.SetRecordByCommand(ActivityRecordCode.TimedOut,message);
    }
}