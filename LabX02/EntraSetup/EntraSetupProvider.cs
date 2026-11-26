using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace LabX02;

internal static class EntraSetupProvider
{
    private const String KeyVaultUriEnvironmentVariableName = "KUSDEPOT_KEYVAULT_URI";

    private static readonly Lazy<Task<EntraSetupRecord>> Cache = new(LoadCoreAsync,LazyThreadSafetyMode.ExecutionAndPublication);

    public static Task<EntraSetupRecord> LoadAsync() => Cache.Value;

    private static async Task<EntraSetupRecord> LoadCoreAsync()
    {
        String keyVaultUri = Environment.GetEnvironmentVariable(KeyVaultUriEnvironmentVariableName) ?? String.Empty;

        if(Uri.TryCreate(keyVaultUri,UriKind.Absolute,out Uri? vaultUri) is false)
        {
            throw new InvalidOperationException($"Required environment variable '{KeyVaultUriEnvironmentVariableName}' is missing or invalid.");
        }

        TokenCredential credential = new ChainedTokenCredential(new EnvironmentCredential(),new ManagedIdentityCredential());

        SecretClient client = new(vaultUri,credential,new SecretClientOptions()
        {
            Retry =
            {
                Delay = TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(16),
                MaxRetries = 5,
                Mode = RetryMode.Exponential,
            },
        });

        Task<String>[] secretTasks =
        [
            GetRequiredSecretValueAsync(client,"EntraSetup--TenantID"),
            GetRequiredSecretValueAsync(client,"EntraSetup--ClientID"),
            GetRequiredSecretValueAsync(client,"EntraSetup--Scope"),
            GetRequiredSecretValueAsync(client,"EntraSetup--AdminUserName"),
            GetRequiredSecretValueAsync(client,"EntraSetup--AdminUserPass"),
            GetRequiredSecretValueAsync(client,"EntraSetup--ReadUserName"),
            GetRequiredSecretValueAsync(client,"EntraSetup--ReadUserPass"),
            GetRequiredSecretValueAsync(client,"EntraSetup--WriteUserName"),
            GetRequiredSecretValueAsync(client,"EntraSetup--WriteUserPass"),
            GetRequiredSecretValueAsync(client,"EntraSetup--ConnectionString"),
        ];

        String[] values = await Task.WhenAll(secretTasks).ConfigureAwait(false);

        return new EntraSetupRecord()
        {
            TenantID = values[0],
            ClientID = values[1],
            Scope = values[2],
            AdminUserName = values[3],
            AdminUserPass = values[4],
            ReadUserName = values[5],
            ReadUserPass = values[6],
            WriteUserName = values[7],
            WriteUserPass = values[8],
            ConnectionString = values[9],
        };
    }

    private static async Task<String> GetRequiredSecretValueAsync(SecretClient client , String secretName)
    {
        KeyVaultSecret secret = await client.GetSecretAsync(secretName).ConfigureAwait(false);

        if(String.IsNullOrWhiteSpace(secret.Value))
        {
            throw new InvalidOperationException($"Key Vault secret '{secretName}' is missing a value.");
        }

        return secret.Value;
    }
}
