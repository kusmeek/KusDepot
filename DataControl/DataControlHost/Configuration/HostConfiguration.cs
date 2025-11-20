namespace KusDepot.Data;

internal sealed partial class DataControlHost
{
    private static IToolWebHostBuilder ConfigureDataControlHost(DataControl datacontrol)
    {
        IToolWebHostBuilder builder = ToolBuilderFactory.CreateWebHostBuilder().UseBuilderOptions(DataControl.GetDataControlOptions()).ConfigureWebApplication(app => datacontrol.ConfigureDataControlServer(app));

        builder.Builder.Services.ConfigureHttpJsonOptions( (o) => { o.SerializerOptions.PropertyNameCaseInsensitive = true; o.SerializerOptions.PropertyNamingPolicy = null; o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip; });

        builder.Builder.Services.AddOpenTelemetry().ConfigureResource(datacontrol.GetResourceConfigurator()).WithMetrics(DataControl.GetMetricsConfigurator()).WithTracing(DataControl.GetTracingConfigurator());

        builder.Builder.Services.AddTransient<IDataConfigs>( (_) => { return ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService); });

        builder.Builder.Services.AddTransient<ICoreCache>( (_) => { return ActorProxy.Create<ICoreCache>(ActorIds.CoreCache,ServiceLocators.CoreCacheService); });

        builder.Builder.Services.AddTransient<IBlob>( (_) => { return ActorProxy.Create<IBlob>(new ActorId(Guid.NewGuid()),ServiceLocators.BlobService); });

        builder.ConfigureTool((c,t) => { HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey; }).Seal();

        builder.Builder.Services.AddTransient<ICatalogDBFactory>( (_) => { return new CatalogDBFactory(); });

        builder.Builder.Services.AddExceptionHandler<DataControlGlobalHandler>();

        builder.Builder.Configuration.AddJsonFile(ConfigFilePath,true,true);

        builder.Builder.Services.AddHttpContextAccessor();

        builder.Builder.Services.AddProblemDetails();

        builder.Builder.Services.AddHealthChecks();

        builder.Builder.Services.AddSerilog();

        DataControl.ConfigureOpenApi(builder.Builder);

        DataControl.ConfigureAuth(builder.Builder);

        datacontrol.ConfigureTLS(builder.Builder);

        DataControl.SetupServer(builder.Builder);

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