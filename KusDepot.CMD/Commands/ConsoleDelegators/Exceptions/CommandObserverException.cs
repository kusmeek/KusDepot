namespace KusDepot.Commands;

/**<include file='CommandObserverException.xml' path='CommandObserverException/class[@name="CommandObserverException"]/main/*'/>*/
internal sealed class CommandObserverException : AggregateException
{
    /**<include file='CommandObserverException.xml' path='CommandObserverException/class[@name="CommandObserverException"]/constructor[@name="Constructor"]/*'/>*/
    public CommandObserverException(ProcessCommandResult? result , IEnumerable<Exception> errors)
        : base("One or more command observer callbacks failed.",errors)
    {
        Result = result;
    }

    /**<include file='CommandObserverException.xml' path='CommandObserverException/class[@name="CommandObserverException"]/property[@name="Result"]/*'/>*/
    public ProcessCommandResult? Result { get; }
}
