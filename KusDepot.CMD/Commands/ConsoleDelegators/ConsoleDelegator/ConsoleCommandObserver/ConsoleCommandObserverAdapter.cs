namespace KusDepot.Commands;

/**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/main/*'/>*/
internal sealed class ConsoleCommandObserverAdapter : IObserver<CommandEvent>
{
    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="sync"]/*'/>*/
    private readonly Lock sync = new();

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="source"]/*'/>*/
    private readonly TaskCompletionSource<ConsoleCommandObserverCompletion> source = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="callbackErrors"]/*'/>*/
    private readonly List<Exception> callbackErrors = new();

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="observer"]/*'/>*/
    private readonly ConsoleCommandObserver observer;

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="cancel"]/*'/>*/
    private readonly CancellationToken cancel;

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="started"]/*'/>*/
    private Task started = Task.CompletedTask;

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="stdout"]/*'/>*/
    private Task stdout = Task.CompletedTask;

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="stderr"]/*'/>*/
    private Task stderr = Task.CompletedTask;

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="exited"]/*'/>*/
    private ExitedCommandEvent? exited;

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="completionStarted"]/*'/>*/
    private Boolean completionStarted;

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/constructor[@name="Constructor"]/*'/>*/
    public ConsoleCommandObserverAdapter(ConsoleCommandObserver observer , CancellationToken cancel)
    {
        this.observer = observer; this.cancel = cancel;
    }

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/property[@name="Completion"]/*'/>*/
    public Task<ConsoleCommandObserverCompletion> Completion => source.Task;

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/method[@name="OnCompleted"]/*'/>*/
    public void OnCompleted() => BeginCompletion(null);

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/method[@name="OnError"]/*'/>*/
    public void OnError(Exception error) => BeginCompletion(error);

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/method[@name="OnNext"]/*'/>*/
    public void OnNext(CommandEvent value)
    {
        lock(sync)
        {
            if(completionStarted) { return; }

            switch(value)
            {
                case StartedCommandEvent e: { started = QueueCallback(started,() => observer.OnStarted(e,cancel)); break; }

                case StandardOutputCommandEvent e: { stdout = QueueCallback(Task.WhenAll(started,stdout),() => observer.OnStdOut(e,cancel)); break; }

                case StandardErrorCommandEvent e: { stderr = QueueCallback(Task.WhenAll(started,stderr),() => observer.OnStdErr(e,cancel)); break; }

                case ExitedCommandEvent e: { exited = e; break; }
            }
        }
    }

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/method[@name="QueueCallback"]/*'/>*/
    private Task QueueCallback(Task previous , Func<Task> callback)
    {
        return Task.Run(async () =>
        {
            await previous.ConfigureAwait(false);

            await InvokeCallbackAsync(callback).ConfigureAwait(false);
        });
    }

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/method[@name="InvokeCallbackAsync"]/*'/>*/
    private async Task InvokeCallbackAsync(Func<Task> callback)
    {
        try { await callback().ConfigureAwait(false); }

        catch ( OperationCanceledException e ) when(cancel.IsCancellationRequested && e.CancellationToken == cancel) { }

        catch ( Exception e ) { lock(sync) { callbackErrors.Add(e); } }
    }

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/method[@name="BeginCompletion"]/*'/>*/
    private void BeginCompletion(Exception? processError)
    {
        Task startedCallback; Task stdoutCallback; Task stderrCallback; ExitedCommandEvent? exitedEvent;

        lock(sync)
        {
            if(completionStarted) { return; }

            completionStarted = true; startedCallback = started; stdoutCallback = stdout; stderrCallback = stderr; exitedEvent = exited;
        }

        _ = CompleteAsync(startedCallback,stdoutCallback,stderrCallback,exitedEvent,processError);
    }

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/method[@name="CompleteAsync"]/*'/>*/
    private async Task CompleteAsync(Task startedCallback , Task stdoutCallback , Task stderrCallback , ExitedCommandEvent? exitedEvent , Exception? processError)
    {
        try
        {
            await Task.WhenAll(startedCallback,stdoutCallback,stderrCallback).ConfigureAwait(false);

            if(exitedEvent is not null) { await InvokeCallbackAsync(() => observer.OnExited(exitedEvent,cancel)).ConfigureAwait(false); }

            Exception[] errors;

            lock(sync) { errors = callbackErrors.ToArray(); }

            ProcessCommandResult result = new(exitedEvent?.ExitCode ?? -1,default,default);

            source.TrySetResult(new(result,processError,errors));
        }
        catch ( Exception e ) { source.TrySetException(e); }
    }
}

/**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverCompletion"]/main/*'/>*/
internal sealed record ConsoleCommandObserverCompletion(ProcessCommandResult? Result , Exception? ProcessError , Exception[] CallbackErrors);
