namespace KusDepot;

/**<include file='CommandEventArgs.xml' path='CommandEventArgs/class[@name="CommandEventArgs"]/main/*'/>*/
public class CommandEventArgs : AlertEventArgs
{
    /**<include file='CommandEventArgs.xml' path='CommandEventArgs/class[@name="CommandEventArgs"]/field[@name="Handle"]/*'/>*/
    public readonly String Handle;

    /**<include file='CommandEventArgs.xml' path='CommandEventArgs/class[@name="CommandEventArgs"]/field[@name="ID"]/*'/>*/
    public readonly Guid? ID;

    /**<include file='CommandEventArgs.xml' path='CommandEventArgs/class[@name="CommandEventArgs"]/constructor[@name="Constructor"]/*'/>*/
    public CommandEventArgs(AlertCode code , String handle , Guid? id) : base(code) { this.Handle = handle; this.ID = id; }
}