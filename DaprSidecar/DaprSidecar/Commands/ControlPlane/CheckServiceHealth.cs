namespace KusDepot.Dapr;

internal sealed class CheckServiceHealth : Command
{
    public CheckServiceHealth() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            AddActivity(a); var cfg = a.Details?.GetArgument<ManagerConfig>("ManagerConfig"); if( cfg is null ) { AddOutput(a.ID); return null; }

            a.Logger?.Information(ExecutingCommand,this.GetType().Name,cfg.ToString());

            if( CheckHealth(a,cfg.ProcessName,cfg.ListenAddress,cfg.GrpcPort) is true ) { return a.ID; } else { AddOutput(a.ID); return null; }
        }
        catch ( Exception _ ) { a?.Logger?.Error(_,CommandExecuteFail,this.GetType().Name,a?.Details?.Handle); AddOutput(a?.ID); return null; }

        finally { CleanUp(a); }
    }

    public override async Task<Guid?> ExecuteAsync(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            async Task<Object?> WorkAsync()
            {
                try
                {
                    AddActivity(a); var cfg = a.Details?.GetArgument<ManagerConfig>("ManagerConfig");

                    if( cfg is null ) { AddOutput(a.ID); return false; }

                    a.Logger?.Information(ExecutingCommand,this.GetType().Name,cfg.ToString());

                    if( CheckHealth(a,cfg.ProcessName,cfg.ListenAddress,cfg.GrpcPort) is true )

                    { await Task.CompletedTask; return true; } else { AddOutput(a.ID); return false; }
                }
                catch ( OperationCanceledException ) { AddOutput(a.ID); return false; }

                catch ( Exception _ ) { a.Logger?.Error(_,CommandExecuteFail,this.GetType().Name,a.Details?.Handle); AddOutput(a.ID); return false; }

                finally { CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch ( Exception _ ) { a?.Logger?.Error(_,CommandExecuteFail,this.GetType().Name,a?.Details?.Handle); AddOutput(a?.ID); CleanUp(a); return null; }
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