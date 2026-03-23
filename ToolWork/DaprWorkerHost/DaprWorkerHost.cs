using Serilog;

namespace ToolWork;

internal sealed partial class DaprWorkerHost : StatelessService
{
    public DaprWorkerHost(StatelessServiceContext context) : base(context)
    {
        try { SetupLogging(); }

        catch ( Exception _ ) { Log.Fatal(_,DaprWorkerFail); Log.CloseAndFlush(); throw; }
    }

    protected override async Task RunAsync(CancellationToken token)
    {
        IToolWebHost? host = null;

        try
        {
            host = await ConfigureDaprWorkerHost().BuildWebHostAsync(cancel:token); await host.StartHostAsync(key:HostKey,cancel:token);

            Log.Information(DaprWorkerStarted); await Task.Delay(Timeout.Infinite,token);
        }
        catch ( OperationCanceledException ) { Log.Information(DaprWorkerStopped); } 

        catch ( Exception _ ) { Log.Fatal(_,DaprWorkerFail); await Log.CloseAndFlushAsync(); throw; }

        finally { if(host is not null) { await host.StopHostAsync(CancellationToken.None,key:HostKey); await host.DisposeAsync(HostKey); } }
    }

    protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners() { return Array.Empty<ServiceInstanceListener>(); }

    protected override async Task OnCloseAsync(CancellationToken token) { Log.Information(DaprWorkerClosing); await Log.CloseAndFlushAsync(); await base.OnCloseAsync(token); }
}