namespace KusDepot.Reacts;

internal static partial class Server
{
    public static async Task Main()
    {
        try { SetupLogging(); await ReactorFactory.CreateReactor().RunAsync(); }

        catch ( Exception _ ) { Log.Logger.Fatal(_,ReactorServerFail); }
    }
}