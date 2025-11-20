namespace Weblzr;

internal sealed partial class Server
{
    private static void Main()
    {
        ConfigureWeblzrBuilder(ToolBuilderFactory.CreateWebHostBuilder().UseBuilderOptions(GetWeblzrOptions())).ConfigureWebApplication(a => ConfigureWeblzrServer(a)).BuildWebHost().Run();            
    }
}