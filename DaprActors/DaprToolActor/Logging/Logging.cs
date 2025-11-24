using Serilog;
using Serilog.Extensions.Logging;

namespace KusDepot.DaprActors;

public partial class DaprToolActor
{
    private static void SetupLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File($"{LogDirectory}{"DaprToolActor"}-{ProcessId}.log",formatProvider:InvariantCulture,shared:true)
            .CreateLogger();
    }
}