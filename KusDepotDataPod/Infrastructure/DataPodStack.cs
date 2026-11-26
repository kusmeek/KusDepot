namespace KusDepotDataPod.Infrastructure;

internal static class DataPodStack
{
    public static Dictionary<String,Object?> Create(DataPodConfiguration config)
    {
        var resourceGroup = new ResourceGroup("datapod-resource-group", new()
        {
            ResourceGroupName = config.ResourceGroupName,
            Location = config.Location
        });

        var environment = ManagedEnvironmentFactory.Create(config,resourceGroup);
        var catalog = ContainerAppFactory.Create(ContainerAppProfiles.Catalog(config),config,resourceGroup,environment);
        var control = ContainerAppFactory.Create(ContainerAppProfiles.Control(config),config,resourceGroup,environment);
        var silo = ContainerAppFactory.Create(ContainerAppProfiles.Silo(config),config,resourceGroup,environment);

        return new Dictionary<String,Object?>
        {
            ["resourceGroupName"] = resourceGroup.Name,
            ["environmentName"] = environment.Name,
            ["catalogName"] = catalog.Name,
            ["controlName"] = control.Name,
            ["siloName"] = silo.Name
        };
    }
}
