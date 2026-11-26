namespace DataPodServices.DataConfigs;

internal interface IDataConfigsSnapshotLoader
{
    Task<DataConfigsSnapshot> LoadAsync(ConfigurationGeneration generation , CancellationToken cancel = default);
}
