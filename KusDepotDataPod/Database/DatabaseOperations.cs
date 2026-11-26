namespace KusDepotDataPod.Database;

internal static class DatabaseOperations
{
    public static async Task<Int32> RunAsync(Program.Arguments arguments , IConfiguration configuration)
    {
        using CancellationTokenSource cancellation = new();

        Console.CancelKeyPress += (_,eventArgs) => { eventArgs.Cancel = true; cancellation.Cancel(); };

        if(arguments.Operation == "list-catalogs")
        {
            IReadOnlyList<String> catalogs = await CatalogDiscovery.GetActiveCatalogsAsync(cancellation.Token);

            if(arguments.Catalog is not null)
            {
                catalogs = catalogs.Where(name => String.Equals(name,arguments.Catalog,StringComparison.Ordinal)).ToArray();

                if(catalogs.Count == 0) { throw new InvalidOperationException($"Catalog '{arguments.Catalog}' is not active in Key Vault."); }
            }

            foreach(String catalog in catalogs) { Console.WriteLine(catalog); }

            return 0;
        }

        DataControlDatabaseOptions? dataControlOptions = null;

        if(arguments.Operation is "migrate-datacontrol" or "migrate-all")
        {
            dataControlOptions = ConfigurationFactory.ResolveDataControlOptions(configuration);

            NpgsqlConnectionStringBuilder target = new(dataControlOptions.BuildConnectionString());

            Console.WriteLine($"DataControl migration target: host={target.Host}, port={target.Port}, database={target.Database}");
        }

        PostgresCredentials credentials = await PostgresCredentials.LoadAsync(cancellation.Token);

        if(arguments.Operation is "migrate-catalogs" or "migrate-all")
        {
            IReadOnlyList<String> catalogs = await CatalogDiscovery.GetActiveCatalogsAsync(cancellation.Token);

            if(arguments.Catalog is not null)
            {
                catalogs = catalogs.Where(name => String.Equals(name,arguments.Catalog,StringComparison.Ordinal)).ToArray();

                if(catalogs.Count == 0) { throw new InvalidOperationException($"Catalog '{arguments.Catalog}' is not active in Key Vault."); }
            }

            foreach(String catalog in catalogs)
            {
                cancellation.Token.ThrowIfCancellationRequested();

                String connectionString = BuildCatalogConnectionString(configuration,catalog,credentials);

                await ProvisionAsync(connectionString,catalog,credentials,static async (connection,token) =>
                {
                    DbContextOptions<CatalogDBContext> options = new DbContextOptionsBuilder<CatalogDBContext>().UseNpgsql(connection).Options;

                    using CatalogDBContext context = new(options);

                    await context.Database.MigrateAsync(token);
                },cancellation.Token);

                Console.WriteLine($"CatalogDB migrated: {catalog}");
            }
        }

        if(arguments.Operation is "migrate-datacontrol" or "migrate-all")
        {
            DataControlDatabaseOptions options = dataControlOptions!;

            if(String.IsNullOrWhiteSpace(options.ConnectionString))
            {
                options.PGUSER = credentials.Username;

                options.PGPASSWORD = credentials.Password;
            }

            String connectionString = BuildDataControlConnectionString(options,credentials);

            String databaseName = options.GetEffectiveDatabaseName();

            await ProvisionAsync(connectionString,databaseName,credentials,static async (connection,token) =>
            {
                DbContextOptions<DataControlDbContext> dbOptions = new DbContextOptionsBuilder<DataControlDbContext>().UseNpgsql(connection).Options;

                using DataControlDbContext context = new(dbOptions);

                await context.Database.MigrateAsync(token);
            },cancellation.Token);

            Console.WriteLine($"DataControl migrated: {databaseName}");
        }

        return 0;
    }

