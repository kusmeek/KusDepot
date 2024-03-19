namespace ToolActor;

[DataContract(Name = "ActorCommand" , Namespace = "KusDepot")]
public abstract class ActorCommand : Common
{
    public abstract Boolean Attach(Guid? guidid = null , String? stringid = null);

    public abstract Boolean Detach();

    public abstract Boolean Disable();

    public abstract Boolean Enable();

    public abstract Activity? ExecuteAsync(Object[]? parameter);

    public abstract Dictionary<String,String> GetSpecifications();
}