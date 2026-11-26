namespace DataPodServices.DataConfigs;

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.DataConfigs.ConfiguredCatalogNames")]
public sealed record class ConfiguredCatalogNames
{
    [Id(0)]
    public String[] CatalogNames { get; init; }

    [Id(1)]
    public ConfigurationGeneration ConfigurationGeneration { get; init; }

    public ConfiguredCatalogNames(String[] catalogNames , ConfigurationGeneration configurationGeneration)
    {
        this.CatalogNames = catalogNames ?? throw new ArgumentNullException(nameof(catalogNames));
        this.ConfigurationGeneration = configurationGeneration;
    }
}
