namespace KusDepotDataPod.Infrastructure;

internal sealed record DataPodConfiguration(
    String ResourceGroupName,
    String Location,
    String EnvironmentName,
    String RegistryLoginServer,
    String IdentityResourceId,
    String IdentityClientId,
    String AppConfigurationEndpoint,
    String KeyVaultUri,
    String LogAnalyticsCustomerId,
    Output<String> LogAnalyticsSharedKey,
    Output<String> ApplicationInsightsConnectionString,
    String CatalogImage,
    String ControlImage,
    String SiloImage)
{
    public static DataPodConfiguration Load(Config config)
    {
        var result = new DataPodConfiguration(
            Require(config,"resourceGroupName"),
            config.Get("location") ?? "eastus2",
            Require(config,"environmentName"),
            Require(config,"registryLoginServer"),
            RequireResourceId(config,"identityResourceId"),
            Require(config,"identityClientId"),
            RequireHttpsUri(config,"appConfigurationEndpoint"),
            RequireHttpsUri(config,"keyVaultUri"),
            RequireGuid(config,"logAnalyticsCustomerId"),
            config.RequireSecret("logAnalyticsSharedKey"),
            config.RequireSecret("applicationInsightsConnectionString"),
            RequireImage(config,"catalogImage"),
            RequireImage(config,"controlImage"),
            RequireImage(config,"siloImage"));

        return result;
    }

    private static String Require(Config config , String key)
    {
        var value = config.Require(key).Trim();

        if(value.Length == 0)
        {
            throw new InvalidOperationException($"Required Pulumi configuration '{key}' cannot be empty.");
        }

        return value;
    }

    private static String RequireResourceId(Config config , String key)
    {
        var value = Require(config,key);

        if(!value.StartsWith("/subscriptions/",StringComparison.OrdinalIgnoreCase)
            || !value.Contains("/providers/",StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Pulumi configuration '{key}' must be an Azure resource ID.");
        }

        return value;
    }

    private static String RequireHttpsUri(Config config , String key)
    {
        var value = Require(config,key);

        if(!Uri.TryCreate(value,UriKind.Absolute,out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException($"Pulumi configuration '{key}' must be an absolute HTTPS URI.");
        }

        return value;
    }

    private static String RequireImage(Config config  ,String key)
    {
        var value = Require(config,key);

        if(!value.Contains(':',StringComparison.Ordinal) && !value.Contains("@sha256:",StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Pulumi configuration '{key}' must include an image tag or digest.");
        }

        return value;
    }

    private static String RequireGuid(Config config , String key)
    {
        var value = Require(config,key);

        if(!Guid.TryParse(value,out _))
        {
            throw new InvalidOperationException($"Pulumi configuration '{key}' must be a GUID.");
        }

        return value;
    }
}
