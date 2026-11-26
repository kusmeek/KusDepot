namespace KusDepot.Exams.Tools;

public partial class CommandExams
{
    [Test]
    public async Task ExecuteCommandDelegator_Template_TracksNestedExecutionWithoutReplacingOutput()
    {
        const String targethandle = "CommandDelegatorTemplateTarget";
        const String handle = "CommandDelegatorTemplate"; Guid id = Guid.NewGuid(); Object output = new();
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TemplateCommand target = new(asynchronous:async (_,cancel) =>
        {
            await release.Task.WaitAsync(cancel).ConfigureAwait(false);
            return KusDepot.CommandResult.SuccessWithOutput(output);
        });
        CommandDelegator command = new(targethandle);
        Tool tool = new();

        Check.That(await tool.RegisterCommand(targethandle,target)).IsTrue();
        Check.That(await tool.RegisterCommand(handle,command)).IsTrue();
        Check.That(await tool.Activate()).IsTrue();
        Check.That(await tool.EnableAllCommands()).IsTrue();

        Task<Guid?> execution = tool.ExecuteCommandAsync(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));
        _ = await target.Started.WaitAsync(TemplateTestTimeout);
        Activity activity = tool.GetActivities()!.Single(a => String.Equals(a.Details?.Handle,handle,StringComparison.Ordinal));
        Task<Object?> tracked = activity.Task!;

        try { Check.That(tracked.IsCompleted).IsFalse(); }
        finally { _ = release.TrySetResult(true); }

        Guid? result = await execution.WaitAsync(TemplateTestTimeout);
        KusDepot.CommandResult envelope = (KusDepot.CommandResult)(await tracked.WaitAsync(TemplateTestTimeout))!;

        Check.That(result).IsEqualTo(id);
        Check.That(envelope.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(envelope.HasOutput).IsFalse();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(activity.Record.HasOutput).IsFalse();
        Check.That(tool.GetOutput(id)).IsSameReferenceAs(output);
    }

    [Test]
    public async Task ExecuteCommandDelegator_Template_ExpectedFailurePublishesNull()
    {
        const String handle = "CommandDelegatorTemplateFailure"; Guid id = Guid.NewGuid();
        CommandDelegator command = new("MissingCommandDelegatorTemplateTarget");
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = await tool.ExecuteCommandAsync(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);
        KusDepot.CommandResult envelope = (KusDepot.CommandResult)(await activity.Task!)!;

        Check.That(result).IsNull();
        Check.That(envelope.Code).IsEqualTo(ActivityRecordCode.Failed);
        Check.That(envelope.HasOutput).IsTrue();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Failed);
        Check.That(activity.Record.HasOutput).IsTrue();
        Check.That(tool.GetOutputIDs()?.Contains(id) ?? false).IsTrue();
        Check.That(tool.GetOutput(id)).IsNull();
    }

    [Test]
    public async Task ExecuteDynamicDelegator_Template_FreeModePreservesCancellation()
    {
        const String handle = "DynamicDelegatorTemplateCanceled"; Guid id = Guid.NewGuid();
        OperationCanceledException expected = new("dynamic delegation canceled");
        TaskCompletionSource<Activity> started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Func<ITool,Activity,CommandKey,Task<Activity?>> executor = async (_,activity,_) =>
        {
            _ = started.TrySetResult(activity);
            TaskCompletionSource<Boolean> canceled = new(TaskCreationOptions.RunContinuationsAsynchronously);
            using CancellationTokenRegistration registration = activity.Details.GetCancel().Register(() => canceled.TrySetResult(true));
            await canceled.Task.ConfigureAwait(false); throw expected;
        };
        DynamicDelegator command = new(executor);
        Tool tool = await CreateTemplateToolAsync(handle,command);
        using CancellationTokenSource cancellation = new();

        Guid? result = await tool.ExecuteCommandAsync(
            CommandDetails.Create(handle).SetID(id).SetFreeMode(true).PreserveActivity(true),cancellation.Token);
        Activity activity = await started.Task.WaitAsync(TemplateTestTimeout);

        cancellation.Cancel();
        KusDepot.CommandResult envelope = (KusDepot.CommandResult)(await activity.Task!.WaitAsync(TemplateTestTimeout))!;

        Check.That(result).IsEqualTo(id);
        Check.That(envelope.Code).IsEqualTo(ActivityRecordCode.Canceled);
        Check.That(envelope.HasOutput).IsTrue();
        Check.That(activity.Task.IsCompletedSuccessfully).IsTrue();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Canceled);
        Check.That(activity.Record.HasOutput).IsTrue();
        Check.That(activity.Record.Exception).IsSameReferenceAs(expected);
        Check.That(activity.Record.Exception).IsSameReferenceAs(envelope.Exception);
        Check.That(tool.GetOutputIDs()?.Contains(id) ?? false).IsTrue();
        Check.That(tool.GetOutput(id)).IsNull();
    }

    [Test]
    public async Task ExecuteToolDelegator_Template_TracksAndRelaysTargetOutput()
    {
        const String handle = "ToolDelegatorTemplate"; Guid id = Guid.NewGuid(); Object output = new();
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TemplateCommand target = new(asynchronous:async (_,cancel) =>
        {
            await release.Task.WaitAsync(cancel).ConfigureAwait(false);
            return KusDepot.CommandResult.SuccessWithOutput(output);
        });
        Tool targettool = await CreateTemplateToolAsync(handle,target);
        ToolDelegator command = new(targettool);
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Task<Guid?> execution = tool.ExecuteCommandAsync(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));
        _ = await target.Started.WaitAsync(TemplateTestTimeout);
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);
        Task<Object?> tracked = activity.Task!;

        try { Check.That(tracked.IsCompleted).IsFalse(); }
        finally { _ = release.TrySetResult(true); }

        Guid? result = await execution.WaitAsync(TemplateTestTimeout);
        KusDepot.CommandResult envelope = (KusDepot.CommandResult)(await tracked.WaitAsync(TemplateTestTimeout))!;

        Check.That(result).IsEqualTo(id);
        Check.That(envelope.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(envelope.HasOutput).IsTrue();
        Check.That(envelope.Output).IsSameReferenceAs(output);
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(activity.Record.HasOutput).IsTrue();
        Check.That(tool.GetOutput(id)).IsSameReferenceAs(output);
        Check.That(targettool.GetOutputIDs()?.Contains(id) ?? false).IsFalse();
    }
}
