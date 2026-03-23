using Serilog;

namespace KusDepot.ToolActor;

public partial class ToolActor
{
    private static void SetupLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File($"{LogDirectory}{"ToolActor"}-{ProcessId}.log",formatProvider:InvariantCulture,shared:true)
            .CreateLogger();

        AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); };
    }
}