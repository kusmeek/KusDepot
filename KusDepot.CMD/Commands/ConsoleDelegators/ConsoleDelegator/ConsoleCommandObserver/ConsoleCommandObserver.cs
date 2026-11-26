namespace KusDepot.Commands;

/**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/main/*'/>*/
public class ConsoleCommandObserver
{
    /**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/property[@name="OnExited"]/*'/>*/
    public virtual Func<ExitedCommandEvent,CancellationToken,Task> OnExited { get; init; }

    /**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/property[@name="OnStarted"]/*'/>*/
    public virtual Func<StartedCommandEvent,CancellationToken,Task> OnStarted { get; init; }

    /**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/property[@name="OnStdErr"]/*'/>*/
    public virtual Func<StandardErrorCommandEvent,CancellationToken,Task> OnStdErr { get; init; }

    /**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/property[@name="OnStdOut"]/*'/>*/
    public virtual Func<StandardOutputCommandEvent,CancellationToken,Task> OnStdOut { get; init; }

    /**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/constructor[@name="Constructor"]/*'/>*/
    public ConsoleCommandObserver()
    {
        OnExited  = (_,_) => Task.CompletedTask;
        OnStarted = (_,_) => Task.CompletedTask;
        OnStdErr  = (_,_) => Task.CompletedTask;
        OnStdOut  = (_,_) => Task.CompletedTask;
    }
}