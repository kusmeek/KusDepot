namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    internal void ConfigureHTTP(WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(GetPort(builder.Configuration)));
    }
}
