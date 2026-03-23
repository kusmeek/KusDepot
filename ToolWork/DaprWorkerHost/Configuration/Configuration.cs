using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;

namespace ToolWork;

internal sealed partial class DaprWorkerHost
{
    private static HostKey? HostKey;

    private static IToolWebHostBuilder ConfigureDaprWorkerHost()
    {
        var builder = ToolBuilderFactory.CreateWebHostBuilder();

        builder.Builder.Services.AddSerilog(); builder.Builder.Configuration.AddJsonFile(ConfigFilePath);

        DaprWorker.ConfigureActors(builder); DaprWorker.ConfigureWorkflows(builder); builder.UseRandomLocalPorts();

        builder.ConfigureTool((c,t) => { HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey; });

        builder.UseLogger(new SerilogLoggerFactory());

        builder.ConnectWithApplication().Seal();

        builder.UseConsoleLifetime();

        return builder;
    }

    private static IDisposable? UpdateCall;

    private static void UpdateConfig(Object? o)
    {
        var z = o as Tuple<IConfigurationRoot,LoggingLevelSwitch>;

        LoggingLevelSwitch? s = z!.Item2; IConfigurationRoot? c = z!.Item1;

        if(Enum.TryParse(c?["Serilog:MinimumLevel"] ?? "Verbose",out LogEventLevel l)) { s!.MinimumLevel = l; }

        if(Boolean.TryParse(c?["KusDepotExceptions:Enabled"] ?? "False",out Boolean e)) { Settings.NoExceptions = !e; }

        UpdateCall = c?.GetReloadToken().RegisterChangeCallback(UpdateConfig,new Tuple<IConfigurationRoot,LoggingLevelSwitch>(c,s));
    }

    private static String ConfigFilePath => AppContext.BaseDirectory + @"\appsettings.json";
}