    private static async Task ProvisionAsync(String connectionString , String databaseName , PostgresCredentials credentials , Func<NpgsqlDataSource,CancellationToken,Task> migrate , CancellationToken cancel)
    {
        NpgsqlConnectionStringBuilder adminBuilder = new(connectionString) { Database = "postgres" };

        await using(NpgsqlDataSource adminSource = credentials.CreateDataSource(adminBuilder.ConnectionString))

        await using(NpgsqlConnection adminConnection = await adminSource.OpenConnectionAsync(cancel))
        {
            await using NpgsqlCommand exists = new("SELECT EXISTS (SELECT 1 FROM pg_database WHERE datname = @name)",adminConnection);

            exists.Parameters.AddWithValue("name",databaseName);

            if(await exists.ExecuteScalarAsync(cancel) is not true)
            {
                #pragma warning disable CA2100
                await using NpgsqlCommand create = new($"CREATE DATABASE {QuoteIdentifier(databaseName)}",adminConnection);
                #pragma warning restore CA2100

                try { await create.ExecuteNonQueryAsync(cancel); }

                catch ( PostgresException error ) when(error.SqlState == "42P04") { }
            }
        }

        await using NpgsqlDataSource dataSource = credentials.CreateDataSource(connectionString);

        await using NpgsqlConnection lockConnection = await dataSource.OpenConnectionAsync(cancel);

        Int64 lockKey = ComputeLockKey(databaseName);

        await using(NpgsqlCommand acquire = new("SELECT pg_advisory_lock(@key)",lockConnection))
        {
            acquire.Parameters.AddWithValue("key",lockKey);

            await acquire.ExecuteNonQueryAsync(cancel);
        }

        try
        {
            await migrate(dataSource,cancel);
        }
        finally
        {
            await using NpgsqlCommand release = new("SELECT pg_advisory_unlock(@key)",lockConnection);

            release.Parameters.AddWithValue("key",lockKey);

            await release.ExecuteNonQueryAsync(CancellationToken.None);
        }
    }

    private static String BuildCatalogConnectionString(IConfiguration configuration , String catalog , PostgresCredentials credentials)
    {
        String host = configuration["CatalogDB:Database:Address"] ?? "127.0.0.1";

        String portText = configuration["CatalogDB:Database:Port"] ?? "5432";

        if(Int32.TryParse(portText,NumberStyles.None,CultureInfo.InvariantCulture,out Int32 port) is false || port is < 1 or > 65535)
        {
            throw new InvalidOperationException("CatalogDB:Database:Port must be between 1 and 65535.");
        }

        NpgsqlConnectionStringBuilder builder = new()
        {
            Host = host,
            Port = port,
            Database = catalog,
            Pooling = true,
            MaxPoolSize = 50
        };

        ApplyAuthentication(builder,credentials);

        return builder.ConnectionString;
    }

    private static String BuildDataControlConnectionString(DataControlDatabaseOptions options , PostgresCredentials credentials)
    {
        NpgsqlConnectionStringBuilder builder = new(options.BuildConnectionString());

        ApplyAuthentication(builder,credentials);

        return builder.ConnectionString;
    }

    private static void ApplyAuthentication(NpgsqlConnectionStringBuilder builder , PostgresCredentials credentials)
    {
        if(credentials.Credential is not null)
        {
            builder.Username = null;
            builder.Password = null;
            builder.SslMode = SslMode.Require;
        }
        else
        {
            builder.Username = credentials.Username;
            builder.Password = credentials.Password;
        }
    }

    private static Int64 ComputeLockKey(String databaseName)
    {
        return BitConverter.ToInt64(SHA256.HashData(databaseName.ToByteArrayFromUTF16String()),0);
    }

    private static String QuoteIdentifier(String identifier)
    {
        if(String.IsNullOrWhiteSpace(identifier)) { throw new ArgumentException("A database name is required.",nameof(identifier)); }

        return '"' + identifier.Replace("\"","\"\"",StringComparison.Ordinal) + '"';
    }
}
