namespace KusDepot.Dapr;

internal sealed class CheckServiceHealth : Command
{
    public CheckServiceHealth() { ExecutionMode.AllowBoth(); }

    ///<inheritdoc/>
    protected override CommandResult ExecuteCore(Activity activity , CommandKey? key)
    {
        ManagerConfig? config = activity.Details?.GetArgument<ManagerConfig>("ManagerConfig");

        if(config is null) { return CommandResult.FailedWithOutput(null); }

        LogExecuting(activity,ExecutingCommand,this.GetType().Name,config.ToString());

        Int32? processid = CheckHealth(config.ProcessName,config.ListenAddress,config.GrpcPort);

        return processid.HasValue
            ? CommandResult.SuccessWithOutput(processid)
            : CommandResult.FailedWithOutput(null);
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

    private static Int32? CheckHealth(String? processname , String? address , Int32? port)
    {
        try { return GetListeningProcess(processname,address,port); }

        catch { return null; }
    }
}