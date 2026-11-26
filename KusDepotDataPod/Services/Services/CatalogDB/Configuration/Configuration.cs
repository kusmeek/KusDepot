namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    private IConfiguration? Config;

    public void SetupConfiguration(IConfiguration configuration)
    {
        this.Config = configuration.GetSection("CatalogDB");
    }
}