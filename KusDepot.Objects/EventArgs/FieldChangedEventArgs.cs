namespace KusDepot;

/**<include file='FieldChangedEventArgs.xml' path='FieldChangedEventArgs/class[@name="FieldChangedEventArgs"]/main/*'/>*/
public class FieldChangedEventArgs : AlertEventArgs
{
    /**<include file='FieldChangedEventArgs.xml' path='FieldChangedEventArgs/class[@name="FieldChangedEventArgs"]/field[@name="FieldName"]/*'/>*/
    public readonly String FieldName;

    /**<include file='FieldChangedEventArgs.xml' path='FieldChangedEventArgs/class[@name="FieldChangedEventArgs"]/constructor[@name="Constructor"]/*'/>*/
    public FieldChangedEventArgs(AlertCode code , String fieldname) : base(code) { this.FieldName = fieldname; }
}