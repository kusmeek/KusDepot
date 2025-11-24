namespace KusDepot.Commands;

/**<include file='ToolDelegator.xml' path='ToolDelegator/class[@name="ToolDelegator"]/main/*'/>*/
public sealed class ToolDelegator : Command
{
    /**<include file='ToolDelegator.xml' path='ToolDelegator/class[@name="ToolDelegator"]/field[@name="Target"]/*'/>*/
    private readonly ITool Target;

    /**<include file='ToolDelegator.xml' path='ToolDelegator/class[@name="ToolDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public ToolDelegator(ITool target) { Target = target; ExecutionMode.AllowBoth(); }

    ///<inheritdoc/>
    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            AddActivity(a); a.Logger?.Information(ToolDelegatorExecuting,a.Details?.Handle,Target.GetID()?.ToStringInvariant());

            AccessKey? k = a.Details?.GetArgument<AccessKey>("AccessKey");

            if(Target.GetDisposed() is not false) { AddOutput(a.ID); return null; }

            if(AttachedTool!.AddOutput(a.ID,
                Target.GetRemoveOutput(
                    Target.ExecuteCommand(a.Details,k),key:k)) is true)
            { return a.ID; } else { AddOutput(a.ID); return null; }
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
                    AddActivity(a); a.Logger?.Information(ToolDelegatorExecuting,a.Details?.Handle,Target.GetID()?.ToStringInvariant());

                    AccessKey? k = a.Details?.GetArgument<AccessKey>("AccessKey");

                    if(Target.GetDisposed() is not false) { AddOutput(a.ID); return false; }

                    if(AttachedTool!.AddOutput(a.ID,
                         await Target.GetRemoveOutputAsync(
                            await Target.ExecuteCommandAsync(a.Details,a.Cancel!.Token,key:k).ConfigureAwait(false),
                            a.Cancel!.Token,DefaultCommandTimeout,key:k).ConfigureAwait(false)) is true)
                    { return true; } else { AddOutput(a.ID); return false; }
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