namespace DataPodServices.CatalogDB;

public sealed class CatalogDBInitializationCoordinator : IAsyncDisposable
{
    private Boolean disposed;
    private readonly Lock gate = new();
    private readonly CancellationTokenSource shutdown = new();
    private readonly HashSet<String> initialized = new(StringComparer.Ordinal);
    private readonly Dictionary<String,Task> initializations = new(StringComparer.Ordinal);

    public async Task EnsureInitializedAsync(String catalog , Func<CancellationToken,Task> initialize , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalog);

        ArgumentNullException.ThrowIfNull(initialize);

        Task initialization;

        TaskCompletionSource? completion = null;

        lock(this.gate)
        {
            ObjectDisposedException.ThrowIf(this.disposed,this);

            if(this.initialized.Contains(catalog)) { return; }

            if(!this.initializations.TryGetValue(catalog,out initialization!))
            {
                completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

                initialization = completion.Task;

                this.initializations.Add(catalog,initialization);
            }
        }

        if(completion is not null) { _ = RunInitializationAsync(catalog,initialization,completion,initialize); }

        await initialization.WaitAsync(cancel);
    }

    private async Task RunInitializationAsync(String catalog , Task task , TaskCompletionSource completion , Func<CancellationToken,Task> initialize)
    {
        try
        {
            await initialize(this.shutdown.Token);

            lock(this.gate)
            {
                if(this.initializations.TryGetValue(catalog,out Task? current) && ReferenceEquals(current,task))
                {
                    this.initializations.Remove(catalog);

                    this.initialized.Add(catalog);
                }
            }

            completion.TrySetResult();
        }
        catch(Exception exception)
        {
            lock(this.gate)
            {
                if(this.initializations.TryGetValue(catalog,out Task? current) && ReferenceEquals(current,task))
                {
                    this.initializations.Remove(catalog);
                }
            }

            completion.TrySetException(exception);
        }
    }

    public async ValueTask DisposeAsync()
    {
        Task[] pending;

        lock(this.gate)
        {
            if(this.disposed) { return; }

            this.disposed = true;

            pending = this.initializations.Values.ToArray();
        }

        await this.shutdown.CancelAsync();

        try { await Task.WhenAll(pending); } catch { }

        this.shutdown.Dispose();
    }
}

public sealed class CatalogDBInitializationException : Exception
{
    public CatalogDBInitializationException(String catalog , Exception innerException)
        : base($"Catalog database initialization failed for '{catalog}'.",innerException)
    {}
}
