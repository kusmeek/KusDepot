namespace DataPodServices.DataConfigs;

[Alias("DataPodServices.DataConfigs.IDataConfigsCoordinatorGrain")]
internal interface IDataConfigsCoordinatorGrain : IGrainWithStringKey
{
    [Alias("GetSnapshotIfNewerAsync")]
    Task<SnapshotTransferResult> GetSnapshotIfNewerAsync(ConfigurationGeneration knownGeneration , CancellationToken cancel = default);
}
