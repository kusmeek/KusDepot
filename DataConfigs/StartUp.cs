namespace KusDepot.Data;

internal static class DataConfigsStartUp
{
    private static void Main()
    {
        try
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File(LogFilePath,formatProvider:System.Globalization.CultureInfo.InvariantCulture).CreateLogger();

            ActorRuntime.RegisterActorAsync<DataConfigs>( (context,type) => { return new ActorService(context,type); }).GetAwaiter().GetResult();

            AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); };

            Log.Information(StartUpSuccess,ProcessId); Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception _ ) { Log.Fatal(_,StartUpFail); Log.CloseAndFlush(); }
    }
}