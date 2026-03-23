using Serilog;

namespace KusDepot.ToolFabric;

internal sealed partial class ToolFabricHost : StatefulService
{
    public ToolFabricHost(StatefulServiceContext context) : base(context)
    {
        try { SetupLogging(); }

        catch ( Exception _ ) { Log.Fatal(_,ToolFabricFail); Log.CloseAndFlush(); throw; }
    }

    protected override async Task RunAsync(CancellationToken token)
    {
        IToolWebHost? host = null;

        try
        {
            var service = await ToolFabricFactory.Create(this.Context);

            using(var _ = new FabricClient()) { await _.PropertyManager.PutPropertyAsync(this.Context.ServiceName,"EndpointLocator",service.URL,TimeSpan.MaxValue,token); }

            host = await ConfigureToolFabricHost(service).BuildWebHostAsync(cancel:token); await host.StartHostAsync(key:HostKey);

            Log.Information(ToolFabricStartedURL,service.URL); await Task.Delay(Timeout.Infinite,token);
        }
        catch ( OperationCanceledException ) { Log.Information(ToolFabricStopped); }

        catch ( Exception _ ) { Log.Fatal(_,ToolFabricFail); await Log.CloseAndFlushAsync(); throw; }

        finally { if(host is not null) { await host.StopHostAsync(CancellationToken.None,key:HostKey); await host.DisposeAsync(HostKey); } }
    }

    protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners() { return Array.Empty<ServiceReplicaListener>(); }

    protected override async Task OnCloseAsync(CancellationToken token) { Log.Information(ToolFabricClosing); await Log.CloseAndFlushAsync(); await base.OnCloseAsync(token); }
}