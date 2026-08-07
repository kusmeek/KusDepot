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
        TransferConfiguration stc = builder.Configuration.GetSection("TransferConfiguration").Get<TransferConfiguration>() ?? new();

        builder.Services.Configure<TransferStorageOptions>( (o) => { o.RootPath = DiskStorePath; });

        builder.Services.Configure<DataItemTransferServiceOptions>( (o) =>
        {
            o.TemporaryUploadPath = Path.Combine(DiskStorePath,"TransferTemp");
            o.EnforceMinimumSegmentBytes = stc.Enforcement.EnforceMinimumSegmentBytes;
            o.EnforceMaximumSegmentBytes = stc.Enforcement.EnforceMaximumSegmentBytes;

            if(stc.DefaultSegmentSizePolicy?.Validate() is true) { o.DefaultSegmentSizePolicy = stc.DefaultSegmentSizePolicy; }
        });

        builder.Services.AddSignalR( (o) => { o.StatefulReconnectBufferSize = TransferConfiguration.NotificationReconnectBufferSize; });

        builder.Services.AddSingleton<IStreamTransferStorage,StreamTransferStorage>();

        builder.Services.AddSingleton<ISegmentedTransferStorage,SegmentedTransferStorage>();

        builder.Services.AddSingleton<IDataItemTransferRegistry,DataItemTransferRegistry>();

        builder.Services.AddSingleton<IDataItemTransferCoordinator,DataItemTransferCoordinator>();

        builder.Services.AddSingleton<IDataControlNotificationPublisher,SignalRDataControlNotificationPublisher>();

        builder.Services.AddSingleton<IDataControlNotificationTopicResolver,DataControlNotificationTopicResolver>();

        builder.Services.AddSingleton<IDataStreamTransferService,DataStreamTransferService>();

        builder.Services.AddScoped<IBlob,BlobService>();

        builder.Services.AddScoped<PublishedTransferRequestContext>();

        builder.Services.AddScoped<IPublishedTransferRequestContext>(s => s.GetRequiredService<PublishedTransferRequestContext>());

        builder.Services.AddScoped<IPublishedTransferSource,AzureBlobPublishedTransferSource>();

        builder.Services.AddScoped<IDataItemSegmentedTransferService,DataItemSegmentedTransferService>();
    }

    private static void ConfigureWebInfrastructure(WebApplicationBuilder builder , DataControl datacontrol)
    {
        DataControl.SetupServer(builder);

        datacontrol.ConfigureTLS(builder);

        DataControl.ConfigureAuth(builder);

        DataControl.ConfigureOpenApi(builder);

        builder.Services.AddHealthChecks();

        builder.Services.AddProblemDetails();

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