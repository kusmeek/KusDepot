namespace KusDepot.Data;

internal sealed partial class CatalogHost : StatelessService
{
    public CatalogHost(StatelessServiceContext context) : base(context)
    {
        try { this.SetupLogging(); }

        catch (Exception _ ) { Log.Fatal(_,CatalogFail); Log.CloseAndFlush(); throw; }
    }

    protected override async Task RunAsync(CancellationToken token)
    {
        IToolWebHost? host = null;

        try
        {
            var catalog = CatalogFactory.Create(this.Context);

            using(var _ = new FabricClient()) { await _.PropertyManager.PutPropertyAsync(this.Context.ServiceName,"EndpointLocator",catalog.URL,TimeSpan.MaxValue,token); }

            host = await ConfigureCatalogHost(catalog).BuildWebHostAsync(cancel:token); await host.StartHostAsync(key:HostKey,cancel:token);

            Log.Information(CatalogStartedURL,catalog.URL); await Task.Delay(Timeout.Infinite,token);
        }
        catch ( OperationCanceledException ) { Log.Information(CatalogStopped); }

        catch ( Exception _ ) { Log.Fatal(_,CatalogFail); await Log.CloseAndFlushAsync(); throw; }

        finally { if(host is not null) { await host.StopHostAsync(CancellationToken.None,key:HostKey); await host.DisposeAsync(HostKey); } }
    }

    protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners() => Array.Empty<ServiceInstanceListener>();

    protected override async Task OnCloseAsync(CancellationToken token) { Log.Information(CatalogClosing); await Log.CloseAndFlushAsync(); await base.OnCloseAsync(token); }
}