using Serilog;
using Serilog.Core;

namespace KusDepot.ToolGrains;

public partial class ToolGrain
{
    private Logger? Logger;

    private void SetupLogging()
    {
        this.Logger = new LoggerConfiguration()
            .WriteTo.File($"{LogDirectory}{"ToolGrain"}-{ProcessId}.log",formatProvider:InvariantCulture,shared:true)
            .CreateLogger();
    }
}