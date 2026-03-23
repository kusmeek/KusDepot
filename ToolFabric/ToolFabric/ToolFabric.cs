using Serilog;

namespace KusDepot.ToolFabric;

[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple , InstanceContextMode = InstanceContextMode.Single , Name = "ToolFabric" , Namespace = "KusDepot")]
public partial class ToolFabric : IToolFabric
{
    public readonly String URL;

    private readonly ITool _tool;

    private readonly StatefulServiceContext Context;

    public ToolFabric(StatefulServiceContext context , ITool tool)
    {
        try
        {
            this.Context = context; _tool = tool; this.URL = GetURL();
        }
        catch ( Exception _ ) { Log.Fatal(_,ToolFabricFail); Log.CloseAndFlush(); throw; }
    }

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