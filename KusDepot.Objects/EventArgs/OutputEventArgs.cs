namespace KusDepot;

/**<include file='OutputEventArgs.xml' path='OutputEventArgs/class[@name="OutputEventArgs"]/main/*'/>*/
public class OutputEventArgs : AlertEventArgs
{
    /**<include file='OutputEventArgs.xml' path='OutputEventArgs/class[@name="OutputEventArgs"]/field[@name="ID"]/*'/>*/
    public readonly Guid ID;

    /**<include file='OutputEventArgs.xml' path='OutputEventArgs/class[@name="OutputEventArgs"]/constructor[@name="Constructor"]/*'/>*/
    public OutputEventArgs(AlertCode code , Guid id) : base(code) { this.ID = id; }
}