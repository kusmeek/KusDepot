namespace KusDepotDataPod.Infrastructure;

internal sealed record ContainerAppProfile
{
    public required String LogicalName { get; init; }

    public required String AppName { get; init; }

    public required String Image { get; init; }

    public required Int32 IngressTargetPort { get; init; }

    public required Boolean IngressExternal { get; init; }

    public required String IngressTransport { get; init; }

    public required Int32 MinReplicas { get; init; }

    public required Int32 MaxReplicas { get; init; }

    public required String StartupPath { get; init; }

    public required String LivenessPath { get; init; }

    public required String ReadinessPath { get; init; }

    public required Int32 HealthPort { get; init; }

    public IngressPortMappingArgs[]? AdditionalPortMappings { get; init; }
}
