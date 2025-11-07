namespace LabX02;

internal class Space
{
    private static async Task Main()
    {
        SetupLog();

        Settings.NoExceptions = false;

        await Task.Delay(Timeout.Infinite);
    }

    private static void SetupLog()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();
    }
}