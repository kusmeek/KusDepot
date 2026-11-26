namespace KusDepotDataPod.Database;

internal static class CatalogDiscovery
{
    public static async Task<IReadOnlyList<String>> GetActiveCatalogsAsync(CancellationToken cancel)
    {
        SecretClient client = DataPodKeyVault.CreateSecretClient();

        Dictionary<String,CatalogBuilder> builders = new(StringComparer.Ordinal);

        await foreach(SecretProperties properties in client.GetPropertiesOfSecretsAsync(cancellationToken:cancel))
        {
            if(properties.Enabled.HasValue && properties.Enabled.Value is false) { continue; }

            if(properties.Name.StartsWith(DataConfigsConstants.CatalogsPrefix,StringComparison.Ordinal) is false) { continue; }

            String key = properties.Name[DataConfigsConstants.CatalogsPrefix.Length..];

            String[] segments = key.Split("--",StringSplitOptions.None);

            if(segments.Length != 2 || String.IsNullOrWhiteSpace(segments[0]) || String.IsNullOrWhiteSpace(segments[1])) { continue; }

            KeyVaultSecret secret = await client.GetSecretAsync(properties.Name,cancellationToken:cancel);

            if(String.IsNullOrWhiteSpace(secret.Value))
            {
                throw new InvalidOperationException($"Catalog secret '{properties.Name}' is missing or empty.");
            }

            String catalogName = segments[0];

            if(builders.TryGetValue(catalogName,out CatalogBuilder? builder) is false)
            {
                builder = new CatalogBuilder(catalogName);

                builders.Add(catalogName,builder);
            }

            builder.Set(segments[1],secret.Value);
        }

        if(builders.Count == 0)
        {
            throw new InvalidOperationException("No active catalogs are configured in Key Vault.");
        }

        HashSet<(Guid TenantId,Guid AppClientId)> identities = new();

        foreach(CatalogBuilder builder in builders.Values)
        {
            if(identities.Add(builder.Validate()) is false)
            {
                throw new InvalidOperationException($"Catalog '{builder.Name}' duplicates an active catalog identity.");
            }
        }

        return builders.Keys.OrderBy(static name => name,StringComparer.Ordinal).ToArray();
    }

    private sealed class CatalogBuilder(String catalogName)
    {
        private String? TenantId;
        private String? AppClientId;
        private String? ConnectionString;

        public String Name => catalogName;

        public void Set(String fieldName , String value)
        {
            switch(fieldName)
            {
                case DataConfigsConstants.TenantIdFieldName: this.TenantId = value; break;
                case DataConfigsConstants.AppClientIdFieldName: this.AppClientId = value; break;
                case DataConfigsConstants.ConnectionStringFieldName: this.ConnectionString = value; break;
            }
        }

        public (Guid TenantId,Guid AppClientId) Validate()
        {
            if(String.IsNullOrWhiteSpace(this.TenantId)) { throw Missing(DataConfigsConstants.TenantIdFieldName); }
            if(String.IsNullOrWhiteSpace(this.AppClientId)) { throw Missing(DataConfigsConstants.AppClientIdFieldName); }
            if(String.IsNullOrWhiteSpace(this.ConnectionString)) { throw Missing(DataConfigsConstants.ConnectionStringFieldName); }
            if(Guid.TryParse(this.TenantId,out Guid tenantId) is false) { throw new InvalidOperationException($"Catalog '{catalogName}' has an invalid TenantID."); }
            if(Guid.TryParse(this.AppClientId,out Guid appClientId) is false) { throw new InvalidOperationException($"Catalog '{catalogName}' has an invalid AppClientID."); }

            return (tenantId,appClientId);
        }

        private InvalidOperationException Missing(String fieldName)
        {
            return new InvalidOperationException($"Catalog '{catalogName}' is missing required field '{fieldName}'.");
        }
    }
}
