using Serilog;

namespace KusDepot.ToolFabric;

internal static class ToolFabricStartUp
{
    private static async Task Main()
    {
        Serilog.Core.Logger? _ = default;

        try
        {
            _ = new LoggerConfiguration().WriteTo.File(LogFilePath,formatProvider:CultureInfo.InvariantCulture).CreateLogger();

            await ServiceRuntime.RegisterServiceAsync("ToolFabricType",context => new ToolFabricHost(context));

            if(_ is not null) { _.Information(RegisterSuccess,ProcessId); await _.DisposeAsync(); }

            Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception __ ) { if(_ is not null) { _.Fatal(__,StartUpFail); await _.DisposeAsync(); } }
    }

    private static String LogFilePath => @"C:\KusDepotLogs\ToolFabric\ToolFabric-" + ProcessId + @".log"; 
}