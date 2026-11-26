namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private static readonly String[] Metrics = ["System.Net.Http"];

    internal static void ConfigureTelemetry(IServiceCollection services , ProcessTelemetryResource resource)
    {
        services.AddSingleton(Sdk.CreateTracerProviderBuilder()
            .ConfigureResource((rb) => rb.Clear().AddAttributes(resource.Attributes))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(TraceSourceName)
            .AddConfiguredExporters()
            .Build());

        services.AddSingleton(Sdk.CreateMeterProviderBuilder()
            .ConfigureResource((rb) => rb.Clear().AddAttributes(resource.Attributes))
            .AddMeter(Metrics)
            .AddProcessInstrumentation()
            .AddConfiguredExporters()
            .Build());
    }
}