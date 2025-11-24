namespace KusDepot.ToolService;

[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple , InstanceContextMode = InstanceContextMode.Single , Name = "ToolService" , Namespace = "KusDepot")]
public partial class ToolService : IToolService
{
    private readonly ITool _tool;

    public ToolService() { _tool = new Tool(); }

    public ToolService(ITool tool) { _tool = tool; }

    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Guid? ExecuteCommand(CommandDetails? details = null , AccessKey? key = null)
    {
        return _tool.ExecuteCommand(details,key);
    }

    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Guid? ExecuteCommandCab(KusDepotCab? cab = null , AccessKey? key = null)
    {
        return _tool.ExecuteCommandCab(cab,key);
    }

    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Object? GetOutput(Guid? id , AccessKey? key = null)
    {
        return _tool.GetOutput(id,key);
    }

    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public AccessKey? RequestAccess(AccessRequest? request = null)
    {
        return _tool.RequestAccess(request);
    }

    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean RevokeAccess(AccessKey? key = null)
    {
        return _tool.RevokeAccess(key);
    }
}