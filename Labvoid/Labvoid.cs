namespace Labvoid;

internal sealed class Space
{
    private static async Task Main()
    {
        SetupLog();

        Settings.NoExceptions = false;
    }

    private static void SetupLog()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();
    }
}