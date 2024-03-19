﻿namespace KusDepot;

/**<include file='SyncTimeoutException.xml' path='SyncTimeoutException/class[@name="SyncTimeoutException"]/main/*'/>*/
[Serializable]
public class SyncTimeoutException : TimeoutException
{
    /**<include file='SyncTimeoutException.xml' path='SyncTimeoutException/class[@name="SyncTimeoutException"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public SyncTimeoutException() : base() { }

    /**<include file='SyncTimeoutException.xml' path='SyncTimeoutException/class[@name="SyncTimeoutException"]/constructor[@name="MessageConstructor"]/*'/>*/
    public SyncTimeoutException(String message) : base(message) { }

    /**<include file='SyncTimeoutException.xml' path='SyncTimeoutException/class[@name="SyncTimeoutException"]/constructor[@name="MessageInnerExceptionConstructor"]/*'/>*/
    public SyncTimeoutException(String message , Exception innerException) : base(message,innerException) { }

    /**<include file='SyncTimeoutException.xml' path='SyncTimeoutException/class[@name="SyncTimeoutException"]/constructor[@name="SerializationInfoConstructor"]/*'/>*/
    protected SyncTimeoutException(SerializationInfo info , StreamingContext context) : base(info,context) { }
}