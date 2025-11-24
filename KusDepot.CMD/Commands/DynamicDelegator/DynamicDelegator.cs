namespace KusDepot.Commands;

/**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/main/*'/>*/
public sealed class DynamicDelegator : Command
{
    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/property[@name="CommandExecutor"]/*'/>*/
    private Func<ITool,Activity,AccessKey,ValueTask<Activity?>> CommandExecutor { get; set; }

    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public DynamicDelegator(Func<ITool,Activity,AccessKey,ValueTask<Activity?>> executor) { CommandExecutor = executor; ExecutionMode.AllowBoth(); }

    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/constructor[@name="ConstructorTask"]/*'/>*/
    public DynamicDelegator(Func<ITool,Activity,AccessKey,Task<Activity?>> executor) { CommandExecutor = (t,a,k) => new ValueTask<Activity?>(executor(t,a,k)); ExecutionMode.AllowBoth(); }

    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/constructor[@name="ConstructorSync"]/*'/>*/
    public DynamicDelegator(Func<ITool,Activity,AccessKey,Activity?> executor) { CommandExecutor = (t,a,k) => new ValueTask<Activity?>(executor(t,a,k)); ExecutionMode.AllowBoth(); }

    ///<inheritdoc/>
    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            AddActivity(a); a.Logger?.Information(DynamicDelegatorExecuting,a.Details?.Handle);

            var t = CommandExecutor(AttachedTool!,a,key ?? Key!);

            Activity? r = t.IsCompleted ? t.Result : t.AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            if(r is not null) { return a.ID; } else { AddOutput(a.ID); return null; }
        }
        catch ( Exception _ ) { a?.Logger?.Error(_,CommandExecuteFail,GetType().Name,a?.Details?.Handle); AddOutput(a?.ID); return null; }

        finally { CleanUp(a); }
    }

    ///<inheritdoc/>
    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            async Task<Object?> WorkAsync()
            {
                try
                {
                    AddActivity(a); a.Logger?.Information(DynamicDelegatorExecuting,a.Details?.Handle);

                    var r = await CommandExecutor(AttachedTool!,a,key ?? Key!).ConfigureAwait(false);

                    if(r is not null) { return true; } else { AddOutput(a.ID); return false; }
                }
                catch ( OperationCanceledException ) { AddOutput(a.ID); return false; }

                catch ( Exception _ ) { a.Logger?.Error(_,CommandExecuteFail,GetType().Name,a.Details?.Handle); AddOutput(a.ID); return false; }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch ( Exception _ ) { a?.Logger?.Error(_,CommandExecuteFail,GetType().Name,a?.Details?.Handle); AddOutput(a?.ID); return null; }

        finally { CleanUp(a); }
    }
}