using Serilog;
using Serilog.Extensions.Logging;

namespace KusDepot.ToolGrains;

public partial class TestToolGrain
{
    private static void SetupLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File($"{LogDirectory}{"ToolGrain"}-{ProcessId}.log",formatProvider:InvariantCulture,shared:true)
            .CreateLogger();

        AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); };
    }
}