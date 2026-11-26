using static System.Globalization.CultureInfo;

namespace DataPodServices;

public sealed class DataPodServiceLoggers : IDisposable
{
    public DataPodServiceLoggers()
    {
        this.DataConfigsLoggingSwitch = new LoggingLevelSwitch(LogEventLevel.Information);

        this.CatalogDBLoggingSwitch = new LoggingLevelSwitch(LogEventLevel.Information);

        this.DataConfigs = CreateLogger(this.DataConfigsLoggingSwitch,DataPodSilo.TelemetryLogForwarder.CreateLogger("DataPodSilo.DataConfigs"));

        this.CatalogDB = CreateLogger(this.CatalogDBLoggingSwitch,DataPodSilo.TelemetryLogForwarder.CreateLogger("DataPodSilo.CatalogDB"));
    }

    public ILogger DataConfigs { get; }

    public ILogger CatalogDB { get; }

    public LoggingLevelSwitch DataConfigsLoggingSwitch { get; }

    public LoggingLevelSwitch CatalogDBLoggingSwitch { get; }

    public void Dispose()
    {
        (this.DataConfigs as IDisposable)?.Dispose();

        (this.CatalogDB as IDisposable)?.Dispose();
    }

    private static Logger CreateLogger(LoggingLevelSwitch levels , Microsoft.Extensions.Logging.ILogger telemetryLogger)
    {
        return new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levels)
            .WriteTo.Console(formatProvider:InvariantCulture)
            .WriteTo.Sink(new SerilogToMicrosoftLoggerSink(telemetryLogger))
            .CreateLogger();
    }
}