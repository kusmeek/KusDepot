namespace KusDepot.Dapr;

internal sealed class RestartSidecar : Command
{
    public RestartSidecar() { ExecutionMode.AllowBoth(); }

    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            var cfg = a.Details?.GetArgument<SidecarConfig>("SidecarConfig");
            var PC = a.Details?.GetArgument<ManagerConfig>("PlacementConfig");
            var SC = a.Details?.GetArgument<ManagerConfig>("SchedulerConfig");

            if( cfg is null || PC is null || SC is null ) { AddOutput(a.ID); return null; }

            AddActivity(a); a.Logger?.Information(ExecutingCommand,this.GetType().Name,cfg.ToString());

            if( Restart(a,cfg.BinPath,cfg.ConfigPath,cfg.ResourcesPath,cfg.GrpcPort,cfg.HttpPort,cfg.MetricsPort,cfg.AppID,cfg.AppPort,PC.ListenAddress,PC.GrpcPort,SC.ListenAddress,SC.GrpcPort,a.Logger) is true )

            { return a.ID; } else { AddOutput(a.ID); return null; }
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
                    var cfg = a.Details?.GetArgument<SidecarConfig>("SidecarConfig");
                    var PC = a.Details?.GetArgument<ManagerConfig>("PlacementConfig");
                    var SC = a.Details?.GetArgument<ManagerConfig>("SchedulerConfig");

                    if( cfg is null || PC is null || SC is null ) { AddOutput(a.ID); return false; }

                    AddActivity(a); a.Logger?.Information(ExecutingCommand,this.GetType().Name,cfg.ToString());

                    if( await RestartAsync(a,cfg.BinPath,cfg.ConfigPath,cfg.ResourcesPath,cfg.GrpcPort,cfg.HttpPort,cfg.MetricsPort,cfg.AppID,cfg.AppPort,PC.ListenAddress,PC.GrpcPort,SC.ListenAddress,SC.GrpcPort,a.Logger,a.Cancel!.Token).ConfigureAwait(false) )
                    { return true; } else { AddOutput(a.ID); return false; }
                }
                catch ( OperationCanceledException ) { AddOutput(a.ID); return false; }

                catch ( Exception _ ) { a.Logger?.Error(_,CommandExecuteFail,this.GetType().Name,a.Details?.Handle); AddOutput(a.ID); return false; }

                finally { CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch ( Exception _ ) { a?.Logger?.Error(_,CommandExecuteFail,this.GetType().Name,a?.Details?.Handle); AddOutput(a?.ID); CleanUp(a); return null; }
    }

    private Boolean Restart(Activity a , String? binpath , String? configpath , String? resourcespath , Int32? grpcport , Int32? httpport , Int32? metricsport , String? appid , Int32? appport , String? placementaddress , Int32? placementport , String? scheduleraddress , Int32? schedulerport , ILogger? logger)
    {
        try
        {
            var _ = CliWrap.Cli.Wrap(Path.Join(binpath,"dapr")).WithArguments(args =>
            {
                args.Add("run")
                    .Add("--enable-api-logging")
                    .Add("--log-level").Add("debug")
                    .Add("--app-id").Add($"{appid}")
                    .Add("--app-protocol").Add("http")
                    .Add("--app-port").Add($"{appport}")
                    .Add("--config").Add($"{configpath}")
                    .Add("--app-max-concurrency").Add($"8")
                    .Add("--runtime-path").Add($"{binpath}")
                    .Add("--dapr-http-port").Add($"{httpport}")
                    .Add("--dapr-grpc-port").Add($"{grpcport}")
                    .Add("--metrics-port").Add($"{metricsport}")
                    .Add("--app-health-probe-interval").Add($"24")
                    .Add("--resources-path").Add($"{resourcespath}")
                    .Add("--dapr-http-max-request-size").Add($"100");

                if( String.Equals(placementaddress,"0.0.0.0") ) { args.Add("--placement-host-address").Add($"127.0.0.1:{placementport}"); }
                else                                            { args.Add("--placement-host-address").Add($"{placementaddress}:{placementport}"); }

                if( String.Equals(scheduleraddress,"0.0.0.0") ) { args.Add("--scheduler-host-address").Add($"127.0.0.1:{schedulerport}"); }
                else                                            { args.Add("--scheduler-host-address").Add($"{scheduleraddress}:{schedulerport}"); }
            })

            .WithStandardErrorPipe(CliWrap.PipeTarget.ToDelegate( l => { logger?.Error(l); } ))

            .WithStandardOutputPipe(CliWrap.PipeTarget.ToDelegate( l => { logger?.Information(l); } ));

            Task.Run(async () => await _.ExecuteAsync()); AddOutput(a.ID!,true); return true;

        }
        catch { return false; }
    }

    private async ValueTask<Boolean> RestartAsync(Activity a , String? binpath , String? configpath , String? resourcespath , Int32? grpcport , Int32? httpport , Int32? metricsport , String? appid , Int32? appport , String? placementaddress , Int32? placementport , String? scheduleraddress , Int32? schedulerport , ILogger? logger , CancellationToken cancel = default)
    {
        try
        {
            var _ = CliWrap.Cli.Wrap(Path.Join(binpath,"dapr")).WithArguments(args =>
            {
                args.Add("run")
                    .Add("--enable-api-logging")
                    .Add("--log-level").Add("debug")
                    .Add("--app-id").Add($"{appid}")
                    .Add("--app-protocol").Add("http")
                    .Add("--app-port").Add($"{appport}")
                    .Add("--config").Add($"{configpath}")
                    .Add("--app-max-concurrency").Add($"8")
                    .Add("--runtime-path").Add($"{binpath}")
                    .Add("--dapr-http-port").Add($"{httpport}")
                    .Add("--dapr-grpc-port").Add($"{grpcport}")
                    .Add("--metrics-port").Add($"{metricsport}")
                    .Add("--app-health-probe-interval").Add($"24")
                    .Add("--resources-path").Add($"{resourcespath}")
                    .Add("--dapr-http-max-request-size").Add($"100");

                if( String.Equals(placementaddress,"0.0.0.0") ) { args.Add("--placement-host-address").Add($"127.0.0.1:{placementport}"); }
                else                                            { args.Add("--placement-host-address").Add($"{placementaddress}:{placementport}"); }

                if( String.Equals(scheduleraddress,"0.0.0.0") ) { args.Add("--scheduler-host-address").Add($"127.0.0.1:{schedulerport}"); }
                else                                            { args.Add("--scheduler-host-address").Add($"{scheduleraddress}:{schedulerport}"); }
            })

            .WithStandardErrorPipe(CliWrap.PipeTarget.ToDelegate( l => { logger?.Error(l); } ))

            .WithStandardOutputPipe(CliWrap.PipeTarget.ToDelegate( l => { logger?.Information(l); }));

            await _.ExecuteAsync(cancel).ConfigureAwait(false);

            AddOutput(a.ID!,true); return true;
        }
        catch { return false; }
    }
}