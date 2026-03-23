namespace KusDepot.AI;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ToolPodResult
{
    [JsonRequired]
    [Description(ResultSuccess)]
    public Boolean Success { get; init; }

    [JsonRequired]
    [Description(ResultKind)]
    public ToolPodResultKind Kind { get; init; }

    [JsonRequired]
    [Description(ResultData)]
    public String? Data { get; init; }

    [JsonRequired]
    [Description(ResultType)]
    public String? Type { get; init; }

    [JsonRequired]
    [Description(ResultReference)]
    public ToolPodRef? Reference { get; init; }

    [JsonRequired]
    [Description(ResultToolValue)]
    public ToolValue? ToolValue { get; init; }

    [JsonRequired]
    [Description(ResultError)]
    public String? Error { get; init; }
}