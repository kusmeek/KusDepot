namespace DataPodServices.DataConfigs;

[GenerateSerializer]
[Alias("DataPodServices.DataConfigs.CatalogSnapshotResult")]
public sealed record class CatalogSnapshotResult
{
    [Id(0)]
    public CatalogSnapshotEntry[] Catalogs { get; init; }

    [Id(1)]
    public ConfigurationGeneration ConfigurationGeneration { get; init; }

    public CatalogSnapshotResult(CatalogSnapshotEntry[] catalogs , ConfigurationGeneration configurationGeneration)
    {
        this.Catalogs = catalogs; this.ConfigurationGeneration = configurationGeneration;
    }
}
