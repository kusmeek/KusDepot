namespace DataPodServices.Catalog;

internal sealed partial class CatalogHost
{
    private void SetupLogging()
    {
        LoggingLevelSwitch s = new LoggingLevelSwitch(LogEventLevel.Verbose);

        IConfigurationRoot c = new ConfigurationBuilder().AddJsonFile(ConfigFilePath,true,true).Build();

        Log.Logger = new LoggerConfiguration()
            .Enrich.WithClientIp()
            .MinimumLevel.ControlledBy(s)
            .WriteTo.OpenTelemetry(GetLoggingExporterConfigurator(s))
            .WriteTo.Console(formatProvider:CultureInfo.InvariantCulture)
            .WriteTo.File(LogFilePath,formatProvider:CultureInfo.InvariantCulture)
            .CreateLogger();

        AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); };

        UpdateCall = c.GetReloadToken().RegisterChangeCallback(UpdateConfig,new Tuple<IConfigurationRoot,LoggingLevelSwitch>(c,s)); c.Reload();
    }

    private static Action<BatchedOpenTelemetrySinkOptions> GetLoggingExporterConfigurator(LoggingLevelSwitch s)
    {
        return (o) =>
        {
            o.LevelSwitch = s;
            o.ResourceAttributes = Catalog.GetResourceAttributes();
            o.Endpoint = GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") ?? o.Endpoint;
        };
    }
}