using static DataPodServices.DataConfigs.DataConfigsStrings;
using static DataPodServices.DataConfigs.DataConfigsSettings;

namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService
{
    private TracerProvider? Tracer;

    private Dictionary<String,Object>? ResourceAttributes;

    private Dictionary<String,Object> GetResourceAttributes()
    {
        ResourceAttributes ??=
            new Dictionary<String,Object>()
                {{"service.name",TraceServiceName},{"process.pid",ProcessId},{"service.version",1},
                {"service.instance.id",$"{TraceServiceName}-{this.GetActorID()}-{Guid.NewGuid().ToString("N").ToUpperInvariant()}"}};

        return ResourceAttributes;
    }

    private Action<ResourceBuilder> GetResourceConfigurator() { return (rb) => { rb.Clear().AddAttributes(this.GetResourceAttributes()); }; }

    private void ShutdownTelemetry() { try { Tracer?.Shutdown(TelemetryShutdownTimeout); } catch { } }

    private static Action<OtlpExporterOptions> GetTracingExporterConfigurator()
        { return (o) => { if(Uri.TryCreate(GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"),UriKind.Absolute,out Uri? e)) { o.Endpoint = e; } }; }

    private void SetupTelemetry()
    {
        Tracer = Sdk.CreateTracerProviderBuilder().ConfigureResource(this.GetResourceConfigurator()).AddSource(TraceSourceName).AddOtlpExporter(GetTracingExporterConfigurator()).Build();
    }
}