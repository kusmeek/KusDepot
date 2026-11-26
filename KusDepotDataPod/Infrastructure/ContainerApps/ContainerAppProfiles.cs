namespace KusDepotDataPod.Infrastructure;

internal static class ContainerAppProfiles
{
    public static ContainerAppProfile Catalog(DataPodConfiguration config) => new()
    {
        LogicalName = "datapod-catalog",
        AppName = "datapod-catalog",
        Image = config.CatalogImage,
        IngressTargetPort = 5006,
        IngressExternal = true,
        IngressTransport = "http",
        MinReplicas = 0,
        MaxReplicas = 1,
        StartupPath = "/Catalog/Health",
        LivenessPath = "/Catalog/Health",
        ReadinessPath = "/Catalog/Ready",
        HealthPort = 5006
    };

    public static ContainerAppProfile Control(DataPodConfiguration config) => new()
    {
        LogicalName = "datapod-control",
        AppName = "datapod-control",
        Image = config.ControlImage,
        IngressTargetPort = 5007,
        IngressExternal = true,
        IngressTransport = "http",
        MinReplicas = 0,
        MaxReplicas = 1,
        StartupPath = "/DataControl/Health",
        LivenessPath = "/DataControl/Health",
        ReadinessPath = "/DataControl/Ready",
        HealthPort = 5007
    };

    public static ContainerAppProfile Silo(DataPodConfiguration config) => new()
    {
        LogicalName = "datapod-silo",
        AppName = "datapod-silo",
        Image = config.SiloImage,
        IngressTargetPort = 30003,
        IngressExternal = false,
        IngressTransport = "tcp",
        MinReplicas = 1,
        MaxReplicas = 1,
        StartupPath = "/startup",
        LivenessPath = "/live",
        ReadinessPath = "/ready",
        HealthPort = 8081,
        AdditionalPortMappings =
        [
            new IngressPortMappingArgs
            {
                ExposedPort = 11115,
                External = false,
                TargetPort = 11115
            }
        ]
    };
}
