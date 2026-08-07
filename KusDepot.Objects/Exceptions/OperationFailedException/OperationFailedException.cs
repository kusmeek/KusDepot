namespace KusDepot.Exceptions;

/**<include file='OperationFailedException.xml' path='OperationFailedException/class[@name="OperationFailedException"]/main/*'/>*/
public class OperationFailedException : Exception
{
    /**<include file='OperationFailedException.xml' path='OperationFailedException/class[@name="OperationFailedException"]/property[@name="FailureCode"]/*'/>*/
    public Enum? FailureCode { get; }

    /**<include file='OperationFailedException.xml' path='OperationFailedException/class[@name="OperationFailedException"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public OperationFailedException() : base() { }

    /**<include file='OperationFailedException.xml' path='OperationFailedException/class[@name="OperationFailedException"]/constructor[@name="MessageConstructor"]/*'/>*/
    public OperationFailedException(String message) : base(message) { }

    /**<include file='OperationFailedException.xml' path='OperationFailedException/class[@name="OperationFailedException"]/constructor[@name="MessageInnerExceptionConstructor"]/*'/>*/
    public OperationFailedException(String message , Exception innerexception) : base(message,innerexception) { }

    /**<include file='OperationFailedException.xml' path='OperationFailedException/class[@name="OperationFailedException"]/constructor[@name="MessageFailureCodeConstructor"]/*'/>*/
    public OperationFailedException(String message,Enum failurecode) : base(message) => this.FailureCode = failurecode;

    /**<include file='OperationFailedException.xml' path='OperationFailedException/class[@name="OperationFailedException"]/constructor[@name="MessageFailureCodeInnerExceptionConstructor"]/*'/>*/
    public OperationFailedException(String message,Enum failurecode,Exception innerexception) : base(message,innerexception) => this.FailureCode = failurecode;
}