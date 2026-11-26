namespace KusDepot.Data.Transfer;

/**<include file='StreamCompletionMode.xml' path='StreamCompletionMode/enum[@name="StreamCompletionMode"]/main/*'/>*/
public enum StreamCompletionMode
{
    /**<include file='StreamCompletionMode.xml' path='StreamCompletionMode/enum[@name="StreamCompletionMode"]/value[@name="UntilSourceCompletes"]/*'/>*/
    UntilSourceCompletes = 0,

    /**<include file='StreamCompletionMode.xml' path='StreamCompletionMode/enum[@name="StreamCompletionMode"]/value[@name="UntilByteLimit"]/*'/>*/
    UntilByteLimit = 1,
}