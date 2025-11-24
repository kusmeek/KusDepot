namespace KusDepot.Data;

internal static class CatalogStartUp
{
    private static async Task Main()
    {
        Logger? _ = default;

        try
        {
            _ = new LoggerConfiguration().WriteTo.File(LogFilePath,formatProvider:CultureInfo.InvariantCulture).CreateLogger();

            await ServiceRuntime.RegisterServiceAsync("CatalogType",(context) => new CatalogHost(context) );

            if(_ is not null) { _.Information(RegisterSuccess,ProcessId); await _.DisposeAsync(); }

            Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception __ ) { if(_ is not null) { _.Fatal(__,StartUpFail); await _.DisposeAsync(); } }
    }
}