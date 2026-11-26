namespace KusDepot.Commands;

/**<include file='CommandDelegator.xml' path='CommandDelegator/class[@name="CommandDelegator"]/main/*'/>*/
public sealed class CommandDelegator : Command
{
    /**<include file='CommandDelegator.xml' path='CommandDelegator/class[@name="CommandDelegator"]/property[@name="TargetHandle"]/*'/>*/
    public String TargetHandle { get; private set; }

    /**<include file='CommandDelegator.xml' path='CommandDelegator/class[@name="CommandDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public CommandDelegator(String targethandle)
    {
        TargetHandle = targethandle; ExecutionMode.AllowAll();
    }

    ///<inheritdoc/>
    protected override CommandResult ExecuteCore(Activity activity , CommandKey? key)
    {
        LogExecuting(activity,CommandDelegatorExecuting,activity.Details?.Handle,TargetHandle);

        Guid? id = AttachedTool!.ExecuteCommand(
            CommandDetails.Create(TargetHandle,activity.Details?.Arguments,activity.ID,activity.Details?.Workflow),Key);

        return id is not null ? CommandResult.Success() : CommandResult.FailedWithOutput(null);
    }

    ///<inheritdoc/>
    protected override async ValueTask<CommandResult> ExecuteCoreAsync(Activity activity , CommandKey? key , CancellationToken cancel)
    {
        LogExecuting(activity,CommandDelegatorExecuting,activity.Details?.Handle,TargetHandle);

        Guid? id = await AttachedTool!.ExecuteCommandAsync(
            CommandDetails.Create(TargetHandle,activity.Details?.Arguments,activity.ID,activity.Details?.Workflow),
            cancel,Key).ConfigureAwait(false);

        return id is not null ? CommandResult.Success() : CommandResult.FailedWithOutput(null);
    }

    ///<inheritdoc/>
    protected override CommandResult CreateCanceledResult(Activity activity , OperationCanceledException exception)
    {
        return activity.IsTimedOut()
            ? CommandResult.TimedOutWithOutput(null,exception)
            : CommandResult.CanceledWithOutput(null,exception);
    }

    ///<inheritdoc/>
    protected override CommandResult CreateFaultedResult(Activity activity , Exception exception)
    {
        return CommandResult.FaultedWithOutput(exception,null);
    }
}