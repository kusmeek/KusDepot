namespace DataPodServices.DataConfigs;

public static class DataConfigsClientRegistration
{
    public static IServiceCollection AddCatalogDataConfigsClient(this IServiceCollection services)
    {
        services.TryAddSingleton<ICatalogDataConfigsClient,CatalogDataConfigsClient>();

        return services;
    }

    public static IServiceCollection AddDataControlDataConfigsClient(this IServiceCollection services)
    {
        services.TryAddSingleton<IDataControlDataConfigsClient,DataControlDataConfigsClient>();

        return services;
    }

    public static IServiceCollection AddDataConfigsAdminClient(this IServiceCollection services)
    {
        services.TryAddSingleton<IDataConfigsAdminClient,DataConfigsAdminClient>();

        return services;
    }
}
