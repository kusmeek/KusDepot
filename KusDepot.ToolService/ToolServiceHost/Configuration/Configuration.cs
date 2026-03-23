namespace KusDepot.ToolService;

internal sealed partial class ToolServiceHost
{
    private static IToolWebHostBuilder ConfigureToolServiceHost(ToolService service)
    {
        IToolWebHostBuilder builder = ToolBuilderFactory.CreateWebHostBuilder()
            .UseBuilderOptions(ToolService.GetToolServiceOptions())
            .ConfigureWebApplication((a) => service.ConfigureToolServiceServer(a));

        builder.ConfigureTool((c,t) => { HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey; });

        builder.Builder.Configuration.AddJsonFile(ToolService.ConfigFilePath);

        builder.Builder.Services.AddServiceModelServices();

        if(builder.Builder.Environment.IsDevelopment())
        {
            builder.Builder.Services.AddServiceModelMetadata();
        }

        builder.Builder.Services.AddSingleton(service);

        ToolService.ConfigureTLS(builder.Builder);

        ToolService.SetupServer(builder.Builder);

        builder.UseConsoleLifetime().Seal();

        SetupLogging(builder.Builder);

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