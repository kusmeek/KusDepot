namespace DataPodServices.DataConfigs;

internal sealed class DataConfigsAdminClient(IGrainFactory grainFactory) : IDataConfigsAdminClient
{
    private const String CoordinatorGrainKey = "DataConfigsCoordinator";

    public Task<RefreshResult> RefreshCatalogsAsync(String token , TraceContext? tracecontext , CancellationToken cancel = default)
    {
        return grainFactory.GetGrain<IDataConfigsAdminGrain>(CoordinatorGrainKey).RefreshCatalogsAsync(token,tracecontext,cancel);
    }
}
