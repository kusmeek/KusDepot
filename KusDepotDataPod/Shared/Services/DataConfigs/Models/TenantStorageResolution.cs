namespace DataPodServices.DataConfigs;

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.DataConfigs.TenantStorageResolution")]
public sealed record class TenantStorageResolution
{
    [Id(0)]
    public CatalogResolutionStatus Status { get; init; }

    [Id(1)]
    public String? CatalogName { get; init; }

    [Id(2)]
    public String? ConnectionString { get; init; }

    [Id(3)]
    public ConfigurationGeneration ConfigurationGeneration { get; init; }

    public TenantStorageResolution(CatalogResolutionStatus status , String? catalogName , String? connectionString , ConfigurationGeneration configurationGeneration)
    {
        this.Status = status;
        this.CatalogName = catalogName;
        this.ConnectionString = connectionString;
        this.ConfigurationGeneration = configurationGeneration;
    }
}
