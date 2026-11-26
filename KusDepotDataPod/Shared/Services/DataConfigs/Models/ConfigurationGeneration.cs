namespace DataPodServices.DataConfigs;

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.DataConfigs.ConfigurationGeneration")]
public readonly record struct ConfigurationGeneration([property: Id(0)] Guid Value)
{
    public static ConfigurationGeneration Empty { get; } = new(Guid.Empty);
}
