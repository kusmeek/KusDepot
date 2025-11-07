namespace KusDepot.ToolFabric;

public sealed partial class ToolFabric
{
    public static void SetupServer(WebApplicationBuilder builder)
    {
        builder.Services.AddRouting();

        builder.WebHost.UseKestrelCore();

        builder.WebHost.ConfigureKestrel( (o) => { o.Limits.MaxRequestBufferSize = null; o.Limits.MaxRequestBodySize = null; o.Limits.MaxResponseBufferSize = null; });
    }
}