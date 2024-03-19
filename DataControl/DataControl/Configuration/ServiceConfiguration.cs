namespace DataControl;

public static class ServiceConfiguration
{
    public static void ConfigureDataControlServices(this IServiceCollection services)
    {
        services.AddTransient<IArkKeeperFactory>((_)=>{ return new ArkKeeperFactory(); } );

        services.AddTransient<IBlob>((_)=>{ return ActorProxy.Create<IBlob>(new ActorId(Guid.NewGuid()),ServiceLocators.BlobService); });

        services.AddTransient<IUniverse>((_)=>{ return ActorProxy.Create<IUniverse>(ActorIds.Universe,ServiceLocators.UniverseService); });

        services.AddTransient<ICoreCache>((_)=>{ return ActorProxy.Create<ICoreCache>(ActorIds.CoreCache,ServiceLocators.CoreCacheService); });

        services.AddTransient<IDataConfigs>((_)=>{ return ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService); });

        services.AddOpenTelemetry().WithTracing((pb)=>{ pb.ConfigureResource(_=>{_.Clear();}).AddSource("KusDepot.Data.Control").AddAspNetCoreInstrumentation().AddOtlpExporter().AddConsoleExporter(); });
    }
}