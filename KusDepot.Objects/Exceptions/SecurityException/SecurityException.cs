namespace KusDepot;

/**<include file='SecurityException.xml' path='SecurityException/class[@name="SecurityException"]/main/*'/>*/
public class SecurityException : Exception
{
    /**<include file='SecurityException.xml' path='SecurityException/class[@name="SecurityException"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public SecurityException() : base() { }

    /**<include file='SecurityException.xml' path='SecurityException/class[@name="SecurityException"]/constructor[@name="MessageConstructor"]/*'/>*/
    public SecurityException(String message) : base(message) { }

    /**<include file='SecurityException.xml' path='SecurityException/class[@name="SecurityException"]/constructor[@name="MessageInnerExceptionConstructor"]/*'/>*/
    public SecurityException(String message , Exception innerException) : base(message,innerException) { }
}