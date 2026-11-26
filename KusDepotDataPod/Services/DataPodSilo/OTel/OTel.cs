using static DataPodServices.DataPodSiloStrings;

namespace DataPodServices;

internal sealed partial class DataPodSilo
{
    private static readonly ProcessTelemetryResource TelemetryResource = new(TraceServiceName);

    private static void SetupTelemetry(IServiceCollection services)
    {
        services.AddSingleton(Sdk.CreateTracerProviderBuilder()
            .ConfigureResource((rb) => rb.Clear().AddAttributes(TelemetryResource.Attributes))
            .AddSource(DataPodTelemetrySources.Configuration)
            .AddSource(DataPodTelemetrySources.CatalogDB)
            .AddConfiguredExporters()
            .Build());

        services.AddSingleton(Sdk.CreateMeterProviderBuilder()
            .ConfigureResource((rb) => rb.Clear().AddAttributes(TelemetryResource.Attributes))
            .AddProcessInstrumentation()
            .AddConfiguredExporters()
            .Build());
    }
}