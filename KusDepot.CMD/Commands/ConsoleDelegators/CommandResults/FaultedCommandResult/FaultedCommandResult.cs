namespace KusDepot.Commands;

/**<include file='FaultedCommandResult.xml' path='FaultedCommandResult/class[@name="FaultedCommandResult"]/main/*'/>*/
public sealed class FaultedCommandResult : CommandResult
{
    /**<include file='FaultedCommandResult.xml' path='FaultedCommandResult/class[@name="FaultedCommandResult"]/constructor[@name="Constructor"]/*'/>*/
    public FaultedCommandResult(Int32 exitcode) : base(exitcode,default,default) {}
}