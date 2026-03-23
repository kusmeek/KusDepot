namespace KusDepot.Data;

internal static class CatalogDBStartUp
{
    private static async Task Main()
    {
        Logger? _ = default;

        try
        {
            _ = new LoggerConfiguration().WriteTo.File(LogFilePath,formatProvider:InvariantCulture).CreateLogger();

            await ActorRuntime.RegisterActorAsync<CatalogDB>( (context,type) => new ActorService(context,type) );

            if(_ is not null) { _.Information(RegisterSuccess,ProcessId); await _.DisposeAsync(); }

            Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception __ ) { if(_ is not null) { _.Fatal(__,StartUpFail); await _.DisposeAsync(); } }
    }
}