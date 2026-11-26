namespace KusDepotSolutions;

internal static partial class Server
{
    internal static HostKey? HostKey;

    public static async Task Main()
    {
        IToolWebHost? host = null;

        try
        {
            SetupLogging();

            host = await WebFactory.CreateWeb().BuildWebHostAsync();

            await host.StartHostAsync(key:HostKey); await Task.Delay(Timeout.Infinite);
        }
        catch ( OperationCanceledException ) { }

        catch ( Exception _ ) { Log.Logger.Fatal(_,WebServerFail); Environment.Exit(1); }

        finally
        {
            if(host is not null)
            {
                await host.StopHostAsync(key:HostKey,cancel:CancellationToken.None); await host.DisposeAsync(key:HostKey);
            }
        }
    }
}