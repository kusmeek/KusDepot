namespace DataPodServices.DataConfigs;

public interface IDataConfigsAdminClient
{
    Task<RefreshResult> RefreshCatalogsAsync(String token , TraceContext? tracecontext , CancellationToken cancel = default);
}
