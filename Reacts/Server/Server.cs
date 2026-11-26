namespace KusDepot.Reacts;

internal static partial class Server
{
    internal static HostKey? HostKey;

    public static async Task Main()
    {
        IToolWebHost? host = null;

        try
        {
            SetupLogging();

            host = ReactorFactory.CreateReactor(); await host.StartHostAsync(key:HostKey,cancel:CancellationToken.None);

            await Task.Delay(Timeout.Infinite,CancellationToken.None);
        }
        catch ( OperationCanceledException ) { }

        catch ( Exception _ ) { Log.Fatal(_,ReactorServerFail); await Log.CloseAndFlushAsync(); throw; }

        finally { if(host is not null) { await host.StopHostAsync(CancellationToken.None,key:HostKey); await host.DisposeAsync(HostKey); } }
    }
}