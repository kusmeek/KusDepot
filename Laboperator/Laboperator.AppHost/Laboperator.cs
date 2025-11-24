internal class Laboperator
{
    private static async Task Main(String[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console(formatProvider:InvariantCulture).CreateLogger();

        var ab = ToolBuilderFactory.CreateAspireHostBuilder(); ab.Builder.Configuration.AddJsonFile("appsettings.json");

        ab.ConfigureServices((x,s) => s.AddSingletonWithInterfaces(ToolBuilderFactory.CreateHostBuilder().BuildHost()));

        var kds = Environment.GetEnvironmentVariable("KusDepotSolution"); var config = ab.Builder.Configuration;;

        var o = new DistributedApplicationOptions() { AllowUnsecuredTransport = true , Args = args , DisableDashboard = true };

        ab.UseBuilderOptions(o).UseConsoleLifetime().UseLogger(new SerilogLoggerFactory()).Seal().AutoStart(); ab.Builder.Services.AddSerilog(Log.Logger);

        var Lab = ab.Builder.AddExecutable("Laboratory",Path.Join(kds,config["Executables:Laboratory:Command"]!),Path.Join(kds,config["Executables:Laboratory:WorkingDirectory"]!));

        ab.Builder.AddExecutable("Labspace",Path.Join(kds,config["Executables:Labspace:Command"]!),Path.Join(kds,config["Executables:Labspace:WorkingDirectory"]!)).WaitFor(Lab);

        var aspirehost = ab.BuildAspireHost();

        await Task.Delay(Timeout.Infinite);
    }
}