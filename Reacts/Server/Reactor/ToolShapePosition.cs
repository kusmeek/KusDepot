namespace KusDepot.Reacts;

public sealed record ToolShapePosition
{
    public Guid?   id { get; init; }
    public Double? x  { get; init; }
    public Double? y  { get; init; }
}