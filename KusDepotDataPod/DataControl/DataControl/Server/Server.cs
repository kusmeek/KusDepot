namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    internal static void SetupServer(WebApplicationBuilder builder)
    {
        builder.Services.AddRouting();

        builder.WebHost.UseKestrelCore();
    }
}