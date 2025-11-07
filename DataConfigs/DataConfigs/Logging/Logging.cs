namespace KusDepot.Data;

internal sealed partial class DataConfigs
{
    private void SetupLogging()
    {
        LoggingLevelSwitch s = new LoggingLevelSwitch(LogEventLevel.Verbose);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(s)
            .WriteTo.File(LogFilePath,formatProvider:InvariantCulture)
            .WriteTo.OpenTelemetry(this.GetLoggingExporterConfigurator(s))
            .CreateLogger();

        UpdateCall = Config?.GetReloadToken().RegisterChangeCallback(UpdateConfig,s); Config?.Reload();

        AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); };
    }

    private Action<BatchedOpenTelemetrySinkOptions> GetLoggingExporterConfigurator(LoggingLevelSwitch s)
    {
        return (o) =>
        {
            o.LevelSwitch = s;
            o.ResourceAttributes = this.GetResourceAttributes();
            o.Endpoint = GetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT") ?? o.Endpoint;
        };
    }
}