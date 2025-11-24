namespace LabX02;

internal class Space
{
    private static async Task Main()
    {
        SetupLog();

        Settings.NoExceptions = false;
    }

    private static void SetupLog()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();

        KusDepotLog.Initialize(new Serilog.Extensions.Logging.SerilogLoggerFactory());
    }
}