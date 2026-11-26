namespace KusDepot.Exams.Tools;

public partial class CommandExams
{
    [Test]
    public async Task ExecuteConsoleDelegator_ObserverPreservesAsyncLocalAcrossSuspension()
    {
        AsyncLocal<String?> context = new(); context.Value = "command-context";
        TaskCompletionSource<Boolean> callbackEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> releaseCallback = new(TaskCreationOptions.RunContinuationsAsynchronously);
        String? observed = null;
        ConsoleCommandObserver observer = new()
        {
            OnStdOut = async (_,cancel) =>
            {
                callbackEntered.TrySetResult(true);
                await releaseCallback.Task.WaitAsync(cancel).ConfigureAwait(false);
                observed = context.Value;
            }
        };
        Tool tool = await CreateObserverToolAsync("ConsoleObserverAsyncLocal",new ConsoleDelegator(Cli.Wrap("cmd.exe").WithArguments("/c echo async-context"),streamed:true,buffered:null,observer:observer));
        Guid? id = await tool.ExecuteCommandAsync(CommandDetails.Create("ConsoleObserverAsyncLocal").SetFreeMode(true).PreserveActivity(true));
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);

        try
        {
            await callbackEntered.Task.WaitAsync(TimeSpan.FromSeconds(20));
        }
        finally { releaseCallback.TrySetResult(true); }

