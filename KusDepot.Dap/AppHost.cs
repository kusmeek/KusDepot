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

        ab.Builder.AddExecutable("DaprSidecar",Path.Join(kds,config["Executables:DaprSidecar:Command"]!),Path.Join(kds,config["Executables:DaprSidecar:WorkingDirectory"]!)).WithOtlpExporter();

        var aspirehost = await ab.BuildAspireHostAsync().ConfigureAwait(false);

        await Task.Delay(Timeout.Infinite);
    }
}
