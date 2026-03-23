using static DataPodServices.DataPodSiloStrings;
using static DataPodServices.DataPodSiloSettings;

namespace DataPodServices;

internal sealed partial class DataPodSilo
{
    private static MeterProvider? Meter;

    private static Dictionary<String,Object>? ResourceAttributes;

    private static Dictionary<String,Object> GetResourceAttributes()
    {
        ResourceAttributes ??=
            new Dictionary<String,Object>()
                {{"service.name",TraceServiceName},{"process.pid",ProcessId},
                {"service.version",1}};

        return ResourceAttributes;
    }

    private static Action<ResourceBuilder> GetResourceConfigurator() { return (rb) => { rb.Clear().AddAttributes(GetResourceAttributes()); }; }

    private static void ShutdownTelemetry() { try { Meter?.Shutdown(TelemetryShutdownTimeout); } catch { } }

    private static Action<OtlpExporterOptions> GetMetricsExporterConfigurator()
        { return (o) => { if(Uri.TryCreate(GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"),UriKind.Absolute,out Uri? e)) { o.Endpoint = e; } }; }

    private static void SetupTelemetry()
    {
        Meter = Sdk.CreateMeterProviderBuilder().ConfigureResource(GetResourceConfigurator()).AddProcessInstrumentation().AddRuntimeInstrumentation().AddOtlpExporter(GetMetricsExporterConfigurator()).Build();
    }
}