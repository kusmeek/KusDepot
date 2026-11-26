namespace KusDepot.Commands;

/**<include file='ToolDelegator.xml' path='ToolDelegator/class[@name="ToolDelegator"]/main/*'/>*/
public sealed class ToolDelegator : Command
{
    /**<include file='ToolDelegator.xml' path='ToolDelegator/class[@name="ToolDelegator"]/field[@name="Target"]/*'/>*/
    private readonly ITool Target;

    /**<include file='ToolDelegator.xml' path='ToolDelegator/class[@name="ToolDelegator"]/constructor[@name="Constructor"]/*'/>*/
    public ToolDelegator(ITool target)
    {
        Target = target; ExecutionMode.AllowAll();
    }

    ///<inheritdoc/>
    protected override CommandResult ExecuteCore(Activity activity , CommandKey? key)
    {
        LogExecuting(activity,ToolDelegatorExecuting,activity.Details?.Handle,Target.GetID()?.ToStringInvariant());

        AccessKey? accesskey = activity.Details?.GetAccessKey();

        if(Target.GetDisposed() is not false) { return CommandResult.FailedWithOutput(null); }

        Guid? id = Target.ExecuteCommand(activity.Details,accesskey);

        if(id is null) { return CommandResult.FailedWithOutput(null); }

        Object? output = Target.GetRemoveOutput(id,key:accesskey);

        return CommandResult.SuccessWithOutput(output);
    }

    ///<inheritdoc/>
    protected override async ValueTask<CommandResult> ExecuteCoreAsync(Activity activity , CommandKey? key , CancellationToken cancel)
    {
        LogExecuting(activity,ToolDelegatorExecuting,activity.Details?.Handle,Target.GetID()?.ToStringInvariant());

        AccessKey? accesskey = activity.Details?.GetAccessKey();

        if(Target.GetDisposed() is not false) { return CommandResult.FailedWithOutput(null); }

        Guid? id = await Target.ExecuteCommandAsync(activity.Details,cancel,key:accesskey).ConfigureAwait(false);

        if(id is null) { return CommandResult.FailedWithOutput(null); }

        Object? output = await Target.GetRemoveOutputAsync(id,cancel,GetOutputTimeout,key:accesskey).ConfigureAwait(false);

        return CommandResult.SuccessWithOutput(output);
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