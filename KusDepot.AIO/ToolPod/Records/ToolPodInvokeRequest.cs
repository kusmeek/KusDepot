namespace KusDepot.AI;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ToolPodInvokeRequest
{
    [JsonRequired]
    [Description(InvokeRequestTargetId)]
    public String? TargetId { get; init; }

    [JsonRequired]
    [Description(InvokeRequestMethod)]
    public String? Method { get; init; }

    [JsonRequired]
    [Description(InvokeRequestArguments)]
    public ToolPodArgument[]? Arguments { get; init; }
}