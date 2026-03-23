namespace DataPodServices.DataConfigs;

[Alias("DataPodServices.IDataConfigs")]
public interface IDataConfigs : IGrainWithStringKey
{
    [Alias("GetAuthorizedReadSilo")]
    Task<StorageSilo?> GetAuthorizedReadSilo([Immutable] String token , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("GetAuthorizedWriteSilo")]
    Task<StorageSilo?> GetAuthorizedWriteSilo([Immutable] String token , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("GetStorageSilos")]
    Task<HashSet<StorageSilo>?> GetStorageSilos([Immutable] String token , [Immutable] String? traceid , [Immutable] String? spanid);

    [Alias("SetStorageSilos")]
    Task<Boolean> SetStorageSilos([Immutable] HashSet<StorageSilo>? silos , [Immutable] String token , [Immutable] String? traceid , [Immutable] String? spanid);
}