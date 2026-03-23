namespace KusDepot.Commands;

/**<include file='ToolDelegator.xml' path='ToolDelegator/class[@name="ToolDelegator"]/main/*'/>*/
public sealed class ToolDelegator : Command
{
    /**<include file='ToolDelegator.xml' path='ToolDelegator/class[@name="ToolDelegator"]/field[@name="Target"]/*'/>*/
    private readonly ITool Target;

    /**<include file='ToolDelegator.xml' path='ToolDelegator/class[@name="ToolDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public ToolDelegator(ITool target)
    {
        Target = target; ExecutionMode.AllowAll();
    }

    ///<inheritdoc/>
    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try
        {
            AddActivity(a); LogExecuting(a,ToolDelegatorExecuting,a.Details?.Handle,Target.GetID()?.ToStringInvariant());

            var k = a.Details?.GetAccessKey();

            if(Target.GetDisposed() is not false) { SetFailed(a); AddOutput(a.ID); return null; }

            Guid? id = Target.ExecuteCommand(a.Details,k); if(id is null) { SetFailed(a); AddOutput(a.ID); return null; }

            Object? r = Target.GetRemoveOutput(id,key:k);

            if(AttachedTool!.AddOutput(a.ID,r) is true)
            {
                SetSuccess(a); return a.ID;
            }
            else { SetFailed(a); AddOutput(a.ID); return null; }
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
            AddActivity(a); LogExecuting(a,ToolDelegatorExecuting,a.Details?.Handle,Target.GetID()?.ToStringInvariant());

            var k = a.Details?.GetAccessKey();

            if(Target.GetDisposed() is not false) { SetFailed(a); AddOutput(a.ID); return false; }

            Guid? id = await Target.ExecuteCommandAsync(a.Details,a.Details?.GetCancel(),key:k).ConfigureAwait(false);

            if(id is null) { SetFailed(a); AddOutput(a.ID); return false; }

            Object? r = await Target.GetRemoveOutputAsync(id,a.Details?.GetCancel(),GetOutputTimeout,key:k).ConfigureAwait(false);

            if(AttachedTool!.AddOutput(a.ID,r) is true) { SetSuccess(a); return true; } else { SetFailed(a); AddOutput(a.ID); return false; }
        }
        catch ( OperationCanceledException ) { SetCanceled(a); AddOutput(a.ID); return false; }

        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a.ID); return false; }

        finally { this.CleanUpFreeMode(a); }
    }
}