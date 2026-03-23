namespace DataPodServices.CoreCache;

public sealed partial class CoreCacheService
{
    private String GetActorID() => this.GetPrimaryKeyString();
}