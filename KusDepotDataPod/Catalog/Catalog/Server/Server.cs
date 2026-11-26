namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    public void SetupServer(WebApplicationBuilder builder)
    {
        builder.Services.AddRouting();

        builder.WebHost.UseKestrelCore();
    }
}