using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;

namespace KusDepot.ToolGrains;

internal sealed partial class ToolSilo
{
    private static HostKey? HostKey;

    private static IToolGenericHostBuilder ConfigureToolSilo()
    {
        var builder = ToolBuilderFactory.CreateGenericHostBuilder();

        builder.ConfigureTool((c,t) =>
        {
            HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey;
        });

        builder.Builder.Configuration.AddJsonFile(ConfigFilePath);

        builder.Builder.Services.AddSerilog(Log.Logger);

        builder.UseLogger(new SerilogLoggerFactory());

        builder.UseConsoleLifetime().Seal();

        ConfigureOrleans(builder);

        return builder;
    }

    private static IToolGenericHostBuilder ConfigureOrleans(IToolGenericHostBuilder builder)
    {
        SiloConfig? sc = builder.Builder.Configuration?.GetSection("Silo")?.Get<SiloConfig>();

        if(sc is null || sc.Validate() is false) { throw new InvalidOperationException("Invalid Silo Configuration"); }

        builder.UseOrleans((b) =>
        {
            b.UseLocalhostClustering(sc.SiloPort,sc.GatewayPort,null,sc.ServiceId,sc.ClusterId);

            b.ConfigureEndpoints(IPAddress.Parse(sc.IPAddress),sc.SiloPort,sc.GatewayPort);

            b.AddMemoryGrainStorageAsDefault(); b.UseTransactions();

            b.Configure<GrainCollectionOptions>((o) =>
            {
                o.CollectionQuantum = TimeSpan.FromSeconds(60);

                o.CollectionAge = TimeSpan.FromSeconds(120);
            });
        });

        return builder;
    }

    private static IDisposable? UpdateCall;

    private static void UpdateConfig(Object? o)
    {
        var z = o as Tuple<IConfigurationRoot,LoggingLevelSwitch>;

        LoggingLevelSwitch? s = z!.Item2; IConfigurationRoot? c = z!.Item1;

        if(Enum.TryParse(c?["Serilog:MinimumLevel"] ?? "Verbose",out LogEventLevel l)) { s!.MinimumLevel = l; }

        if(Boolean.TryParse(c?["KusDepotExceptions:Enabled"] ?? "False",out Boolean e)) { Settings.NoExceptions = !e; }

        UpdateCall = c?.GetReloadToken().RegisterChangeCallback(UpdateConfig,new Tuple<IConfigurationRoot,LoggingLevelSwitch>(c,s));
    }

    private static String ConfigFilePath => AppContext.BaseDirectory + @"\appsettings.json";
}