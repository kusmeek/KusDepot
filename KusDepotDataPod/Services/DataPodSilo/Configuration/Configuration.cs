using DataPodServices.CatalogDB;

namespace DataPodServices;

internal sealed partial class DataPodSilo
{
    public static IToolGenericHostBuilder ConfigureDataPodSilo()
    {
        var builder = ToolBuilderFactory.CreateGenericHostBuilder();

        DataPodAppConfiguration.AddDataPodSiloProvider(builder.Builder.Configuration);

        builder.Builder.Services.AddAzureAppConfiguration();

        builder.Builder.Services.AddHostedService<SiloAppConfigurationRefreshService>();

        SetupLogging(); builder.UseLogger(new SerilogLoggerFactory());

        builder.Builder.Services.AddSerilog(Log.Logger);

        builder.Builder.Services.AddSingleton(LoggingSwitch);

        builder.Builder.Services.AddSingleton<SiloProbeHealthState>();

        builder.Builder.Services.AddHostedService<SiloProbeHealthServer>();

        builder.Builder.Services.AddSingleton(TelemetryResource);

        DataPodServiceLoggers loggers = new();

        UpdateConfig(builder.Builder.Configuration,loggers);

        builder.Builder.Services.AddSingleton(loggers);

        builder.Builder.Services.AddSingleton<CatalogDBDataSourceRegistry>();

        builder.UseConsoleLifetime().AutoStart().Seal();

        ConfigureOrleans(builder);

        SetupTelemetry(builder.Builder.Services);

        return builder;
    }

    private static IToolGenericHostBuilder ConfigureOrleans(IToolGenericHostBuilder builder)
    {
        ConfigurationManager configuration = builder.Builder.Configuration;

        SiloConfig? sc = configuration.GetSection("DataPodSilo").Get<SiloConfig>();

        Boolean useAzureContainer = DataPodAppConfiguration.IsAzureContainerProfile();

        if(sc is null || sc.Validate() is false) { throw new InvalidOperationException("Invalid Silo Configuration"); }

        String? siloHostName = useAzureContainer ? GetAzureContainerSiloHostName() : null;

        OrleansConfig? oc = OrleansConfig.FromConfiguration(configuration,useAzureContainer);

        if(oc is null) { throw new InvalidOperationException("Invalid Orleans Configuration"); }

        builder.UseOrleans((b) =>
        {
            b.ConfigureDataConfigs();

            b.ConfigureServices(services => services.AddSingleton<CatalogDBDataSourceRegistry>());

            b.ConfigureServices(services => services.AddSingleton<CatalogDBInitializationCoordinator>());

            b.ConfigureServices(services => services.AddSingleton<ILifecycleParticipant<ISiloLifecycle>>(serviceProvider => serviceProvider.GetRequiredService<SiloProbeHealthState>()));

            b.Configure<ClusterOptions>((o) => { o.ClusterId = oc.ClusterId; o.ServiceId = oc.ServiceId; });

            b.ConfigureServices(services => services.AddDataConfigs());

            if(useAzureContainer)
            {
                b.UseAzureStorageClustering((o) => o.TableServiceClient = new TableServiceClient(new Uri(oc.AzureTableEndpoint!),DataPodKeyVault.CreateCredential()));

                b.ConfigureEndpoints(siloHostName!,sc.SiloPort,sc.GatewayPort,listenOnAnyHostAddress:true);
            }
            else
            {
                b.UseLocalhostClustering(sc.SiloPort,sc.GatewayPort,null,oc.ServiceId,oc.ClusterId);

                b.ConfigureEndpoints(IPAddress.Parse(sc.IPAddress),sc.SiloPort,sc.GatewayPort,sc.ListenOnAnyHostAddress);
            }

            b.AddMemoryGrainStorageAsDefault(); b.UseTransactions();

            b.Configure<GrainCollectionOptions>((o) =>
            {
                o.CollectionQuantum = TimeSpan.FromSeconds(60);

                o.CollectionAge = TimeSpan.FromSeconds(120);
            });
        });

        return builder;
    }

    private static String GetAzureContainerSiloHostName()
    {
        String? appName = GetEnvironmentVariable("CONTAINER_APP_NAME");

        if(String.IsNullOrWhiteSpace(appName))
        {
            throw new InvalidOperationException("Azure Container Apps app name environment variable is missing.");
        }

        return appName;
    }

    private static IDisposable? UpdateCall;

    private static void UpdateConfig(IConfigurationRoot configuration , DataPodServiceLoggers loggers)
    {
        if(Enum.TryParse(configuration["DataPodSilo:Serilog:MinimumLevel"] ?? "Information",out LogEventLevel l)) { LoggingSwitch.MinimumLevel = l; }

        if(Enum.TryParse(configuration["CatalogDB:Serilog:MinimumLevel"] ?? "Information",out LogEventLevel c)) { loggers.CatalogDBLoggingSwitch.MinimumLevel = c; }

        if(Enum.TryParse(configuration["DataConfiguration:Serilog:MinimumLevel"] ?? "Information",out LogEventLevel d)) { loggers.DataConfigsLoggingSwitch.MinimumLevel = d; }

        if(Boolean.TryParse(configuration["DataPodSilo:KusDepotExceptions:Enabled"] ?? "False",out Boolean e)) { Settings.NoExceptions = !e; }

        UpdateCall?.Dispose();

        UpdateCall = configuration.GetReloadToken().RegisterChangeCallback((_) => UpdateConfig(configuration,loggers),null);
    }
}