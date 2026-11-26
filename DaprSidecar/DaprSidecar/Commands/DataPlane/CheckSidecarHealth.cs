namespace KusDepot.Dapr;

internal sealed class CheckSidecarHealth : Command
{
    public CheckSidecarHealth() { ExecutionMode.AllowBoth(); }

    ///<inheritdoc/>
    protected override CommandResult ExecuteCore(Activity activity , CommandKey? key)
    {
        return this.ExecuteCoreAsync(activity,key,activity.Details.GetCancel()).AsTask().GetAwaiter().GetResult();
    }

    ///<inheritdoc/>
    protected override async ValueTask<CommandResult> ExecuteCoreAsync(Activity activity , CommandKey? key , CancellationToken cancel)
    {
        SidecarConfig? config = activity.Details?.GetArgument<SidecarConfig>("SidecarConfig");

        if(config is null) { return CommandResult.FailedWithOutput(null); }

        LogExecuting(activity,ExecutingCommand,this.GetType().Name,config.ToString());

        (Boolean healthy , Int32? processid) = await CheckHealthAsync(
            config.ProcessName,config.ListenAddress,config.GrpcPort,config.HttpPort,cancel).ConfigureAwait(false);

        return healthy
            ? CommandResult.SuccessWithOutput(processid)
            : CommandResult.FailedWithOutput(processid);
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

    private static async Task<(Boolean Healthy , Int32? ProcessID)> CheckHealthAsync(String? processname , String? address , Int32? grpcport , Int32? httpport , CancellationToken cancel = default)
    {
        try
        {
            using HttpClient client = new();

            Boolean ok = (await client.GetAsync($"http://localhost:{httpport}/v1.0/healthz/outbound",cancel).ConfigureAwait(false)).IsSuccessStatusCode;

            Int32? processid = GetListeningProcess(processname,address,grpcport);

            return (processid.HasValue && ok,processid);
        }
        catch ( OperationCanceledException ) { throw; }

        catch { return (false,null); }
    }
}