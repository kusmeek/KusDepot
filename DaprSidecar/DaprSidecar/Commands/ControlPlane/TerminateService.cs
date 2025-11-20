namespace KusDepot.Dapr;

internal sealed class TerminateService : Command
{
    public TerminateService() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            AddActivity(a); var cfg = a.Details?.GetArgument<ManagerConfig>("ManagerConfig"); if( cfg is null ) { AddOutput(a.ID); return null; }

            var pid = a.Details?.GetArgument<Int32?>("ProcessID"); if(pid is null) { a.Logger?.Error(InvalidProcessID,this.GetType().Name,cfg.ToString()); AddOutput(a.ID); return null; }

            a.Logger?.Information(ExecutingCommand,this.GetType().Name,cfg.ToString());

            if( Terminate(a,pid) is true ) { return a.ID; } else { AddOutput(a.ID); return null; }
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
                    AddActivity(a); var cfg = a.Details?.GetArgument<ManagerConfig>("ManagerConfig"); if( cfg is null ) { AddOutput(a.ID); return false; }

                    var pid = a.Details?.GetArgument<Int32?>("ProcessID"); if(pid is null) { a.Logger?.Error(InvalidProcessID,this.GetType().Name,cfg.ToString()); AddOutput(a.ID); return false; }

                    a.Logger?.Information(ExecutingCommand,this.GetType().Name,cfg.ToString());

                    if( Terminate(a,pid) is true ) { await Task.CompletedTask; return true; } else { AddOutput(a.ID); return false; }
                }
                catch ( OperationCanceledException ) { AddOutput(a.ID); return false; }

                catch ( Exception _ ) { a.Logger?.Error(_,CommandExecuteFail,this.GetType().Name,a.Details?.Handle); AddOutput(a.ID); return false; }

                finally { CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch ( Exception _ ) { a?.Logger?.Error(_,CommandExecuteFail,this.GetType().Name,a?.Details?.Handle); AddOutput(a?.ID); CleanUp(a); return null; }
    }

    private Boolean Terminate(Activity a , Int32? processid)
    {
        try
        {
            Boolean result = TerminateProcess(processid);

            AddOutput(a.ID!,result); return result;
        }
        catch { return false; }
    }
}