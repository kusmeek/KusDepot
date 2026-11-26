using FrameworkCommand = KusDepot.Command;
using FrameworkCommandResult = KusDepot.CommandResult;

namespace KusDepot.Exams.Tools;

internal sealed class LifecycleGateService : IHostedLifecycleService
{
    private readonly Boolean blockStart;
    private readonly Boolean blockStop;
    private readonly Boolean faultStart;
    private readonly Boolean faultStop;

    private Int32 startingCount;
    private Int32 startCount;
    private Int32 startedCount;
    private Int32 stoppingCount;
    private Int32 stopCount;
    private Int32 stoppedCount;

    private readonly TaskCompletionSource<Boolean> startEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<Boolean> startRelease = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<Boolean> stopEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<Boolean> stopRelease = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<Boolean> cancellationObserved = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public LifecycleGateService(Boolean blockStart = false , Boolean blockStop = false , Boolean faultStart = false , Boolean faultStop = false)
    {
        this.blockStart = blockStart; this.blockStop = blockStop; this.faultStart = faultStart; this.faultStop = faultStop;
    }

    public Int32 StartingCount => Volatile.Read(ref this.startingCount);

    public Int32 StartCount => Volatile.Read(ref this.startCount);

    public Int32 StartedCount => Volatile.Read(ref this.startedCount);

    public Int32 StoppingCount => Volatile.Read(ref this.stoppingCount);

    public Int32 StopCount => Volatile.Read(ref this.stopCount);

    public Int32 StoppedCount => Volatile.Read(ref this.stoppedCount);

    public Task StartEntered => this.startEntered.Task;

    public Task StopEntered => this.stopEntered.Task;

    public Task CancellationObserved => this.cancellationObserved.Task;

    public void ReleaseStart() => this.startRelease.TrySetResult(true);

    public void ReleaseStop() => this.stopRelease.TrySetResult(true);

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        _ = Interlocked.Increment(ref this.startingCount); return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Interlocked.Increment(ref this.startCount); this.startEntered.TrySetResult(true);

        if(this.faultStart) { throw new InvalidOperationException(nameof(LifecycleGateService)); }

        if(this.blockStart) { await this.WaitForReleaseAsync(this.startRelease.Task,cancellationToken).ConfigureAwait(false); }
    }

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        _ = Interlocked.Increment(ref this.startedCount); return Task.CompletedTask;
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        _ = Interlocked.Increment(ref this.stoppingCount); return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _ = Interlocked.Increment(ref this.stopCount); this.stopEntered.TrySetResult(true);

        if(this.faultStop) { throw new InvalidOperationException(nameof(LifecycleGateService)); }

        if(this.blockStop) { await this.WaitForReleaseAsync(this.stopRelease.Task,cancellationToken).ConfigureAwait(false); }
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        _ = Interlocked.Increment(ref this.stoppedCount); return Task.CompletedTask;
    }

    private async Task WaitForReleaseAsync(Task release , CancellationToken cancellationToken)
    {
        try { await release.WaitAsync(cancellationToken).ConfigureAwait(false); }

        catch(OperationCanceledException) { this.cancellationObserved.TrySetResult(true); throw; }
    }
}

internal sealed class LifecycleDisposalProbeTool : Tool
{
    private readonly ConcurrentQueue<String> events;
    private readonly TaskCompletionSource<Boolean> disposalRequested = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public LifecycleDisposalProbeTool(ConcurrentQueue<String> events)
    {
        this.events = events; this.EnableMyExceptions();
    }

    public Task DisposalRequested => this.disposalRequested.Task;

    public override async ValueTask DisposeAsync()
    {
        this.disposalRequested.TrySetResult(true); await base.DisposeAsync().ConfigureAwait(false);
    }

    protected override async ValueTask DisposeAsyncCore(Boolean disposing)
    {
        this.events.Enqueue("dispose"); await base.DisposeAsyncCore(disposing).ConfigureAwait(false);
    }
}

internal sealed class GatedExecutionCommand : FrameworkCommand
{
    private readonly TaskCompletionSource<Activity> started = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public GatedExecutionCommand() { this.ExecutionMode.AllowAsynchronousOnly(); }

    public Task<Activity> Started => this.started.Task;

    public void Release() => this.release.TrySetResult(true);

    protected override async ValueTask<FrameworkCommandResult> ExecuteCoreAsync(Activity activity , CommandKey? key , CancellationToken cancel)
    {
        this.started.TrySetResult(activity); await this.release.Task.WaitAsync(cancel).ConfigureAwait(false);

        return FrameworkCommandResult.SuccessWithOutput("completed after disposal");
    }
}

internal class OutputWaiterProbeTool : Tool
{
    public void SetLogger(ILogger logger) => this.Logger = logger;

    public Int32 GetOutputWaiterCount(Guid id)
    {
        Boolean entered = false;

        try
        {
            entered = this.Sync.Outputs.Wait(TimeSpan.FromSeconds(5));

            if(entered is false) { throw new TimeoutException(); }

            return this.OutputWaiters?.TryGetValue(id,out List<TaskCompletionSource<Object?>>? waiters) is true ? waiters.Count : 0;
        }
        finally { if(entered) { this.Sync.Outputs.Release(); } }
    }
}

internal sealed class OutputWaitingToolHost : ToolHost
{
    private Task<Object?>? pendingOutput;

    public Object? WaitedOutput { get; private set; }

