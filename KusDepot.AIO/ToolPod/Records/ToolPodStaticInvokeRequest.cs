namespace KusDepot.AI;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ToolPodStaticInvokeRequest
{
    [JsonRequired]
    [Description(InvokeStaticRequestType)]
    public String? Type { get; init; }

    [JsonRequired]
    [Description(InvokeStaticRequestMethod)]
    public String? Method { get; init; }

    [JsonRequired]
    [Description(InvokeStaticRequestArguments)]
    public ToolPodArgument[]? Arguments { get; init; }
}