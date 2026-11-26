namespace DataPodServices.DataConfigs;

[Alias("DataPodServices.DataConfigs.IDataConfigsRecoveryGrain")]
public interface IDataConfigsRecoveryGrain : IGrainWithIntegerKey
{
    [Alias("GetConfiguredCatalogNamesAsync")]
    Task<ConfiguredCatalogNames> GetConfiguredCatalogNamesAsync(CancellationToken cancel = default);

    [Alias("GetConfiguredStorageForRecoveryAsync")]
    Task<TenantStorageResolution> GetConfiguredStorageForRecoveryAsync([Immutable] String catalogName , CancellationToken cancel = default);
}
