namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    internal void ConfigureHTTP(WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(GetPort(builder.Configuration)));
    }
}
