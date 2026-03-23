namespace KusDepot.Exams.Tools;

internal static class LoggerHelpers
{
    public static ILogger? CreateILogger(Serilog.ILogger? seriloglogger = null , String? category = null)
    {
        if(seriloglogger is null) { seriloglogger = new Serilog.LoggerConfiguration().CreateLogger(); }

        try
        {
            Serilog.Extensions.Logging.SerilogLoggerFactory factory = new(seriloglogger,dispose:false);

            return factory.CreateLogger(String.IsNullOrEmpty(category) ? "KusDepot.Exams" : category!);
        }
        catch { return null; }
    }
}