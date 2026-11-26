using static DataPodServices.DataConfigs.DataConfigsConstants;

namespace DataPodServices.DataConfigs;

internal sealed class KeyVaultDataConfigsSnapshotLoader : IDataConfigsSnapshotLoader
{
    public async Task<DataConfigsSnapshot> LoadAsync(ConfigurationGeneration generation , CancellationToken cancel = default)
    {
        SecretClient client = DataPodKeyVault.CreateSecretClient();

        String adminTenantId = await GetRequiredSecretValueAsync(client,AdminTenantSecretName,cancel);

        String adminClientId = await GetRequiredSecretValueAsync(client,AdminClientIdSecretName,cancel);

        SnapshotCatalogIdentity adminIdentity = new(ParseIdentifier(adminTenantId,"Admin",TenantIdFieldName),ParseIdentifier(adminClientId,"Admin",AppClientIdFieldName));

        Dictionary<String,CatalogBuilder> builders = new(StringComparer.Ordinal);

        await foreach(SecretProperties properties in client.GetPropertiesOfSecretsAsync(cancellationToken:cancel))
        {
            if(properties.Enabled is false || properties.Name.StartsWith(CatalogsPrefix, StringComparison.Ordinal) is false)
            {
                continue;
            }

            String key = properties.Name[CatalogsPrefix.Length..];

            String[] segments = key.Split("--", StringSplitOptions.None);

            if(segments.Length != 2 || String.IsNullOrWhiteSpace(segments[0]) || String.IsNullOrWhiteSpace(segments[1]))
            {
                Log.Warning(InvalidSecretName, properties.Name);

                continue;
            }

            KeyVaultSecret secret = await client.GetSecretAsync(properties.Name,cancellationToken:cancel);

            if(String.IsNullOrWhiteSpace(secret.Value))
            {
                throw new InvalidOperationException(String.Format(MissingRequiredValue, properties.Name));
            }

            if(builders.TryGetValue(segments[0], out CatalogBuilder? builder) is false)
            {
                builder = new CatalogBuilder(segments[0]); builders.Add(segments[0],builder);
            }

            if(builder.Set(segments[1],secret.Value) is false)
            {
                Log.Warning("Ignoring unknown field {FieldName} for catalog {CatalogName}.",segments[1],segments[0]);
            }
        }

        if(builders.Count == 0)
        {
            throw new InvalidOperationException(NoCatalogsConfigured);
        }

        List<SnapshotCatalog> catalogs = new(builders.Count);

        HashSet<SnapshotCatalogIdentity> identities = new();

        foreach(CatalogBuilder builder in builders.Values)
        {
            SnapshotCatalog catalog = builder.Build();

            if(identities.Add(catalog.Identity) is false)
            {
                throw new InvalidOperationException(String.Format(DuplicateCatalogIdentity,catalog.CatalogName));
            }

            catalogs.Add(catalog);
        }

        return new DataConfigsSnapshot(generation,DateTimeOffset.UtcNow,adminIdentity,catalogs);
    }

    private static async Task<String> GetRequiredSecretValueAsync(SecretClient client , String secretName , CancellationToken cancel)
    {
        KeyVaultSecret secret = await client.GetSecretAsync(secretName,cancellationToken:cancel);

        if(String.IsNullOrWhiteSpace(secret.Value))
        {
            throw new InvalidOperationException(String.Format(MissingRequiredValue, secretName));
        }

        return secret.Value;
    }

    private static Guid ParseIdentifier(String value , String catalogName , String fieldName)
    {
        if(Guid.TryParse(value, out Guid result) is false || result == Guid.Empty)
        {
            throw new InvalidOperationException(String.Format(InvalidIdentifier,catalogName,fieldName));
        }

        return result;
    }

    private sealed class CatalogBuilder(String catalogName)
    {
        private String? tenantId;
        private String? appClientId;
        private String? connectionString;

        public Boolean Set(String fieldName , String value)
        {
            switch(fieldName)
            {
                case TenantIdFieldName:
                    this.tenantId = value;
                    return true;
                case AppClientIdFieldName:
                    this.appClientId = value;
                    return true;
                case ConnectionStringFieldName:
                    this.connectionString = value;
                    return true;
                default:
                    return false;
            }
        }

        public SnapshotCatalog Build()
        {
            if(String.IsNullOrWhiteSpace(this.tenantId))
            {
                throw new InvalidOperationException(String.Format(MissingCatalogField, catalogName, TenantIdFieldName));
            }

            if(String.IsNullOrWhiteSpace(this.appClientId))
            {
                throw new InvalidOperationException(String.Format(MissingCatalogField, catalogName, AppClientIdFieldName));
            }

            if(String.IsNullOrWhiteSpace(this.connectionString))
            {
                throw new InvalidOperationException(String.Format(MissingCatalogField, catalogName, ConnectionStringFieldName));
            }

            SnapshotCatalogIdentity identity = new(ParseIdentifier(this.tenantId,catalogName,TenantIdFieldName),ParseIdentifier(this.appClientId,catalogName,AppClientIdFieldName));

            return new SnapshotCatalog(catalogName,identity,this.connectionString);
        }
    }

    private const String MissingRequiredValue = "Required Key Vault secret '{0}' is missing or empty.";
    private const String InvalidSecretName = "Catalog secret name {SecretName} does not follow the expected catalog field naming convention.";
    private const String MissingCatalogField = "Catalog '{0}' is missing required field '{1}'.";
    private const String InvalidIdentifier = "Catalog '{0}' has an invalid identifier in field '{1}'.";
    private const String NoCatalogsConfigured = "No enabled catalog configurations are available in Key Vault.";
    private const String DuplicateCatalogIdentity = "Catalog identity is assigned more than once, including catalog '{0}'.";
}
