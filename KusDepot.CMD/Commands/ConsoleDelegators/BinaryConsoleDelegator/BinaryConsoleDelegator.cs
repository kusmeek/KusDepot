namespace KusDepot.Commands;

/**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/main/*'/>*/
public sealed class BinaryConsoleDelegator : Command
{
    /**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/property[@name="CommandResult"]/*'/>*/
    public CommandResult? CommandResult { get; private set; }

    /**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/property[@name="ConsoleCommand"]/*'/>*/
    public CliWrap.Command ConsoleCommand { get; private set; }

    /**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/property[@name="Observer"]/*'/>*/
    private BinaryCommandObserver Observer { get; set; }

    /**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/property[@name="Buffered"]/*'/>*/
    private Boolean Buffered { get; }

    /**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public BinaryConsoleDelegator(CliWrap.Command command , Boolean buffered = false , BinaryCommandObserver? observer = null)
    {
        ConsoleCommand = command; Observer = observer ?? new(); Buffered = buffered;

        ExecutionMode.AllowAll();
    }

    ///<inheritdoc/>
    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try { return ExecuteAsync(a,key).GetAwaiter().GetResult(); }

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

        finally { this.CleanUp(a); }
    }

    private async Task<Object?> WorkAsync(Activity a)
    {
        try
        {
            AddActivity(a); LogExecuting(a,BinaryConsoleDelegatorExecuting,a.Details?.Handle);

            CancellationToken ct = a.Details!.GetCancel();
            var i = a.Details.GetArgument<Stream?>("InputStream");
            Observer = a.Details.GetArgument<BinaryCommandObserver?>("BinaryCommandObserver") ?? Observer;
            if(i is not null) { ConsoleCommand = ConsoleCommand.WithStandardInputPipe(PipeSource.FromStream(i)); }
            CancellationToken gt = a.Details.GetArgument<CancellationToken?>("GracefulCancellationToken") ?? CancellationToken.None;

            using var runner = new BinaryCommandRunner(Observer,Buffered);

            CommandResult = await runner.RunAsync(ConsoleCommand,ct,gt).ConfigureAwait(false);

            if(AttachedTool!.AddOutput(a.ID,CommandResult) is true) { SetSuccess(a); return true; } else { SetFailed(a); AddOutput(a.ID); return false; }
        }
        catch ( OperationCanceledException ) { SetCanceled(a); AddOutput(a.ID,new CanceledCommandResult()); return false; }

        catch ( CommandExecutionException _ )
        {
            SetFaulted(a,_); LogFaulted(a,_);

            AddOutput(a.ID,new FaultedCommandResult(ConsoleDelegator.ExtractExitCode(_))); return false;
        }

        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a.ID); return false; }

        finally { this.CleanUpFreeMode(a); }
    }
}