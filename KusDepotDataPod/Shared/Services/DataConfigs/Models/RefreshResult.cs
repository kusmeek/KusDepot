namespace DataPodServices.DataConfigs;

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.DataConfigs.RefreshResult")]
public sealed record class RefreshResult
{
    [Id(0)]
    public Boolean Published { get; init; }

    [Id(1)]
    public ConfigurationGeneration ConfigurationGeneration { get; init; }

    [Id(2)]
    public Int32 CatalogCount { get; init; }

    public RefreshResult(Boolean published , ConfigurationGeneration configurationGeneration , Int32 catalogCount)
    {
        this.Published = published;
        this.ConfigurationGeneration = configurationGeneration;
        this.CatalogCount = catalogCount;
    }
}
