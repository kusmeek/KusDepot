namespace DataPodServices.DataConfigs;

[Alias("DataPodServices.DataConfigs.IDataConfigsReaderGrain")]
public interface IDataConfigsReaderGrain : IGrainWithIntegerKey
{
    [Alias("ResolveCatalogAsync")]
    Task<CatalogResolution> ResolveCatalogAsync([Immutable] String token , CatalogAccess access , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    [Alias("ResolveTenantStorageAsync")]
    Task<TenantStorageResolution> ResolveTenantStorageAsync([Immutable] String token , CatalogAccess access , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    [Alias("CheckAdminAsync")]
    Task<AdminCheckResult> CheckAdminAsync([Immutable] String token , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);

    [Alias("GetCatalogSnapshotAsync")]
    Task<CatalogSnapshotResult> GetCatalogSnapshotAsync([Immutable] String token , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);
}
