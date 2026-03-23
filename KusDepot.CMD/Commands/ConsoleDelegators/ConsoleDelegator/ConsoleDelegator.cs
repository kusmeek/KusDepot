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

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="Observer"]/*'/>*/
    private ConsoleCommandObserver Observer { get; set; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="StdErrEncoding"]/*'/>*/
    private Encoding StdErrEncoding { get; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="StdOutEncoding"]/*'/>*/
    private Encoding StdOutEncoding { get; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/property[@name="Streamed"]/*'/>*/
    private Boolean? Streamed { get; set; }

    /**<include file='ConsoleDelegator.xml' path='ConsoleDelegator/class[@name="ConsoleDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public ConsoleDelegator(CliWrap.Command command , Boolean? streamed = false , Boolean? buffered = false , ConsoleCommandObserver? observer = null , Encoding? stdout = null , Encoding? stderr = null)
    {
        ConsoleCommand = command; Streamed = streamed; Buffered = buffered; Observer = observer ?? new ConsoleCommandObserver();

        StdOutEncoding = stdout ?? Encoding.UTF8; StdErrEncoding = stderr ?? Encoding.UTF8;

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

        finally { CleanUp(a); }
    }

    private async Task<Object?> WorkAsync(Activity a)
    {
        try
        {
            AddActivity(a); LogExecuting(a,ConsoleDelegatorExecuting,a.Details?.Handle);

            CancellationToken ct = a.Details!.GetCancel();
            var i = a.Details.GetArgument<Stream?>("InputStream");
            Observer = a.Details.GetArgument<ConsoleCommandObserver?>("ConsoleCommandObserver") ?? Observer;
            if(i is not null) { ConsoleCommand = ConsoleCommand.WithStandardInputPipe(PipeSource.FromStream(i)); }
            CancellationToken gt = a.Details.GetArgument<CancellationToken?>("GracefulCancellationToken") ?? CancellationToken.None;

            if(Streamed is true)
            {
                var o = new ConsoleCommandObserverAdapter(Observer);

                using(ConsoleCommand.Observe(StdOutEncoding,StdErrEncoding,ct,gt).Subscribe(o))
                {
                    CommandResult = await o.Completion.ConfigureAwait(false);
                }
            }
            else if(Buffered is true)
            {
                CommandResult = await ConsoleCommand.ExecuteBufferedAsync(StdOutEncoding,StdErrEncoding,ct,gt).ConfigureAwait(false);
            }
            else { CommandResult = await ConsoleCommand.ExecuteAsync(ct,gt).ConfigureAwait(false); }

            if(AttachedTool!.AddOutput(a.ID,CommandResult) is true) { SetSuccess(a); return true; } else { SetFailed(a); AddOutput(a.ID); return false; }
        }
        catch ( OperationCanceledException ) { SetCanceled(a); AddOutput(a.ID,new CanceledCommandResult()); return false; }

        catch ( CommandExecutionException _ )
        {
            SetFaulted(a,_); LogFaulted(a,_); AddOutput(a.ID,new FaultedCommandResult(ExtractExitCode(_))); return false;
        }

        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a.ID); return false; }

        finally { this.CleanUpFreeMode(a); }
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
}