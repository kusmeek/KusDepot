namespace KusDepot.Exams.Tools;

public partial class ToolExam
{
    [Test]
    public async Task StartHostAsync_ConcurrentCalls_StartsServiceOnce()
    {
        using Tool host = CreateConcurrencyHost(); LifecycleGateService service = new(blockStart:true);
        Check.That(await host.AddHostedService(service,start:false)).IsTrue();

        Task<Boolean> first = host.StartHostAsync(); await service.StartEntered.WaitAsync(ConcurrencyTestTimeout);
        Task<Boolean> second = host.StartHostAsync();

        Check.That(second.IsCompleted).IsFalse(); service.ReleaseStart();
        Boolean[] results = await Task.WhenAll(first,second).WaitAsync(ConcurrencyTestTimeout);

        Check.That(results).ContainsExactly(true,true);
        Check.That(service.StartingCount).IsEqualTo(1); Check.That(service.StartCount).IsEqualTo(1); Check.That(service.StartedCount).IsEqualTo(1);
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test]
    public async Task StopHostAsync_ConcurrentCalls_StopsServiceOnce()
    {
        using Tool host = CreateConcurrencyHost(); LifecycleGateService service = new(blockStop:true);
        Check.That(await host.AddHostedService(service,start:false)).IsTrue(); Check.That(await host.StartHostAsync()).IsTrue();

        Task<Boolean> first = host.StopHostAsync(); await service.StopEntered.WaitAsync(ConcurrencyTestTimeout);
        Task<Boolean> second = host.StopHostAsync();

        Check.That(second.IsCompleted).IsFalse(); service.ReleaseStop();
        Boolean[] results = await Task.WhenAll(first,second).WaitAsync(ConcurrencyTestTimeout);

        Check.That(results).ContainsExactly(true,true);
        Check.That(service.StoppingCount).IsEqualTo(1); Check.That(service.StopCount).IsEqualTo(1); Check.That(service.StoppedCount).IsEqualTo(1);
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public async Task StartAndStopHostAsync_Overlap_CompletesInLifecycleOrder()
    {
        using Tool host = CreateConcurrencyHost(); LifecycleGateService service = new(blockStart:true);
        Check.That(await host.AddHostedService(service,start:false)).IsTrue();

        Task<Boolean> start = host.StartHostAsync(); await service.StartEntered.WaitAsync(ConcurrencyTestTimeout);
        Task<Boolean> stop = host.StopHostAsync();

        Check.That(stop.IsCompleted).IsFalse(); service.ReleaseStart();
        Check.That(await start.WaitAsync(ConcurrencyTestTimeout)).IsTrue(); Check.That(await stop.WaitAsync(ConcurrencyTestTimeout)).IsTrue();
        Check.That(service.StartCount).IsEqualTo(1); Check.That(service.StopCount).IsEqualTo(1);
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public async Task StartHostAsync_ServiceFault_ReturnsFalseAndMovesToError()
    {
        using Tool host = CreateConcurrencyHost(enableExceptions:false); LifecycleGateService service = new(faultStart:true);
        Check.That(await host.AddHostedService(service,start:false)).IsTrue();

        Check.That(await host.StartHostAsync().WaitAsync(ConcurrencyTestTimeout)).IsFalse();

        Check.That(service.StartingCount).IsEqualTo(1); Check.That(service.StartCount).IsEqualTo(1); Check.That(service.StartedCount).IsEqualTo(0);
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Error);
    }

    [Test]
    public async Task StopHostAsync_ServiceFault_ReturnsFalseAndMovesToError()
    {
        using Tool host = CreateConcurrencyHost(enableExceptions:false); LifecycleGateService service = new(faultStop:true);
        Check.That(await host.AddHostedService(service,start:false)).IsTrue(); Check.That(await host.StartHostAsync()).IsTrue();

        Check.That(await host.StopHostAsync().WaitAsync(ConcurrencyTestTimeout)).IsFalse();

        Check.That(service.StoppingCount).IsEqualTo(1); Check.That(service.StopCount).IsEqualTo(1); Check.That(service.StoppedCount).IsEqualTo(0);
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Error);
    }

