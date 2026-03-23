using static System.Globalization.CultureInfo;
using static DataPodServices.CoreCache.CoreCacheStrings;

namespace DataPodServices.CoreCache;

public sealed partial class CoreCacheService
{
    private Logger Logger = null!;

    private void SetupLogging()
    {
        LoggingLevelSwitch s = new LoggingLevelSwitch(LogEventLevel.Verbose);

        Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(s)
            .WriteTo.OpenTelemetry(this.GetLoggingExporterConfigurator(s))
            .WriteTo.File(Path.Join(LogDirectory,$"{this.GetPrimaryKeyString()}.log"),formatProvider:InvariantCulture)
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