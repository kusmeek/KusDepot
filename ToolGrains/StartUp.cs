using Serilog;

namespace KusDepot.ToolGrains;

internal static class ToolGrainsStartUp
{
    private static async Task Main()
    {
        Serilog.Core.Logger? _ = default;

        try
        {
            _ = new LoggerConfiguration().WriteTo.File(LogFilePath,formatProvider:CultureInfo.InvariantCulture).CreateLogger();

            await ServiceRuntime.RegisterServiceAsync("ToolGrainsType",statefulcontext => new ToolSilo(statefulcontext));

            if(_ is not null) { _.Information(RegisterSuccess,ProcessId); await _.DisposeAsync(); }

            Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception __ ) { if(_ is not null) { _.Fatal(__,StartUpFail); await _.DisposeAsync(); } }
    }

    private static String LogFilePath => @"C:\KusDepotLogs\ToolSilo\ToolSilo-" + ProcessId + @".log"; 
}