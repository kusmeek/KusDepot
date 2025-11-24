namespace KusDepot.Data;

internal sealed partial class CatalogHost
{
    private static IToolWebHostBuilder ConfigureCatalogHost(Catalog catalog)
    {
        IToolWebHostBuilder builder = ToolBuilderFactory.CreateWebHostBuilder().UseBuilderOptions(Catalog.GetCatalogOptions()).ConfigureWebApplication(app => catalog.ConfigureCatalogServer(app));

        builder.Builder.Services.ConfigureHttpJsonOptions((o) => { o.SerializerOptions.PropertyNamingPolicy = null; o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip; o.SerializerOptions.WriteIndented = true; });

        builder.Builder.Services.AddOpenTelemetry().ConfigureResource(catalog.GetResourceConfigurator()).WithMetrics(Catalog.GetMetricsConfigurator()).WithTracing(Catalog.GetTracingConfigurator());

        builder.Builder.Services.AddTransient((_) => { return ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService); });

        builder.ConfigureTool((c,t) => { HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey; }).Seal();

        builder.Builder.Services.AddTransient<ICatalogDBFactory>((_) => { return new CatalogDBFactory(); });

        builder.Builder.Services.AddExceptionHandler<CatalogGlobalHandler>();

        builder.Builder.Configuration.AddJsonFile(ConfigFilePath,true,true);

        builder.Builder.Services.AddHttpContextAccessor();

        builder.Builder.Services.AddProblemDetails();

        builder.Builder.Services.AddHealthChecks();

        builder.Builder.Services.AddSerilog();

        Catalog.ConfigureOpenApi(builder.Builder);

        Catalog.ConfigureAuth(builder.Builder);

        catalog.ConfigureTLS(builder.Builder);

        Catalog.SetupServer(builder.Builder);

        return builder;
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