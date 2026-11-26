namespace DataPodServices.DataConfigs;

internal sealed class DataControlDataConfigsClient(IGrainFactory grainFactory) : IDataControlDataConfigsClient
{
    private const Int64 ReaderWorkerKey = 0;

    public Task<AdminCheckResult> CheckAdminAsync(String token , TraceContext? tracecontext , CancellationToken cancel = default)
    {
        return grainFactory.GetGrain<IDataConfigsReaderGrain>(ReaderWorkerKey).CheckAdminAsync(token,tracecontext,cancel);
    }

    public Task<CatalogSnapshotResult> GetCatalogSnapshotAsync(String token , TraceContext? tracecontext , CancellationToken cancel = default)
    {
        return grainFactory.GetGrain<IDataConfigsReaderGrain>(ReaderWorkerKey).GetCatalogSnapshotAsync(token,tracecontext,cancel);
    }

    public Task<ConfiguredCatalogNames> GetConfiguredCatalogNamesAsync(CancellationToken cancel = default)
    {
        return grainFactory.GetGrain<IDataConfigsRecoveryGrain>(ReaderWorkerKey).GetConfiguredCatalogNamesAsync(cancel);
    }

    public Task<TenantStorageResolution> GetConfiguredStorageForRecoveryAsync(String catalogName , CancellationToken cancel = default)
    {
        return grainFactory.GetGrain<IDataConfigsRecoveryGrain>(ReaderWorkerKey).GetConfiguredStorageForRecoveryAsync(catalogName,cancel);
    }

    public Task<TenantStorageResolution> ResolveTenantStorageAsync(String token , CatalogAccess access , TraceContext? tracecontext , CancellationToken cancel = default)
    {
        return grainFactory.GetGrain<IDataConfigsReaderGrain>(ReaderWorkerKey).ResolveTenantStorageAsync(token,access,tracecontext,cancel);
    }
}
