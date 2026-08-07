namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService
{
    private String? ActorID;

    private String GetActorID() => this.ActorID ??= this.GetPrimaryKeyString();
}