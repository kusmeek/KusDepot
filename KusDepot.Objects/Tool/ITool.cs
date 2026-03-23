namespace KusDepot;

/**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/main/*'/>*/
public interface ITool : ICommon , IComparable<ITool> , IEquatable<ITool> , IDisposable , IAsyncDisposable , IHostedLifecycleService
{
    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Activate"]/*'/>*/
    Task<Boolean> Activate(CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddActivity"]/*'/>*/
    Boolean AddActivity(Activity? activity , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddDataItems"]/*'/>*/
    Boolean AddDataItems(IEnumerable<DataItem>? data , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddHostedService"]/*'/>*/
    Task<Boolean> AddHostedService(IHostedService? service , String? name = null , ImmutableArray<Int32>? permissions = null , HostRequest? request = null , Boolean start = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddHostedServiceFactory"]/*'/>*/
    Task<Boolean> AddHostedService(Func<IHostedService?>? factory , String? name = null , ImmutableArray<Int32>? permissions = null , HostRequest? request = null , Boolean start = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddHostedServiceType"]/*'/>*/
    Task<Boolean> AddHostedService(Type? servicetype , String? name = null , Object?[]? arguments = null , ImmutableArray<Int32>? permissions = null , HostRequest? request = null , Boolean start = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddInput"]/*'/>*/
    Boolean AddInput(Object? input , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddInstance"]/*'/>*/
    Boolean AddInstance(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddOutput"]/*'/>*/
    Boolean AddOutput(Guid? id , Object? output , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AttachCommand"]/*'/>*/
    Task<Boolean> AttachCommand(ICommand? command , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AttachCommandFactory"]/*'/>*/
    Task<Boolean> AttachCommand(Func<ICommand?>? factory , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AttachCommandType"]/*'/>*/
    Task<Boolean> AttachCommand(Type? commandtype , Object?[]? arguments = null , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="CheckOwner"]/*'/>*/
    Boolean CheckOwner(ManagementKey? managementkey);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ClearMyHostKey"]/*'/>*/
    Boolean ClearMyHostKey(HostKey? hostkey);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="CreateExecutiveKey"]/*'/>*/
    ExecutiveKey? CreateExecutiveKey(ManagementKey? managementkey);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="CreateManagementKey"]/*'/>*/
    ManagementKey? CreateManagementKey(String? subject , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="CreateOwnerKey"]/*'/>*/
    OwnerKey? CreateOwnerKey(String? subject , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Deactivate"]/*'/>*/
    Task<Boolean> Deactivate(CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="DetachCommand"]/*'/>*/
    Task<Boolean> DetachCommand(ICommand? command , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="DisableAllCommands"]/*'/>*/
    Task<Boolean> DisableAllCommands(CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="DisableCommand"]/*'/>*/
    Task<Boolean> DisableCommand(String? handle , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="DisableMyExceptions"]/*'/>*/
    Boolean DisableMyExceptions(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Dispose"]/*'/>*/
    void Dispose(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="DisposeAsync"]/*'/>*/
    ValueTask DisposeAsync(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="EnableAllCommands"]/*'/>*/
    Task<Boolean> EnableAllCommands(CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="EnableCommand"]/*'/>*/
    Task<Boolean> EnableCommand(String? handle , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="EnableMyExceptions"]/*'/>*/
    Boolean EnableMyExceptions(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ExecuteCommand"]/*'/>*/
    Guid? ExecuteCommand(CommandDetails? details , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ExecuteCommandAsync"]/*'/>*/
    Task<Guid?> ExecuteCommandAsync(CommandDetails? details , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ExecuteCommandCab"]/*'/>*/
    Guid? ExecuteCommandCab(KusDepotCab? cab , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ExecuteCommandCabAsync"]/*'/>*/
    Task<Guid?> ExecuteCommandCabAsync(KusDepotCab? cab , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetAccessManager"]/*'/>*/
    IAccessManager? GetAccessManager(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetActivities"]/*'/>*/
    IList<Activity>? GetActivities(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetAttachedCommands"]/*'/>*/
    IList<ICommand>? GetAttachedCommands(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetCommand"]/*'/>*/
    ICommand? GetCommand(String? handle , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetCommandHandles"]/*'/>*/
    IList<String>? GetCommandHandles(ICommand? command , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetCommands"]/*'/>*/
    Dictionary<String,ICommand>? GetCommands(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetCommandDescriptor"]/*'/>*/
    CommandDescriptor? GetCommandDescriptor(String? handle , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetCommandDescriptors"]/*'/>*/
    IList<CommandDescriptor>? GetCommandDescriptors(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetConfiguration"]/*'/>*/
    IConfiguration? GetConfiguration(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetDataDescriptors"]/*'/>*/
    IList<Descriptor>? GetDataDescriptors(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetDataItem"]/*'/>*/
    DataItem? GetDataItem(Guid? id , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetDataItems"]/*'/>*/
    HashSet<DataItem>? GetDataItems(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetDisposed"]/*'/>*/
    Boolean GetDisposed();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetHostedServices"]/*'/>*/
    IList<IHostedService>? GetHostedServices(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetHostedServiceNames"]/*'/>*/
    IList<String>? GetHostedServiceNames(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetHostedService"]/*'/>*/
    IHostedService? GetHostedService(String? name , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetInput"]/*'/>*/
    Object? GetInput(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetInputs"]/*'/>*/
    Queue<Object>? GetInputs(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetLifeCycleState"]/*'/>*/
    LifeCycleState GetLifeCycleState();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetOutputIDs"]/*'/>*/
    IList<Guid>? GetOutputIDs(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetOutput"]/*'/>*/
    Object? GetOutput(Guid? id , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetOutputType"]/*'/>*/
    TResult? GetOutput<TResult>(Guid? id , AccessKey? key = null) { try { return (TResult?)this.GetOutput(id,key); } catch { return default; } }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetOutputAsync"]/*'/>*/
    Task<Object?> GetOutputAsync(Guid? id , CancellationToken? cancel = null , TimeSpan? timeout = null , Int32? interval = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetRemoveOutput"]/*'/>*/
    Object? GetRemoveOutput(Guid? id , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetRemoveOutputAsync"]/*'/>*/
    Task<Object?> GetRemoveOutputAsync(Guid? id , CancellationToken? cancel = null , TimeSpan? timeout = null , Int32? interval = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetStatus"]/*'/>*/
    Dictionary<String,Object?>? GetStatus(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetToolDescriptor"]/*'/>*/
    ToolDescriptor? GetToolDescriptor(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetToolServiceScope"]/*'/>*/
    IServiceScope? GetToolServiceScope(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetToolServices"]/*'/>*/
    IServiceProvider? GetToolServices(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetWorkingSet"]/*'/>*/
    ConcurrentDictionary<String,Object?>? GetWorkingSet(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Initialize"]/*'/>*/
    Boolean Initialize(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="IsHostingServiceInstance"]/*'/>*/
    Boolean IsHosting(IHostedService? serviceinstance , ITool? caller = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="IsHostingServiceType"]/*'/>*/
    Boolean IsHosting(Type? servicetype , ITool? caller = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="IsHostingServiceGeneric"]/*'/>*/
    Boolean IsHosting<TService>(ITool? caller = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="MaskCommandTypes"]/*'/>*/
    Boolean MaskCommandTypes(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="MaskHostedServices"]/*'/>*/
    Boolean MaskHostedServices(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RegisterCommand"]/*'/>*/
    Task<Boolean> RegisterCommand(String? handle , ICommand? command , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RegisterCommandFactory"]/*'/>*/
    Task<Boolean> RegisterCommand(String? handle , Func<ICommand?>? factory , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RegisterCommandType"]/*'/>*/
    Task<Boolean> RegisterCommand(String? handle , Type? commandtype , Object?[]? arguments = null , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RegisterManager"]/*'/>*/
    Boolean RegisterManager(ManagementKey? managementkey , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ReleaseOwnership"]/*'/>*/
    Boolean ReleaseOwnership(ManagementKey? managementkey);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveActivity"]/*'/>*/
    Boolean RemoveActivity(Activity? activity , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveDataItem"]/*'/>*/
    Boolean RemoveDataItem(Guid? id , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveHostedService"]/*'/>*/
    Task<Boolean> RemoveHostedService(IHostedService? service , Boolean stop = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveHostedServiceName"]/*'/>*/
    Task<Boolean> RemoveHostedService(String? name , Boolean stop = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveOutput"]/*'/>*/
    Boolean RemoveOutput(Guid? id , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveStatus"]/*'/>*/
    Boolean RemoveStatus(String? index , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RequestAccess"]/*'/>*/
    AccessKey? RequestAccess(AccessRequest? request);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RevokeAccess"]/*'/>*/
    Boolean RevokeAccess(AccessKey? key);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetID"]/*'/>*/
    Boolean SetID(Guid? id , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetMyHostKey"]/*'/>*/
    Boolean SetMyHostKey(HostKey? hostkey , MyHostKey? myhostkey);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetStatus"]/*'/>*/
    Boolean SetStatus(String? index , Object? status , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartingAsync"]/*'/>*/
    Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartAsync"]/*'/>*/
    Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartedAsync"]/*'/>*/
    Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartHostAsync"]/*'/>*/
    Task<Boolean> StartHostAsync(CancellationToken? cancel = null , TimeSpan? timeout = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartHostedServiceInstance"]/*'/>*/
    Task<Boolean> StartHostedService(IHostedService? service , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartHostedServiceName"]/*'/>*/
    Task<Boolean> StartHostedService(String? name , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StopHostAsync"]/*'/>*/
    Task<Boolean> StopHostAsync(CancellationToken? cancel = null , TimeSpan? timeout = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StopHostedServiceInstance"]/*'/>*/
    Task<Boolean> StopHostedService(IHostedService? service , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StopHostedServiceName"]/*'/>*/
    Task<Boolean> StopHostedService(String? name , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StoppingAsync"]/*'/>*/
    Task StoppingAsync(CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StopAsync"]/*'/>*/
    Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StoppedAsync"]/*'/>*/
    Task StoppedAsync(CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="TakeOwnership"]/*'/>*/
    Boolean TakeOwnership(ManagementKey? managementkey);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="UnMaskCommandTypes"]/*'/>*/
    Boolean UnMaskCommandTypes(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="UnMaskHostedServices"]/*'/>*/
    Boolean UnMaskHostedServices(AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="UnRegisterCommand"]/*'/>*/
    Task<Boolean> UnRegisterCommand(String? handle , Boolean detach = false , CancellationToken? cancel = null , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="UnRegisterManager"]/*'/>*/
    Boolean UnRegisterManager(ManagementKey? managementkey , AccessKey? key = null);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="UpdateInputs"]/*'/>*/
    Boolean UpdateInputs(IEnumerable<Object>? inputs , AccessKey? key = null);
}