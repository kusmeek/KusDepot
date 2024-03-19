namespace KusDepot;

/**<include file='DataItemEventArgs.xml' path='DataItemEventArgs/class[@name="DataItemEventArgs"]/main/*'/>*/
public class DataItemEventArgs : AlertEventArgs
{
    /**<include file='DataItemEventArgs.xml' path='DataItemEventArgs/class[@name="DataItemEventArgs"]/field[@name="ID"]/*'/>*/
    public readonly Guid? ID;

    /**<include file='DataItemEventArgs.xml' path='DataItemEventArgs/class[@name="DataItemEventArgs"]/constructor[@name="Constructor"]/*'/>*/
    public DataItemEventArgs(AlertCode code , Guid? id) : base(code) { this.ID = id; }
}