namespace Weblzr;

internal sealed partial class Server
{
    private static void SetupLogging(WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog();

        LoggingLevelSwitch s = new LoggingLevelSwitch(LogEventLevel.Verbose);

        IConfigurationRoot c = new ConfigurationBuilder().AddJsonFile(ConfigFilePath,true,true).Build();

        AppDomain.CurrentDomain.ProcessExit += (s,e) => { Log.Information(HostProcessExit,ProcessId); Log.CloseAndFlush(); };

        UpdateCall = c.GetReloadToken().RegisterChangeCallback(UpdateConfig,new Tuple<IConfigurationRoot,LoggingLevelSwitch>(c,s)); c.Reload();

        Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(s).WriteTo.Console(formatProvider:InvariantCulture).WriteTo.File(LogFilePath,formatProvider:InvariantCulture).CreateLogger();
    }

    private static String LogFilePath => @"C:\KusDepotLogs\Weblzr\Weblzr-" + ProcessId + @".log"; 
}