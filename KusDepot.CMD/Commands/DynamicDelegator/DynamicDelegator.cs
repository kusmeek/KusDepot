namespace KusDepot.Commands;

/**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/main/*'/>*/
public sealed class DynamicDelegator : Command
{
    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/property[@name="CommandExecutor"]/*'/>*/
    private Func<ITool,Activity,CommandKey,ValueTask<Activity?>> CommandExecutor { get; set; }

    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public DynamicDelegator(Func<ITool,Activity,CommandKey,ValueTask<Activity?>> executor) { CommandExecutor = executor; ExecutionMode.AllowAll(); }

    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/constructor[@name="ConstructorTask"]/*'/>*/
    public DynamicDelegator(Func<ITool,Activity,CommandKey,Task<Activity?>> executor) { CommandExecutor = (t,a,k) => new ValueTask<Activity?>(executor(t,a,k)); ExecutionMode.AllowAll(); }

    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/constructor[@name="ConstructorSync"]/*'/>*/
    public DynamicDelegator(Func<ITool,Activity,CommandKey,Activity?> executor) { CommandExecutor = (t,a,k) => new ValueTask<Activity?>(executor(t,a,k)); ExecutionMode.AllowAll(); }

    ///<inheritdoc/>
    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            AddActivity(a); LogExecuting(a,DynamicDelegatorExecuting,a.Details?.Handle);

            var t = CommandExecutor(AttachedTool!,a,key!);

            Activity? r = t.IsCompleted ? t.Result : t.AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            if(r is not null) { SetSuccess(a); return a.ID; } else { SetFailed(a); AddOutput(a.ID); return null; }
        }
        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a?.ID); return null; }

        finally { CleanUp(a); }
    }

    ///<inheritdoc/>
    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            a.Task = WorkAsync(a,key);

            if(a.FreeMode is true) { return a.ID; } else { await a.Task.ConfigureAwait(false); return a.ID; }
        }
        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a?.ID); return null; }

        finally { CleanUp(a); }
    }

    private async Task<Object?> WorkAsync(Activity a , CommandKey? key)
    {
        try
        {
            AddActivity(a); LogExecuting(a,DynamicDelegatorExecuting,a.Details?.Handle);

            var r = await CommandExecutor(AttachedTool!,a,key!).ConfigureAwait(false);

            if(r is not null) { SetSuccess(a); return true; } else { SetFailed(a); AddOutput(a.ID); return false; }
        }
        catch ( OperationCanceledException ) { SetCanceled(a); AddOutput(a.ID); return false; }

        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a.ID); return false; }

        finally { this.CleanUpFreeMode(a); }
    }
}