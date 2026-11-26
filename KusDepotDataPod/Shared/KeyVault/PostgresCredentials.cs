namespace DataPodServices;

public sealed class PostgresCredentials
{
    private PostgresCredentials(String? username , String? password , TokenCredential? credential)
    {
        this.Username = username; this.Password = password; this.Credential = credential;
    }

    public String? Username { get; }

    public String? Password { get; }

    public TokenCredential? Credential { get; }

    public static PostgresCredentials Load()
    {
        SecretClient client = DataPodKeyVault.CreateSecretClient();

        String? username = GetOptionalSecret(client,DataPodKeyVault.PostgresUsernameSecretName);

        String? password = GetOptionalSecret(client,DataPodKeyVault.PostgresPasswordSecretName);

        if(username is null && password is null)
        {
            return new PostgresCredentials(null,null,DataPodKeyVault.CreateCredential());
        }

        if(String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Both PostgreSQL username and password Key Vault secrets must be configured, or both must be absent to use Microsoft Entra authentication.");
        }

        return new PostgresCredentials(username,password,null);
    }

    public static async Task<PostgresCredentials> LoadAsync(CancellationToken cancel)
    {
        SecretClient client = DataPodKeyVault.CreateSecretClient();

        String? username = await GetOptionalSecretAsync(client,DataPodKeyVault.PostgresUsernameSecretName,cancel);

        String? password = await GetOptionalSecretAsync(client,DataPodKeyVault.PostgresPasswordSecretName,cancel);

        if(username is null && password is null)
        {
            return new PostgresCredentials(null,null,DataPodKeyVault.CreateCredential());
        }

        if(String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Both PostgreSQL username and password Key Vault secrets must be configured, or both must be absent to use Microsoft Entra authentication.");
        }

        return new PostgresCredentials(username,password,null);
    }

    public NpgsqlDataSource CreateDataSource(String connectionString)
    {
        NpgsqlConnectionStringBuilder connectionStringBuilder = new(connectionString);

        if(DataPodAppConfiguration.IsAzureContainerProfile()) { connectionStringBuilder.SslMode = SslMode.VerifyFull; }

        NpgsqlDataSourceBuilder builder = new(connectionStringBuilder.ConnectionString);

        if(this.Credential is not null) { builder.UseEntraAuthentication(this.Credential); }

        return builder.Build();
    }

    private static String? GetOptionalSecret(SecretClient client , String name)
    {
        try
        {
            KeyVaultSecret secret = client.GetSecret(name).Value;

            if(String.IsNullOrWhiteSpace(secret.Value)) { throw new InvalidOperationException($"PostgreSQL Key Vault secret '{name}' is empty."); }

            return secret.Value;
        }
        catch ( RequestFailedException error ) when(error.Status == 404)
        {
            return null;
        }
    }

    private static async Task<String?> GetOptionalSecretAsync(SecretClient client , String name , CancellationToken cancel)
    {
        try
        {
            KeyVaultSecret secret = await client.GetSecretAsync(name,cancellationToken:cancel);

            if(String.IsNullOrWhiteSpace(secret.Value)) { throw new InvalidOperationException($"PostgreSQL Key Vault secret '{name}' is empty."); }

            return secret.Value;
        }
        catch ( RequestFailedException error ) when(error.Status == 404)
        {
            return null;
        }
    }
}
