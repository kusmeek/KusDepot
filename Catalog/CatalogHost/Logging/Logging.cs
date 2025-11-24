namespace KusDepot.Data;

internal sealed partial class CatalogHost
{
    private void SetupLogging()
    {
        LoggingLevelSwitch s = new LoggingLevelSwitch(LogEventLevel.Verbose);

        IConfigurationRoot c = new ConfigurationBuilder().AddJsonFile(ConfigFilePath,true,true).Build();

        Log.Logger = new LoggerConfiguration()
            .Enrich.WithClientIp()
            .MinimumLevel.ControlledBy(s)
            .WriteTo.Console(formatProvider:CultureInfo.InvariantCulture)
            .WriteTo.OpenTelemetry(this.GetLoggingExporterConfigurator(s))
            .WriteTo.File(LogFilePath,formatProvider:CultureInfo.InvariantCulture)
            .CreateLogger();

        AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); };

        UpdateCall = c.GetReloadToken().RegisterChangeCallback(UpdateConfig,new Tuple<IConfigurationRoot,LoggingLevelSwitch>(c,s)); c.Reload();
    }

    private Action<BatchedOpenTelemetrySinkOptions> GetLoggingExporterConfigurator(LoggingLevelSwitch s)
    {
        return (o) =>
        {
            o.LevelSwitch = s;
            o.ResourceAttributes = Catalog.GetResourceAttributes(this.Context);
            o.Endpoint = GetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT") ?? o.Endpoint;
        };
    }
}