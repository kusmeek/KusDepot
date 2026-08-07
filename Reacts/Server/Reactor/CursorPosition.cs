namespace KusDepot.Reacts;

public sealed record CursorPosition
{
    public String? connectionId { get; init; }
    public String? name         { get; init; }
    public String? rgb          { get; init; }
    public Double? x            { get; init; }
    public Double? y            { get; init; }
}