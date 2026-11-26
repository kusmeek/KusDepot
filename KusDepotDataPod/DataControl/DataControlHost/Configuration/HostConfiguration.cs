namespace DataPodServices.DataControl;

internal sealed partial class DataControlHost
{
    private IToolWebHostBuilder ConfigureDataControlHost(DataControl datacontrol)
    {
        IToolWebHostBuilder builder = ToolBuilderFactory.CreateWebHostBuilder()
            .UseBuilderOptions(DataControl.GetDataControlOptions())
            .ConfigureWebApplication(app => datacontrol.ConfigureDataControlServer(app));

        builder.ConfigureTool((c,t) => { HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey; }).Seal();

        this.ConfigureServices(builder.Builder,datacontrol); builder.UseConsoleLifetime(); return builder;
    }

    private void ConfigureServices(WebApplicationBuilder builder , DataControl datacontrol)
    {
        OrleansClusterConnectionStatus clusterStatus = new();

        ConfigureCoreServices(builder);

        ConfigureOrleansServices(builder,clusterStatus);

        ConfigureDataControlServices(builder);

        ConfigureWebInfrastructure(builder,datacontrol,clusterStatus);

        datacontrol.SetURL(builder.Configuration);
    }

    private void ConfigureCoreServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog();

        DataPodAppConfiguration.AddDataControlProvider(builder.Configuration);

        builder.Services.AddAzureAppConfiguration();

        this.SetupLogging(builder.Configuration);

