namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService
{
    private String GetActorID() => this.GetPrimaryKeyString();
}