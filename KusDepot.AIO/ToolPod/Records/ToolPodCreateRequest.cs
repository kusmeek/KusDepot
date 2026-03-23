namespace KusDepot.AI;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ToolPodCreateRequest
{
    [JsonRequired]
    [Description(CreateRequestType)]
    public String? Type { get; init; }

    [JsonRequired]
    [Description(CreateRequestAlias)]
    public String? Alias { get; init; }

    [JsonRequired]
    [Description(CreateRequestArguments)]
    public ToolPodArgument[]? Arguments { get; init; }
}