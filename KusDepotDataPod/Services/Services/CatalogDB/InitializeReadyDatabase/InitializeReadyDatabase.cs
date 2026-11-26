using Npgsql;
using static System.Globalization.CultureInfo;
using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    private async Task InitializeReadyDatabase(CancellationToken cancel = default)
    {
        try
        {
            String catalog = GetActorID();

            await this.initializationCoordinator.EnsureInitializedAsync(catalog,InitializeDatabaseAsync,cancel);
        }
        catch ( OperationCanceledException )
        {
            throw;
        }
        catch ( Exception _ )
        {
            Logger.Error(_,InitializeReadyDatabaseFail,GetActorID());

            throw new CatalogDBInitializationException(GetActorID(),_);
        }
    }

    private async Task InitializeDatabaseAsync(CancellationToken cancel)
    {
        String catalog = GetActorID(); String cs = this.dataSources.BuildConnectionString(catalog);

        var adminsb = new NpgsqlConnectionStringBuilder(cs) { Database = "postgres" };

        NpgsqlDataSource adminSource = this.dataSources.GetDataSource(adminsb.ConnectionString);

        await using (var adminconnect = await adminSource.OpenConnectionAsync(cancel))
        {
            await using (var exists = new NpgsqlCommand(SqlCheckDatabaseExists,adminconnect))
            {
                exists.Parameters.AddWithValue("@n",catalog);

                if(await exists.ExecuteScalarAsync(cancel) is false)
                {
                    #pragma warning disable CA2100

                    await using var create = new NpgsqlCommand(String.Format(InvariantCulture,SqlCreateDatabaseFormat,catalog),adminconnect);

                    #pragma warning restore CA2100

                    try { await create.ExecuteNonQueryAsync(cancel); } catch ( NpgsqlException _ ) when (String.Equals(_.SqlState,SqlStateDuplicateDB)) { }
                }
            }
        }

        NpgsqlDataSource catalogSource = this.dataSources.GetDataSource(cs);

        await using var lockconnect = await catalogSource.OpenConnectionAsync(cancel);

        Int64 key = ComputeLockKey(catalog);

        await using (var lockCmd = new NpgsqlCommand(SqlAcquireAdvisoryLock,lockconnect))
        {
            lockCmd.Parameters.AddWithValue("@k",key); await lockCmd.ExecuteNonQueryAsync(cancel);
        }

        try
        {
            using var ctx = ctxfactory.Create(catalogSource);

            await ctx.Database.MigrateAsync(cancel);
        }
        finally
        {
            await using var unlock = new NpgsqlCommand(SqlReleaseAdvisoryLock,lockconnect);

            unlock.Parameters.AddWithValue("@k",key); await unlock.ExecuteNonQueryAsync(CancellationToken.None);
        }

    }

    private const String SqlCheckDatabaseExists  = "SELECT EXISTS (SELECT 1 FROM pg_database WHERE datname = @n)";
    private const String SqlCreateDatabaseFormat = "CREATE DATABASE \"{0}\"";
    private const String SqlAcquireAdvisoryLock  = "SELECT pg_advisory_lock(@k)";
    private const String SqlReleaseAdvisoryLock  = "SELECT pg_advisory_unlock(@k)";
    private const String SqlStateDuplicateDB     = "42P04";
}