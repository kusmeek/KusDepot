namespace DataPodServices.DataConfigs;

[Alias("DataPodServices.IDataConfigs")]
public interface IDataConfigs : IGrainWithStringKey
{
    [Alias("GetAuthorizedReadSilo")]
    Task<StorageSilo?> GetAuthorizedReadSilo([Immutable] String token , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("GetAuthorizedWriteSilo")]
    Task<StorageSilo?> GetAuthorizedWriteSilo([Immutable] String token , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("GetStorageSilos")]
    Task<HashSet<StorageSilo>?> GetStorageSilos([Immutable] String token , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("IsAdmin")]
    Task<Boolean> IsAdmin([Immutable] String token , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);

    [Alias("SetStorageSilos")]
    Task<Boolean> SetStorageSilos([Immutable] HashSet<StorageSilo>? silos , [Immutable] String token , [Immutable] String? traceid , [Immutable] String? spanid , CancellationToken cancel = default);
}