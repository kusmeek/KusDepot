namespace KusDepot.Data;

internal sealed partial class CatalogDB
{
    private Boolean initialized;

    private async Task InitializeReadyDatabase()
    {
        if(this.initialized) { return; }

        try
        {
            String catalog = GetActorID(); String cs = BuildConnectionString(catalog);

            var adminsb = new NpgsqlConnectionStringBuilder(cs) { Database = "postgres" };

            await using (var adminconnect = new NpgsqlConnection(adminsb.ConnectionString))
            {
                await adminconnect.OpenAsync();

                await using (var exists = new NpgsqlCommand(SqlCheckDatabaseExists,adminconnect))
                {
                    exists.Parameters.AddWithValue("@n",catalog);

                    if(await exists.ExecuteScalarAsync() is false)
                    {
                        #pragma warning disable CA2100

                        await using var create = new NpgsqlCommand(String.Format(InvariantCulture,SqlCreateDatabaseFormat,catalog),adminconnect);

                        #pragma warning restore CA2100

                        try { await create.ExecuteNonQueryAsync(); } catch ( NpgsqlException _ ) when (String.Equals(_.SqlState,SqlStateDuplicateDB)) { }
                    }
                }
            }

            await using var lockconnect = new NpgsqlConnection(cs); await lockconnect.OpenAsync();

            Int64 key = ComputeLockKey(catalog);

            await using (var lockCmd = new NpgsqlCommand(SqlAcquireAdvisoryLock,lockconnect))
            {
                lockCmd.Parameters.AddWithValue("@k",key); await lockCmd.ExecuteNonQueryAsync();
            }

            try
            {
                using var ctx = ctxfactory.Create(cs);

                await ctx.Database.MigrateAsync();
            }
            finally
            {
                await using var unlock = new NpgsqlCommand(SqlReleaseAdvisoryLock,lockconnect);

                unlock.Parameters.AddWithValue("@k",key); await unlock.ExecuteNonQueryAsync();
            }

            this.initialized = true;
        }
        catch ( Exception _ )
        {
            Log.Error(_,ActivateFail,GetActorID()); ETW.Log.OnActivateError(_.Message,GetActorID());

            throw;
        }
    }

    private const String SqlCheckDatabaseExists  = "SELECT EXISTS (SELECT 1 FROM pg_database WHERE datname = @n)";
    private const String SqlCreateDatabaseFormat = "CREATE DATABASE \"{0}\"";
    private const String SqlAcquireAdvisoryLock  = "SELECT pg_advisory_lock(@k)";
    private const String SqlReleaseAdvisoryLock  = "SELECT pg_advisory_unlock(@k)";
    private const String SqlStateDuplicateDB     = "42P04";
}