namespace KusDepot.Reacts;

public sealed record ToolMessage
{
    public Guid?   id      { get; init; }
    public String? sender  { get; init; }
    public String? message { get; init; }
}