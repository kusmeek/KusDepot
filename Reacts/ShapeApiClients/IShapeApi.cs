namespace KusDepot.Reacts;

public interface IShapeApi
{
    String? Endpoint { get; } Boolean Online { get; } String? Service { get; }

    Task<ToolShape?> GenerateShape(ToolShape? input = null);

    Task<Boolean> IsOnline();
}