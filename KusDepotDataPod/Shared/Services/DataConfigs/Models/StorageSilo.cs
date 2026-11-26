namespace DataPodServices.DataConfigs;

[GenerateSerializer] [Immutable]
[Alias("DataPodServices.DataConfigs.StorageSilo")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public sealed record class StorageSilo
{
    [JsonPropertyName("CatalogName")] [JsonInclude] [Id(0)]
    public String CatalogName { get; init; }

    [JsonPropertyName("ConnectionString")] [JsonInclude] [Id(1)]
    public String ConnectionString { get; init; }

    public StorageSilo(String catalogname , String connectionstring)
    {
        this.CatalogName = catalogname; this.ConnectionString = connectionstring;
    }
}
