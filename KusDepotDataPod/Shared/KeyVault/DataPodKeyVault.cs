namespace DataPodServices;

public static class DataPodKeyVault
{
    public const String AzureClientIdEnvironmentVariableName = "AZURE_CLIENT_ID";

    public const String KeyVaultUriEnvironmentVariableName = "KUSDEPOT_KEYVAULT_URI";

    public const String PostgresUsernameSecretName = "DataPod--Postgres--PGUSER";

    public const String PostgresPasswordSecretName = "DataPod--Postgres--PGPASSWORD";

    public const String MissingKeyVaultUri = @"Required environment variable 'KUSDEPOT_KEYVAULT_URI' is missing or invalid.";

    public const String MissingSecretValue = "Required Key Vault secret '{0}' is missing or empty.";

    public static TokenCredential CreateCredential()
    {
        String? managedIdentityClientId = GetEnvironmentVariable(AzureClientIdEnvironmentVariableName);

        ManagedIdentityCredential managedIdentityCredential = String.IsNullOrWhiteSpace(managedIdentityClientId)
            ? new ManagedIdentityCredential(new ManagedIdentityCredentialOptions())
            : new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(managedIdentityClientId));

        return new ChainedTokenCredential(new EnvironmentCredential(),managedIdentityCredential);
    }

    public static SecretClient CreateSecretClient()
    {
        return new SecretClient(GetRequiredVaultUri(),CreateCredential(),CreateSecretClientOptions());
    }

    public static String GetRequiredSecretValue(String secretName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(secretName);

        KeyVaultSecret secret = CreateSecretClient().GetSecret(secretName).Value;

        if(String.IsNullOrWhiteSpace(secret.Value))
        {
            throw new OperationFailedException(String.Format(MissingSecretValue,secretName));
        }

        return secret.Value;
    }

    private static SecretClientOptions CreateSecretClientOptions()
    {
        return new SecretClientOptions()
        {
            Retry =
            {
                Delay = TimeSpan.FromSeconds(2) , MaxDelay = TimeSpan.FromSeconds(16) , MaxRetries = 5 , Mode = RetryMode.Exponential
            }
        };
    }

    public static Uri GetRequiredVaultUri()
    {
        String keyVaultUri = GetEnvironmentVariable(KeyVaultUriEnvironmentVariableName) ?? String.Empty;

        if(Uri.TryCreate(keyVaultUri,UriKind.Absolute,out Uri? vaultUri) is false)
        {
            throw new OperationFailedException(MissingKeyVaultUri);
        }

        return vaultUri;
    }
}