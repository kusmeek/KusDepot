namespace DataPodServices.DataConfigs;

public interface IDataControlDataConfigsClient
{
    Task<AdminCheckResult> CheckAdminAsync(String token , TraceContext? tracecontext , CancellationToken cancel = default);

    Task<CatalogSnapshotResult> GetCatalogSnapshotAsync(String token , TraceContext? tracecontext , CancellationToken cancel = default);

    Task<ConfiguredCatalogNames> GetConfiguredCatalogNamesAsync(CancellationToken cancel = default);

    Task<TenantStorageResolution> GetConfiguredStorageForRecoveryAsync(String catalogName , CancellationToken cancel = default);

    Task<TenantStorageResolution> ResolveTenantStorageAsync(String token , CatalogAccess access , TraceContext? tracecontext , CancellationToken cancel = default);
}
