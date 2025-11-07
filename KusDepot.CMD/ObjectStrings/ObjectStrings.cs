namespace KusDepot.Commands;

/**<include file='ObjectStrings.xml' path='ObjectStrings/class[@name="ObjectStrings"]/main/*'/>*/
public static class ObjectStrings
{
    #pragma warning disable CS1591
    public const String AlertLogging              = @"Tool Type [{@ToolType}] ID [{@ToolID}] AlertCode [{@AlertCode}]";

    public const String AlertLoggingField         = @"Tool Type [{@ToolType}] ID [{@ToolID}] AlertCode [{@AlertCode}] [{@FieldName}]";

    public const String AlertLoggerExecuting      = @"Alert Logger Registered As {@Handle} Executing";

    public const String CommandDelegatorExecuting = @"Command Delegator Registered As {@Handle} Executing {@TargetHandle}";

    public const String ConsoleDelegatorExecuting = @"Console Delegator Registered As {@Handle} Executing";

    public const String DynamicDelegatorExecuting = @"Dynamic Delegator Registered As {@Handle} Executing";

    public const String ToolDelegatorExecuting    = @"Tool Delegator Registered As {@Handle} Invoking Tool [{@ToolID}]";
}