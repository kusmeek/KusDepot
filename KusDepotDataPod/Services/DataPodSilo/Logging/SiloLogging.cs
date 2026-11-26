using static DataPodServices.DataPodSiloStrings;
using static System.Globalization.CultureInfo;

namespace DataPodServices;

internal sealed partial class DataPodSilo
{
    internal static LoggingLevelSwitch LoggingSwitch = null!;

    internal static OpenTelemetryLogForwarder TelemetryLogForwarder = null!;

    private static void SetupLogging()
    {
        LoggingLevelSwitch s = new LoggingLevelSwitch(LogEventLevel.Information); LoggingSwitch = s;

        TelemetryLogForwarder = new OpenTelemetryLogForwarder(TelemetryResource);

        AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); TelemetryLogForwarder.Dispose(); };

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(s)
            .WriteTo.Console(formatProvider:InvariantCulture)
            .WriteTo.Sink(new SerilogToMicrosoftLoggerSink(TelemetryLogForwarder.CreateLogger("DataPodSilo")))
            .CreateLogger();

        KusDepotLog.Initialize(new SerilogLoggerFactory());
    }
}