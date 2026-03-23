namespace KusDepot.Commands;

/**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/main/*'/>*/
public class BinaryCommandObserver
{
    /**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/field[@name="OnExited"]/*'/>*/
    public virtual Func<ExitedCommandEvent,Task> OnExited { get; init; }

    /**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/field[@name="OnStarted"]/*'/>*/
    public virtual Func<StartedCommandEvent,Task> OnStarted { get; init; }

    /**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/field[@name="OnStdErr"]/*'/>*/
    public virtual Func<Stream,Task> OnStdErr { get; init; }

    /**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/field[@name="OnStdOut"]/*'/>*/
    public virtual Func<Stream,Task> OnStdOut { get; init; }

    /**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/constructor[@name="Constructor"]/*'/>*/
    public BinaryCommandObserver()
    {
        OnExited  = _ => Task.CompletedTask;
        OnStarted = _ => Task.CompletedTask;
        OnStdErr  = _ => Task.CompletedTask;
        OnStdOut  = _ => Task.CompletedTask;
    }
}