namespace KusDepot.Commands;

/**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/main/*'/>*/
public sealed class ConsoleDelegator : Command
{
    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="Buffered"]/*'/>*/
    private Boolean? Buffered { get; set; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="CommandResult"]/*'/>*/
    public ProcessCommandResult? CommandResult { get; private set; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="ConsoleCommand"]/*'/>*/
    public CliWrap.Command ConsoleCommand { get; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="Observer"]/*'/>*/
    private ConsoleCommandObserver Observer { get; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="StdErrEncoding"]/*'/>*/
    private Encoding StdErrEncoding { get; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="StdOutEncoding"]/*'/>*/
    private Encoding StdOutEncoding { get; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="Streamed"]/*'/>*/
    private Boolean? Streamed { get; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public ConsoleDelegator(CliWrap.Command command , Boolean? streamed = false , Boolean? buffered = false , ConsoleCommandObserver? observer = null , Encoding? stdout = null , Encoding? stderr = null)
    {
        ConsoleCommand = command; Streamed = streamed; Buffered = buffered; Observer = observer ?? new ConsoleCommandObserver();

        StdOutEncoding = stdout ?? Encoding.UTF8; StdErrEncoding = stderr ?? Encoding.UTF8;

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
        LogExecuting(activity,ConsoleDelegatorExecuting,activity.Details?.Handle);

        Stream? input = activity.Details!.GetArgument<Stream?>("InputStream");

        ConsoleCommandObserver observer = activity.Details.GetArgument<ConsoleCommandObserver?>("ConsoleCommandObserver") ?? Observer;

        CliWrap.Command command = input is not null
            ? ConsoleCommand.WithStandardInputPipe(PipeSource.FromStream(input))
            : ConsoleCommand;

        CancellationToken gracefulcancel = activity.Details.GetArgument<CancellationToken?>("GracefulCancellationToken") ?? CancellationToken.None;

        ProcessCommandResult? commandResult;

        if(Streamed is true)
        {
            ConsoleCommandObserverAdapter observerAdapter = new(observer,cancel);

            using(command.Observe(StdOutEncoding,StdErrEncoding,cancel,gracefulcancel).Subscribe(observerAdapter))
            {
                ConsoleCommandObserverCompletion completion = await observerAdapter.Completion.ConfigureAwait(false);

                commandResult = completion.Result; CommandResult = commandResult;

                if(completion.ProcessError is OperationCanceledException canceled)
                {
                    if(completion.CallbackErrors.Length is 0) { ExceptionDispatchInfo.Capture(canceled).Throw(); }

                    Exception[] failures = new[] { canceled }.Concat(completion.CallbackErrors).ToArray();

                    throw new OperationCanceledException(canceled.Message,new AggregateException(failures),canceled.CancellationToken);
                }

                if(completion.ProcessError is not null)
                {
                    if(completion.CallbackErrors.Length is 0) { ExceptionDispatchInfo.Capture(completion.ProcessError).Throw(); }

                    throw new CommandObserverException(commandResult,new[] { completion.ProcessError }.Concat(completion.CallbackErrors));
                }

                if(completion.CallbackErrors.Length is not 0) { throw new CommandObserverException(commandResult,completion.CallbackErrors); }
            }
        }
        else if(Buffered is true)
        {
            commandResult = await command.ExecuteBufferedAsync(StdOutEncoding,StdErrEncoding,cancel,gracefulcancel).ConfigureAwait(false);
        }
        else { commandResult = await command.ExecuteAsync(cancel,gracefulcancel).ConfigureAwait(false); }

        CommandResult = commandResult;

        return KusDepot.CommandResult.SuccessWithOutput(commandResult);
    }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/method[@name="ExtractExitCode"]/*'/>*/
    internal static Int32 ExtractExitCode(Exception? exception , Int32 defaultcode = -1)
    {
        String? message = exception?.InnerException?.Message ?? exception?.Message; if(String.IsNullOrEmpty(message)) { return defaultcode; }

        const String marker = "exit code ("; Int32 start = message.IndexOf(marker,StringComparison.OrdinalIgnoreCase); if(start < 0) { return defaultcode; }

        start += marker.Length; Int32 end = message.IndexOf(')',start); if(end <= start) { return defaultcode; }

        ReadOnlySpan<Char> slice = message.AsSpan(start,end - start);

        return Int32.TryParse(slice,NumberStyles.Integer,CultureInfo.InvariantCulture,out Int32 code) ? code : defaultcode;
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
            Int32 exitcode = observererror.Result?.ExitCode ?? ExtractExitCode(exception);

            if(exitcode is -1) { exitcode = ExtractExitCode(exception); }

            return KusDepot.CommandResult.FaultedWithOutput(exception,new FaultedCommandResult(exitcode));
        }

        if(exception is CommandExecutionException)
        {
            return KusDepot.CommandResult.FaultedWithOutput(exception,new FaultedCommandResult(ExtractExitCode(exception)));
        }

        return base.CreateFaultedResult(activity,exception);
    }
}