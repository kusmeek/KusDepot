namespace KusDepot.DaprActors;

public interface IDaprToolActor : IActor
{
    Task<Guid?> ExecuteCommand(CommandDetails? details , AccessKey? key);

    Task<Guid?> ExecuteCommandCab(KusDepotCab? cab , AccessKey? key);

    Task<Object?> GetOutput(Guid? id , AccessKey? key);

    Task<AccessKey?> RequestAccess(AccessRequest? request);

    Task<Boolean> RevokeAccess(AccessKey? key);
}