namespace KusDepot.Security;

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
        ProtectedOperation.AddOutput,
        ProtectedOperation.RemoveActivity,
    });

    /**<include file='ProtectedOperationSets.xml' path='ProtectedOperationSets/class[@name="ProtectedOperationSets"]/field[@name="Executive"]'/>*/
    public static readonly ImmutableArray<Int32> Executive = ImmutableArray.Create(new Int32[]
    {
        ProtectedOperation.Activate,
        ProtectedOperation.AddActivity,
        ProtectedOperation.AddDataItems,
        ProtectedOperation.AddHostedService,
        ProtectedOperation.AddInput,
        ProtectedOperation.AddInstance,
        ProtectedOperation.AddOutput,
        ProtectedOperation.AttachCommand,
        ProtectedOperation.ClearMyHostKey,
        ProtectedOperation.CreateManagementKey,
        ProtectedOperation.CreateOwnerKey,
        ProtectedOperation.Deactivate,
        ProtectedOperation.DestroySecrets,
        ProtectedOperation.DetachCommand,
        ProtectedOperation.DisableAllCommands,
        ProtectedOperation.DisableCommand,
        ProtectedOperation.Dispose,
        ProtectedOperation.EnableAllCommands,
        ProtectedOperation.EnableCommand,
        ProtectedOperation.ExecuteCommand,
        ProtectedOperation.ExportAccessManagerState,
        ProtectedOperation.GenerateAccessKey,
        ProtectedOperation.GetAccessManager,
        ProtectedOperation.GetActivities,
        ProtectedOperation.GetCommandDescriptor,
        ProtectedOperation.GetCommandDescriptors,
        ProtectedOperation.GetCommand,
        ProtectedOperation.GetCommandHandles,
        ProtectedOperation.GetCommands,
        ProtectedOperation.GetConfiguration,
        ProtectedOperation.GetDataDescriptors,
        ProtectedOperation.GetDataItem,
        ProtectedOperation.GetDataItems,
        ProtectedOperation.GetHostedServiceNames,
        ProtectedOperation.GetHostedServices,
        ProtectedOperation.GetInput,
        ProtectedOperation.GetInputs,
        ProtectedOperation.GetOutput,
        ProtectedOperation.GetOutputIDs,
        ProtectedOperation.GetRemoveOutput,
        ProtectedOperation.GetServices,
        ProtectedOperation.GetStatus,
        ProtectedOperation.GetToolDescriptor,
        ProtectedOperation.GetToolServices,
        ProtectedOperation.GetToolServiceScope,
        ProtectedOperation.GetWorkingSet,
        ProtectedOperation.ImportAccessManagementKeys,
        ProtectedOperation.Initialize,
        ProtectedOperation.MaskCommandTypes,
        ProtectedOperation.MaskHostedServices,
        ProtectedOperation.MyExceptions,
        ProtectedOperation.RegisterCommand,
        ProtectedOperation.RegisterManager,
        ProtectedOperation.RemoveActivity,
        ProtectedOperation.RemoveDataItem,
        ProtectedOperation.RemoveHostedService,
        ProtectedOperation.RemoveOutput,
        ProtectedOperation.RemoveStatus,
        ProtectedOperation.SetID,
        ProtectedOperation.SetMyHostKey,
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
        ProtectedOperation.UnMaskCommandTypes,
        ProtectedOperation.UnMaskHostedServices,
        ProtectedOperation.UnRegisterCommand,
        ProtectedOperation.UnRegisterManager,
        ProtectedOperation.UpdateInputs,
    });

    /**<include file='ProtectedOperationSets.xml' path='ProtectedOperationSets/class[@name="ProtectedOperationSets"]/field[@name="Host"]'/>*/
    public static readonly ImmutableArray<Int32> Host = ImmutableArray.Create(new Int32[]
    {
        ProtectedOperation.Activate,
        ProtectedOperation.ClearMyHostKey,
        ProtectedOperation.Deactivate,
        ProtectedOperation.Dispose,
        ProtectedOperation.RegisterManager,
        ProtectedOperation.SetMyHostKey,
        ProtectedOperation.StartingAsync,
        ProtectedOperation.StartAsync,
        ProtectedOperation.StartedAsync,
        ProtectedOperation.StartHostAsync,
        ProtectedOperation.StopHostAsync,
        ProtectedOperation.StoppingAsync,
        ProtectedOperation.StopAsync,
        ProtectedOperation.StoppedAsync,
        ProtectedOperation.UnRegisterManager
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