namespace KusDepot.Data;

internal sealed partial class CatalogHost
{
    private static IToolWebHostBuilder ConfigureCatalogHost(Catalog catalog)
    {
        IToolWebHostBuilder builder = ToolBuilderFactory.CreateWebHostBuilder()
            .UseBuilderOptions(Catalog.GetCatalogOptions())
            .ConfigureWebApplication(app => catalog.ConfigureCatalogServer(app));

        builder.ConfigureTool((c,t) => { HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey; }).Seal();

        ConfigureServices(builder.Builder,catalog); return builder;
    }

    private static void ConfigureServices(WebApplicationBuilder builder , Catalog catalog)
    {
        ConfigureCoreServices(builder,catalog);

        ConfigureCatalogServices(builder);

        ConfigureWebInfrastructure(builder,catalog);
    }

    private static void ConfigureCoreServices(WebApplicationBuilder builder , Catalog catalog)
    {
        builder.Services.AddSerilog();

        builder.Configuration.AddJsonFile(ConfigFilePath,true,true);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(catalog.GetResourceConfigurator())
            .WithMetrics(Catalog.GetMetricsConfigurator())
            .WithTracing(Catalog.GetTracingConfigurator());
    }

    private static void ConfigureCatalogServices(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ICatalogDBFactory>((_) => { return new CatalogDBFactory(); });

        builder.Services.AddTransient((_) => { return ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService); });
    }

    private static void ConfigureWebInfrastructure(WebApplicationBuilder builder , Catalog catalog)
    {
        Catalog.SetupServer(builder);

        catalog.ConfigureTLS(builder);

        Catalog.ConfigureAuth(builder);

        Catalog.ConfigureOpenApi(builder);

        builder.Services.AddHealthChecks();

        builder.Services.AddProblemDetails();

        builder.Services.AddHttpContextAccessor();

        builder.Services.ConfigureHttpJsonOptions( (o) =>
        {
            o.SerializerOptions.PropertyNamingPolicy = null;

            o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;

            o.SerializerOptions.WriteIndented = true;
        });

        builder.Services.AddExceptionHandler<CatalogGlobalHandler>();
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