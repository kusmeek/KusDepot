using static DataPodServices.CatalogDB.CatalogDBStrings;
using static DataPodServices.CatalogDB.CatalogDBSettings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    private TracerProvider? Tracer;

    private Dictionary<String,Object>? ResourceAttributes;

    private Dictionary<String,Object> GetResourceAttributes()
    {
        ResourceAttributes ??=
            new Dictionary<String,Object>()
                {{"catalog.name",GetActorID()},
                {"service.name",TraceServiceName},{"process.pid",ProcessId},{"service.version",1},
                {"service.instance.id",$"{TraceServiceName}-{GetActorID()}-{Guid.NewGuid().ToString("N").ToUpperInvariant()}"}};

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