namespace KusDepot.ToolActor;

internal sealed class ToolActorSettings
{
    public const Int32 TelemetryShutdownTimeout = 5000;

    internal static ActorServiceSettings ServiceSettings = new ActorServiceSettings(){ActorGarbageCollectionSettings = new ActorGarbageCollectionSettings(21600,21600)};
}