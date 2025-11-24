namespace KusDepot;

/**<include file='Command.xml' path='Command/class[@name="Command"]/main/*'/>*/
public abstract class Command : Common , ICommand , IComparable<Command> , IEquatable<Command>
{
    /**<include file='Command.xml' path='Command/class[@name="Command"]/field[@name="Key"]/*'/>*/
    protected AccessKey? Key;

    /**<include file='Command.xml' path='Command/class[@name="Command"]/field[@name="enabled"]/*'/>*/
    protected Boolean enabled;

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

    ///<inheritdoc/>
    public virtual Guid? Execute(Activity? activity = null , AccessKey? key = null) { return null; }

    ///<inheritdoc/>
    public virtual Task<Guid?> ExecuteAsync(Activity? activity , AccessKey? key = null) { return Task.FromResult<Guid?>(null); }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="AccessCheck"]/*'/>*/
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected Boolean AccessCheck(AccessKey? key) { return Key is null ? true : Equals(Key,key); }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="AddActivity"]/*'/>*/
    protected Boolean AddActivity(Activity a)
    {
        try { this.Activities?.Add(a); return this.AttachedTool?.AddActivity(a,Key) ?? false; }

        catch { return false; }
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="RemoveActivity"]/*'/>*/
    protected Boolean RemoveActivity(Activity a)
    {
        try { this.Activities?.Remove(a); return this.AttachedTool?.RemoveActivity(a,Key) ?? false; }

        catch { return false; }
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="AddOutput"]/*'/>*/
    protected Boolean AddOutput(Guid? id = null , Object? output = null)
    {
        try { return this.AttachedTool?.AddOutput(id,output,Key) ?? false; }

        catch { return false; }
    }

    ///<inheritdoc/>
    public virtual Boolean Attach(ITool tool , AccessKey key)
    {
        if( (Locked && AccessCheck(key) is false) || ( AttachedTool is not null && Equals(AttachedTool,tool) is false ) || this.Enabled is true ) { return false; }

        AttachedTool = tool; Key = key; return true;
    }

    /**<include file='Command.xml' path='Command/class[@name="Command"]/method[@name="CleanUp"]/*'/>*/
    public virtual void CleanUp(Activity? activity = null) { if(activity is not null) { this.RemoveActivity(activity); activity.Dispose(); } }

    ///<inheritdoc/>
    public virtual Boolean Detach(AccessKey? key = null)
    {
        if( (Locked && AccessCheck(key) is false) || this.Enabled is true ) { return false; }

        AttachedTool = null; Key?.ClearKey(); Key = null; return true;
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> Disable(CancellationToken cancel = default , AccessKey? key = null)
    {
        if(Locked && AccessCheck(key) is false) { return Task.FromResult(false); }

        enabled = false; return Task.FromResult(true);
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> Enable(CancellationToken cancel = default , AccessKey? key = null)
    {
        if(Locked && AccessCheck(key) is false) { return Task.FromResult(false); }

        this.enabled = true; this.ExecutionMode.Lock(); return Task.FromResult(true);
    }

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
    public override Int32 GetHashCode()
    {
        try { return HashCode.Combine(this.ID); }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }
}