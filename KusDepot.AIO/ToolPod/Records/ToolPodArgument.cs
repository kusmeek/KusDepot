namespace KusDepot.AI;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ToolPodArgument
{
    [JsonRequired]
    [Description(ArgumentKind)]
    public ToolPodArgumentKind Kind { get; init; }

    [JsonRequired]
    [Description(ArgumentData)]
    public String? Data { get; init; }

    [JsonRequired]
    [Description(ArgumentRefId)]
    public String? RefId { get; init; }

    [JsonRequired]
    [Description(TypeNameDescription)]
    public String? Type { get; init; }
}