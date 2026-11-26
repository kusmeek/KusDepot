using FrameworkCommand = KusDepot.Command;
using TemplateResult = KusDepot.CommandResult;

namespace KusDepot.Exams.Tools;

public partial class CommandExams
{
    private static readonly TimeSpan TemplateTestTimeout = TimeSpan.FromSeconds(5);

    [Test]
    public async Task ExecuteCommand_Template_SuccessPublishesOutput()
    {
        const String handle = "TemplateSyncSuccess"; Guid id = Guid.NewGuid(); Object output = new();
        TemplateResult expected = TemplateResult.SuccessWithOutput(output,"sync success");
        TemplateCommand command = new(synchronous:_ => expected);
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = tool.ExecuteCommand(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));

        Check.That(result).IsEqualTo(id);
        Check.That(tool.GetOutput(id)).IsSameReferenceAs(output);
        Check.That(command.LastActivity).IsNotNull();
        Check.That(command.LastActivity!.Record).IsNotNull();
        Check.That(command.LastActivity.Record!.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(command.LastActivity.Record.HasOutput).IsTrue();
        Check.That(command.LastActivity.Record.Message).IsEqualTo("sync success");
        Check.That(command.LastActivity.Record.Exception).IsNull();
        Check.That(tool.GetActivities()?.Any(a => ReferenceEquals(a,command.LastActivity)) ?? false).IsTrue();
    }

    [Test]
    public async Task ExecuteCommand_Template_ExpectedFailure()
    {
        const String handle = "TemplateSyncFailure"; Guid id = Guid.NewGuid();
        TemplateCommand command = new(synchronous:_ => TemplateResult.Failed("expected failure"));
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = tool.ExecuteCommand(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));

