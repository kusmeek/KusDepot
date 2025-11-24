namespace KusDepot.Data;

internal sealed partial class Management
{
    private MeterProvider? Meter;

    private TracerProvider? Tracer;

    private Dictionary<String,Object>? ResourceAttributes;

    private Dictionary<String,Object> GetResourceAttributes()
    {
        ResourceAttributes ??=
            new Dictionary<String,Object>()
                {{"service.name",TraceServiceName},{"process.pid",ProcessId},
                {"service.version",this.ActorService.Context.CodePackageActivationContext.CodePackageVersion},
                {"service.instance.id",$"{TraceServiceName}-{this.GetActorID()}-{Guid.NewGuid().ToString("N").ToUpperInvariant()}"}};

        return ResourceAttributes;
    }

    private Action<ResourceBuilder> GetResourceConfigurator() { return (rb) => { rb.Clear().AddAttributes(this.GetResourceAttributes()); }; }

    private void ShutdownTelemetry() { try { Meter?.Shutdown(TelemetryShutdownTimeout); Tracer?.Shutdown(TelemetryShutdownTimeout); } catch { } }

    private static Action<OtlpExporterOptions> GetTracingExporterConfigurator()
        { return (o) => { if(Uri.TryCreate(GetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT"),UriKind.Absolute,out Uri? e)) { o.Endpoint = e; } }; }

    private static Action<OtlpExporterOptions> GetMetricsExporterConfigurator()
        { return (o) => { if(Uri.TryCreate(GetEnvironmentVariable("OTEL_EXPORTER_OTLP_METRICS_ENDPOINT"),UriKind.Absolute,out Uri? e)) { o.Endpoint = e; } }; }

    private void SetupTelemetry()
    {
        Tracer = Sdk.CreateTracerProviderBuilder().ConfigureResource(this.GetResourceConfigurator()).AddSource(TraceSourceName).AddOtlpExporter(GetTracingExporterConfigurator()).Build();

        Meter = Sdk.CreateMeterProviderBuilder().ConfigureResource(this.GetResourceConfigurator()).AddProcessInstrumentation().AddRuntimeInstrumentation().AddOtlpExporter(GetMetricsExporterConfigurator()).Build();
    }
}