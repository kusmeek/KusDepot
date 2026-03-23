namespace KusDepot.Strings;

/**<include file='ObjectStrings.xml' path='ObjectStrings/class[@name="ObjectStrings"]/main/*'/>*/
public static class ObjectStrings
{
    #pragma warning disable CS1591

    public const String CommandWorkflowLog        = "[Command Workflow Event] {Key} {Data}";

    public const String ToolAspireHostDisposing   = @"ToolAspireHost [{@ToolType}] Disposing [{@ToolID}]";

    public const String ToolAspireHostStarted     = @"ToolAspireHost [{@ToolType}] Started [{@ToolID}]";

    public const String ToolAspireHostStartEnter  = @"ToolAspireHost [{@ToolType}] Start Enter [{@ToolID}]";

    public const String ToolAspireHostStartExit   = @"ToolAspireHost [{@ToolType}] Start Exit [{@ToolID}]";

    public const String ToolAspireHostStarting    = @"ToolAspireHost [{@ToolType}] Starting [{@ToolID}]";

    public const String ToolAspireHostStopEnter   = @"ToolAspireHost [{@ToolType}] Stop Enter [{@ToolID}]";

    public const String ToolAspireHostStopExit    = @"ToolAspireHost [{@ToolType}] Stop Exit [{@ToolID}]";

    public const String ToolAspireHostStopping    = @"ToolAspireHost [{@ToolType}] Stopping [{@ToolID}]";

    public const String ToolAspireHostStopped     = @"ToolAspireHost [{@ToolType}] Stopped [{@ToolID}]";

    public const String ToolGenericHostDisposing  = @"ToolGenericHost [{@ToolType}] Disposing [{@ToolID}]";

    public const String ToolGenericHostStartEnter = @"ToolGenericHost [{@ToolType}] Start Enter [{@ToolID}]";

    public const String ToolGenericHostStartExit  = @"ToolGenericHost [{@ToolType}] Start Exit [{@ToolID}]";

    public const String ToolGenericHostStarted    = @"ToolGenericHost [{@ToolType}] Started [{@ToolID}]";

    public const String ToolGenericHostStarting   = @"ToolGenericHost [{@ToolType}] Starting [{@ToolID}]";

    public const String ToolGenericHostStopEnter  = @"ToolGenericHost [{@ToolType}] Stop Enter [{@ToolID}]";

    public const String ToolGenericHostStopExit   = @"ToolGenericHost [{@ToolType}] Stop Exit [{@ToolID}]";

    public const String ToolGenericHostStopping   = @"ToolGenericHost [{@ToolType}] Stopping [{@ToolID}]";

    public const String ToolGenericHostStopped    = @"ToolGenericHost [{@ToolType}] Stopped [{@ToolID}]";

    public const String ToolHostDisposing         = @"ToolHost [{@ToolType}] Disposing [{@ToolID}]";

    public const String ToolHostStarted           = @"ToolHost [{@ToolType}] Started [{@ToolID}]";

    public const String ToolHostStartEnter        = @"ToolHost [{@ToolType}] Start Enter [{@ToolID}]";

    public const String ToolHostStartExit         = @"ToolHost [{@ToolType}] Start Exit [{@ToolID}]";

    public const String ToolHostStarting          = @"ToolHost [{@ToolType}] Starting [{@ToolID}]";

    public const String ToolHostStopEnter         = @"ToolHost [{@ToolType}] Stop Enter [{@ToolID}]";

    public const String ToolHostStopExit          = @"ToolHost [{@ToolType}] Stop Exit [{@ToolID}]";

    public const String ToolHostStopping          = @"ToolHost [{@ToolType}] Stopping [{@ToolID}]";

    public const String ToolHostStopped           = @"ToolHost [{@ToolType}] Stopped [{@ToolID}]";

    public const String ToolDisposing             = @"Tool [{@ToolType}] Disposing [{@ToolID}]";

