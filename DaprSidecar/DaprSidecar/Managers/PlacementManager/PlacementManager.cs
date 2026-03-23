namespace KusDepot.Dapr;

public class PlacementManager : ServiceManager
{
    public PlacementManager(){}

    public PlacementManager(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null,
           ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null , ILoggerFactory? logger = null)
           : base(accessmanager:accessmanager,data:data,id:id,services:services,commands:commands,configuration:configuration,logger:logger){}

    public static CliWrap.Command BuildRestartServiceCommand(IConfiguration configuration)
    {
        DaprSupervisorConfig? sc = configuration.GetSection("DaprSupervisorConfig").Get<DaprSupervisorConfig>();

        if(sc is null || sc.Validate() is false) { throw new InvalidOperationException("Invalid Configuration"); }

        ManagerConfig cfg = sc.PlacementManager;

        return CliWrap.Cli.Wrap(Path.Join(cfg.BinPath,cfg.ProcessName)).WithArguments(args =>
        {
            args.Add("--listen-address")
                .Add($"{cfg.ListenAddress}")
                .Add("--port").Add($"{cfg.GrpcPort}")
                .Add("--healthz-port").Add($"{cfg.HealthzPort}")
                .Add("--metrics-port").Add($"{cfg.MetricsPort}");
        });
    }
}