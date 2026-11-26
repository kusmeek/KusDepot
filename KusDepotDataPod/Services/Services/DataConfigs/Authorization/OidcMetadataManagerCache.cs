namespace DataPodServices.DataConfigs;

internal sealed class OidcMetadataManagerCache
{
    private readonly Lock gate = new();

    private readonly Dictionary<Guid,ConfigurationManager<OpenIdConnectConfiguration>> managers = new();

    public ConfigurationManager<OpenIdConnectConfiguration>? Get(Guid tenantId , DataConfigsSnapshot snapshot)
    {
        if(snapshot.ContainsTenant(tenantId) is false)
        {
            return null;
        }

        lock(this.gate)
        {
            if(this.managers.TryGetValue(tenantId, out ConfigurationManager<OpenIdConnectConfiguration>? manager))
            {
                return manager;
            }

            String metadataAddress = String.Concat("https://login.microsoftonline.com/",tenantId.ToString("D"),"/v2.0/.well-known/openid-configuration");

            manager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress,new OpenIdConnectConfigurationRetriever());

            this.managers.Add(tenantId,manager);

            return manager;
        }
    }

    public void RemoveTenantsNotIn(DataConfigsSnapshot snapshot)
    {
        HashSet<Guid> tenants = snapshot.CatalogsByIdentity.Keys.Select(static identity => identity.TenantId).Append(snapshot.AdminIdentity.TenantId).ToHashSet();

        lock(this.gate)
        {
            foreach(Guid tenantId in this.managers.Keys.Where(tenantId => tenants.Contains(tenantId) is false).ToArray())
            {
                this.managers.Remove(tenantId);
            }
        }
    }
}
