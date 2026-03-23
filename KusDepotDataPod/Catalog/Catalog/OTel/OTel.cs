namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    private MeterProvider? Meter;

    private TracerProvider? Tracer;

    public static Dictionary<String,Object> GetResourceAttributes()
    {
        return new Dictionary<String,Object>()
                {{"service.name",TraceServiceName},{"process.pid",ProcessId},{"service.version",1},
                {"service.instance.id",$"{Guid.NewGuid().ToString("N").ToUpperInvariant()}"}};
    }

    public Action<ResourceBuilder> GetResourceConfigurator() { return (rb) => { rb.Clear().AddAttributes(GetResourceAttributes()); }; }

    private static Action<OtlpExporterOptions> GetTracingExporterConfigurator()
        { return (o) => { if(Uri.TryCreate(GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"),UriKind.Absolute,out Uri? e)) { o.Endpoint = e; } }; }

    private static Action<OtlpExporterOptions> GetMetricsExporterConfigurator()
        { return (o) => { if(Uri.TryCreate(GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"),UriKind.Absolute,out Uri? e)) { o.Endpoint = e; } }; }

    public static Action<TracerProviderBuilder> GetTracingConfigurator()
        { return (tpb) => { tpb.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddSource(TraceSourceName).AddOtlpExporter(GetTracingExporterConfigurator()); }; }

    public static Action<MeterProviderBuilder> GetMetricsConfigurator()
        { return (mpb) => { mpb.AddAspNetCoreInstrumentation().AddMeter(Metrics).AddProcessInstrumentation().AddRuntimeInstrumentation().AddOtlpExporter(GetMetricsExporterConfigurator()); }; }

    private static readonly String[] Metrics = ["System.Net.Http"];

    private void SetupTelemetry()
    {
        Tracer = Sdk.CreateTracerProviderBuilder().ConfigureResource(this.GetResourceConfigurator()).AddSource(TraceSourceName).AddOtlpExporter(GetTracingExporterConfigurator()).Build();

        Meter = Sdk.CreateMeterProviderBuilder().ConfigureResource(this.GetResourceConfigurator()).AddProcessInstrumentation().AddRuntimeInstrumentation().AddOtlpExporter(GetMetricsExporterConfigurator()).Build();
    }
}