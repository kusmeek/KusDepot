namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    public void SetupServer(WebApplicationBuilder builder)
    {
        builder.Services.AddRouting();

        builder.WebHost.UseKestrelCore();

        builder.WebHost.ConfigureKestrel( (o) => { o.Limits.MaxRequestBufferSize = null; o.Limits.MaxRequestBodySize = null; o.Limits.MaxResponseBufferSize = null; });
    }
}