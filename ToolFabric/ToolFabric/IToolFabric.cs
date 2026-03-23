namespace KusDepot.ToolFabric;

[ServiceContract(Name = "IToolFabric" , Namespace = "KusDepot")]
public interface IToolFabric
{
    [OperationContract(Name = "ExecuteCommand")]
    Guid? ExecuteCommand(CommandDetails? details = null , AccessKey? key = null);

    [OperationContract(Name = "ExecuteCommandCab")]
    Guid? ExecuteCommandCab(KusDepotCab? cab = null , AccessKey? key = null);

    [OperationContract(Name = "GetOutput")]
    Object? GetOutput(Guid? id , AccessKey? key = null);

    [OperationContract(Name = "RequestAccess")]
    AccessKey? RequestAccess(AccessRequest? request = null);

    [OperationContract(Name = "RevokeAccess")]
    Boolean RevokeAccess(AccessKey? key = null);
}