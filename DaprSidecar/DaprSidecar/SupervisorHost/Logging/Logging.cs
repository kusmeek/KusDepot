using Serilog;

namespace KusDepot.Dapr;

internal partial class SupervisorHost
{
    private static void SetupLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(formatProvider:CultureInfo.InvariantCulture)
            .WriteTo.File(LogFilePath,formatProvider:CultureInfo.InvariantCulture)
            .CreateLogger();

        KusDepotLog.Initialize(new SerilogLoggerFactory(Log.Logger));

        AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); };
    }
}