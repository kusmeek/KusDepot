using Serilog;

namespace KusDepot.ToolGrains;

internal sealed partial class ToolSilo : StatefulService
{
    public ToolSilo(StatefulServiceContext context) : base(context)
    {
        try { SetupLogging(); }

        catch ( Exception _ ) { Log.Fatal(_,ToolSiloFail); Log.CloseAndFlush(); throw; }
    }

    protected override async Task RunAsync(CancellationToken token)
    {
        IToolGenericHost? host = null;

        try
        {
            host = await ConfigureToolSilo().BuildGenericHostAsync(cancel:token);

            await host.StartHostAsync(token,key:HostKey);

            Log.Information(ToolSiloStarted); await Task.Delay(Timeout.Infinite,token);
        }
        catch ( OperationCanceledException ) { Log.Information(ToolSiloStopped); }

        catch ( Exception _ ) { Log.Fatal(_,ToolSiloFail); await Log.CloseAndFlushAsync(); throw; }

        finally { if(host is not null) { await host.StopHostAsync(CancellationToken.None,key:HostKey); await host.DisposeAsync(HostKey); } }
    }

    protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners() { return Array.Empty<ServiceReplicaListener>(); }

    protected override async Task OnCloseAsync(CancellationToken token) { Log.Information(ToolSiloClosing); await Log.CloseAndFlushAsync(); await base.OnCloseAsync(token); }
}