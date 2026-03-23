namespace KusDepot.Reacts;

internal partial class Server
{
    private static void SetupLogging()
    {
        Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
            .WriteTo.Console(formatProvider:CultureInfo.InvariantCulture)
            .CreateLogger();
    }
}