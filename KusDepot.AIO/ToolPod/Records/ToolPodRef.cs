namespace KusDepot.AI;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ToolPodRef
{
    [JsonRequired]
    public String? Id { get; init; }

    [JsonRequired]
    public String? Alias { get; init; }

    [JsonRequired]
    public String? Type { get; init; }

    [JsonRequired]
    public String? Assembly { get; init; }
}