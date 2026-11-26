using Serilog.Core;
using Serilog.Events;

internal class Laboperator
{
    private static async Task Main(String[] args)
    {
        var s = new LoggingLevelSwitch(LogEventLevel.Information);

        Log.Logger = new LoggerConfiguration().WriteTo.Console(formatProvider:InvariantCulture).MinimumLevel.ControlledBy(s).CreateLogger();

        var ab = ToolBuilderFactory.CreateAspireHostBuilder(); ab.Builder.Configuration.AddJsonFile("appsettings.json");

        var o = new DistributedApplicationOptions() { AllowUnsecuredTransport = true , Args = args , DisableDashboard = true };

        ab.UseBuilderOptions(o).UseConsoleLifetime().UseLogger(new SerilogLoggerFactory()).Seal().AutoStart();
        
        ab.Builder.Services.AddSerilog(Log.Logger);

        var kds = Environment.GetEnvironmentVariable("KusDepotSolution"); var config = ab.Builder.Configuration;

        ab.Builder.AddExecutable("Laboratory",Path.Join(kds,config["Executables:Laboratory:Command"]!),Path.Join(kds,config["Executables:Laboratory:WorkingDirectory"]!)).WithOtlpExporter();

        var aspirehost = ab.BuildAspireHost();

        await Task.Delay(Timeout.Infinite);
    }
}