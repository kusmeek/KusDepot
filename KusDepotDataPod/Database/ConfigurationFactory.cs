namespace KusDepotDataPod.Database;

internal static class ConfigurationFactory
{
    public static Task<IConfiguration> CreateAsync(String operation , String? configPath)
    {
        ConfigurationBuilder builder = new();
        Boolean dataControl = operation is "migrate-datacontrol" or "migrate-all";

        if(operation is "migrate-catalogs" or "migrate-all")
        {
            DataPodAppConfiguration.AddDataPodSiloProvider(builder);
        }

        if(dataControl)
        {
            DataPodAppConfiguration.AddDataControlProvider(builder);
        }

        if(configPath is not null)
        {
            String fullPath = Path.GetFullPath(configPath);
            String directory = Path.GetDirectoryName(fullPath)!;
            String fileName = Path.GetFileName(fullPath);

            IConfiguration json = new ConfigurationBuilder().SetBasePath(directory).AddJsonFile(fileName,optional:false,reloadOnChange:false).Build();

            AddLocalConfiguration(builder,json,dataControl);
        }

        IConfiguration environment = new ConfigurationBuilder().AddEnvironmentVariables().Build();

        AddLocalConfiguration(builder,environment,dataControl);

        return Task.FromResult<IConfiguration>(builder.Build());
    }

    private static void AddLocalConfiguration(IConfigurationBuilder builder , IConfiguration local , Boolean dataControl)
    {
        builder.AddConfiguration(local);

        if(dataControl)
        {
            IConfigurationSection database = local.GetSection("DataControl:Database");

            if(database.Value is not null)
            {
                throw new InvalidOperationException("DataControl:Database must be a configuration section.");
            }

            builder.AddInMemoryCollection(database.AsEnumerable(makePathsRelative:true)
                .Where(setting => !String.IsNullOrEmpty(setting.Key))
                .Select(setting => new KeyValuePair<String,String?>($"Database:{setting.Key}",setting.Value)));
        }
    }

    internal static DataControlDatabaseOptions ResolveDataControlOptions(IConfiguration configuration)
    {
        IConfigurationSection database = configuration.GetSection("Database");

        foreach(IConfigurationSection setting in database.GetChildren())
        {
            if(!new[] { "ConnectionString", "Name", "Address", "Port", "PGUSER", "PGPASSWORD" }.Contains(setting.Key,StringComparer.OrdinalIgnoreCase) || setting.GetChildren().Any())
            {
                throw new InvalidOperationException($"Invalid DataControl database setting: DataControl:Database:{setting.Key}.");
            }
        }

        DataControlDatabaseOptions options = new();database.Bind(options);

        NpgsqlConnectionStringBuilder target = new(options.BuildConnectionString());

        if(String.IsNullOrWhiteSpace(target.Host) || target.Port is < 1 or > 65535)
        {
            throw new InvalidOperationException("The DataControl database target must specify a host and a port between 1 and 65535.");
        }

        if(String.IsNullOrWhiteSpace(options.ConnectionString) is false &&
            ((database["Address"] is String address && !String.Equals(address,target.Host,StringComparison.OrdinalIgnoreCase)) ||
             (database["Port"] is not null && options.Port != target.Port)))
        {
            throw new InvalidOperationException("DataControl Database:Address or Database:Port conflicts with Database:ConnectionString.");
        }

        return options;
    }
}
