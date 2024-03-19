namespace KusDepot;

/**<include file='ActivityEventArgs.xml' path='ActivityEventArgs/class[@name="ActivityEventArgs"]/main/*'/>*/
public class ActivityEventArgs : AlertEventArgs
{
    /**<include file='ActivityEventArgs.xml' path='ActivityEventArgs/class[@name="ActivityEventArgs"]/field[@name="Activity"]/*'/>*/
    public readonly Activity Activity;

    /**<include file='ActivityEventArgs.xml' path='ActivityEventArgs/class[@name="ActivityEventArgs"]/constructor[@name="Constructor"]/*'/>*/
    public ActivityEventArgs(AlertCode code , Activity activity) : base(code) { this.Activity = activity; }
}