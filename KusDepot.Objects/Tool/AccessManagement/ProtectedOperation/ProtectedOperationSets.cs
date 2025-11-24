namespace KusDepot;

/**<include file='ProtectedOperationSets.xml' path='ProtectedOperationSets/class[@name="ProtectedOperationSets"]/main/*'/>*/
internal static class ProtectedOperationSets
{
    /**<include file='ProtectedOperationSets.xml' path='ProtectedOperationSets/class[@name="ProtectedOperationSets"]/field[@name="Client"]'/>*/
    public static readonly ImmutableArray<Int32> Client = ImmutableArray.Create(new Int32[]
    {
        ProtectedOperation.ExecuteCommand,
        ProtectedOperation.GetOutput,
        ProtectedOperation.GetOutputIDs,
    });

    /**<include file='ProtectedOperationSets.xml' path='ProtectedOperationSets/class[@name="ProtectedOperationSets"]/field[@name="Command"]'/>*/
    public static readonly ImmutableArray<Int32> Command = ImmutableArray.Create(new Int32[]
    {
        ProtectedOperation.AddActivity,
        ProtectedOperation.AddDataItems,
        ProtectedOperation.AddInput,
        ProtectedOperation.AddInstance,
        ProtectedOperation.AddOutput,
        ProtectedOperation.CreateManagementKey,
        ProtectedOperation.CreateOwnerKey,
        ProtectedOperation.ExecuteCommand,
        ProtectedOperation.ExportAccessManagerState,
        ProtectedOperation.GenerateCommandKey,
        ProtectedOperation.GetAccessManager,
        ProtectedOperation.GetActivities,
        ProtectedOperation.GetCommands,
        ProtectedOperation.GetConfiguration,
        ProtectedOperation.GetDataDescriptors,
        ProtectedOperation.GetDataItem,
        ProtectedOperation.GetDataItems,
        ProtectedOperation.GetHostedServices,
        ProtectedOperation.GetInput,
        ProtectedOperation.GetInputs,
        ProtectedOperation.GetOutput,
        ProtectedOperation.GetOutputIDs,
        ProtectedOperation.GetRemoveOutput,
        ProtectedOperation.GetServices,
        ProtectedOperation.GetStatus,
        ProtectedOperation.GetToolServices,
        ProtectedOperation.GetToolServiceScope,
        ProtectedOperation.GetWorkingSet,
        ProtectedOperation.ImportAccessManagementKeys,
        ProtectedOperation.Initialize,
        ProtectedOperation.KusDepotExceptions,
        ProtectedOperation.MaskCommandTypes,
        ProtectedOperation.MaskHostedServices,
        ProtectedOperation.MyExceptions,
        ProtectedOperation.RegisterCommand,
        ProtectedOperation.RegisterManager,
        ProtectedOperation.RemoveActivity,
        ProtectedOperation.RemoveDataItem,
        ProtectedOperation.RemoveOutput,
        ProtectedOperation.RemoveStatus,
        ProtectedOperation.SetID,
        ProtectedOperation.SetStatus,
        ProtectedOperation.ToStringOp,
        ProtectedOperation.UnRegisterCommand,
        ProtectedOperation.UnRegisterManager,
        ProtectedOperation.UpdateInputs,
    });

