namespace DataPodServices.DataConfigs;

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.DataConfigs.CatalogSnapshotEntry")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public sealed record class CatalogSnapshotEntry
{
    [JsonPropertyName("CatalogName")] [JsonInclude] [Id(0)]
    public String CatalogName { get; init; } = String.Empty;

    [JsonPropertyName("TenantId")] [JsonInclude] [Id(1)]
    public String TenantId { get; init; } = String.Empty;

    [JsonPropertyName("AppClientId")] [JsonInclude] [Id(2)]
    public String AppClientId { get; init; } = String.Empty;
}
