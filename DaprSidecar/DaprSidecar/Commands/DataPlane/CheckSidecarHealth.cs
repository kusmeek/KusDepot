namespace KusDepot.Dapr;

internal sealed class CheckSidecarHealth : Command
{
    public CheckSidecarHealth() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try { return ExecuteAsync(a,key).GetAwaiter().GetResult(); }

        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a?.ID); return null; }

        finally { CleanUp(a); }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity , CommandKey? key = null)
    {
        if(activity is null || !EnabledAllowed(key)) { return null; } Activity a = activity;

        try { a.Task = WorkAsync(a); await a.Task.ConfigureAwait(false); return a.ID; }

        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a?.ID); return null; }

        finally { CleanUp(a); }
    }

    private async Task<Object?> WorkAsync(Activity a)
    {
        try
        {
            AddActivity(a); var cfg = a.Details?.GetArgument<SidecarConfig>("SidecarConfig");

            if(cfg is null) { SetFailed(a); AddOutput(a.ID); return false; }

            LogExecuting(a,ExecutingCommand,this.GetType().Name,cfg.ToString()); var ct = a.Details!.GetCancel();

            if( await CheckHealthAsync(a,cfg.ProcessName,cfg.ListenAddress,cfg.GrpcPort,cfg.HttpPort,ct).ConfigureAwait(false) is true )

            { SetSuccess(a); return true; } else { SetFailed(a); AddOutput(a.ID); return false; }
        }
        catch ( OperationCanceledException ) { SetCanceled(a); LogCanceled(a); AddOutput(a.ID); return false; }

        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a.ID); return false; }

        finally { this.CleanUp(a); }
    }

    private async Task<Boolean> CheckHealthAsync(Activity a , String? processname , String? address , Int32? grpcport , Int32? httpport , CancellationToken cancel = default)
    {
        try
        {
            using var client = new HttpClient();

            Boolean ok = (await client.GetAsync($"http://localhost:{httpport}/v1.0/healthz/outbound",cancel).ConfigureAwait(false)).IsSuccessStatusCode;

            Int32? pid = GetListeningProcess(processname,address,grpcport); AddOutput(a.ID!,pid);

            return pid.HasValue && ok;
        }
        catch { return false; }
    }
}