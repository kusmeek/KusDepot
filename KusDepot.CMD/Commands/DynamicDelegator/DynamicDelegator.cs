namespace KusDepot.Commands;

/**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/main/*'/>*/
public sealed class DynamicDelegator : Command
{
    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/property[@name="CommandExecutor"]/*'/>*/
    private Func<ITool,Activity,CommandKey,ValueTask<Activity?>> CommandExecutor { get; set; }

    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public DynamicDelegator(Func<ITool,Activity,CommandKey,ValueTask<Activity?>> executor) { CommandExecutor = executor; ExecutionMode.AllowAll(); }

    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/constructor[@name="ConstructorTask"]/*'/>*/
    public DynamicDelegator(Func<ITool,Activity,CommandKey,Task<Activity?>> executor) { CommandExecutor = (t,a,k) => new ValueTask<Activity?>(executor(t,a,k)); ExecutionMode.AllowAll(); }

    /**<include file='DynamicDelegator.xml' path='DynamicDelegator/class[@name="DynamicDelegator"]/constructor[@name="ConstructorSync"]/*'/>*/
    public DynamicDelegator(Func<ITool,Activity,CommandKey,Activity?> executor) { CommandExecutor = (t,a,k) => new ValueTask<Activity?>(executor(t,a,k)); ExecutionMode.AllowAll(); }

    ///<inheritdoc/>
    protected override CommandResult ExecuteCore(Activity activity , CommandKey? key)
    {
        LogExecuting(activity,DynamicDelegatorExecuting,activity.Details?.Handle);

        ValueTask<Activity?> execution = CommandExecutor(AttachedTool!,activity,key!);

        Activity? result = execution.IsCompleted
            ? execution.Result
            : execution.AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

        return result is not null ? CommandResult.Success() : CommandResult.FailedWithOutput(null);
    }

    ///<inheritdoc/>
    protected override async ValueTask<CommandResult> ExecuteCoreAsync(Activity activity , CommandKey? key , CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        LogExecuting(activity,DynamicDelegatorExecuting,activity.Details?.Handle);

        Activity? result = await CommandExecutor(AttachedTool!,activity,key!).ConfigureAwait(false);

        return result is not null ? CommandResult.Success() : CommandResult.FailedWithOutput(null);
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