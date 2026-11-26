namespace KusDepot.Commands;

/**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/main/*'/>*/
public class BinaryCommandObserver
{
    /**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/field[@name="OnExited"]/*'/>*/
    public virtual Func<ExitedCommandEvent,CancellationToken,Task> OnExited { get; init; }

    /**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/field[@name="OnStarted"]/*'/>*/
    public virtual Func<StartedCommandEvent,CancellationToken,Task> OnStarted { get; init; }

    /**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/field[@name="OnStdErr"]/*'/>*/
    public virtual Func<Stream,CancellationToken,Task> OnStdErr { get; init; }

    /**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/field[@name="OnStdOut"]/*'/>*/
    public virtual Func<Stream,CancellationToken,Task> OnStdOut { get; init; }

    /**<include file='BinaryCommandObserver.xml' path='BinaryCommandObserver/class[@name="BinaryCommandObserver"]/constructor[@name="Constructor"]/*'/>*/
    public BinaryCommandObserver()
    {
        OnExited  = (_,_) => Task.CompletedTask;
        OnStarted = (_,_) => Task.CompletedTask;
        OnStdErr  = (_,_) => Task.CompletedTask;
        OnStdOut  = (_,_) => Task.CompletedTask;
    }
}