namespace KusDepot.Dapr;

public class SchedulerManager : ServiceManager
{
    public SchedulerManager(){}

    public SchedulerManager(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , IEnumerable<Meta<ICommand>>? commands = null,
           IConfiguration? configuration = null , IContainer? container = null , IEnumerable<Object>? inputs = null , ILogger? logger = null)
           : base(accessmanager:accessmanager,data:data,id:id,commands:commands,configuration:configuration,container:container,inputs:inputs,logger:logger){}
}