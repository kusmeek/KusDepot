namespace Catalog;

public static class ServiceConfiguration
{
    public static void ConfigureCatalogServices(this IServiceCollection services)
    {
        services.AddTransient<IArkKeeperFactory>((sp)=>{ return new ArkKeeperFactory(); } );

        services.AddTransient<IDataConfigs>((sp)=>{ return ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService); });

        services.AddOpenTelemetry().WithTracing((pb)=>{ pb.ConfigureResource(_=>{_.Clear();}).AddSource("KusDepot.Data.Catalog").AddAspNetCoreInstrumentation().AddOtlpExporter().AddConsoleExporter(); });
    }
}