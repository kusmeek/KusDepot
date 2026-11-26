namespace DataPodServices.DataConfigs;

public interface ICatalogDataConfigsClient
{
    Task<CatalogResolution> ResolveCatalogAsync(String token , CatalogAccess access , TraceContext? tracecontext , CancellationToken cancel = default);
}
