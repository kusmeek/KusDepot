namespace KusDepot.Commands;

/**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/main/*'/>*/
public sealed class BinaryConsoleDelegator : Command
{
    /**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/property[@name="CommandResult"]/*'/>*/
    public ProcessCommandResult? CommandResult { get; private set; }

    /**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/property[@name="ConsoleCommand"]/*'/>*/
    public CliWrap.Command ConsoleCommand { get; }

    /**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/property[@name="Observer"]/*'/>*/
    private BinaryCommandObserver Observer { get; }

    /**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/property[@name="Buffered"]/*'/>*/
    private Boolean Buffered { get; }

    /**<include file='BinaryConsoleDelegator.xml' path='BinaryConsoleDelegator/class[@name="BinaryConsoleDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public BinaryConsoleDelegator(CliWrap.Command command , Boolean buffered = false , BinaryCommandObserver? observer = null)
    {
        ConsoleCommand = command; Observer = observer ?? new(); Buffered = buffered;

        ExecutionMode.AllowAll();
    }

    ///<inheritdoc/>
    protected override CommandResult ExecuteCore(Activity activity , CommandKey? key)
    {
        return this.ExecuteCoreAsync(activity,key,activity.Details.GetCancel()).AsTask().GetAwaiter().GetResult();
    }

    ///<inheritdoc/>
    protected override async ValueTask<CommandResult> ExecuteCoreAsync(Activity activity , CommandKey? key , CancellationToken cancel)
    {
        LogExecuting(activity,BinaryConsoleDelegatorExecuting,activity.Details?.Handle);

        Stream? input = activity.Details!.GetArgument<Stream?>("InputStream");

        BinaryCommandObserver observer = activity.Details.GetArgument<BinaryCommandObserver?>("BinaryCommandObserver") ?? Observer;

        CliWrap.Command command = input is not null
            ? ConsoleCommand.WithStandardInputPipe(PipeSource.FromStream(input))
            : ConsoleCommand;

        CancellationToken gracefulcancel = activity.Details.GetArgument<CancellationToken?>("GracefulCancellationToken") ?? CancellationToken.None;

        using BinaryCommandRunner runner = new(observer,Buffered);

        ProcessCommandResult? commandResult;

        try { commandResult = await runner.RunAsync(command,cancel,gracefulcancel).ConfigureAwait(false); }

        catch ( CommandObserverException e ) { CommandResult = e.Result; throw; }

        CommandResult = commandResult;

        return KusDepot.CommandResult.SuccessWithOutput(commandResult);
    }

    ///<inheritdoc/>
    protected override CommandResult CreateCanceledResult(Activity activity , OperationCanceledException exception)
    {
        return activity.IsTimedOut()
            ? KusDepot.CommandResult.TimedOutWithOutput(new CanceledCommandResult(),exception)
            : KusDepot.CommandResult.CanceledWithOutput(new CanceledCommandResult(),exception);
    }

    ///<inheritdoc/>
    protected override CommandResult CreateFaultedResult(Activity activity , Exception exception)
    {
        if(exception is CommandObserverException observererror)
        {
            return KusDepot.CommandResult.FaultedWithOutput(exception,observererror.Result ?? new FaultedCommandResult(ConsoleDelegator.ExtractExitCode(exception)));
        }

        if(exception is CommandExecutionException)
        {
            return KusDepot.CommandResult.FaultedWithOutput(exception,new FaultedCommandResult(ConsoleDelegator.ExtractExitCode(exception)));
        }

        return base.CreateFaultedResult(activity,exception);
    }
}