        CommandResult completed = (CommandResult)(await activity.Task!.WaitAsync(TimeSpan.FromSeconds(20)))!;
        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(observed).IsEqualTo("command-context");
    }

    [Test]
    public async Task ExecuteConsoleDelegator_ExitCallbackCompletesBeforeResult()
    {
        TaskCompletionSource<Boolean> exitEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> releaseExit = new(TaskCreationOptions.RunContinuationsAsynchronously);
        ConsoleCommandObserver observer = new()
        {
            OnExited = async (_,cancel) =>
            {
                exitEntered.TrySetResult(true);
                await releaseExit.Task.WaitAsync(cancel).ConfigureAwait(false);
            }
        };
        Tool tool = await CreateObserverToolAsync("ConsoleObserverExitGate",new ConsoleDelegator(Cli.Wrap("cmd.exe").WithArguments("/c exit /b 0"),streamed:true,buffered:null,observer:observer));
        Guid? id = await tool.ExecuteCommandAsync(CommandDetails.Create("ConsoleObserverExitGate").SetFreeMode(true).PreserveActivity(true));
        Check.That(id).IsNotNull();
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);

        try
        {
            await exitEntered.Task.WaitAsync(TimeSpan.FromSeconds(20));
            Check.That(activity.Task!.IsCompleted).IsFalse();
        }
        finally { releaseExit.TrySetResult(true); }

        KusDepot.CommandResult completed = (KusDepot.CommandResult)(await activity.Task!.WaitAsync(TimeSpan.FromSeconds(20)))!;
        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(completed.Output).IsInstanceOf<ProcessCommandResult>();
        Check.That(((ProcessCommandResult)completed.Output!).ExitCode).IsEqualTo(0);
    }

    [Test]
    public async Task ExecuteConsoleDelegator_ObserverFailuresAreAggregatedAfterDispatch()
    {
        Int32 exited = 0;
        ConsoleCommandObserver observer = new()
        {
            OnStarted = (_,_) => Task.FromException(new InvalidOperationException("started")),
            OnStdOut = (_,_) => Task.FromException(new InvalidOperationException("stdout")),
            OnStdErr = (_,_) => Task.FromException(new InvalidOperationException("stderr")),
            OnExited = (_,_) =>
            {
                Interlocked.Increment(ref exited);
                return Task.FromException(new InvalidOperationException("exited"));
            }
        };
        Tool tool = await CreateObserverToolAsync("ConsoleObserverFailures",new ConsoleDelegator(Cli.Wrap("cmd.exe").WithArguments("/c \"echo out & echo err 1>&2 & exit /b 0\""),streamed:true,buffered:null,observer:observer));
        Guid? id = await tool.ExecuteCommandAsync(CommandDetails.Create("ConsoleObserverFailures").SetFreeMode(true).PreserveActivity(true));
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);

        KusDepot.CommandResult completed = (KusDepot.CommandResult)(await activity.Task!.WaitAsync(TimeSpan.FromSeconds(20)))!;

        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.Faulted);
        Check.That(completed.Exception is AggregateException).IsTrue();
        Check.That(((AggregateException)completed.Exception!).Flatten().InnerExceptions.Count).IsEqualTo(4);
        Check.That(exited).IsEqualTo(1);
        Check.That(completed.Output).IsInstanceOf<FaultedCommandResult>();
        Check.That(((FaultedCommandResult)completed.Output!).ExitCode).IsEqualTo(0);
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegator_ExitWaitsForBothStreamsAndCancellationIsAggregated()
    {
        TaskCompletionSource<Boolean> stdoutEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> stderrEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> releaseStreams = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> exited = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Int32> started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        BinaryCommandObserver observer = new()
        {
            OnStarted = (e,_) =>
            {
                started.TrySetResult(e.ProcessId);
                return Task.CompletedTask;
            },
            OnStdOut = async (stream,cancel) =>
            {
                stdoutEntered.TrySetResult(true);
                await releaseStreams.Task.WaitAsync(cancel).ConfigureAwait(false);
                await stream.CopyToAsync(Stream.Null,cancel).ConfigureAwait(false);
            },
            OnStdErr = async (stream,cancel) =>
            {
                stderrEntered.TrySetResult(true);
                await releaseStreams.Task.WaitAsync(cancel).ConfigureAwait(false);
                await stream.CopyToAsync(Stream.Null,cancel).ConfigureAwait(false);
            },
            OnExited = (_,_) =>
            {
                exited.TrySetResult(true);
                return Task.CompletedTask;
            }
        };
        Tool tool = await CreateObserverToolAsync("BinaryObserverExitGate",new BinaryConsoleDelegator(Cli.Wrap("cmd.exe").WithArguments("/c \"echo out & echo err 1>&2 & exit /b 0\""),buffered:true,observer:observer));
        Guid? id = await tool.ExecuteCommandAsync(CommandDetails.Create("BinaryObserverExitGate").SetFreeMode(true).PreserveActivity(true));
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);

        try
        {
            await Task.WhenAll(started.Task,stdoutEntered.Task,stderrEntered.Task).WaitAsync(TimeSpan.FromSeconds(20));
            Assert.That(await started.Task,Is.GreaterThan(0));
            Check.That(activity.Task!.IsCompleted).IsFalse();
            Check.That(exited.Task.IsCompleted).IsFalse();
        }
        finally { releaseStreams.TrySetResult(true); }

        KusDepot.CommandResult completed = (KusDepot.CommandResult)(await activity.Task!.WaitAsync(TimeSpan.FromSeconds(20)))!;
        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(exited.Task.IsCompletedSuccessfully).IsTrue();

        using CancellationTokenSource cancellation = new();
        TaskCompletionSource<Boolean> callbackEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        BinaryCommandObserver cancelObserver = new()
        {
            OnStdOut = async (_,cancel) =>
            {
                callbackEntered.TrySetResult(true);
                try { await Task.Delay(Timeout.InfiniteTimeSpan,cancel).ConfigureAwait(false); }
                catch ( OperationCanceledException ) { throw new InvalidOperationException("observer failure"); }
            }
        };
        Tool cancelTool = await CreateObserverToolAsync("BinaryObserverCancel",new BinaryConsoleDelegator(Cli.Wrap("powershell.exe").WithArguments("-NoProfile -Command \"Start-Sleep -Seconds 30\""),observer:cancelObserver));
        Guid? cancelID = await cancelTool.ExecuteCommandAsync(CommandDetails.Create("BinaryObserverCancel").SetFreeMode(true).PreserveActivity(true),cancellation.Token);
        Activity cancelActivity = cancelTool.GetActivities()!.Single(a => a.ID == cancelID);
        await callbackEntered.Task.WaitAsync(TimeSpan.FromSeconds(20));
        cancellation.Cancel();

        KusDepot.CommandResult canceled = (KusDepot.CommandResult)(await cancelActivity.Task!.WaitAsync(TimeSpan.FromSeconds(20)))!;
        Check.That(canceled.Code).IsEqualTo(ActivityRecordCode.Canceled);
        Check.That(canceled.Exception).IsInstanceOf<OperationCanceledException>();
        Check.That(canceled.Exception!.InnerException).IsInstanceOf<AggregateException>();
        Check.That(((AggregateException)canceled.Exception.InnerException!).Flatten().InnerExceptions.Any(e => e is InvalidOperationException)).IsTrue();
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegator_ProcessStartFailureCompletesStreamsWithoutExitCallback()
    {
        TaskCompletionSource<Int32> stdoutCompleted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Int32> stderrCompleted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Int32 exited = 0;
        BinaryCommandObserver observer = new()
        {
            OnStdOut = async (stream,cancel) =>
            {
                using MemoryStream output = new();
                await stream.CopyToAsync(output,cancel).ConfigureAwait(false);
                stdoutCompleted.TrySetResult((Int32)output.Length);
            },
            OnStdErr = async (stream,cancel) =>
            {
                using MemoryStream output = new();
                await stream.CopyToAsync(output,cancel).ConfigureAwait(false);
                stderrCompleted.TrySetResult((Int32)output.Length);
            },
            OnExited = (_,_) =>
            {
                Interlocked.Increment(ref exited);
                return Task.CompletedTask;
            }
        };
        String missingExecutable = Path.Combine(Path.GetTempPath(),$"KusDepot-{Guid.NewGuid():N}.exe");
        Tool tool = await CreateObserverToolAsync("BinaryObserverStartFailure",new BinaryConsoleDelegator(Cli.Wrap(missingExecutable),observer:observer));
        Guid? id = await tool.ExecuteCommandAsync(CommandDetails.Create("BinaryObserverStartFailure").SetFreeMode(true).PreserveActivity(true));
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);

        CommandResult completed = (CommandResult)(await activity.Task!.WaitAsync(TimeSpan.FromSeconds(20)))!;

        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.Faulted);
        Check.That(completed.Exception is System.ComponentModel.Win32Exception).IsTrue();
        Check.That(await stdoutCompleted.Task.WaitAsync(TimeSpan.FromSeconds(5))).IsEqualTo(0);
        Check.That(await stderrCompleted.Task.WaitAsync(TimeSpan.FromSeconds(5))).IsEqualTo(0);
        Check.That(exited).IsEqualTo(0);
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegator_StartedObserverFailureIsReported()
    {
        Int32 started = 0;
        Int32 processID = 0;
        BinaryCommandObserver observer = new()
        {
            OnStarted = (e,_) =>
            {
                Interlocked.Increment(ref started);
                processID = e.ProcessId;
                return Task.FromException(new InvalidOperationException("started"));
            }
        };
        Tool tool = await CreateObserverToolAsync("BinaryObserverStartedFailure",new BinaryConsoleDelegator(Cli.Wrap("cmd.exe").WithArguments("/c exit /b 0"),buffered:true,observer:observer));
        Guid? id = await tool.ExecuteCommandAsync(CommandDetails.Create("BinaryObserverStartedFailure").SetFreeMode(true).PreserveActivity(true));
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);

        CommandResult completed = (CommandResult)(await activity.Task!.WaitAsync(TimeSpan.FromSeconds(20)))!;

        Check.That(completed.Code).IsEqualTo(ActivityRecordCode.Faulted);
        Check.That(started).IsEqualTo(1);
        Assert.That(processID,Is.GreaterThan(0));
        Check.That(completed.Exception is AggregateException).IsTrue();
        Check.That(((AggregateException)completed.Exception!).Flatten().InnerExceptions.Any(e => e.Message == "started")).IsTrue();
    }

    private static async Task<Tool> CreateObserverToolAsync(String handle , Command command)
    {
        Tool tool = new();
        Check.That(await tool.RegisterCommand(handle,command)).IsTrue();
        Check.That(await tool.Activate()).IsTrue();
        Check.That(await tool.EnableAllCommands()).IsTrue();
        return tool;
    }
}
