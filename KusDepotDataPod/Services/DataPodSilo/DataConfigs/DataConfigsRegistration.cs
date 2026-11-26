namespace DataPodServices.DataConfigs;

public static class DataConfigsRegistration
{
    public static IServiceCollection AddDataConfigs(this IServiceCollection services)
    {
        services.TryAddSingleton<OidcMetadataManagerCache>();
        services.TryAddSingleton<IDataConfigsSnapshotProvider,DataConfigsSnapshotProvider>();
        services.TryAddSingleton<IDataConfigsSnapshotLoader,KeyVaultDataConfigsSnapshotLoader>();
        services.TryAddSingleton<DataConfigsAuthorizer>();
        services.TryAddSingleton<DataConfigsDiagnostics>();
        services.TryAddSingleton<DataConfigsSnapshotUpdater>();
        services.AddSingleton<ILifecycleParticipant<ISiloLifecycle>>(s => s.GetRequiredService<DataConfigsSnapshotUpdater>());

        return services;
    }

    public static ISiloBuilder ConfigureDataConfigs(this ISiloBuilder siloBuilder)
    {
        siloBuilder.Configure<GrainCollectionOptions>(options =>
        {
            options.ClassSpecificCollectionAge ??= new Dictionary<String,TimeSpan>(StringComparer.Ordinal);
            options.ClassSpecificCollectionAge[typeof(DataConfigsCoordinatorGrain).FullName!] = TimeSpan.FromMinutes(30);
        });

        return siloBuilder;
    }
}
