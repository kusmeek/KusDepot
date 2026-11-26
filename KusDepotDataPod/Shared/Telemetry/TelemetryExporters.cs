namespace DataPodServices;

public static class TelemetryExporters
{
    public static TracerProviderBuilder AddConfiguredExporters(this TracerProviderBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if(HasEndpoint("TRACES")) { builder.AddOtlpExporter(); }

        String? connectionString = GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

        if(!String.IsNullOrWhiteSpace(connectionString))
        {
            builder.AddAzureMonitorTraceExporter(options => options.ConnectionString = connectionString);
        }

        return builder;
    }

    public static MeterProviderBuilder AddConfiguredExporters(this MeterProviderBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if(HasEndpoint("METRICS")) { builder.AddOtlpExporter(); }

        String? connectionString = GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

        if(!String.IsNullOrWhiteSpace(connectionString))
        {
            builder.AddAzureMonitorMetricExporter(options => options.ConnectionString = connectionString);
        }

        return builder;
    }

    private static Boolean HasEndpoint(String signal) =>
        !String.IsNullOrWhiteSpace(GetEnvironmentVariable($"OTEL_EXPORTER_OTLP_{signal}_ENDPOINT"))
        || !String.IsNullOrWhiteSpace(GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"));
}
