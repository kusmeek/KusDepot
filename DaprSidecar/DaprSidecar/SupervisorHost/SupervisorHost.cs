using Serilog;

namespace KusDepot.Dapr;

internal sealed partial class SupervisorHost
{
    public async Task RunAsync(CancellationToken cancel)
    {
        IToolHost? host = null;

        try
        {
            SetupLogging();

            host = await ConfigureDaprSupervisorHost(ToolBuilderFactory.CreateHostBuilder(),await DaprSupervisorFactory.BuildSupervisorAsync(cancel)).BuildHostAsync(cancel);

            await host.StartHostAsync(key:HostKey,cancel:cancel); Log.Information(DaprSupervisorStarted); await Task.Delay(Timeout.Infinite,cancel);
        }
        catch ( OperationCanceledException ) { Log.Information(DaprSupervisorStopped); }

        catch ( Exception _ ) { Log.Fatal(_,DaprSupervisorFail); await Log.CloseAndFlushAsync(); throw; }

        finally { if(host is not null) { await host.StopHostAsync(CancellationToken.None,key:HostKey); await host.DisposeAsync(HostKey); } }
    }
}