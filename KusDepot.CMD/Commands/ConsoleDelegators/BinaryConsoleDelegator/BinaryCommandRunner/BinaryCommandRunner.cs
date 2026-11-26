namespace KusDepot.Commands;

/**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/main/*'/>*/
internal sealed class BinaryCommandRunner : IDisposable
{
    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/constructor[@name="Constructor"]/*'/>*/
    public BinaryCommandRunner(BinaryCommandObserver observer , Boolean buffered)
    {
        this.observer = observer; this.buffered = buffered; StdOutRelay = new(buffered); StdErrRelay = new(buffered);
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/method[@name="CreateStdErrTarget"]/*'/>*/
    private PipeTarget CreateStdErrTarget()
    {
        return PipeTarget.Create((stream,cancel) => StdErrRelay.PumpAsync(stream,cancel));
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/method[@name="CreateStdOutTarget"]/*'/>*/
    private PipeTarget CreateStdOutTarget()
    {
        return PipeTarget.Create((stream,cancel) => StdOutRelay.PumpAsync(stream,cancel));
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/method[@name="RunAsync"]/*'/>*/
    public async Task<ProcessCommandResult?> RunAsync(CliWrap.Command command , CancellationToken ct , CancellationToken gt)
    {
        Task<Exception?> output = Task.Run(() => ObserveAndDrainAsync(observer.OnStdOut,StdOutRelay.Stream,ct));

        Task<Exception?> error = Task.Run(() => ObserveAndDrainAsync(observer.OnStdErr,StdErrRelay.Stream,ct));

        ProcessCommandResult? processResult = null; Exception? processError = null; Boolean processStarted = false; List<Exception> errors = new();

        try
        {
            var cmd = command.WithStandardOutputPipe(CreateStdOutTarget()).WithStandardErrorPipe(CreateStdErrTarget());

            using CommandTask<ProcessCommandResult> process = cmd.ExecuteAsync(ct,gt); processStarted = true;

            try { await observer.OnStarted(new StartedCommandEvent(process.ProcessId),ct).ConfigureAwait(false); }

            catch ( OperationCanceledException e ) when(IsExpectedCancellation(e,ct) is true) {}

            catch ( Exception e ) { errors.Add(e); }

            processResult = await process.ConfigureAwait(false);
        }
        catch ( Exception e ) { processError = e; }

        if(processStarted is false) { StdOutRelay.Complete(); StdErrRelay.Complete(); }

        Exception?[] streamErrors = await Task.WhenAll(output,error).ConfigureAwait(false);

        foreach(Exception? streamError in streamErrors) { if(streamError is not null) { errors.Add(streamError); } }

        if(processResult is null && processError is CommandExecutionException)
        {
            processResult = new ProcessCommandResult(ConsoleDelegator.ExtractExitCode(processError),default,default);
        }

        ProcessCommandResult? result = processResult is null ? null : buffered ? CreateBufferedResult(processResult) : processResult;

        if(processStarted && (result is not null || processError is CommandExecutionException))
        {
            Int32 exitCode = result?.ExitCode ?? ConsoleDelegator.ExtractExitCode(processError);

            try { await observer.OnExited(new ExitedCommandEvent(exitCode),ct).ConfigureAwait(false); }

            catch ( OperationCanceledException e ) when(IsExpectedCancellation(e,ct) is true) { }

            catch ( Exception e ) { errors.Add(e); }
        }

        if(processError is OperationCanceledException canceled)
        {
            if(errors.Count is 0) { ExceptionDispatchInfo.Capture(canceled).Throw(); }

            Exception[] failures = new Exception[errors.Count + 1]; failures[0] = canceled; errors.CopyTo(failures,1);

            throw new OperationCanceledException(canceled.Message,new AggregateException(failures),canceled.CancellationToken);
        }

        if(processError is not null)
        {
            if(errors.Count is 0) { ExceptionDispatchInfo.Capture(processError).Throw(); }

            errors.Insert(0,processError);
        }

        if(errors.Count is not 0) { throw new CommandObserverException(result,errors); }

        return result;
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/method[@name="ObserveAndDrainAsync"]/*'/>*/
    private static async Task<Exception?> ObserveAndDrainAsync(Func<Stream,CancellationToken,Task> callback , Stream stream , CancellationToken cancel)
    {
        List<Exception> errors = new();

        try { await callback(stream,cancel).ConfigureAwait(false); }

        catch ( OperationCanceledException e ) when(IsExpectedCancellation(e,cancel) is true) { }

        catch ( Exception e ) { errors.Add(e); }

        try { await stream.CopyToAsync(Stream.Null,cancel).ConfigureAwait(false); }

        catch ( OperationCanceledException e ) when(IsExpectedCancellation(e,cancel) is true) { }

        catch ( Exception e ) { errors.Add(e); }

        return errors.Count switch
        {
            0 => null,
            1 => errors[0],
            _ => new AggregateException(errors)
        };
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/method[@name="IsExpectedCancellation"]/*'/>*/
    private static Boolean IsExpectedCancellation(OperationCanceledException e , CancellationToken cancel)
    {
        return cancel.IsCancellationRequested && e.CancellationToken == cancel;
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/method[@name="CreateBufferedResult"]/*'/>*/
    private BinaryCommandResult CreateBufferedResult(ProcessCommandResult baseresult)
    {
        return new BinaryCommandResult(baseresult.ExitCode,baseresult.StartTime,baseresult.ExitTime,StdOutRelay.CopyCapturedBytes(),StdErrRelay.CopyCapturedBytes());
    }

    public void Dispose()
    {
        StdOutRelay.Dispose(); StdErrRelay.Dispose();
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/field[@name="buffered"]/*'/>*/
    private readonly Boolean buffered;

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/field[@name="observer"]/*'/>*/
    private readonly BinaryCommandObserver observer;

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/field[@name="StdErrRelay"]/*'/>*/
    private readonly BinaryOutputRelay StdErrRelay;

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/field[@name="StdOutRelay"]/*'/>*/
    private readonly BinaryOutputRelay StdOutRelay;
}