namespace KusDepot.Data;

internal sealed partial class CoreCache
{
    private void SetupLogging()
    {
        LoggingLevelSwitch s = new LoggingLevelSwitch(LogEventLevel.Verbose);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(s)
            .WriteTo.OpenTelemetry(this.GetLoggingExporterConfigurator(s))
            .WriteTo.File(LogFilePath,formatProvider:CultureInfo.InvariantCulture)
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