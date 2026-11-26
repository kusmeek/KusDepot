namespace KusDepotDataPod.Infrastructure;

internal static class ContainerAppProbes
{
    public static ContainerAppProbeArgs[] Create(ContainerAppProfile profile) =>
    [
        new()
        {
            Type = "Startup",
            HttpGet = new ContainerAppProbeHttpGetArgs
            {
                Path = profile.StartupPath,
                Port = profile.HealthPort
            },
            PeriodSeconds = 24,
            TimeoutSeconds = 3,
            FailureThreshold = 10
        },
        new()
        {
            Type = "Liveness",
            HttpGet = new ContainerAppProbeHttpGetArgs
            {
                Path = profile.LivenessPath,
                Port = profile.HealthPort
            },
            PeriodSeconds = 30,
            TimeoutSeconds = 3,
            FailureThreshold = 3
        },
        new()
        {
            Type = "Readiness",
            HttpGet = new ContainerAppProbeHttpGetArgs
            {
                Path = profile.ReadinessPath,
                Port = profile.HealthPort
            },
            PeriodSeconds = 15,
            TimeoutSeconds = 3,
            FailureThreshold = 3
        }
    ];
}
