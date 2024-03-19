namespace KusDepot;

/**<include file='AlertEventArgs.xml' path='AlertEventArgs/class[@name="AlertEventArgs"]/main/*'/>*/
public class AlertEventArgs : EventArgs
{
    /**<include file='AlertEventArgs.xml' path='AlertEventArgs/class[@name="AlertEventArgs"]/field[@name="Code"]/*'/>*/
    public readonly AlertCode Code;

    /**<include file='AlertEventArgs.xml' path='AlertEventArgs/class[@name="AlertEventArgs"]/constructor[@name="Constructor"]/*'/>*/
    public AlertEventArgs(AlertCode code) { this.Code = code; }
}