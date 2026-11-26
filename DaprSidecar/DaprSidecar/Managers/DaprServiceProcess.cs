namespace KusDepot.Dapr;

internal sealed class DaprServiceProcess : IAsyncDisposable
{
    private readonly CancellationTokenSource Lifetime;

    private readonly Task<Object?> Output;

    public Int32 ProcessID { get; }

    private DaprServiceProcess(Int32 processid , CancellationTokenSource lifetime , Task<Object?> output)
    {
        ProcessID = processid; Lifetime = lifetime; Output = output; _ = ObserveOutputAsync();
    }

    public static async Task<DaprServiceProcess?> StartAsync(
        Func<ConsoleCommandObserver,CancellationToken,Task<Guid?>> start,
        Func<Guid,Task<Object?>> getoutput,
        CancellationToken ownercancel,
        CancellationToken startcancel)
    {
        CancellationTokenSource lifetime = CancellationTokenSource.CreateLinkedTokenSource(ownercancel);
        TaskCompletionSource<Int32> started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<Object?>? output = null;
        Boolean transferred = false;

        try
        {
            ConsoleCommandObserver observer = DaprConsoleObserverFactory.Create(
                (e,_) => { started.TrySetResult(e.ProcessId); return Task.CompletedTask; });

            Guid? id = await start(observer,lifetime.Token).ConfigureAwait(false); if(id is null) { return null; }

            output = getoutput(id.Value);

            Task completed = await Task.WhenAny(started.Task,output).WaitAsync(startcancel).ConfigureAwait(false);

            if(completed == output) { await output.ConfigureAwait(false); return null; }

            Int32 processid = await started.Task.ConfigureAwait(false);

            if(output.IsCompleted) { await output.ConfigureAwait(false); return null; }

            DaprServiceProcess process = new(processid,lifetime,output);

            transferred = true;

            return process;
        }
        finally
        {
            if(!transferred)
            {
                lifetime.Cancel();

                if(output is null) { lifetime.Dispose(); }

                else { _ = DisposeAfterCompletionAsync(output,lifetime); }
            }
        }
    }

    public async Task StopAsync(CancellationToken cancel = default)
    {
        Lifetime.Cancel();

        await Output.WaitAsync(cancel).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        Lifetime.Cancel();

        try { await Output.ConfigureAwait(false); }

        catch ( Exception ) {}

        finally { Lifetime.Dispose(); }
    }

    private async Task ObserveOutputAsync()
    {
        try { await Output.ConfigureAwait(false); }

        catch ( Exception ) {}
    }

    private static async Task DisposeAfterCompletionAsync(Task<Object?> output , CancellationTokenSource lifetime)
    {
        try { await output.ConfigureAwait(false); }

        catch ( Exception ) {}

        finally { lifetime.Dispose(); }
    }
}
