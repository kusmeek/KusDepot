namespace KusDepot.Commands;

/**<include file='CommandDelegator.xml' path='CommandDelegator/class[@name="CommandDelegator"]/main/*'/>*/
public sealed class CommandDelegator : Command
{
    /**<include file='CommandDelegator.xml' path='CommandDelegator/class[@name="CommandDelegator"]/property[@name="TargetHandle"]/*'/>*/
    public String TargetHandle { get; private set; }

    /**<include file='CommandDelegator.xml' path='CommandDelegator/class[@name="CommandDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public CommandDelegator(String targethandle) { TargetHandle = targethandle; ExecutionMode.AllowBoth(); }

    ///<inheritdoc/>
    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            AddActivity(a); a.Logger?.Information(CommandDelegatorExecuting,a.Details?.Handle,TargetHandle);

            var id = AttachedTool!.ExecuteCommand(CommandDetails.Create(TargetHandle,a.Details?.Arguments,a.ID),Key);

            if(id is null)
            { id = AttachedTool.ExecuteCommandAsync(CommandDetails.Create(TargetHandle,a.Details?.Arguments,a.ID),null,Key ).ConfigureAwait(false).GetAwaiter().GetResult(); }

            if( id is not null) { return a.ID; } else { AddOutput(a.ID); return null; }
        }
        catch ( Exception _ ) { a?.Logger?.Error(_,CommandExecuteFail,GetType().Name,a?.Details?.Handle); AddOutput(a?.ID); return null; }

        finally { CleanUp(a); }
    }

    ///<inheritdoc/>
    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        try
        {
            if(Enabled is false || AccessCheck(key) is false) { return null; }

            async Task<Object?> WorkAsync()
            {
                try
                {
                    AddActivity(a); a.Logger?.Information(CommandDelegatorExecuting,a.Details?.Handle,TargetHandle);

                    var id = await AttachedTool!.ExecuteCommandAsync( CommandDetails.Create( TargetHandle , a.Details?.Arguments , a.ID ),a.Cancel!.Token,Key ).ConfigureAwait(false);

                    if(id is null)
                    { id = AttachedTool.ExecuteCommand( CommandDetails.Create(TargetHandle,a.Details?.Arguments,a.ID),Key ); }

                    if(id is not null) { return true; } else { AddOutput(a.ID); return false; }
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