namespace KusDepot.Commands;

/**<include file='CanceledCommandResult.xml' path='CanceledCommandResult/class[@name="CanceledCommandResult"]/main/*'/>*/
public sealed class CanceledCommandResult : CommandResult
{
    /**<include file='CanceledCommandResult.xml' path='CanceledCommandResult/class[@name="CanceledCommandResult"]/constructor[@name="Constructor"]/*'/>*/
    public CanceledCommandResult() : base(1,default,default) {}
}