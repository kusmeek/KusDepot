namespace KusDepot.Commands;

/**<include file='CommandDelegator.xml' path='CommandDelegator/class[@name="CommandDelegator"]/main/*'/>*/
public sealed class CommandDelegator : Command
{
    /**<include file='CommandDelegator.xml' path='CommandDelegator/class[@name="CommandDelegator"]/property[@name="TargetHandle"]/*'/>*/
    public String TargetHandle { get; private set; }

    /**<include file='CommandDelegator.xml' path='CommandDelegator/class[@name="CommandDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public CommandDelegator(String targethandle)
    {
        TargetHandle = targethandle; ExecutionMode.AllowAll();
    }

    ///<inheritdoc/>
    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            AddActivity(a); LogExecuting(a,CommandDelegatorExecuting,a.Details?.Handle,TargetHandle);

            var id = AttachedTool!.ExecuteCommand(CommandDetails.Create(TargetHandle,a.Details?.Arguments,a.ID,a.Details?.Workflow),Key);

            if(id is not null) { SetSuccess(a); return a.ID; } else { SetFailed(a); AddOutput(a.ID); return null; }
        }
        catch ( OperationCanceledException ) { SetCanceled(a); AddOutput(a.ID); return null; }

        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a?.ID); return null; }

        finally { CleanUp(a); }
    }

    ///<inheritdoc/>
    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            a.Task = WorkAsync(a);

            if(a.FreeMode is true) { return a.ID; } else { await a.Task.ConfigureAwait(false); return a.ID; }
        }
        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a?.ID); return null; }

        finally { CleanUp(a); }
    }

    private async Task<Object?> WorkAsync(Activity a)
    {
        try
        {
            AddActivity(a); LogExecuting(a,CommandDelegatorExecuting,a.Details?.Handle,TargetHandle);

            var id = await AttachedTool!.ExecuteCommandAsync(
                          CommandDetails.Create(TargetHandle,a.Details?.Arguments,a.ID,a.Details?.Workflow),
                          a.Cancel!.Token,Key).ConfigureAwait(false);

            if(id is not null) { SetSuccess(a); return true; } else { SetFailed(a); AddOutput(a.ID); return false; }
        }
        catch ( OperationCanceledException ) { SetCanceled(a); AddOutput(a.ID); return false; }

        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a.ID); return false; }

        finally { this.CleanUpFreeMode(a); }
    }
}