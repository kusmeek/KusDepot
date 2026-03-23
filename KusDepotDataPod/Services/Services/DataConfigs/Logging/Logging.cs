using static DataPodServices.DataConfigs.DataConfigsStrings;
using static System.Globalization.CultureInfo;
namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService
{
    private Logger Logger = null!;

    private void SetupLogging()
    {
        LoggingLevelSwitch s = new LoggingLevelSwitch(LogEventLevel.Verbose);

        Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(s)
            .WriteTo.File(LogFilePath,formatProvider:InvariantCulture)
            .WriteTo.OpenTelemetry(this.GetLoggingExporterConfigurator(s))
            .CreateLogger();

        UpdateCall = Config?.GetReloadToken().RegisterChangeCallback(UpdateConfig,s); Config?.Reload();
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