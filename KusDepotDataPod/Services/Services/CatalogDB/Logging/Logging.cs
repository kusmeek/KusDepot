using static DataPodServices.CatalogDB.CatalogDBStrings;
using static System.Globalization.CultureInfo;
namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    private Logger Logger = null!;

    private void SetupLogging()
    {
        LoggingLevelSwitch s = new LoggingLevelSwitch(LogEventLevel.Verbose);

        Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(s)
            .WriteTo.OpenTelemetry(this.GetLoggingExporterConfigurator(s))
            .WriteTo.File($"{LogDirectory}{"CatalogDB"}-{GetActorID()}.log",formatProvider:InvariantCulture,shared:true)
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