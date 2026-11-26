namespace KusDepotDataPod.Infrastructure;

internal static class ManagedEnvironmentFactory
{
    public static ManagedEnvironment Create(DataPodConfiguration config, ResourceGroup resourceGroup) =>
        new("datapod-environment", new()
        {
            ResourceGroupName = resourceGroup.Name,
            EnvironmentName = config.EnvironmentName,
            Location = config.Location,
            ZoneRedundant = false,
            AppLogsConfiguration = new AppLogsConfigurationArgs
            {
                Destination = "log-analytics",
                LogAnalyticsConfiguration = new LogAnalyticsConfigurationArgs
                {
                    CustomerId = config.LogAnalyticsCustomerId,
                    SharedKey = config.LogAnalyticsSharedKey
                }
            }
        });
}
