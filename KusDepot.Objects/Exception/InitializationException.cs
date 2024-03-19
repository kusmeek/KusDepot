namespace KusDepot;

/**<include file='InitializationException.xml' path='InitializationException/class[@name="InitializationException"]/main/*'/>*/
[Serializable]
public class InitializationException : Exception
{
    /**<include file='InitializationException.xml' path='InitializationException/class[@name="InitializationException"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public InitializationException() : base() { }

    /**<include file='InitializationException.xml' path='InitializationException/class[@name="InitializationException"]/constructor[@name="MessageConstructor"]/*'/>*/
    public InitializationException(String message) : base(message) { }

    /**<include file='InitializationException.xml' path='InitializationException/class[@name="InitializationException"]/constructor[@name="MessageInnerExceptionConstructor"]/*'/>*/
    public InitializationException(String message , Exception innerException) : base(message,innerException) { }

    /**<include file='InitializationException.xml' path='InitializationException/class[@name="InitializationException"]/constructor[@name="SerializationInfoConstructor"]/*'/>*/
    protected InitializationException(SerializationInfo info , StreamingContext context) : base(info,context) { }
}