    [Test]
    public async Task StartHostAsync_CallerCancellation_ReachesHostedService()
    {
        using Tool host = CreateConcurrencyHost(); LifecycleGateService service = new(blockStart:true); using CancellationTokenSource cancel = new();
        Check.That(await host.AddHostedService(service,start:false)).IsTrue();
        Task<Boolean> start = host.StartHostAsync(cancel.Token);

        await service.StartEntered.WaitAsync(ConcurrencyTestTimeout); cancel.Cancel();

        await service.CancellationObserved.WaitAsync(ConcurrencyTestTimeout);
        Check.ThatCode(async () => await start.WaitAsync(ConcurrencyTestTimeout)).Throws<OperationCanceledException>();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Error);
    }

    [Test]
    public async Task StopHostAsync_OperationTimeout_CancelsHostedService()
    {
        using Tool host = CreateConcurrencyHost(enableExceptions:false); LifecycleGateService service = new(blockStop:true);
        Check.That(await host.AddHostedService(service,start:false)).IsTrue(); Check.That(await host.StartHostAsync()).IsTrue();
        Task<Boolean> stop = host.StopHostAsync(timeout:TimeSpan.FromMilliseconds(100));

        await service.StopEntered.WaitAsync(ConcurrencyTestTimeout); Check.That(await stop.WaitAsync(ConcurrencyTestTimeout)).IsFalse();

        await service.CancellationObserved.WaitAsync(ConcurrencyTestTimeout);
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Error);
    }

    [Test]
    public async Task DisposeAsync_DuringLifecycleCall_WaitsForLifeLock()
    {
        ConcurrentQueue<String> events = new(); LifecycleDisposalProbeTool host = new(events); LifecycleGateService service = new(blockStart:true);
        Check.That(host.AddInstance()).IsTrue(); Check.That(host.UnMaskHostedServices()).IsTrue(); Check.That(await host.AddHostedService(service,start:false)).IsTrue();
        Task<Boolean> start = host.StartHostAsync(); await service.StartEntered.WaitAsync(ConcurrencyTestTimeout);

        Task disposal = host.DisposeAsync().AsTask(); await host.DisposalRequested.WaitAsync(ConcurrencyTestTimeout);

        Check.That(disposal.IsCompleted).IsFalse(); Check.That(events).IsEmpty(); service.ReleaseStart();
        Check.That(await start.WaitAsync(ConcurrencyTestTimeout)).IsTrue(); await disposal.WaitAsync(ConcurrencyTestTimeout);

        Check.That(events).ContainsExactly("dispose"); Check.That(host.GetDisposed()).IsTrue();
    }

    [Test]
    public async Task DisposeAsync_DuringCommandExecution_AllowsTrackedRunnerToFinishSafely()
    {
        const String handle = "GatedDisposal"; Guid id = Guid.NewGuid(); GatedExecutionCommand command = new();
        Tool host = ToolBuilderFactory.CreateBuilder().RegisterCommand(handle,command).Build() as Tool ?? throw new InvalidOperationException();
        Check.That(await host.Activate()).IsTrue(); Check.That(await host.EnableAllCommands()).IsTrue();
        Task<Guid?> execution = host.ExecuteCommandAsync(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));
        Activity activity = await command.Started.WaitAsync(ConcurrencyTestTimeout); Task<Object?> tracked = activity.Task ?? throw new InvalidOperationException();

        Task disposal = host.DisposeAsync().AsTask(); await disposal.WaitAsync(ConcurrencyTestTimeout);

        Check.That(host.GetDisposed()).IsTrue(); Check.That(execution.IsCompleted).IsFalse(); Check.That(tracked.IsCompleted).IsFalse(); command.Release();
        Check.That(await execution.WaitAsync(ConcurrencyTestTimeout)).IsNull();
        Object? completed = await tracked.WaitAsync(ConcurrencyTestTimeout);
        Check.That(completed).IsInstanceOf<CommandResult>(); Check.That(((CommandResult)completed!).Code).IsEqualTo(ActivityRecordCode.Failed);
        Check.That(activity.Record).IsNotNull(); Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Failed);
        Check.That(activity.Record.Message).IsEqualTo("Command output publication failed.");
    }

    private static Tool CreateConcurrencyHost(Boolean enableExceptions = true)
    {
        Tool host = new(); if(enableExceptions) { host.EnableMyExceptions(); } else { host.DisableMyExceptions(); }
        Check.That(host.AddInstance()).IsTrue(); Check.That(host.UnMaskHostedServices()).IsTrue(); return host;
    }
}
