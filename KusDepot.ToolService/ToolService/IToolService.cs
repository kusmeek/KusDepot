namespace KusDepot.ToolService;

[ServiceContract(Name = "IToolService" , Namespace = "KusDepot")]
public interface IToolService
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