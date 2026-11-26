namespace DataPodServices.DataConfigs;

internal readonly record struct SnapshotCatalogIdentity(Guid TenantId , Guid AppClientId);

internal sealed record SnapshotCatalog(String CatalogName , SnapshotCatalogIdentity Identity , String ConnectionString);

internal sealed class DataConfigsSnapshot
{
    public ConfigurationGeneration Generation { get; }

    public DateTimeOffset PublishedAt { get; }

    public SnapshotCatalogIdentity AdminIdentity { get; }

    public IReadOnlyDictionary<String,SnapshotCatalog> CatalogsByName { get; }

    public IReadOnlyDictionary<SnapshotCatalogIdentity,SnapshotCatalog> CatalogsByIdentity { get; }

    public DataConfigsSnapshot(ConfigurationGeneration generation , DateTimeOffset publishedAt , SnapshotCatalogIdentity adminIdentity , IEnumerable<SnapshotCatalog> catalogs)
    {
        ArgumentNullException.ThrowIfNull(catalogs);

        Dictionary<String,SnapshotCatalog> catalogsByName = new(StringComparer.Ordinal);

        Dictionary<SnapshotCatalogIdentity,SnapshotCatalog> catalogsByIdentity = new();

        foreach(SnapshotCatalog catalog in catalogs)
        {
            if(String.IsNullOrWhiteSpace(catalog.CatalogName) || String.IsNullOrWhiteSpace(catalog.ConnectionString) || catalog.Identity.TenantId == Guid.Empty || catalog.Identity.AppClientId == Guid.Empty)
            {
                throw new InvalidOperationException("The DataConfigs snapshot contains an invalid catalog entry.");
            }

            catalogsByName.Add(catalog.CatalogName, catalog);

            if(catalogsByIdentity.TryAdd(catalog.Identity,catalog) is false)
            {
                throw new InvalidOperationException($"Catalog identity is configured more than once: {catalogsByIdentity[catalog.Identity].CatalogName}.");
            }
        }

        if(generation.Value == Guid.Empty || publishedAt == default || adminIdentity.TenantId == Guid.Empty || adminIdentity.AppClientId == Guid.Empty || catalogsByName.Count == 0)
        {
            throw new InvalidOperationException("The DataConfigs snapshot is missing a valid generation, publication time, administrator, or catalogs.");
        }

        this.Generation = generation;
        this.PublishedAt = publishedAt;
        this.AdminIdentity = adminIdentity;
        this.CatalogsByName = new ReadOnlyDictionary<String,SnapshotCatalog>(catalogsByName);
        this.CatalogsByIdentity = new ReadOnlyDictionary<SnapshotCatalogIdentity,SnapshotCatalog>(catalogsByIdentity);
    }

    public Boolean TryGetCatalog(String catalogName , out SnapshotCatalog catalog)
    {
        return this.CatalogsByName.TryGetValue(catalogName,out catalog!);
    }

    public Boolean TryGetCatalog(SnapshotCatalogIdentity identity , out SnapshotCatalog catalog)
    {
        return this.CatalogsByIdentity.TryGetValue(identity,out catalog!);
    }

    public Boolean ContainsTenant(Guid tenantId)
    {
        return this.AdminIdentity.TenantId == tenantId || this.CatalogsByIdentity.Keys.Any(identity => identity.TenantId == tenantId);
    }
}
