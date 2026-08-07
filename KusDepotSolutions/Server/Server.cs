namespace KusDepotSolutions;

internal static partial class Server
{
    public static async Task Main()
    {
        try { SetupLogging(); await WebFactory.CreateWeb().RunAsync(); }

        catch ( Exception _ ) { Log.Logger.Fatal(_,WebServerFail); Environment.Exit(1); }
    }
}