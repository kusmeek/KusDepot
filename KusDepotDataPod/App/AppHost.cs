internal class AppHost
{
    private static async Task Main(String[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console(formatProvider:InvariantCulture).CreateLogger();

        var ab = ToolBuilderFactory.CreateAspireHostBuilder(); ab.Builder.Configuration.AddJsonFile("appsettings.json");

        ab.ConfigureServices((x,s) => s.AddSingletonWithInterfaces(ToolBuilderFactory.CreateHostBuilder().BuildHost()));

        var kds = Environment.GetEnvironmentVariable("KusDepotSolution"); var config = ab.Builder.Configuration;

        var o = new DistributedApplicationOptions() { AllowUnsecuredTransport = true , Args = args };

        ab.UseBuilderOptions(o).UseConsoleLifetime().UseLogger(new SerilogLoggerFactory()).Seal().AutoStart(); ab.Builder.Services.AddSerilog(Log.Logger);

        ab.Builder.AddExecutable("Catalog",Path.Join(kds,config["Executables:Catalog:Command"]!),Path.Join(kds,config["Executables:Catalog:WorkingDirectory"]!));

        ab.Builder.AddExecutable("Services",Path.Join(kds,config["Executables:Services:Command"]!),Path.Join(kds,config["Executables:Services:WorkingDirectory"]!));

        ab.Builder.AddExecutable("DataControl",Path.Join(kds,config["Executables:DataControl:Command"]!),Path.Join(kds,config["Executables:DataControl:WorkingDirectory"]!));

        var aspirehost = ab.BuildAspireHost();

        await Task.Delay(Timeout.Infinite);
    }
}