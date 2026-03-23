namespace KusDepot.ToolService;

internal sealed partial class ToolServiceHost
{
    private static async Task Main()
    {
        IToolWebHost? host = null;

        try
        {
            var service = await ToolServiceFactory.Create();

            host = await ConfigureToolServiceHost(service).BuildWebHostAsync();

            await host.StartHostAsync(key:HostKey); await Task.Delay(Timeout.Infinite);
        }
        catch ( Exception _ ) { Log.Fatal(_,ToolServiceFail); await Log.CloseAndFlushAsync(); }
        
        finally { if(host is not null) { await host.StopHostAsync(key:HostKey); await host.DisposeAsync(HostKey); } }
    }
}