namespace DataPodServices.DataControl;

internal sealed partial class DataControlHost
{
    private static IToolWebHostBuilder ConfigureDataControlHost(DataControl datacontrol)
    {
        IToolWebHostBuilder builder = ToolBuilderFactory.CreateWebHostBuilder()
            .UseBuilderOptions(DataControl.GetDataControlOptions())
            .ConfigureWebApplication(app => datacontrol.ConfigureDataControlServer(app));

        builder.ConfigureTool((c,t) => { HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey; }).Seal();

        ConfigureServices(builder.Builder,datacontrol); builder.UseConsoleLifetime(); return builder;
    }

    private static void ConfigureServices(WebApplicationBuilder builder , DataControl datacontrol)
    {
        ConfigureCoreServices(builder);

        ConfigureDataControlServices(builder);

        ConfigureWebInfrastructure(builder,datacontrol);
    }

    private static void ConfigureCoreServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog();

        builder.Configuration.AddJsonFile(ConfigFilePath,true,true);

        builder.Services.AddOrleansClient((c) => { ConfigureOrleansClient(c); });
    }

    private static void ConfigureDataControlServices(WebApplicationBuilder builder)
    {
        builder.Services.AddCacheServices();

        builder.Services.AddTransient<IBlob,BlobService>();

        builder.Services.AddSingleton<ICoreCacheFactory,OrleansCoreCacheFactory>();
    }

    private static void ConfigureWebInfrastructure(WebApplicationBuilder builder , DataControl datacontrol)
    {
        DataControl.SetupServer(builder);

        datacontrol.ConfigureTLS(builder);

        DataControl.ConfigureAuth(builder);

        DataControl.ConfigureOpenApi(builder);

        builder.Services.AddHealthChecks();

        builder.Services.AddProblemDetails();

        builder.Services.AddHttpContextAccessor();

        builder.Services.ConfigureHttpJsonOptions( (o) =>
        {
            o.SerializerOptions.PropertyNamingPolicy = null;

            o.SerializerOptions.PropertyNameCaseInsensitive = true;

            o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        });

        builder.Services.AddExceptionHandler<DataControlGlobalHandler>();
    }

    private static void ConfigureOrleansClient(IClientBuilder builder)
    {
        builder.UseLocalhostClustering(gatewayPort:DataControl.GetGatewayPort(),serviceId:"default",clusterId:"default");
    }

    private IDisposable? UpdateCall;

    private void UpdateConfig(Object? o)
    {
        var z = o as Tuple<IConfigurationRoot,LoggingLevelSwitch>;

        LoggingLevelSwitch? s = z!.Item2; IConfigurationRoot? c = z!.Item1;

        if(Enum.TryParse(c?["Serilog:MinimumLevel"] ?? "Verbose",out LogEventLevel l)) { s!.MinimumLevel = l; }

        if(Boolean.TryParse(c?["KusDepotExceptions:Enabled"] ?? "False",out Boolean e)) { Settings.NoExceptions = !e; }

        UpdateCall = c?.GetReloadToken().RegisterChangeCallback(UpdateConfig,new Tuple<IConfigurationRoot,LoggingLevelSwitch>(c,s));
    }

    private static HostKey? HostKey;
}