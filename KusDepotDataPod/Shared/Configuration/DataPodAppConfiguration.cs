namespace DataPodServices;

public static class DataPodAppConfiguration
{
    public const String CatalogKeyPrefix = "Catalog:";

    public const String DataControlKeyPrefix = "DataControl:";

    public const String DataPodSiloKeyPrefix = "DataPodSilo:";

    public const String AzureContainerDeploymentProfile = "AzureContainer";

    public const String OrleansKeyPrefix = "Orleans:";

    public const String EndpointEnvironmentVariableName = "KUSDEPOT_APPCONFIGURATION_ENDPOINT";

    public const String DeploymentProfileEnvironmentVariableName = "KUSDEPOT_DEPLOYMENT_PROFILE";

    public const String MissingEndpoint = @"Required environment variable 'KUSDEPOT_APPCONFIGURATION_ENDPOINT' is missing or invalid.";

    public static void AddOrleansProvider(IConfigurationBuilder configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(GetRequiredEndpoint(),DataPodKeyVault.CreateCredential())
                .Select($"{OrleansKeyPrefix}*",LabelFilter.Null)
                .Select($"{OrleansKeyPrefix}*",GetEnvironmentName())
                .ConfigureRefresh(refresh => refresh.RegisterAll().SetRefreshInterval(TimeSpan.FromMinutes(1)));

            if(IsAzureContainerProfile()) { options.Select($"{OrleansKeyPrefix}*",AzureContainerDeploymentProfile); }
        });
    }

    public static void AddDataControlProvider(IConfigurationBuilder configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(GetRequiredEndpoint(),DataPodKeyVault.CreateCredential())
                .Select($"{OrleansKeyPrefix}*",LabelFilter.Null)
                .Select($"{OrleansKeyPrefix}*",GetEnvironmentName())
                .Select($"{DataControlKeyPrefix}*",LabelFilter.Null)
                .Select($"{DataControlKeyPrefix}*",GetEnvironmentName())
                .ConfigureRefresh(refresh => refresh.RegisterAll().SetRefreshInterval(TimeSpan.FromMinutes(1)));

            if(String.Equals(GetDeploymentProfile(),AzureContainerDeploymentProfile,StringComparison.OrdinalIgnoreCase))
            {
                options.Select($"{OrleansKeyPrefix}*",AzureContainerDeploymentProfile);
                options.Select($"{DataControlKeyPrefix}*",AzureContainerDeploymentProfile);
            }

            options.TrimKeyPrefix(DataControlKeyPrefix);

            options.ConfigureKeyVault(keyVault => keyVault.SetCredential(DataPodKeyVault.CreateCredential()));
        });
    }

    public static void AddDataPodSiloProvider(IConfigurationBuilder configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(GetRequiredEndpoint(),DataPodKeyVault.CreateCredential())
                .Select($"{OrleansKeyPrefix}*",LabelFilter.Null)
                .Select($"{OrleansKeyPrefix}*",GetEnvironmentName())
                .Select($"{DataPodSiloKeyPrefix}*",LabelFilter.Null)
                .Select($"{DataPodSiloKeyPrefix}*",GetEnvironmentName())
                .Select("CatalogDB:*",LabelFilter.Null)
                .Select("CatalogDB:*",GetEnvironmentName())
                .Select("DataConfiguration:*",LabelFilter.Null)
                .Select("DataConfiguration:*",GetEnvironmentName())
                .ConfigureRefresh(refresh => refresh.RegisterAll().SetRefreshInterval(TimeSpan.FromMinutes(1)));

            if(String.Equals(GetDeploymentProfile(),AzureContainerDeploymentProfile,StringComparison.OrdinalIgnoreCase))
            {
                options.Select($"{OrleansKeyPrefix}*",AzureContainerDeploymentProfile);
                options.Select($"{DataPodSiloKeyPrefix}*",AzureContainerDeploymentProfile);
                options.Select("CatalogDB:*",AzureContainerDeploymentProfile);
                options.Select("DataConfiguration:*",AzureContainerDeploymentProfile);
            }

            options.ConfigureKeyVault(keyVault => keyVault.SetCredential(DataPodKeyVault.CreateCredential()));
        });
    }

    public static void AddCatalogProvider(IConfigurationBuilder configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(GetRequiredEndpoint(),DataPodKeyVault.CreateCredential())
                .Select($"{OrleansKeyPrefix}*",LabelFilter.Null)
                .Select($"{OrleansKeyPrefix}*",GetEnvironmentName())
                .Select($"{CatalogKeyPrefix}*",LabelFilter.Null)
                .Select($"{CatalogKeyPrefix}*",GetEnvironmentName())
                .ConfigureRefresh(refresh => refresh.RegisterAll().SetRefreshInterval(TimeSpan.FromMinutes(1)));

            if(String.Equals(GetDeploymentProfile(),AzureContainerDeploymentProfile,StringComparison.OrdinalIgnoreCase))
            {
                options.Select($"{OrleansKeyPrefix}*",AzureContainerDeploymentProfile);
                options.Select($"{CatalogKeyPrefix}*",AzureContainerDeploymentProfile);
            }

            options.TrimKeyPrefix(CatalogKeyPrefix);

            options.ConfigureKeyVault(keyVault => keyVault.SetCredential(DataPodKeyVault.CreateCredential()));
        });
    }

    public static String GetEnvironmentName()
    {
        return GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environments.Production;
    }

    public static String? GetDeploymentProfile()
    {
        return GetEnvironmentVariable(DeploymentProfileEnvironmentVariableName);
    }

    public static Boolean IsAzureContainerProfile()
    {
        return String.Equals(GetDeploymentProfile(),AzureContainerDeploymentProfile,StringComparison.OrdinalIgnoreCase);
    }

    public static Uri GetRequiredEndpoint()
    {
        String endpoint = GetEnvironmentVariable(EndpointEnvironmentVariableName) ?? String.Empty;

        if(Uri.TryCreate(endpoint,UriKind.Absolute,out Uri? uri) is false || !String.Equals(uri.Scheme,Uri.UriSchemeHttps,StringComparison.OrdinalIgnoreCase))
        {
            throw new OperationFailedException(MissingEndpoint);
        }

        return uri;
    }
}
