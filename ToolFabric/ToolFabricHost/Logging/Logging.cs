using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;

namespace KusDepot.ToolFabric;

internal sealed partial class ToolFabricHost
{
    private static void SetupLogging()
    {
        LoggingLevelSwitch s = new LoggingLevelSwitch(LogEventLevel.Verbose);

        IConfigurationRoot c = new ConfigurationBuilder().AddJsonFile(ToolFabric.ConfigFilePath,true,true).Build();

        AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); };

        UpdateCall = c.GetReloadToken().RegisterChangeCallback(UpdateConfig,new Tuple<IConfigurationRoot,LoggingLevelSwitch>(c,s)); c.Reload();

        Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(s).WriteTo.Console(formatProvider:InvariantCulture).WriteTo.File(LogFilePath,formatProvider:InvariantCulture).CreateLogger();

        KusDepotLog.Initialize(new SerilogLoggerFactory());
    }

    private static String LogFilePath => @"C:\KusDepotLogs\ToolFabric\ToolFabric-" + ProcessId + @".log"; 
}