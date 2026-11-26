namespace DataPodServices.DataConfigs;

internal sealed class CatalogDataConfigsClient(IGrainFactory grainFactory) : ICatalogDataConfigsClient
{
    private const Int64 ReaderWorkerKey = 0;

    public Task<CatalogResolution> ResolveCatalogAsync(String token , CatalogAccess access , TraceContext? tracecontext , CancellationToken cancel = default)
    {
        return grainFactory.GetGrain<IDataConfigsReaderGrain>(ReaderWorkerKey).ResolveCatalogAsync(token,access,tracecontext,cancel);
    }
}
