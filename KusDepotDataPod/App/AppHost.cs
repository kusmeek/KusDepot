internal class AppHost
{
    private static async Task Main()
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        var ab = ToolBuilderFactory.CreateAspireHostBuilder();

        var o = new DistributedApplicationOptions() { AllowUnsecuredTransport = true};

        ab.UseBuilderOptions(o).UseConsoleLifetime().UseLogger(new SerilogLoggerFactory()).Seal().AutoStart();

        ab.Builder.Configuration.AddJsonFile("appsettings.json"); ab.Builder.Services.AddSerilog(Log.Logger);

        var kds = Environment.GetEnvironmentVariable("KusDepotSolution"); var config = ab.Builder.Configuration;

        ab.Builder.AddExecutable("Catalog",Path.Join(kds,config["Executables:Catalog:Command"]!),Path.Join(kds,config["Executables:Catalog:WorkingDirectory"]!)).WithOtlpExporter();

        ab.Builder.AddExecutable("DataPodSilo",Path.Join(kds,config["Executables:DataPodSilo:Command"]!),Path.Join(kds,config["Executables:DataPodSilo:WorkingDirectory"]!)).WithOtlpExporter();

        ab.Builder.AddExecutable("DataControl",Path.Join(kds,config["Executables:DataControl:Command"]!),Path.Join(kds,config["Executables:DataControl:WorkingDirectory"]!)).WithOtlpExporter();

        var aspirehost = await ab.BuildAspireHostAsync().ConfigureAwait(false);

        await Task.Delay(Timeout.Infinite);
    }
}