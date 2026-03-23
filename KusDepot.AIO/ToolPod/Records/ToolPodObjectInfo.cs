namespace KusDepot.AI;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ToolPodObjectInfo
{
    [JsonRequired]
    public ToolPodRef? Reference { get; init; }

    [JsonRequired]
    public String[]? Properties { get; init; }

    [JsonRequired]
    public String[]? Methods { get; init; }
}