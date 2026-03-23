using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;

namespace KusDepot.ToolFabric;

internal sealed partial class ToolFabricHost
{
    private static IToolWebHostBuilder ConfigureToolFabricHost(ToolFabric service)
    {
        IToolWebHostBuilder builder = ToolBuilderFactory.CreateWebHostBuilder()
            .UseBuilderOptions(ToolFabric.GetToolFabricOptions())
            .ConfigureWebApplication((a) => service.ConfigureToolFabricServer(a));

        builder.ConfigureTool((c,t) => { HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey; });

        builder.Builder.Services.AddServiceModelServices();

        builder.Builder.Services.AddSingleton(service);

        builder.UseLogger(new SerilogLoggerFactory());

        if(builder.Builder.Environment.IsDevelopment())
        {
            builder.Builder.Services.AddServiceModelMetadata();
        }

        builder.Builder.Services.AddSerilog();

        ToolFabric.ConfigureTLS(builder.Builder);

        ToolFabric.SetupServer(builder.Builder);

        builder.UseConsoleLifetime().Seal();

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

    private static HostKey? HostKey;
}