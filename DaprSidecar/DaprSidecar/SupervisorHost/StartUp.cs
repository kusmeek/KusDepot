using Serilog;

namespace KusDepot.Dapr;

internal static class SupervisorStartUp
{
    private static async Task Main()
    {
        Serilog.Core.Logger? _ = default;

        try
        {
            _ = new LoggerConfiguration().WriteTo.File(LogFilePath,formatProvider:CultureInfo.InvariantCulture).CreateLogger();

            await ServiceRuntime.RegisterServiceAsync("DaprSidecarType",(context) => new SupervisorHost(context) );

            if(_ is not null) { _.Information(RegisterSuccess,ProcessId); await _.DisposeAsync(); }

            Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception __ ) { if(_ is not null) { _.Fatal(__,StartUpFail); await _.DisposeAsync(); } }
    }
}