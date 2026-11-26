namespace DataPodServices.Catalog;

internal sealed partial class CatalogHost
{
    private IToolWebHostBuilder ConfigureCatalogHost(Catalog catalog)
    {
        IToolWebHostBuilder builder = ToolBuilderFactory.CreateWebHostBuilder()
            .UseBuilderOptions(Catalog.GetCatalogOptions())
            .ConfigureWebApplication(app => catalog.ConfigureCatalogServer(app));

        builder.ConfigureTool((c,t) => { HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey; }).Seal();

        this.ConfigureServices(builder.Builder,catalog); builder.UseConsoleLifetime(); return builder;
    }

    private void ConfigureServices(WebApplicationBuilder builder , Catalog catalog)
    {
        OrleansClusterConnectionStatus clusterStatus = new();

        ConfigureCoreServices(builder,clusterStatus);

        ConfigureWebInfrastructure(builder,catalog,clusterStatus);

        catalog.SetURL(builder.Configuration);
    }

    private void ConfigureCoreServices(WebApplicationBuilder builder , OrleansClusterConnectionStatus clusterStatus)
    {
        builder.Services.AddSerilog();

        DataPodAppConfiguration.AddCatalogProvider(builder.Configuration);

        builder.Services.AddAzureAppConfiguration();

        this.SetupLogging(builder.Configuration);

        Catalog.ConfigureTelemetry(builder.Services,TelemetryResource);

        builder.Services.AddOrleansClient((c) => { ConfigureOrleansClient(c,builder.Configuration,clusterStatus); });
    }

    private static void ConfigureWebInfrastructure(WebApplicationBuilder builder , Catalog catalog , OrleansClusterConnectionStatus clusterStatus)
    {
        catalog.SetupServer(builder);

        catalog.ConfigureHTTP(builder);

        Catalog.ConfigureOpenApi(builder);

        builder.Services.AddHealthChecks()
            .AddCheck("orleans-cluster", () => clusterStatus.IsConnected ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy(),tags:[DataPodHealthCheckTags.Ready]);

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

    private static void ConfigureOrleansClient(IClientBuilder builder , IConfiguration configuration , OrleansClusterConnectionStatus clusterStatus)
    {
        Boolean useAzureContainer = DataPodAppConfiguration.IsAzureContainerProfile();

        OrleansConfig? orleans = OrleansConfig.FromConfiguration(configuration,useAzureContainer);

        if(orleans is null) { throw new InvalidOperationException("Invalid Orleans Configuration"); }

        builder.Configure<ClusterOptions>((o) => { o.ClusterId = orleans.ClusterId; o.ServiceId = orleans.ServiceId; });

        builder.Services.AddCatalogDataConfigsClient();

        if(useAzureContainer)
        {
            builder.UseAzureStorageClustering((o) => o.TableServiceClient = new TableServiceClient(new Uri(orleans.AzureTableEndpoint!),DataPodKeyVault.CreateCredential()));
        }
        else
        {
            builder.UseLocalhostClustering(gatewayPort:Catalog.GetGatewayPort(configuration),serviceId:orleans.ServiceId,clusterId:orleans.ClusterId);
        }

        builder.AddClusterConnectionStatusObserver(clusterStatus);
    }

    private IDisposable? UpdateCall;

    private void UpdateConfig(Object? o)
    {
        var z = o as Tuple<IConfigurationRoot,LoggingLevelSwitch>;

        LoggingLevelSwitch? s = z!.Item2; IConfigurationRoot? c = z!.Item1;

        if(Enum.TryParse(c?["Serilog:MinimumLevel"] ?? "Information",out LogEventLevel l)) { s!.MinimumLevel = l; }

        if(Boolean.TryParse(c?["KusDepotExceptions:Enabled"] ?? "False",out Boolean e)) { Settings.NoExceptions = !e; }

        UpdateCall = c?.GetReloadToken().RegisterChangeCallback(UpdateConfig,new Tuple<IConfigurationRoot,LoggingLevelSwitch>(c,s));
    }

    private static HostKey? HostKey;
}