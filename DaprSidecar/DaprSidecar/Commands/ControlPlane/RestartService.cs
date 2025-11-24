namespace KusDepot.Dapr;

internal sealed class RestartService : Command
{
    public RestartService() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            AddActivity(a); var cfg = a.Details?.GetArgument<ManagerConfig>("ManagerConfig");

            if( cfg is null ) { AddOutput(a.ID); return null; }

            a.Logger?.Information(ExecutingCommand,this.GetType().Name,cfg.ToString());

            var cmd = CliWrap.Cli.Wrap(Path.Join(cfg.BinPath,cfg.ProcessName)).WithArguments(args =>
            {
                args.Add("--listen-address")
                    .Add($"{cfg.ListenAddress}")
                    .Add("--port").Add($"{cfg.GrpcPort}")
                    .Add("--healthz-port").Add($"{cfg.HealthzPort}")
                    .Add("--metrics-port").Add($"{cfg.MetricsPort}");
            });

            cmd.ExecuteAsync(a.Cancel!.Token).ConfigureAwait(false).GetAwaiter().GetResult();

            AddOutput(a.ID); return a.ID;
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

                    var cmd = CliWrap.Cli.Wrap(Path.Join(cfg.BinPath,cfg.ProcessName)).WithArguments(args =>
                    {
                        args.Add("--listen-address")
                            .Add($"{cfg.ListenAddress}")
                            .Add("--port").Add($"{cfg.GrpcPort}")
                            .Add("--healthz-port").Add($"{cfg.HealthzPort}")
                            .Add("--metrics-port").Add($"{cfg.MetricsPort}");
                    });

                    await cmd.ExecuteAsync(a.Cancel!.Token).ConfigureAwait(false);

                    AddOutput(a.ID); return true;
                }
                catch ( OperationCanceledException ) { AddOutput(a.ID); return false; }

                catch ( Exception _ ) { a.Logger?.Error(_,CommandExecuteFail,this.GetType().Name,a.Details?.Handle); AddOutput(a.ID); return false; }

                finally { CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch ( Exception _ ) { a?.Logger?.Error(_,CommandExecuteFail,this.GetType().Name,a?.Details?.Handle); AddOutput(a?.ID); CleanUp(a); return null; }
    }
}