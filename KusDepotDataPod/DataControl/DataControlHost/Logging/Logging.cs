namespace DataPodServices.DataControl;

internal sealed partial class DataControlHost
{
    private OpenTelemetryLogForwarder? TelemetryLogForwarder;

    private void SetupLogging(IConfigurationRoot configuration)
    {
        LoggingLevelSwitch s = new LoggingLevelSwitch(LogEventLevel.Information);

        this.TelemetryLogForwarder = new OpenTelemetryLogForwarder(TelemetryResource);

        Log.Logger = new LoggerConfiguration()
            .Enrich.WithClientIp()
            .MinimumLevel.ControlledBy(s)
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics",LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware",LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Diagnostics.HealthChecks",LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Extensions.Diagnostics.HealthChecks",LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command",LogEventLevel.Warning)
            .WriteTo.Sink(new SerilogToMicrosoftLoggerSink(this.TelemetryLogForwarder.CreateLogger("DataControl")))
            .WriteTo.Console(formatProvider:CultureInfo.InvariantCulture)
            .CreateLogger();

        AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); this.TelemetryLogForwarder.Dispose(); };

        UpdateConfig(new Tuple<IConfigurationRoot,LoggingLevelSwitch>(configuration,s));
    }
}