    /**<include file='ProtectedOperationSets.xml' path='ProtectedOperationSets/class[@name="ProtectedOperationSets"]/field[@name="Executive"]'/>*/
    public static readonly ImmutableArray<Int32> Executive = ImmutableArray.Create(new Int32[]
    {
        ProtectedOperation.Activate,
        ProtectedOperation.AddActivity,
        ProtectedOperation.AddDataItems,
        ProtectedOperation.AddInput,
        ProtectedOperation.AddInstance,
        ProtectedOperation.AddOutput,
        ProtectedOperation.CreateManagementKey,
        ProtectedOperation.CreateOwnerKey,
        ProtectedOperation.Deactivate,
        ProtectedOperation.DestroySecrets,
        ProtectedOperation.Dispose,
        ProtectedOperation.ExecuteCommand,
        ProtectedOperation.ExportAccessManagerState,
        ProtectedOperation.GenerateCommandKey,
        ProtectedOperation.GenerateHostedServiceKey,
        ProtectedOperation.GetAccessManager,
        ProtectedOperation.GetActivities,
        ProtectedOperation.GetCommands,
        ProtectedOperation.GetConfiguration,
        ProtectedOperation.GetDataDescriptors,
        ProtectedOperation.GetDataItem,
        ProtectedOperation.GetDataItems,
        ProtectedOperation.GetHostedServices,
        ProtectedOperation.GetInput,
        ProtectedOperation.GetInputs,
        ProtectedOperation.GetOutput,
        ProtectedOperation.GetOutputIDs,
        ProtectedOperation.GetRemoveOutput,
        ProtectedOperation.GetServices,
        ProtectedOperation.GetStatus,
        ProtectedOperation.GetToolServices,
        ProtectedOperation.GetToolServiceScope,
        ProtectedOperation.GetWorkingSet,
        ProtectedOperation.ImportAccessManagementKeys,
        ProtectedOperation.Initialize,
        ProtectedOperation.KusDepotExceptions,
        ProtectedOperation.MaskCommandTypes,
        ProtectedOperation.MaskHostedServices,
        ProtectedOperation.MyExceptions,
        ProtectedOperation.RegisterCommand,
        ProtectedOperation.RegisterManager,
        ProtectedOperation.RemoveActivity,
        ProtectedOperation.RemoveDataItem,
        ProtectedOperation.RemoveOutput,
        ProtectedOperation.RemoveStatus,
        ProtectedOperation.SetID,
        ProtectedOperation.SetStatus,
        ProtectedOperation.StartAsync,
        ProtectedOperation.StartHostAsync,
        ProtectedOperation.StartedAsync,
        ProtectedOperation.StartingAsync,
        ProtectedOperation.StopAsync,
        ProtectedOperation.StopHostAsync,
        ProtectedOperation.StoppedAsync,
        ProtectedOperation.StoppingAsync,
        ProtectedOperation.ToStringOp,
        ProtectedOperation.UnRegisterCommand,
        ProtectedOperation.UnRegisterManager,
        ProtectedOperation.UpdateInputs,
    });

    /**<include file='ProtectedOperationSets.xml' path='ProtectedOperationSets/class[@name="ProtectedOperationSets"]/field[@name="Host"]'/>*/
    public static readonly ImmutableArray<Int32> Host = ImmutableArray.Create(new Int32[]
    {
        ProtectedOperation.Activate,
        ProtectedOperation.Deactivate,
        ProtectedOperation.Dispose,
        ProtectedOperation.StartingAsync,
        ProtectedOperation.StartAsync,
        ProtectedOperation.StartedAsync,
        ProtectedOperation.StartHostAsync,
        ProtectedOperation.StopHostAsync,
        ProtectedOperation.StoppingAsync,
        ProtectedOperation.StopAsync,
        ProtectedOperation.StoppedAsync
    });

    /**<include file='ProtectedOperationSets.xml' path='ProtectedOperationSets/class[@name="ProtectedOperationSets"]/field[@name="MyHost"]'/>*/
    public static readonly ImmutableArray<Int32> MyHost = ImmutableArray.Create(Array.Empty<Int32>());

    /**<include file='ProtectedOperationSets.xml' path='ProtectedOperationSets/class[@name="ProtectedOperationSets"]/field[@name="Service"]'/>*/
    public static readonly ImmutableArray<Int32> Service = ImmutableArray.Create(new Int32[]
    {
        ProtectedOperation.ExecuteCommand,
        ProtectedOperation.AddInput,
        ProtectedOperation.GetDataItem,
        ProtectedOperation.GetDataItems,
        ProtectedOperation.GetDataDescriptors,
        ProtectedOperation.GetOutput,
        ProtectedOperation.GetOutputIDs,
        ProtectedOperation.GetRemoveOutput,
        ProtectedOperation.RemoveOutput
    });
}