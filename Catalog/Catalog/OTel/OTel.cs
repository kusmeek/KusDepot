namespace KusDepot.Data;

public sealed partial class Catalog
{
    public static Dictionary<String,Object> GetResourceAttributes(StatelessServiceContext context)
    {
        return new Dictionary<String,Object>()
                {{"service.name",TraceServiceName},{"process.pid",ProcessId},
                {"service.version",context.CodePackageActivationContext.CodePackageVersion},
                {"service.instance.id",$"{TraceServiceName}-{Guid.NewGuid().ToString("N").ToUpperInvariant()}"}};
    }

    public Action<ResourceBuilder> GetResourceConfigurator() { return (rb) => { rb.Clear().AddAttributes(GetResourceAttributes(this.Context)); }; }

    private static Action<OtlpExporterOptions> GetTracingExporterConfigurator()
        { return (o) => { if(Uri.TryCreate(GetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT"),UriKind.Absolute,out Uri? e)) { o.Endpoint = e; } }; }

    private static Action<OtlpExporterOptions> GetMetricsExporterConfigurator()
        { return (o) => { if(Uri.TryCreate(GetEnvironmentVariable("OTEL_EXPORTER_OTLP_METRICS_ENDPOINT"),UriKind.Absolute,out Uri? e)) { o.Endpoint = e; } }; }

    public static Action<TracerProviderBuilder> GetTracingConfigurator()
        { return (tpb) => { tpb.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddSource(TraceSourceName).AddOtlpExporter(GetTracingExporterConfigurator()); }; }

    public static Action<MeterProviderBuilder> GetMetricsConfigurator()
        { return (mpb) => { mpb.AddAspNetCoreInstrumentation().AddMeter(Metrics).AddProcessInstrumentation().AddRuntimeInstrumentation().AddOtlpExporter(GetMetricsExporterConfigurator()); }; }

    private static readonly String[] Metrics = ["System.Net.Http"];
}