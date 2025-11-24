using Serilog;

namespace KusDepot.Dapr;

internal sealed partial class SupervisorHost : StatelessService
{
    public SupervisorHost(StatelessServiceContext context) : base(context)
    {
        try { SetupLogging(); }

        catch ( Exception _ ) { Log.Fatal(_,DaprSupervisorFail); Log.CloseAndFlush(); throw; }
    }

    protected override async Task RunAsync(CancellationToken token)
    {
        IToolHost? host = null;

        try
        {
            host = await ConfigureDaprSupervisorHost(ToolBuilderFactory.CreateHostBuilder(),await DaprSupervisorFactory.BuildSupervisorAsync(token)).BuildHostAsync(token);

            await host.StartHostAsync(key:HostKey,cancel:token); Log.Information(DaprSupervisorStarted); await Task.Delay(Timeout.Infinite,token);
        }
        catch ( OperationCanceledException ) { Log.Information(DaprSupervisorStopped); }

        catch ( Exception _ ) { Log.Fatal(_,DaprSupervisorFail); await Log.CloseAndFlushAsync(); throw; }

        finally { if(host is not null) { await host.StopHostAsync(CancellationToken.None,key:HostKey); await host.DisposeAsync(HostKey); } }
    }

    protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners() { return Array.Empty<ServiceInstanceListener>(); }

    protected override async Task OnCloseAsync(CancellationToken token) { Log.Information(DaprSupervisorClosing); await Log.CloseAndFlushAsync(); await base.OnCloseAsync(token); }
}