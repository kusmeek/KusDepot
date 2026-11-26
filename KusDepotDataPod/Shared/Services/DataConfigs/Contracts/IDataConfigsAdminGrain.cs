namespace DataPodServices.DataConfigs;

[Alias("DataPodServices.DataConfigs.IDataConfigsAdminGrain")]
public interface IDataConfigsAdminGrain : IGrainWithStringKey
{
    [Alias("RefreshCatalogsAsync")]
    Task<RefreshResult> RefreshCatalogsAsync([Immutable] String token , [Immutable] TraceContext? tracecontext , CancellationToken cancel = default);
}