    public const String ToolLoggerNameFormat      = "{0}-[{1}]";

    public const String ToolStarted               = @"Tool [{@ToolType}] Started [{@ToolID}]";

    public const String ToolStartEnter            = @"Tool [{@ToolType}] Start Enter [{@ToolID}]";

    public const String ToolStartExit             = @"Tool [{@ToolType}] Start Exit [{@ToolID}]";

    public const String ToolStarting              = @"Tool [{@ToolType}] Starting [{@ToolID}]";

    public const String ToolStopEnter             = @"Tool [{@ToolType}] Stop Enter [{@ToolID}]";

    public const String ToolStopExit              = @"Tool [{@ToolType}] Stop Exit [{@ToolID}]";

    public const String ToolStopping              = @"Tool [{@ToolType}] Stopping [{@ToolID}]";

    public const String ToolStopped               = @"Tool [{@ToolType}] Stopped [{@ToolID}]";

    public const String ToolWebHostDisposing      = @"ToolWebHost [{@ToolType}] Disposing [{@ToolID}]";

    public const String ToolWebHostStarted        = @"ToolWebHost [{@ToolType}] Started [{@ToolID}]";

    public const String ToolWebHostStartEnter     = @"ToolWebHost [{@ToolType}] Start Enter [{@ToolID}]";

    public const String ToolWebHostStartExit      = @"ToolWebHost [{@ToolType}] Start Exit [{@ToolID}]";

    public const String ToolWebHostStarting       = @"ToolWebHost [{@ToolType}] Starting [{@ToolID}]";

    public const String ToolWebHostStopEnter      = @"ToolWebHost [{@ToolType}] Stop Enter [{@ToolID}]";

    public const String ToolWebHostStopExit       = @"ToolWebHost [{@ToolType}] Stop Exit [{@ToolID}]";

    public const String ToolWebHostStopping       = @"ToolWebHost [{@ToolType}] Stopping [{@ToolID}]";

    public const String ToolWebHostStopped        = @"ToolWebHost [{@ToolType}] Stopped [{@ToolID}]";



    public const String DataTypeDescription          = "Data type of the content. See Data Resource for valid values.";

    public const String TypeNameDescription          = "Full type name. For custom or ambiguous types, prefer an assembly-qualified or partially assembly-qualified name. For closed generic types, each generic type argument must be fully assembly-qualified.";

    public const String SecurityKeyDescription       = "Base64-encoded key material. Provide Key as a Base64 string.";

    public const String BinaryContentDescription     = "Base64-encoded binary content. Provide Content as a Base64 string.";

    public const String MultiMediaContentDescription = "Base64-encoded multimedia content. Provide Content as a Base64 string.";

    public const String ToolValueModeDescription               = "Transport mode. Parse: value reconstructed via TryParse/Parse. Build: value reconstructed via typed constructor with arguments. Custom: converter-specific serialization. Unhandled: no converter matched.";

    public const String ToolValueDataDescription               = "Serialized payload. For Parse mode, a parseable string. For Custom mode, the serialized object. Null when the value is carried entirely by Arguments.";

    public const String ToolValueArgumentsDescription          = "Constructor arguments used to reconstruct the value in Build mode. Each entry carries an index, mode, type, data, and optional nested parameters. Null for Parse and Custom modes.";

    public const String ToolValueArgumentIndexDescription      = "Zero-based position of this argument in the target constructor parameter list.";

    public const String ToolValueArgumentDataDescription       = "Parseable string value for Parse-mode arguments. Null for Build-mode arguments where nested Parameters carry the data.";

    public const String ToolValueArgumentParametersDescription = "Nested constructor parameters for Build-mode arguments. Each parameter is resolved via TryParse against its type.";

    public const String ToolValueParameterIndexDescription     = "Zero-based position of this parameter in the nested constructor parameter list.";

    public const String ToolValueParameterDataDescription      = "Parseable string value resolved via TryParse against the parameter type.";
}