namespace KusDepot.Commands;

/**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/main/*'/>*/
internal sealed class BinaryCommandRunner : IDisposable
{
    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/constructor[@name="Constructor"]/*'/>*/
    public BinaryCommandRunner(BinaryCommandObserver observer , Boolean buffered)
    {
        this.observer = observer; this.buffered = buffered; StdOutPipe = new Pipe(); StdErrPipe = new Pipe();

        StdOut = StdOutPipe.Reader.AsStream(leaveOpen:false); StdErr = StdErrPipe.Reader.AsStream(leaveOpen:false);
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/method[@name="CreateStdErrTarget"]/*'/>*/
    private PipeTarget CreateStdErrTarget()
    {
        return PipeTarget.Create(async (stream,cancel) =>
        {
            try { await stream.CopyToAsync(StdErrPipe.Writer,cancel).ConfigureAwait(false); }

            finally { await StdErrPipe.Writer.CompleteAsync().ConfigureAwait(false); }
        });
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/method[@name="CreateStdOutTarget"]/*'/>*/
    private PipeTarget CreateStdOutTarget()
    {
        return PipeTarget.Create(async (stream,cancel) =>
        {
            try { await stream.CopyToAsync(StdOutPipe.Writer,cancel).ConfigureAwait(false); }

            finally { await StdOutPipe.Writer.CompleteAsync().ConfigureAwait(false); }
        });
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/method[@name="RunAsync"]/*'/>*/
    public async Task<CommandResult?> RunAsync(CliWrap.Command command , CancellationToken ct , CancellationToken gt)
    {
        try
        {
            var cmd = command.WithStandardOutputPipe(CreateStdOutTarget()).WithStandardErrorPipe(CreateStdErrTarget());

            var o = observer.OnStdOut(StdOut); var e = observer.OnStdErr(StdErr);

            var r = await cmd.ExecuteAsync(ct,gt).ConfigureAwait(false);

            await Task.WhenAll(o,e).ConfigureAwait(false);

            CommandResult? _ = buffered ? await CreateBufferedResultAsync(r,ct).ConfigureAwait(false) : r;

            await observer.OnExited(new ExitedCommandEvent(_!.ExitCode)).ConfigureAwait(false);

            return _;
        }
        catch ( OperationCanceledException ) when ( buffered )
        {
            return await CreateBufferedResultAsync(new CanceledCommandResult()).ConfigureAwait(false);
        }
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/method[@name="CreateBufferedResultAsync"]/*'/>*/
    private async Task<BinaryCommandResult> CreateBufferedResultAsync(CommandResult baseresult , CancellationToken cancel = default)
    {
        Task<Byte[]> o = ReadAllBytesAsync(StdOut,cancel); Task<Byte[]> e = ReadAllBytesAsync(StdErr,cancel);

        await Task.WhenAll(o,e).ConfigureAwait(false);

        return new BinaryCommandResult(baseresult.ExitCode,baseresult.StartTime,baseresult.ExitTime,o.Result,e.Result);
    }

    public void Dispose()
    {
        StdOut?.Dispose(); StdErr?.Dispose(); StdOutPipe?.Reader.Complete(); StdErrPipe?.Reader.Complete();
    }

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/field[@name="buffered"]/*'/>*/
    private readonly Boolean buffered;

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/field[@name="observer"]/*'/>*/
    private readonly BinaryCommandObserver observer;

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/field[@name="StdErr"]/*'/>*/
    private readonly Stream StdErr;

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/field[@name="StdErrPipe"]/*'/>*/
    private readonly Pipe StdErrPipe;

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/field[@name="StdOut"]/*'/>*/
    private readonly Stream StdOut;

    /**<include file='BinaryCommandRunner.xml' path='BinaryCommandRunner/class[@name="BinaryCommandRunner"]/field[@name="StdOutPipe"]/*'/>*/
    private readonly Pipe StdOutPipe;
}