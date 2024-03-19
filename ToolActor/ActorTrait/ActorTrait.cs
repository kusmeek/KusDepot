namespace ToolActor;

[DataContract(Name = "ActorTrait" , Namespace = "KusDepot")]
public abstract class ActorTrait : Common
{
    public abstract Task<Boolean> Activate(CancellationToken? token = null);

    public abstract Task<Boolean> Bind(Guid? guidid = null , String? stringid = null);

    public abstract Task<Boolean> Deactivate(CancellationToken? token = null);
}