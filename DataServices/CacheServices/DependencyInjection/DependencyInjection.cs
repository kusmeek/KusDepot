namespace KusDepot.Data.Services.CoreCache;

/**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/main/*'/>*/
public static class DependencyInjection
{
    /**<include file='DependencyInjection.xml' path='DependencyInjection/class[@name="DependencyInjection"]/method[@name="AddCacheServices"]/*'/>*/
    public static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.AddSingleton<CacheQueue>();

        services.AddHostedService<CacheWorker>();

        services.AddSingleton<ICacheManager,CacheManager>();

        return services;
    }
}