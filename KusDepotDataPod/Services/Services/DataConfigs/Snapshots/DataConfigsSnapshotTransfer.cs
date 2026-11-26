namespace DataPodServices.DataConfigs;

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.DataConfigs.SnapshotCatalogIdentityTransfer")]
internal readonly record struct SnapshotCatalogIdentityTransfer([property: Id(0)] Guid TenantId , [property: Id(1)] Guid AppClientId);

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.DataConfigs.SnapshotCatalogTransfer")]
internal sealed record class SnapshotCatalogTransfer
{
    [Id(0)]
    public String CatalogName { get; init; }

    [Id(1)]
    public SnapshotCatalogIdentityTransfer Identity { get; init; }

    [Id(2)]
    public String ConnectionString { get; init; }

    public SnapshotCatalogTransfer(String catalogName , SnapshotCatalogIdentityTransfer identity , String connectionString)
    {
        this.CatalogName = catalogName; this.Identity = identity; this.ConnectionString = connectionString;
    }
}

[GenerateSerializer]
[Alias("DataPodServices.DataConfigs.DataConfigsSnapshotTransfer")]
internal sealed record class DataConfigsSnapshotTransfer
{
    [Id(0)]
    public ConfigurationGeneration Generation { get; init; }

    [Id(1)]
    public DateTimeOffset PublishedAt { get; init; }

    [Id(2)]
    public SnapshotCatalogIdentityTransfer AdminIdentity { get; init; }

    [Id(3)]
    public SnapshotCatalogTransfer[] Catalogs { get; init; }

    public DataConfigsSnapshotTransfer(ConfigurationGeneration generation , DateTimeOffset publishedAt , SnapshotCatalogIdentityTransfer adminIdentity , SnapshotCatalogTransfer[] catalogs)
    {
        this.Generation = generation; this.PublishedAt = publishedAt;

        this.AdminIdentity = adminIdentity; this.Catalogs = catalogs;
    }

    public static DataConfigsSnapshotTransfer FromSnapshot(DataConfigsSnapshot snapshot)
    {
        SnapshotCatalogTransfer[] catalogs = snapshot.CatalogsByName.Values
            .Select(static catalog => new SnapshotCatalogTransfer(catalog.CatalogName,new SnapshotCatalogIdentityTransfer(catalog.Identity.TenantId,catalog.Identity.AppClientId),catalog.ConnectionString))
            .ToArray();

        return new DataConfigsSnapshotTransfer(snapshot.Generation,snapshot.PublishedAt,new SnapshotCatalogIdentityTransfer(snapshot.AdminIdentity.TenantId,snapshot.AdminIdentity.AppClientId),catalogs);
    }

    public DataConfigsSnapshot ToSnapshot()
    {
        return new DataConfigsSnapshot(
            this.Generation,
            this.PublishedAt,
            new SnapshotCatalogIdentity(this.AdminIdentity.TenantId,this.AdminIdentity.AppClientId),
            this.Catalogs.Select(static catalog => new SnapshotCatalog(catalog.CatalogName,new SnapshotCatalogIdentity(catalog.Identity.TenantId,catalog.Identity.AppClientId),catalog.ConnectionString)));
    }
}

[GenerateSerializer]
[Alias("DataPodServices.DataConfigs.SnapshotTransferResult")]
internal sealed record class SnapshotTransferResult
{
    [Id(0)]
    public Boolean HasUpdate { get; init; }

    [Id(1)]
    public DataConfigsSnapshotTransfer? Snapshot { get; init; }

    public SnapshotTransferResult(Boolean hasUpdate , DataConfigsSnapshotTransfer? snapshot)
    {
        this.HasUpdate = hasUpdate; this.Snapshot = snapshot;
    }
}
