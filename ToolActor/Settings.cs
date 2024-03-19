namespace ToolActor;

internal sealed class ToolActorSettings
{
    internal static ActorServiceSettings Actor = new ActorServiceSettings(){ActorGarbageCollectionSettings = new ActorGarbageCollectionSettings(21600,21600)};
}