        Check.That(result).IsNull();
        Check.That(command.LastActivity).IsNotNull();
        Check.That(command.LastActivity!.Record).IsNotNull();
        Check.That(command.LastActivity.Record!.Code).IsEqualTo(ActivityRecordCode.Failed);
        Check.That(command.LastActivity.Record.HasOutput).IsFalse();
        Check.That(command.LastActivity.Record.Message).IsEqualTo("expected failure");
        Check.That(command.LastActivity.Record.Exception).IsNull();
        Check.That(tool.GetOutputIDs()?.Contains(id) ?? false).IsFalse();
    }

    [Test]
    public async Task ExecuteCommand_Template_PublishesNullOutput()
    {
        const String handle = "TemplateSyncNullOutput"; Guid id = Guid.NewGuid();
        TemplateCommand command = new(synchronous:_ => TemplateResult.SuccessWithOutput(null));
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = tool.ExecuteCommand(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));

        Check.That(result).IsEqualTo(id);
        Check.That(tool.GetOutput(id)).IsNull();
        Check.That(tool.GetOutputIDs()?.Contains(id) ?? false).IsTrue();
        Check.That(command.LastActivity!.Record!.HasOutput).IsTrue();
    }

    [Test]
    public async Task ExecuteCommand_Template_OutputPublicationFailureIsRecorded()
    {
        const String handle = "TemplateOutputPublicationFailure"; Guid id = Guid.NewGuid();
        Object existing = new(); Object rejected = new();
        TemplateCommand command = new(synchronous:_ => TemplateResult.SuccessWithOutput(rejected));
        Tool tool = await CreateTemplateToolAsync(handle,command);
        Check.That(tool.AddOutput(id,existing)).IsTrue();

        Guid? result = tool.ExecuteCommand(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));

        Check.That(result).IsNull();
        Check.That(tool.GetOutput(id)).IsSameReferenceAs(existing);
        Check.That(command.LastActivity).IsNotNull();
        Check.That(command.LastActivity!.Record).IsNotNull();
        Check.That(command.LastActivity.Record!.Code).IsEqualTo(ActivityRecordCode.Failed);
        Check.That(command.LastActivity.Record.HasOutput).IsFalse();
        Check.That(command.LastActivity.Record.Message).IsEqualTo("Command output publication failed.");
    }

    [Test]
    public async Task ExecuteCommandAsync_Template_TracksAwaitedRunner()
    {
        const String handle = "TemplateAsyncSuccess"; Guid id = Guid.NewGuid(); Object output = new();
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TemplateResult expected = TemplateResult.SuccessWithOutput(output,"async success");
        TemplateCommand command = new(asynchronous:async (_,cancel) =>
        {
            await release.Task.WaitAsync(cancel).ConfigureAwait(false); return expected;
        });
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Task<Guid?> execution = tool.ExecuteCommandAsync(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));
        Activity activity = await command.Started.WaitAsync(TemplateTestTimeout);
        Task<Object?>? tracked = activity.Task;

        try
        {
            Check.That(tracked).IsNotNull();
            Check.That(tracked!.IsCompleted).IsFalse();
        }
        finally { _ = release.TrySetResult(true); }

        Guid? result = await execution.WaitAsync(TemplateTestTimeout);
        Object? completed = await tracked!.WaitAsync(TemplateTestTimeout);

        Check.That(result).IsEqualTo(id);
        Check.That(completed).IsSameReferenceAs(expected);
        Check.That(tracked.IsCompletedSuccessfully).IsTrue();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(activity.Record.HasOutput).IsTrue();
        Check.That(activity.Record.Message).IsEqualTo("async success");
        Check.That(tool.GetOutput(id)).IsSameReferenceAs(output);
    }

    [Test]
    public async Task ExecuteCommandAsync_Template_ExpectedFailure()
    {
        const String handle = "TemplateAsyncFailure"; Guid id = Guid.NewGuid(); Object output = new();
        TemplateResult expected = TemplateResult.FailedWithOutput(output,"async expected failure");
        TemplateCommand command = new(asynchronous:(_,_) => ValueTask.FromResult(expected));
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = await tool.ExecuteCommandAsync(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));
        Activity activity = command.LastActivity!;

        Check.That(result).IsNull();
        Check.That(activity).IsNotNull();
        Check.That(activity.Task).IsNotNull();
        Check.That(await activity.Task!).IsSameReferenceAs(expected);
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Failed);
        Check.That(activity.Record.HasOutput).IsTrue();
        Check.That(activity.Record.Message).IsEqualTo("async expected failure");
        Check.That(activity.Record.Exception).IsNull();
        Check.That(tool.GetOutput(id)).IsSameReferenceAs(output);
    }

    [Test]
    public async Task ExecuteCommandAsync_Template_FreeModeRegistersBeforeSuccess()
    {
        const String handle = "TemplateFreeSuccess"; Guid id = Guid.NewGuid(); Object output = new();
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TemplateCommand command = new(asynchronous:async (_,cancel) =>
        {
            await release.Task.WaitAsync(cancel).ConfigureAwait(false); return TemplateResult.SuccessWithOutput(output);
        });
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = await tool.ExecuteCommandAsync(
            CommandDetails.Create(handle).SetID(id).SetFreeMode(true).PreserveActivity(true));
        Activity activity = await command.Started.WaitAsync(TemplateTestTimeout);

        try
        {
            Check.That(result).IsEqualTo(id);
            Check.That(tool.GetActivities()?.Any(a => ReferenceEquals(a,activity)) ?? false).IsTrue();
            Check.That(activity.Task).IsNotNull();
            Check.That(activity.Task!.IsCompleted).IsFalse();
            Check.That(activity.Record).IsNull();
            Check.That(tool.GetOutputIDs()?.Contains(id) ?? false).IsFalse();
        }
        finally { _ = release.TrySetResult(true); }

        TemplateResult completed = (TemplateResult)(await activity.Task!.WaitAsync(TemplateTestTimeout))!;

        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(activity.Task.IsCompletedSuccessfully).IsTrue();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(activity.Record.HasOutput).IsTrue();
        Check.That(tool.GetOutput(id)).IsSameReferenceAs(output);
    }

    [Test]
    public async Task ExecuteCommandAsync_Template_FreeModeRecordsExpectedFailure()
    {
        const String handle = "TemplateFreeFailure"; Guid id = Guid.NewGuid(); Object output = new();
        TemplateCommand command = new(asynchronous:(_,_) =>
            ValueTask.FromResult(TemplateResult.FailedWithOutput(output,"free expected failure")));
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = await tool.ExecuteCommandAsync(
            CommandDetails.Create(handle).SetID(id).SetFreeMode(true).PreserveActivity(true));
        Activity activity = await command.Started.WaitAsync(TemplateTestTimeout);
        TemplateResult completed = (TemplateResult)(await activity.Task!.WaitAsync(TemplateTestTimeout))!;

        Check.That(result).IsEqualTo(id);
        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.Failed);
        Check.That(activity.Task!.IsCompletedSuccessfully).IsTrue();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Failed);
        Check.That(activity.Record.HasOutput).IsTrue();
        Check.That(activity.Record.Message).IsEqualTo("free expected failure");
        Check.That(tool.GetOutput(id)).IsSameReferenceAs(output);
    }

    [Test]
    public async Task ExecuteCommandAsync_Template_FreeModePreservesExternalCancellation()
    {
        const String handle = "TemplateFreeCanceled"; Guid id = Guid.NewGuid();
        OperationCanceledException expected = new("external cancellation");
        TemplateCommand command = new(asynchronous:async (_,cancel) =>
        {
            TaskCompletionSource<Boolean> canceled = new(TaskCreationOptions.RunContinuationsAsynchronously);
            using CancellationTokenRegistration registration = cancel.Register(() => canceled.TrySetResult(true));
            await canceled.Task.ConfigureAwait(false); throw expected;
        });
        Tool tool = await CreateTemplateToolAsync(handle,command);
        using CancellationTokenSource cancellation = new();

        Guid? result = await tool.ExecuteCommandAsync(
            CommandDetails.Create(handle).SetID(id).SetFreeMode(true).PreserveActivity(true),cancellation.Token);
        Activity activity = await command.Started.WaitAsync(TemplateTestTimeout);
        cancellation.Cancel();
        TemplateResult completed = (TemplateResult)(await activity.Task!.WaitAsync(TemplateTestTimeout))!;

        Check.That(result).IsEqualTo(id);
        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.Canceled);
        Check.That(activity.Task!.IsCompletedSuccessfully).IsTrue();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Canceled);
        Check.That(activity.Record.HasOutput).IsFalse();
        Check.That(activity.Record.Exception).IsSameReferenceAs(expected);
        Check.That(tool.GetOutput(id)).IsNull();
        Check.That(tool.GetOutputIDs()?.Contains(id) ?? false).IsFalse();
    }

    [Test]
    public async Task ExecuteCommandAsync_Template_FreeModePreservesTimeout()
    {
        const String handle = "TemplateFreeTimedOut"; Guid id = Guid.NewGuid();
        OperationCanceledException expected = new("execution timed out");
        TemplateCommand command = new(asynchronous:async (_,cancel) =>
        {
            TaskCompletionSource<Boolean> canceled = new(TaskCreationOptions.RunContinuationsAsynchronously);
            using CancellationTokenRegistration registration = cancel.Register(() => canceled.TrySetResult(true));
            await canceled.Task.ConfigureAwait(false); throw expected;
        });
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = await tool.ExecuteCommandAsync(CommandDetails.Create(handle).SetID(id).SetFreeMode(true)
            .SetTimeout(TimeSpan.FromMilliseconds(100)).PreserveActivity(true));
        Activity activity = await command.Started.WaitAsync(TemplateTestTimeout);
        TemplateResult completed = (TemplateResult)(await activity.Task!.WaitAsync(TemplateTestTimeout))!;

        Check.That(result).IsEqualTo(id);
        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.TimedOut);
        Check.That(activity.Task!.IsCompletedSuccessfully).IsTrue();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.TimedOut);
        Check.That(activity.Record.HasOutput).IsFalse();
        Check.That(activity.Record.Exception).IsSameReferenceAs(expected);
        Check.That(tool.GetOutput(id)).IsNull();
        Check.That(tool.GetOutputIDs()?.Contains(id) ?? false).IsFalse();
    }

    [Test]
    public async Task ExecuteCommandAsync_Template_FreeModePreservesFault()
    {
        const String handle = "TemplateFreeFaulted"; Guid id = Guid.NewGuid();
        InvalidOperationException expected = new("template fault");
        TemplateCommand command = new(asynchronous:(_,_) => ValueTask.FromException<TemplateResult>(expected));
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = await tool.ExecuteCommandAsync(
            CommandDetails.Create(handle).SetID(id).SetFreeMode(true).PreserveActivity(true));
        Activity activity = await command.Started.WaitAsync(TemplateTestTimeout);
        TemplateResult completed = (TemplateResult)(await activity.Task!.WaitAsync(TemplateTestTimeout))!;

        Check.That(result).IsEqualTo(id);
        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.Faulted);
        Check.That(activity.Task!.IsCompletedSuccessfully).IsTrue();
        Check.That(activity.Task.IsFaulted).IsFalse();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Faulted);
        Check.That(activity.Record.HasOutput).IsFalse();
        Check.That(activity.Record.Message).IsEqualTo(expected.Message);
        Check.That(activity.Record.Exception).IsSameReferenceAs(expected);
        Check.That(tool.GetOutput(id)).IsNull();
        Check.That(tool.GetOutputIDs()?.Contains(id) ?? false).IsFalse();
    }

    [Test]
    public async Task ExecuteCommandAsync_Template_CleansUpUnpreservedFreeModeActivity()
    {
        const String handle = "TemplateFreeCleanup"; Guid id = Guid.NewGuid();
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TemplateCommand command = new(asynchronous:async (_,cancel) =>
        {
            await release.Task.WaitAsync(cancel).ConfigureAwait(false); return TemplateResult.Success();
        });
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = await tool.ExecuteCommandAsync(CommandDetails.Create(handle).SetID(id).SetFreeMode(true));
        Activity activity = await command.Started.WaitAsync(TemplateTestTimeout);

        try
        {
            Check.That(result).IsEqualTo(id);
            Check.That(tool.GetActivities()?.Any(a => ReferenceEquals(a,activity)) ?? false).IsTrue();
        }
        finally { _ = release.TrySetResult(true); }

        _ = await activity.Task!.WaitAsync(TemplateTestTimeout);

        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(activity.Record.HasOutput).IsFalse();
        Check.That(tool.GetActivities()?.Any(a => ReferenceEquals(a,activity)) ?? false).IsFalse();
    }

    [Test]
    public async Task ExecuteCommand_Template_UnsupportedSynchronousCoreFaults()
    {
        const String handle = "TemplateUnsupportedSync"; Guid id = Guid.NewGuid();
        TemplateCommand command = new();
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = tool.ExecuteCommand(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));
        Activity activity = command.LastActivity!;

        Check.That(result).IsNull();
        Check.That(activity).IsNotNull();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Faulted);
        Check.That(activity.Record.HasOutput).IsFalse();
        Check.That(activity.Record.Exception).IsInstanceOf<NotImplementedException>();
        Check.That(activity.Record.Message).IsEqualTo(activity.Record.Exception!.Message);
    }

    [Test]
    public async Task ExecuteCommandAsync_Template_UnsupportedAsynchronousCoreFaults()
    {
        const String handle = "TemplateUnsupportedAsync"; Guid id = Guid.NewGuid();
        TemplateCommand command = new();
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? result = await tool.ExecuteCommandAsync(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));
        Activity activity = command.LastActivity!;
        TemplateResult completed = (TemplateResult)(await activity.Task!)!;

        Check.That(result).IsNull();
        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.Faulted);
        Check.That(activity.Task!.IsCompletedSuccessfully).IsTrue();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Faulted);
        Check.That(activity.Record.HasOutput).IsFalse();
        Check.That(activity.Record.Exception).IsInstanceOf<NotImplementedException>();
        Check.That(activity.Record.Message).IsEqualTo(activity.Record.Exception!.Message);
    }

    [Test]
    public async Task ExecuteCommand_Template_LegacyPublicOverridesRemainCompatible()
    {
        const String handle = "TemplateLegacyOverride";
        Guid syncID = Guid.NewGuid(); Guid asyncID = Guid.NewGuid();
        const Double x = 12.5; const Double y = 4.0; const Double expected = x * y;
        CommandTest command = new();
        Tool tool = await CreateTemplateToolAsync(handle,command);

        Guid? syncResult = tool.ExecuteCommand(CommandDetails.Create(handle).SetID(syncID)
            .SetArgument("x",x).SetArgument("y",y));
        Guid? asyncResult = await tool.ExecuteCommandAsync(CommandDetails.Create(handle).SetID(asyncID)
            .SetArgument("x",x).SetArgument("y",y));

        Check.That(syncResult).IsEqualTo(syncID);
        Check.That(asyncResult).IsEqualTo(asyncID);
        Check.That(tool.GetOutput(syncID)).IsEqualTo(expected);
        Check.That(tool.GetOutput(asyncID)).IsEqualTo(expected);
    }

    private sealed class TemplateCommand : FrameworkCommand
    {
        private readonly Func<Activity,TemplateResult>? synchronous;
        private readonly Func<Activity,CancellationToken,ValueTask<TemplateResult>>? asynchronous;
        private readonly TaskCompletionSource<Activity> started = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Activity? LastActivity { get; private set; }

        public Task<Activity> Started => this.started.Task;

        public TemplateCommand(
            Func<Activity,TemplateResult>? synchronous = null,
            Func<Activity,CancellationToken,ValueTask<TemplateResult>>? asynchronous = null)
        {
            this.synchronous = synchronous; this.asynchronous = asynchronous; _ = this.ExecutionMode.AllowAll();
        }

        protected override TemplateResult ExecuteCore(Activity activity , CommandKey? key)
        {
            this.LastActivity = activity; _ = this.started.TrySetResult(activity);

            return this.synchronous is null ? base.ExecuteCore(activity,key) : this.synchronous(activity);
        }

        protected override ValueTask<TemplateResult> ExecuteCoreAsync(Activity activity , CommandKey? key , CancellationToken cancel)
        {
            this.LastActivity = activity; _ = this.started.TrySetResult(activity);

            return this.asynchronous is null ? base.ExecuteCoreAsync(activity,key,cancel) : this.asynchronous(activity,cancel);
        }
    }

    private static async Task<Tool> CreateTemplateToolAsync(String handle , FrameworkCommand command)
    {
        Tool tool = new();

        Check.That(await tool.RegisterCommand(handle,command)).IsTrue();
        Check.That(await tool.Activate()).IsTrue();
        Check.That(await tool.EnableAllCommands()).IsTrue();

        return tool;
    }
}
