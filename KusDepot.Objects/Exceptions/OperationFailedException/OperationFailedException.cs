namespace KusDepot;

/**<include file='OperationFailedException.xml' path='OperationFailedException/class[@name="OperationFailedException"]/main/*'/>*/
public class OperationFailedException : Exception
{
    /**<include file='OperationFailedException.xml' path='OperationFailedException/class[@name="OperationFailedException"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public OperationFailedException() : base() { }

    /**<include file='OperationFailedException.xml' path='OperationFailedException/class[@name="OperationFailedException"]/constructor[@name="MessageConstructor"]/*'/>*/
    public OperationFailedException(String message) : base(message) { }

    /**<include file='OperationFailedException.xml' path='OperationFailedException/class[@name="OperationFailedException"]/constructor[@name="MessageInnerExceptionConstructor"]/*'/>*/
    public OperationFailedException(String message , Exception innerException) : base(message,innerException) { }
}