        DataControl.ConfigureTelemetry(builder.Services,TelemetryResource);

    }

    private static void ConfigureOrleansServices(WebApplicationBuilder builder , OrleansClusterConnectionStatus clusterStatus)
    {
        builder.Services.AddOrleansClient((c) => { ConfigureOrleansClient(c,builder.Configuration,clusterStatus); });
    }

    private static void ConfigureDataControlServices(WebApplicationBuilder builder)
    {
        ConfigureDatabaseServices(builder);

        ConfigureTransferServices(builder);

        ConfigureNotificationServices(builder);
    }

    private static void ConfigureDatabaseServices(WebApplicationBuilder builder)
    {
        PostgresCredentials credentials = PostgresCredentials.Load();

        builder.Services.Configure<DataControlDatabaseOptions>( (o) =>
        {
            builder.Configuration.GetSection("Database").Bind(o);

            o.PGUSER = credentials.Username; o.PGPASSWORD = credentials.Password;
        });

        builder.Services.AddSingleton(services =>
        {
            DataControlDatabaseOptions database = services.GetRequiredService<IOptions<DataControlDatabaseOptions>>().Value;

            _ = database.GetEffectiveDatabaseName();

            return new DataControlPostgresDataSources(
                credentials.CreateDataSource(database.BuildConnectionString()),
                credentials.CreateDataSource(database.BuildConnectionString("postgres")));
        });

        builder.Services.AddSingleton(services => services.GetRequiredService<DataControlPostgresDataSources>().DataControl);

        builder.Services.AddSingleton<IPostgresAdvisoryLeaderLock,PostgresAdvisoryLeaderLock>();

        builder.Services.AddDbContextFactory<DataControlDbContext>((services,options) =>
        {
            options.UseNpgsql(services.GetRequiredService<DataControlPostgresDataSources>().DataControl).EnableSensitiveDataLogging(false).EnableDetailedErrors(false);
        });

        builder.Services.AddSingleton<ITransferStateStore,PostgresTransferStateStore>();

        builder.Services.AddSingleton<IDataControlDeletionJobStore,PostgresDataControlDeletionJobStore>();

        builder.Services.AddSingleton<IDataControlPublicationJournalStore,PostgresPublicationJournalStore>();

        builder.Services.AddHostedService<DataControlDatabaseInitializer>();
    }

    private static void ConfigureTransferServices(WebApplicationBuilder builder)
    {
        TransferConfiguration stc = builder.Configuration.GetSection("TransferConfiguration").Get<TransferConfiguration>() ?? new();

        if(stc.PayloadRateLimits.UploadBytesPerSecond < 0 || stc.PayloadRateLimits.DownloadBytesPerSecond < 0)
        {
            throw new InvalidOperationException("DataControl:TransferConfiguration:PayloadRateLimits values must be non-negative.");
        }

        builder.Services.AddSingleton(new TransferPayloadRateLimiters(stc.PayloadRateLimits.UploadBytesPerSecond,stc.PayloadRateLimits.DownloadBytesPerSecond));

        builder.Services.AddSingleton<ITenantTransferStorageFactory,TenantTransferStorageFactory>();

        builder.Services.AddScoped<ITenantTransferStorageContext,TenantTransferStorageContext>();

        builder.Services.AddScoped<IDataControlTransferSessionStore,DataControlTransferSessionStore>();

        builder.Services.AddScoped<IPublicationRecoveryAdminService,PublicationRecoveryAdminService>();

        builder.Services.AddScoped<IDataControlPublicationTransferStore,DataControlPublicationTransferStore>();

        builder.Services.Configure<TransferPublicationRecoveryAgentOptions>( (o) =>
        {
            builder.Configuration.GetSection("Recovery").Bind(o);
        });

        builder.Services.AddHostedService<TransferPublicationRecoveryAgent>();

        builder.Services.Configure<TransferSessionCleanupAgentOptions>((o) =>
        {
            builder.Configuration.GetSection("Cleanup").Bind(o);
        });

        builder.Services.AddHostedService<TransferSessionCleanupAgent>();

        builder.Services.Configure<DeletionAgentOptions>( (o) =>
        {
            builder.Configuration.GetSection("Deletion").Bind(o);
        });

        builder.Services.AddHostedService<DeletionAgent>();

        builder.Services.AddSingleton<DataControlDeletionProcessor>();

        builder.Services.Configure<DataItemTransferServiceOptions>( (o) =>
        {
            o.EnforceMinimumSegmentBytes = stc.Enforcement.EnforceMinimumSegmentBytes;
            o.EnforceMaximumSegmentBytes = stc.Enforcement.EnforceMaximumSegmentBytes;

            if(stc.DefaultSegmentSizePolicy?.Validate() is true) { o.DefaultSegmentSizePolicy = stc.DefaultSegmentSizePolicy; }
        });

        builder.Services.AddSingleton<IDataItemTransferRegistry,PostgresDataItemTransferRegistry>();

        builder.Services.AddSingleton<IDataItemTransferCoordinator,PostgresDataItemTransferCoordinator>();

        builder.Services.AddScoped<IDataStreamTransferService,DataControlStreamTransferService>();

        builder.Services.AddScoped<PublishedTransferRequestContext>();

        builder.Services.AddScoped<IPublishedTransferRequestContext>(s => s.GetRequiredService<PublishedTransferRequestContext>());

        builder.Services.AddScoped<IPublishedTransferSource,AzureBlobPublishedTransferSource>();

        builder.Services.AddScoped<IDataItemSegmentedTransferService,DataControlSegmentedTransferService>();
    }

    private static void ConfigureNotificationServices(WebApplicationBuilder builder)
    {
        ISignalRServerBuilder signalR = builder.Services.AddSignalR((o) => { o.StatefulReconnectBufferSize = TransferConfiguration.NotificationReconnectBufferSize; });

        String? signalREndpoint = builder.Configuration["SignalREndpoint"];

        if(DataPodAppConfiguration.IsAzureContainerProfile() && String.IsNullOrWhiteSpace(signalREndpoint) is false)
        {
            if(Uri.TryCreate(signalREndpoint,UriKind.Absolute,out Uri? endpoint) is false || !String.Equals(endpoint.Scheme,Uri.UriSchemeHttps,OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("DataControl:SignalREndpoint must be an absolute HTTPS URI.");
            }

            signalR.AddAzureSignalR(options =>
            {
                options.Endpoints = [new ServiceEndpoint(endpoint,DataPodKeyVault.CreateCredential())];

                options.ClaimsProvider = context =>
                {
                    List<Claim> claims = context.User.Claims
                        .Where(claim => !String.Equals(claim.Type,DataControlNotificationToken.AuthorizationTokenClaimType,Ordinal))
                        .ToList();

                    String token = DataControlNotificationToken.Get(context);

                    if(String.IsNullOrWhiteSpace(token) is false)
                    {
                        claims.Add(new Claim(DataControlNotificationToken.AuthorizationTokenClaimType,token));
                    }

                    return claims;
                };
            });
        }

        builder.Services.AddSingleton<IDataControlNotificationPublisher,SignalRDataControlNotificationPublisher>();

        builder.Services.AddSingleton<IDataControlNotificationTopicResolver,DataControlNotificationTopicResolver>();

        builder.Services.AddScoped<IDataControlNotificationAuthorization,DataControlNotificationAuthorization>();
    }

    private static void ConfigureWebInfrastructure(WebApplicationBuilder builder , DataControl datacontrol , OrleansClusterConnectionStatus clusterStatus)
    {
        DataControl.SetupServer(builder);

        datacontrol.ConfigureHTTP(builder);

        DataControl.ConfigureOpenApi(builder);

        builder.Services.AddHealthChecks()
            .AddCheck("orleans-cluster", () => clusterStatus.IsConnected ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy(),tags:[DataPodHealthCheckTags.Ready])
            .AddCheck<DataControlDatabaseHealthCheck>("state-database",tags:[DataPodHealthCheckTags.Ready],timeout:TimeSpan.FromSeconds(1));

        builder.Services.AddProblemDetails();

        builder.Services.ConfigureHttpJsonOptions( (o) =>
        {
            o.SerializerOptions.PropertyNamingPolicy = null;

            o.SerializerOptions.PropertyNameCaseInsensitive = true;

            o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        });

        builder.Services.AddExceptionHandler<DataControlGlobalHandler>();
    }

    private static void ConfigureOrleansClient(IClientBuilder builder , IConfiguration configuration , OrleansClusterConnectionStatus clusterStatus)
    {
        Boolean useAzureContainer = DataPodAppConfiguration.IsAzureContainerProfile();

        OrleansConfig? orleans = OrleansConfig.FromConfiguration(configuration,useAzureContainer);

        if(orleans is null) { throw new InvalidOperationException("Invalid Orleans Configuration"); }

        builder.Configure<ClusterOptions>((o) => { o.ClusterId = orleans.ClusterId; o.ServiceId = orleans.ServiceId; });

        builder.Services.AddDataControlDataConfigsClient();

        if(useAzureContainer)
        {
            builder.UseAzureStorageClustering((o) => o.TableServiceClient = new TableServiceClient(new Uri(orleans.AzureTableEndpoint!),DataPodKeyVault.CreateCredential()));
        }
        else
        {
            builder.UseLocalhostClustering(gatewayPort:DataControl.GetGatewayPort(configuration),serviceId:orleans.ServiceId,clusterId:orleans.ClusterId);
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