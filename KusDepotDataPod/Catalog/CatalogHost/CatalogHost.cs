namespace DataPodServices.Catalog;

internal sealed partial class CatalogHost
{
    public CatalogHost()
    {
        try { SetupLogging(); }

        catch ( Exception _ ) { Log.Fatal(_,CatalogFail); Log.CloseAndFlush(); throw; }
    }

    public async Task RunAsync(CancellationToken cancel)
    {
        IToolWebHost? host = null;

        try
        {
            var catalog = CatalogFactory.Create();

            host = await ConfigureCatalogHost(catalog).BuildWebHostAsync(cancel:cancel); await host.StartHostAsync(key:HostKey,cancel:cancel);

            Log.Information(CatalogStartedURL,catalog.URL); await Task.Delay(Timeout.Infinite,cancel);
        }
        catch ( OperationCanceledException ) { Log.Information(CatalogStopped); }

        catch ( Exception _ ) { Log.Fatal(_,CatalogFail); await Log.CloseAndFlushAsync(); throw; }

        finally { if(host is not null) { await host.StopHostAsync(CancellationToken.None,key:HostKey); await host.DisposeAsync(HostKey); } }
    }
}