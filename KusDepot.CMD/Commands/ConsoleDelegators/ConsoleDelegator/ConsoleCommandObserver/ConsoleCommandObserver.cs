namespace KusDepot.Commands;

/**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/main/*'/>*/
public class ConsoleCommandObserver
{
    /**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/property[@name="OnExited"]/*'/>*/
    public virtual Func<ExitedCommandEvent,Task> OnExited { get; init; }

    /**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/property[@name="OnStarted"]/*'/>*/
    public virtual Func<StartedCommandEvent,Task> OnStarted { get; init; }

    /**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/property[@name="OnStdErr"]/*'/>*/
    public virtual Func<StandardErrorCommandEvent,Task> OnStdErr { get; init; }

    /**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/property[@name="OnStdOut"]/*'/>*/
    public virtual Func<StandardOutputCommandEvent,Task> OnStdOut { get; init; }

    /**<include file='ConsoleCommandObserver.xml' path='ConsoleCommandObserver/class[@name="ConsoleCommandObserver"]/constructor[@name="Constructor"]/*'/>*/
    public ConsoleCommandObserver()
    {
        OnExited  = _ => Task.CompletedTask;
        OnStarted = _ => Task.CompletedTask;
        OnStdErr  = _ => Task.CompletedTask;
        OnStdOut  = _ => Task.CompletedTask;
    }
}