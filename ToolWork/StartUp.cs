using Serilog;

namespace ToolWork;

internal static class DaprWorkerStartUp
{
    private static async Task Main()
    {
        Serilog.Core.Logger? _ = default;

        try
        {
            _ = new LoggerConfiguration().WriteTo.File($"{LogDirectory}{"DaprWorker"}-{ProcessId}.log",formatProvider:InvariantCulture).CreateLogger();

            await ServiceRuntime.RegisterServiceAsync("ToolWorkType",(context) => new DaprWorkerHost(context) );

            if(_ is not null) { _.Information(RegisterSuccess,ProcessId); await _.DisposeAsync(); }

            Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception __ ) { if(_ is not null) { _.Fatal(__,StartUpFail); await _.DisposeAsync(); } }
    }
}