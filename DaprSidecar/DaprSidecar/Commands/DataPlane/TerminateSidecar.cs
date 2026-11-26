namespace KusDepot.Dapr;

internal sealed class TerminateSidecar : Command
{
    public TerminateSidecar() { ExecutionMode.AllowBoth(); }

    ///<inheritdoc/>
    protected override CommandResult ExecuteCore(Activity activity , CommandKey? key)
    {
        SidecarConfig? config = activity.Details?.GetArgument<SidecarConfig>("SidecarConfig");

        if(config is null) { return CommandResult.FailedWithOutput(null); }

        Int32? processid = activity.Details?.GetArgument<Int32?>("ProcessID");

        if(processid is null) { return CommandResult.FailedWithOutput(null,InvalidProcessID); }

        LogExecuting(activity,ExecutingCommand,this.GetType().Name,config.ToString());

        Boolean terminated = Terminate(processid);

        return terminated
            ? CommandResult.SuccessWithOutput(true)
            : CommandResult.FailedWithOutput(false);
    }

    ///<inheritdoc/>
    protected override ValueTask<CommandResult> ExecuteCoreAsync(Activity activity , CommandKey? key , CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested(); return ValueTask.FromResult(this.ExecuteCore(activity,key));
    }

    ///<inheritdoc/>
    protected override CommandResult CreateCanceledResult(Activity activity , OperationCanceledException exception)
    {
        return activity.IsTimedOut()
            ? CommandResult.TimedOutWithOutput(null,exception)
            : CommandResult.CanceledWithOutput(null,exception);
    }

    ///<inheritdoc/>
    protected override CommandResult CreateFaultedResult(Activity activity , Exception exception)
    {
        return CommandResult.FaultedWithOutput(exception,null);
    }

    private static Boolean Terminate(Int32? processid)
    {
        try { return TerminateProcess(processid); }

        catch { return false; }
    }
}