    public Task<Object?> StartPendingOutputWait() => this.pendingOutput = this.GetOutputAsync(Guid.NewGuid(),timeout:Timeout.InfiniteTimeSpan);

    protected override void DisposeHostResources(List<Exception> errors)
    {
        try { this.WaitedOutput = this.pendingOutput?.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult(); }
        catch(Exception error) { errors.Add(error); }
    }

    protected override async ValueTask DisposeHostResourcesAsync(List<Exception> errors)
    {
        try { this.WaitedOutput = this.pendingOutput is null ? null : await this.pendingOutput.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false); }
        catch(Exception error) { errors.Add(error); }
    }
}

internal sealed class ThrowingOutputLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public Boolean IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel , EventId eventId , TState state , Exception? exception , Func<TState,Exception?,String> formatter)
        => throw new InvalidOperationException(nameof(ThrowingOutputLogger));
}

internal sealed class CountingScopedService : IDisposable , IAsyncDisposable
{
    private Int32 disposeCount;

    public Int32 DisposeCount => Volatile.Read(ref this.disposeCount);

    public void Dispose() { _ = Interlocked.Increment(ref this.disposeCount); }

    public ValueTask DisposeAsync() { _ = Interlocked.Increment(ref this.disposeCount); return ValueTask.CompletedTask; }
}

internal sealed class ThrowingAsyncScopedService : IAsyncDisposable
{
    private Int32 disposeCount;

    public Int32 DisposeCount => Volatile.Read(ref this.disposeCount);

    public ValueTask DisposeAsync()
    {
        _ = Interlocked.Increment(ref this.disposeCount); throw new InvalidOperationException(nameof(ThrowingAsyncScopedService));
    }
}

internal sealed class GatedAsyncScopedService : IAsyncDisposable
{
    private readonly TaskCompletionSource<Boolean> entered = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private Int32 disposeCount;

    public Task Entered => this.entered.Task;

    public Int32 DisposeCount => Volatile.Read(ref this.disposeCount);

    public void Release() => this.release.TrySetResult(true);

    public async ValueTask DisposeAsync()
    {
        _ = Interlocked.Increment(ref this.disposeCount); this.entered.TrySetResult(true);
        await this.release.Task.ConfigureAwait(false);
    }
}

internal sealed class RecordingToolHost : ToolHost
{
    private readonly ConcurrentQueue<String> events;
    private readonly String name;
    private readonly String? failingPhase;

    public RecordingToolHost(ConcurrentQueue<String> events , String name , String? failingPhase = null)
    {
        this.events = events; this.name = name; this.failingPhase = failingPhase; this.EnableMyExceptions();
    }

    protected override async Task<Boolean> Starting_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        this.events.Enqueue($"{this.name}.Starting");

        return this.failingPhase == "Starting" ? false : await base.Starting_NoSync(cancel,key).ConfigureAwait(false);
    }

    protected override async Task<Boolean> Start_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        this.events.Enqueue($"{this.name}.Start");

        return this.failingPhase == "Start" ? false : await base.Start_NoSync(cancel,key).ConfigureAwait(false);
    }

    protected override async Task<Boolean> Started_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        this.events.Enqueue($"{this.name}.Started");

        return this.failingPhase == "Started" ? false : await base.Started_NoSync(cancel,key).ConfigureAwait(false);
    }

    protected override async Task<Boolean> Stopping_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        this.events.Enqueue($"{this.name}.Stopping");

        return this.failingPhase == "Stopping" ? false : await base.Stopping_NoSync(cancel,key).ConfigureAwait(false);
    }

    protected override async Task<Boolean> Stop_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        this.events.Enqueue($"{this.name}.Stop");

        return this.failingPhase == "Stop" ? false : await base.Stop_NoSync(cancel,key).ConfigureAwait(false);
    }

    protected override async Task<Boolean> Stopped_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        this.events.Enqueue($"{this.name}.Stopped");

        return this.failingPhase == "Stopped" ? false : await base.Stopped_NoSync(cancel,key).ConfigureAwait(false);
    }
}

internal sealed class RecordingToolGenericHost : ToolGenericHost
{
    private readonly ConcurrentQueue<String> events;
    private readonly String name;

    public RecordingToolGenericHost(ConcurrentQueue<String> events , String name , IHost host)
        : base(host:host)
    {
        this.events = events; this.name = name; this.EnableMyExceptions();
    }

    protected override async Task<Boolean> Starting_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        this.events.Enqueue($"{this.name}.Starting"); return await base.Starting_NoSync(cancel,key).ConfigureAwait(false);
    }

    protected override async Task<Boolean> Start_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        this.events.Enqueue($"{this.name}.Start"); return await base.Start_NoSync(cancel,key).ConfigureAwait(false);
    }

    protected override async Task<Boolean> Started_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        this.events.Enqueue($"{this.name}.Started"); return await base.Started_NoSync(cancel,key).ConfigureAwait(false);
    }

    protected override async Task<Boolean> Stopping_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        this.events.Enqueue($"{this.name}.Stopping"); return await base.Stopping_NoSync(cancel,key).ConfigureAwait(false);
    }

    protected override async Task<Boolean> Stop_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean result = await base.Stop_NoSync(cancel,key).ConfigureAwait(false); this.events.Enqueue($"{this.name}.Stop"); return result;
    }

    protected override async Task<Boolean> Stopped_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        this.events.Enqueue($"{this.name}.Stopped"); return await base.Stopped_NoSync(cancel,key).ConfigureAwait(false);
    }
}
