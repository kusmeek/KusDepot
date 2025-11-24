namespace KusDepot.Data;

public sealed partial class Catalog
{
    public void ConfigureCatalogServer(WebApplication server)
    {
        server.UseExceptionHandler(); SetupAuth(server);

        MapSearchElements(server); MapSearchServices(server); MapSearchCommands(server);

        MapSearchMedia(server); MapSearchNotes(server); MapSearchTags(server);

        server.MapHealthChecks("Catalog/Health").DisableHttpMetrics().RequireAuthorization(X509Policy);

        SetupOpenApi(server);
    }

    public static WebApplicationOptions GetCatalogOptions()
    {
        try
        {
            String? _ = JsonDocument.Parse(File.ReadAllText(ConfigFilePath)).RootElement.GetProperty("Environment").GetString();

            if(new[]{Environments.Development,Environments.Production,Environments.Staging}.Contains(_))
            {
                return new(){ ApplicationName = ServiceName , EnvironmentName = _ };
            }

            return new(){ ApplicationName = ServiceName };
        }
        catch { return new(){ ApplicationName = ServiceName }; }
    }
}