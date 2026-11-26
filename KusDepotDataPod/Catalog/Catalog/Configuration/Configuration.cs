namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    public void ConfigureCatalogServer(WebApplication server)
    {
        server.UseAzureAppConfiguration();

        server.UseExceptionHandler();

        MapSearchElements(server); MapSearchServices(server); MapSearchCommands(server);

        MapSearchMedia(server); MapSearchNotes(server); MapSearchTags(server);

        server.MapGet("Catalog/Health", () => Results.Ok()).DisableHttpMetrics();

        server.MapHealthChecks("Catalog/Ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(DataPodHealthCheckTags.Ready)
        }).DisableHttpMetrics();

        SetupOpenApi(server);
    }

    public static WebApplicationOptions GetCatalogOptions()
    {
        return new(){ ApplicationName = ServiceName, EnvironmentName = DataPodAppConfiguration.GetEnvironmentName() };
    }
}