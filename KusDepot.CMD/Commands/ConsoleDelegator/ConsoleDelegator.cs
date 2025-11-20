namespace KusDepot.Commands;

/**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/main/*'/>*/
public sealed class ConsoleDelegator : Command
{
    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="Buffered"]/*'/>*/
    private Boolean? Buffered { get; set; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="CommandResult"]/*'/>*/
    public CommandResult? CommandResult { get; private set; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="ConsoleCommand"]/*'/>*/
    public CliWrap.Command ConsoleCommand { get; private set; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public ConsoleDelegator(CliWrap.Command command , Boolean? buffered = null)
    {
        ConsoleCommand = command; Buffered = buffered; ExecutionMode.AllowBoth();
    }

    ///<inheritdoc/>
    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            AddActivity(a); a.Logger?.Information(ConsoleDelegatorExecuting,a.Details?.Handle);

            if(Buffered == true)
            {
                CommandResult = ConsoleCommand.ExecuteBufferedAsync(a.Cancel!.Token).GetAwaiter().GetResult();
            }
            else { CommandResult = ConsoleCommand.ExecuteAsync(a.Cancel!.Token).GetAwaiter().GetResult(); }

            if(AttachedTool!.AddOutput(a.ID,CommandResult) is true) { return a.ID; } else { AddOutput(a.ID); return null; }
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
                    AddActivity(a); a.Logger?.Information(ConsoleDelegatorExecuting,a.Details?.Handle);

                    if(Buffered == true)
                    {
                        CommandResult = await ConsoleCommand.ExecuteBufferedAsync(a.Cancel!.Token).ConfigureAwait(false);
                    }
                    else { CommandResult = await ConsoleCommand.ExecuteAsync(a.Cancel!.Token).ConfigureAwait(false); }

                    if(AttachedTool!.AddOutput(a.ID,CommandResult) is true) { return true; } else { AddOutput(a.ID); return false; }
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