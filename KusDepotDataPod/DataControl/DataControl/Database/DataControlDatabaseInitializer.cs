namespace DataPodServices.DataControl;

internal sealed class DataControlDatabaseInitializer : IHostedService
{
    private readonly IServiceScopeFactory scopes;

    private readonly DataControlDatabaseOptions options;

    private readonly DataControlPostgresDataSources dataSources;

    public DataControlDatabaseInitializer(IServiceScopeFactory scopes , IOptions<DataControlDatabaseOptions> options , DataControlPostgresDataSources dataSources)
    {
        this.scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        this.dataSources = dataSources ?? throw new ArgumentNullException(nameof(dataSources));

        _ = this.options.GetEffectiveDatabaseName();
    }

    public async Task StartAsync(CancellationToken cancel)
    {
        await EnsureDatabaseAsync(cancel);

        String databaseName = this.options.GetEffectiveDatabaseName();

        Int64 lockKey = BitConverter.ToInt64(SHA256.HashData(databaseName.ToByteArrayFromUTF16String()),0);

        await using NpgsqlConnection lockConnection = await this.dataSources.DataControl.OpenConnectionAsync(cancel);

        await using NpgsqlCommand acquire = new("SELECT pg_advisory_lock(@key)",lockConnection);

        acquire.Parameters.AddWithValue("key",lockKey);

        await acquire.ExecuteNonQueryAsync(cancel);

        try
        {
            await using AsyncServiceScope scope = this.scopes.CreateAsyncScope();

            DataControlDbContext context = scope.ServiceProvider.GetRequiredService<DataControlDbContext>();

            await context.Database.MigrateAsync(cancel);
        }
        finally
        {
            await using NpgsqlCommand release = new("SELECT pg_advisory_unlock(@key)",lockConnection);

            release.Parameters.AddWithValue("key",lockKey);

            await release.ExecuteNonQueryAsync(CancellationToken.None);
        }
    }

    public Task StopAsync(CancellationToken cancel)
    {
        return Task.CompletedTask;
    }

    private async Task EnsureDatabaseAsync(CancellationToken cancel)
    {
        await using NpgsqlConnection connection = await this.dataSources.Admin.OpenConnectionAsync(cancel);

        await using NpgsqlCommand exists = new("SELECT EXISTS (SELECT 1 FROM pg_database WHERE datname = @name)",connection);

        String databaseName = this.options.GetEffectiveDatabaseName();

        exists.Parameters.AddWithValue("name",databaseName);

        if(await exists.ExecuteScalarAsync(cancel) is true) { return; }

        String quotedDatabaseName = QuoteIdentifier(databaseName);

        #pragma warning disable CA2100
        await using NpgsqlCommand create = new($"CREATE DATABASE {quotedDatabaseName}",connection);
        #pragma warning restore CA2100

        try { await create.ExecuteNonQueryAsync(cancel); }
        catch ( PostgresException error ) when(error.SqlState == "42P04") { }
    }

    private static String QuoteIdentifier(String value)
    {
        if(String.IsNullOrWhiteSpace(value)) { throw new ArgumentException("A database name is required.",nameof(value)); }

        return '"' + value.Replace("\"","\"\"",Ordinal) + '"';
    }
}
