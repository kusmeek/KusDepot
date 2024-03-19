namespace TempDB;

internal sealed class Settings
{
    internal static ActorServiceSettings Actor = new ActorServiceSettings(){ActorGarbageCollectionSettings = new ActorGarbageCollectionSettings(21600,3600)};
}