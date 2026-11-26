using Npgsql;

namespace DataPodServices.CatalogDB;

public sealed class CatalogDBDataSourceRegistry : IAsyncDisposable
{
    private Boolean disposed;
    private readonly Lock gate = new();
    private readonly IConfiguration configuration;
    private readonly PostgresCredentials credentials;
    private readonly Dictionary<String,NpgsqlDataSource> dataSources = new(StringComparer.OrdinalIgnoreCase);

    public CatalogDBDataSourceRegistry(IConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        this.credentials = PostgresCredentials.Load();
    }

    public NpgsqlDataSource GetDataSource(String connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        NpgsqlConnectionStringBuilder builder = new(connectionString);

        String? database = builder.Database;

        if(String.IsNullOrWhiteSpace(database)) { throw new ArgumentException("A database name is required.",nameof(connectionString)); }

        String key = String.Concat(builder.Host,'|',builder.Port.ToString(CultureInfo.InvariantCulture),'|',database);

        lock(this.gate)
        {
            ObjectDisposedException.ThrowIf(this.disposed,this);

            if(this.dataSources.TryGetValue(key,out NpgsqlDataSource? dataSource)) { return dataSource; }

            dataSource = this.credentials.CreateDataSource(connectionString);

            this.dataSources.Add(key,dataSource);

            return dataSource;
        }
    }

    public String BuildConnectionString(String database)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(database);

        IConfigurationSection config = this.configuration.GetSection("CatalogDB");

        String host = config["Database:Address"] ?? "127.0.0.1";

        String port = config["Database:Port"] ?? "5432";

        NpgsqlConnectionStringBuilder builder = new()
        {
            Host = host,
            Port = Int32.Parse(port,CultureInfo.InvariantCulture),
            Database = database,
            Pooling = true,
            MaxPoolSize = 10
        };

        if(this.credentials.Credential is null)
        {
            builder.Username = this.credentials.Username;
            builder.Password = this.credentials.Password;
        }
        else
        {
            builder.SslMode = SslMode.Require;
        }

        return builder.ConnectionString;
    }

    public ValueTask DisposeAsync()
    {
        NpgsqlDataSource[] sources;

        lock(this.gate)
        {
            if(this.disposed) { return ValueTask.CompletedTask; }

            this.disposed = true;

            sources = this.dataSources.Values.ToArray();

            this.dataSources.Clear();
        }

        return DisposeSourcesAsync(sources);
    }

    private static async ValueTask DisposeSourcesAsync(IEnumerable<NpgsqlDataSource> sources)
    {
        foreach(NpgsqlDataSource source in sources)
        {
            await source.DisposeAsync();
        }
    }
}
