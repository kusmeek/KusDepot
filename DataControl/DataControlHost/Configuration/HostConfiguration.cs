namespace KusDepot.Data;

internal sealed partial class DataControlHost
{
    private static IToolWebHostBuilder ConfigureDataControlHost(DataControl datacontrol)
    {
        IToolWebHostBuilder builder = ToolBuilderFactory.CreateWebHostBuilder()
            .UseBuilderOptions(DataControl.GetDataControlOptions())
            .ConfigureWebApplication(app => datacontrol.ConfigureDataControlServer(app));

        builder.ConfigureTool((c,t) => { HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey; }).Seal();

        ConfigureServices(builder.Builder,datacontrol); return builder;
    }

    private static void ConfigureServices(WebApplicationBuilder builder , DataControl datacontrol)
    {
        ConfigureCoreServices(builder,datacontrol);

        ConfigureDataControlServices(builder);

        ConfigureWebInfrastructure(builder,datacontrol);
    }

    private static void ConfigureCoreServices(WebApplicationBuilder builder , DataControl datacontrol)
    {
        builder.Services.AddSerilog();

        builder.Configuration.AddJsonFile(ConfigFilePath,true,true);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(datacontrol.GetResourceConfigurator())
            .WithMetrics(DataControl.GetMetricsConfigurator())
            .WithTracing(DataControl.GetTracingConfigurator());
    }

    private static void ConfigureDataControlServices(WebApplicationBuilder builder)
    {
        builder.Services.AddCacheServices();

        builder.Services.AddTransient<IBlob,BlobService>();

        builder.Services.AddSingleton<ICoreCacheFactory,FabricCoreCacheFactory>();

        builder.Services.AddTransient<ICatalogDBFactory>(_ => new CatalogDBFactory());

        builder.Services.AddTransient(_ => ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService));
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

        builder.Services.AddExceptionHandler<DataControlGlobalHandler>();

        builder.Services.ConfigureHttpJsonOptions( (o) =>
        {
            o.SerializerOptions.PropertyNameCaseInsensitive = true;

            o.SerializerOptions.PropertyNamingPolicy = null;

            o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        });
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