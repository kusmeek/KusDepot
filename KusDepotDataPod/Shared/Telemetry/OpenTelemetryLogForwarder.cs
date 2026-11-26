namespace DataPodServices;

public sealed class OpenTelemetryLogForwarder : IDisposable
{
    private readonly ILoggerFactory? loggerFactory;

    public OpenTelemetryLogForwarder(ProcessTelemetryResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        String? connectionString = GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

        Boolean hasOtlpEndpoint = !String.IsNullOrWhiteSpace(GetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT"))
            || !String.IsNullOrWhiteSpace(GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"));

        Boolean hasAzureMonitor = !String.IsNullOrWhiteSpace(connectionString);

        if(!hasOtlpEndpoint && !hasAzureMonitor) { return; }

        this.loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.ClearProviders();

            logging.SetMinimumLevel(LogLevel.Trace);

            logging.AddOpenTelemetry(otel =>
            {
                otel.SetResourceBuilder(ResourceBuilder.CreateEmpty().AddAttributes(resource.Attributes));
                otel.IncludeFormattedMessage = true;
                otel.ParseStateValues = true;
                otel.IncludeScopes = true;

                if(hasOtlpEndpoint) { otel.AddOtlpExporter(); }

                if(hasAzureMonitor)
                {
                    otel.AddAzureMonitorLogExporter(options => options.ConnectionString = connectionString!);
                }
            });
        });
    }

    public ILogger CreateLogger(String categoryName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryName);

        return this.loggerFactory?.CreateLogger(categoryName) ?? NullLogger.Instance;
    }

    public void Dispose() => this.loggerFactory?.Dispose();
}
