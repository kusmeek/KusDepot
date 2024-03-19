namespace KusDepot.Data;

internal static class CatalogStartUp
{
    private static void Main()
    {
        try
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File(LogFilePath,formatProvider:System.Globalization.CultureInfo.InvariantCulture).CreateLogger();

            AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); };

            ServiceRuntime.RegisterServiceAsync("CatalogType",context => new Catalog(context)).GetAwaiter().GetResult();

            Log.Information(StartUpSuccess,ProcessId); Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception _ ) { Log.Fatal(_,StartUpFail); Log.CloseAndFlush(); }
    }
}