namespace DataPodServices.DataConfigs;

[GenerateSerializer]
[Alias("DataPodServices.DataConfigs.AdminCheckStatus")]
public enum AdminCheckStatus : Byte
{
    Authorized = 0,

    Forbidden = 1
}

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.DataConfigs.AdminCheckResult")]
public sealed record class AdminCheckResult
{
    [Id(0)]
    public AdminCheckStatus Status { get; init; }

    [Id(1)]
    public ConfigurationGeneration ConfigurationGeneration { get; init; }

    public AdminCheckResult(AdminCheckStatus status , ConfigurationGeneration configurationGeneration)
    {
        this.Status = status;

        this.ConfigurationGeneration = configurationGeneration;
    }
}
