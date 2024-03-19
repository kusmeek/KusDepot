namespace KusDepot.Data;

internal sealed class BlobSettings
{
    internal static ActorServiceSettings Actor = new ActorServiceSettings(){ActorGarbageCollectionSettings = new ActorGarbageCollectionSettings(1200,1200)};
}