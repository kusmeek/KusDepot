using Serilog.Core;
using Serilog.Events;

internal class Laboperator
{
    private static async Task Main(String[] args)
    {
        var s = new LoggingLevelSwitch(LogEventLevel.Verbose);

        Log.Logger = new LoggerConfiguration().WriteTo.Console(formatProvider:InvariantCulture).MinimumLevel.ControlledBy(s).CreateLogger();

        var ab = ToolBuilderFactory.CreateAspireHostBuilder(); ab.Builder.Configuration.AddJsonFile("appsettings.json");

        ab.ConfigureServices((x,s) => s.AddSingletonWithInterfaces(ToolBuilderFactory.CreateHostBuilder().BuildHost()));

        var kds = Environment.GetEnvironmentVariable("KusDepotSolution"); var config = ab.Builder.Configuration;;

        var o = new DistributedApplicationOptions() { AllowUnsecuredTransport = true , Args = args , DisableDashboard = true };

        ab.UseBuilderOptions(o).UseConsoleLifetime().UseLogger(new SerilogLoggerFactory()).Seal().AutoStart(); ab.Builder.Services.AddSerilog(Log.Logger);

        var aspirehost = ab.BuildAspireHost();

        await Task.Delay(Timeout.Infinite);
    }
}