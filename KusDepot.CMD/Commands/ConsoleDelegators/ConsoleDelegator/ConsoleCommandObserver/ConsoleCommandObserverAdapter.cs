namespace KusDepot.Commands;

/**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/main/*'/>*/
internal sealed class ConsoleCommandObserverAdapter : IObserver<CommandEvent>
{
    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="source"]/*'/>*/
    private readonly TaskCompletionSource<CommandResult?> source;

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/property[@name="Completion"]/*'/>*/
    public Task<CommandResult?> Completion => source.Task;

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/field[@name="observer"]/*'/>*/
    private readonly ConsoleCommandObserver observer;

    /**<include file='ConsoleCommandObserverAdapter.xml' path='ConsoleCommandObserverAdapter/class[@name="ConsoleCommandObserverAdapter"]/constructor[@name="Constructor"]/*'/>*/
    public ConsoleCommandObserverAdapter(ConsoleCommandObserver observer)
    {
        this.observer = observer; source = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public void OnCompleted()
    {
        source.TrySetResult(new CommandResult(source.Task.IsCompletedSuccessfully ? source.Task.Result?.ExitCode ?? -1 : -1,default,default));
    }

    public void OnError(Exception error) { source.TrySetException(error); }

    public void OnNext(CommandEvent value) { _ = DispatchEvent(value); }

    private async Task DispatchEvent(CommandEvent value)
    {
        switch(value)
        {
            case StartedCommandEvent e: { await observer.OnStarted(e).ConfigureAwait(false); break; }

            case StandardOutputCommandEvent e: { await observer.OnStdOut(e).ConfigureAwait(false); break; }

            case StandardErrorCommandEvent e: { await observer.OnStdErr(e).ConfigureAwait(false); break; }

            case ExitedCommandEvent e:
            {
                await observer.OnExited(e).ConfigureAwait(false);

                source.TrySetResult(new CommandResult(e.ExitCode,default,default)); break;
            }
            default: { break; }
        }
    }
}