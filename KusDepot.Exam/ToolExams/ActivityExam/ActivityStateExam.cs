namespace KusDepot.Exams.Tools;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ActivityStateExam
{
    [Test]
    public async Task Execute()
    {
        Tool tool = new(); StateCommand cmd = new();

        Check.That(await tool.RegisterCommand("StateCommand",cmd)).IsTrue();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        CommandDetails details = CommandDetails.Create("StateCommand");

        Guid? id = tool.ExecuteCommand(details);

        Check.That(id).IsNotNull(); Check.That(cmd.LastActivity).IsNotNull();

        Activity act = cmd.LastActivity!;

        Check.That(await tool.GetOutputAsync(id,timeout:TimeSpan.FromSeconds(30))).IsEqualTo(true);

        Check.That(act.State).IsEqualTo(ActivityState.Success);
    }

    [Test]
    public async Task ExecuteAsync()
    {
        Tool tool = new(); StateCommand cmd = new();

        Check.That(await tool.RegisterCommand("StateCommandAsync",cmd)).IsTrue();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        CommandDetails details = CommandDetails.Create("StateCommandAsync");

        Guid? id = await tool.ExecuteCommandAsync(details);

        Check.That(id).IsNotNull(); Check.That(cmd.LastActivity).IsNotNull();

        Activity act = cmd.LastActivity!;

        Check.That(await tool.GetOutputAsync(id,timeout:TimeSpan.FromSeconds(30))).IsEqualTo(true);

        Check.That(act.State).IsEqualTo(ActivityState.Success);
    }

    [Test]
    public async Task ExecuteAsync_FreeMode()
    {
        Tool tool = new(); StateCommand cmd = new();

        Check.That(await tool.RegisterCommand("StateCommandFreeMode",cmd)).IsTrue();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        CommandDetails details = CommandDetails.Create("StateCommandFreeMode").SetFreeMode(true);

        Guid? id = await tool.ExecuteCommandAsync(details);

        Check.That(id).IsNotNull(); Check.That(cmd.LastActivity).IsNotNull();

        Activity act = cmd.LastActivity!;

        Check.That(act.State).IsEqualTo(ActivityState.RunningFreeMode);

        await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        Check.That(act.State).IsEqualTo(ActivityState.RunningFreeMode);

        Check.That(await tool.GetOutputAsync(id,timeout:TimeSpan.FromSeconds(20))).IsEqualTo(true);

        Check.That(act.State).IsEqualTo(ActivityState.Success);
    }

    [Test]
    public async Task Canceled()
    {
        Tool tool = new(); CheckingCommand cmd = new();

        Check.That(await tool.RegisterCommand("CanceledAsyncState",cmd)).IsTrue();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(10));

        CommandDetails details = CommandDetails.Create("CanceledAsyncState").SetCancel(cts.Token);

        Guid? id = null;

        Check.ThatCode(async () => id = await tool.ExecuteCommandAsync(details,cts.Token)).DoesNotThrow();


        Check.That(id).IsNull(); Check.That(cmd.LastActivity).IsNotNull();

        await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

        Activity act = cmd.LastActivity!;

        Check.That(act.State).IsEqualTo(ActivityState.Canceled);
    }

    [Test]
    public async Task CanceledRunning()
    {
        ActivityRecordTool tool = new(); Guid id = Guid.NewGuid(); NonCheckingCommand cmd = new();

        tool.ExpectedCode = ActivityRecordCode.Canceled; tool.ExpectedActivityID = id;

        Check.That(await tool.RegisterCommand("CanceledAsyncState",cmd)).IsTrue();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(10));

        CommandDetails details = CommandDetails.Create("CanceledAsyncState").SetID(id).SetCancel(cts.Token);

        Guid? r = null; Check.ThatCode(async () => r = await tool.ExecuteCommandAsync(details,cts.Token)).DoesNotThrow();

        Check.That(r).IsNull();

        await Task.Delay(TimeSpan.FromSeconds(10));

        Check.That(cmd.LastActivity).IsNotNull(); Activity act = cmd.LastActivity!;

        Check.That(act.State).IsEqualTo(ActivityState.CanceledRunning);
    }

    [Test]
    public async Task TimedOut()
    {
        Tool tool = new(); TimeOutCommand cmd = new();

        Check.That(await tool.RegisterCommand("TimedOutAsyncState",cmd)).IsTrue();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        CommandDetails details = CommandDetails.Create("TimedOutAsyncState").SetTimeout(TimeSpan.FromSeconds(10));

        Guid? id = null;

        Check.ThatCode(async () => id = await tool.ExecuteCommandAsync(details)).DoesNotThrow();

        Check.That(id).IsNull(); Check.That(cmd.LastActivity).IsNotNull();

        await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

        Activity act = cmd.LastActivity!;

        Check.That(act.State).IsEqualTo(ActivityState.TimedOut);
    }

    [Test]
    public async Task TimedOutRunning()
    {
        ActivityRecordTool tool = new(); Guid id = Guid.NewGuid(); NonCheckingCommand cmd = new();

        Check.That(await tool.RegisterCommand("TimedOutAsyncState",cmd)).IsTrue();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        CommandDetails details = CommandDetails.Create("TimedOutAsyncState").SetID(id).SetTimeout(TimeSpan.FromSeconds(10));

        Guid? r = null; Check.ThatCode(async () => r = await tool.ExecuteCommandAsync(details)).DoesNotThrow();

        Check.That(r).IsNull();

        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

        Check.That(cmd.LastActivity).IsNotNull(); Activity act = cmd.LastActivity!;

        Check.That(act.State).IsEqualTo(ActivityState.TimedOutRunning);
    }

    [Test]
    public async Task Failed()
    {
        ActivityRecordTool tool = new(); FailingCommand cmd = new(); Guid id = Guid.NewGuid();

        Check.That(await tool.RegisterCommand("Failing",cmd)).IsTrue();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        CommandDetails details = CommandDetails.Create("Failing").SetID(id);

        Guid? r = null;

        Check.ThatCode(async () => r = await tool.ExecuteCommandAsync(details)).DoesNotThrow();

        Check.That(r).IsNotNull();

        Check.That(cmd.LastActivity).IsNotNull();

        Activity act = cmd.LastActivity!;

        Check.That(act.State).IsEqualTo(ActivityState.Failed);
    }

    [Test]
    public async Task Faulted()
    {
        Tool tool = new(); FaultingCommand cmd = new();

        Check.That(await tool.RegisterCommand("Faulting",cmd)).IsTrue();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        CommandDetails details = CommandDetails.Create("Faulting");

        Guid? id = null;

        Check.ThatCode(async () => id = await tool.ExecuteCommandAsync(details)).DoesNotThrow();

        Check.That(id).IsNotNull();

        Check.That(cmd.LastActivity).IsNotNull();

        Activity act = cmd.LastActivity!;

        Check.That(act.State).IsEqualTo(ActivityState.Faulted);
    }
}