namespace DataPodServices.DataConfigs;

[GenerateSerializer]
[Alias("DataPodServices.DataConfigs.CatalogResolutionStatus")]
public enum CatalogResolutionStatus : Byte
{
    Authorized = 0,

    Forbidden = 1,

    NotConfigured = 2
}

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.DataConfigs.CatalogResolution")]
public sealed record class CatalogResolution
{
    [Id(0)]
    public CatalogResolutionStatus Status { get; init; }

    [Id(1)]
    public String? CatalogName { get; init; }

    [Id(2)]
    public ConfigurationGeneration ConfigurationGeneration { get; init; }

    public CatalogResolution(CatalogResolutionStatus status , String? catalogName , ConfigurationGeneration configurationGeneration)
    {
        this.Status = status;
        this.CatalogName = catalogName;
        this.ConfigurationGeneration = configurationGeneration;
    }
}
