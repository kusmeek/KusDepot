namespace KusDepot.Dapr;

internal sealed class CheckServiceHealth : Command
{
    public CheckServiceHealth() { ExecutionMode.AllowBoth(); }

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
            AddActivity(a); var cfg = a.Details?.GetArgument<ManagerConfig>("ManagerConfig"); if(cfg is null) { SetFailed(a); AddOutput(a.ID); return false; }

            LogExecuting(a,ExecutingCommand,this.GetType().Name,cfg.ToString());

            if( CheckHealth(a,cfg.ProcessName,cfg.ListenAddress,cfg.GrpcPort) is true ) { SetSuccess(a); return true; } else { SetFailed(a); AddOutput(a.ID); return false; }
        }
        catch ( OperationCanceledException ) { SetCanceled(a); LogCanceled(a); AddOutput(a.ID); return false; }

        catch ( Exception _ ) { SetFaulted(a,_); LogFaulted(a,_); AddOutput(a.ID); return false; }

        finally { this.CleanUp(a); await Task.CompletedTask; }
    }

    private Boolean CheckHealth(Activity a , String? processname , String? address , Int32? port)
    {
        try
        {
            Int32? pid = GetListeningProcess(processname,address,port);

            AddOutput(a.ID!,pid); return pid.HasValue;
        }
        catch { return false; }
    }
}