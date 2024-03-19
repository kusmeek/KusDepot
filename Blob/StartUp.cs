namespace KusDepot.Data;

internal static class BlobStartUp
{
    private static void Main()
    {
        try
        {
            ActorRuntime.RegisterActorAsync<Blob>( (context,type) => { return new ActorService(context,type,null,null,null,BlobSettings.Actor);}).GetAwaiter().GetResult();

            Log.Logger = new LoggerConfiguration().WriteTo.File(LogFilePath,formatProvider:System.Globalization.CultureInfo.InvariantCulture).CreateLogger();

            AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); };

            Log.Information(StartUpSuccess,ProcessId); Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception _ ) { Log.Fatal(_,StartUpFail); Log.CloseAndFlush(); }
    }
}