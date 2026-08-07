namespace KusDepot.ToolManifests.Diagnostics;

/**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/main/*'/>*/
internal static class ToolManifestDiagnostics
{
    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/field[@name="InvalidToolTypeRule"]/*'/>*/
    internal static readonly DiagnosticDescriptor InvalidToolTypeRule = new(
        id: "KDT001",
        title: "ToolManifest target must derive from KusDepot.Tool",
        messageFormat: "Type '{0}' is marked with ToolManifestAttribute but does not derive from KusDepot.Tool",
        category: "KusDepot.ToolManifests",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/field[@name="UnsupportedGenericToolRule"]/*'/>*/
    internal static readonly DiagnosticDescriptor UnsupportedGenericToolRule = new(
        id: "KDT002",
        title: "Generic tool types are not supported",
        messageFormat: "Type '{0}' is marked with ToolManifestAttribute but generic tool types are not supported",
        category: "KusDepot.ToolManifests",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/field[@name="AmbiguousMethodNameRule"]/*'/>*/
    internal static readonly DiagnosticDescriptor AmbiguousMethodNameRule = new(
        id: "KDT003",
        title: "Custom tool operation method names must resolve to one protected-operation index",
        messageFormat: "Tool '{0}' declares multiple AccessCheck indexes for method name '{1}', which is ambiguous for method-name resolution",
        category: "KusDepot.ToolManifests",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/field[@name="CustomOperationIndexOutOfRangeRule"]/*'/>*/
    internal static readonly DiagnosticDescriptor CustomOperationIndexOutOfRangeRule = new(
        id: "KDT004",
        title: "Custom operation index is out of valid range",
        messageFormat: "Tool '{0}' declares custom operation index '{1}' on method '{2}', but custom operation indexes must be between 256 and 16383",
        category: "KusDepot.ToolManifests",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/field[@name="MissingToolSchemaIDRule"]/*'/>*/
    internal static readonly DiagnosticDescriptor MissingToolSchemaIDRule = new(
        id: "KDT005",
        title: "ToolManifest target must declare ToolSchemaID",
        messageFormat: "Type '{0}' is marked with ToolManifestAttribute but does not declare a ToolSchemaID",
        category: "KusDepot.ToolManifests",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/field[@name="SyncAsyncSemanticDriftRule"]/*'/>*/
    internal static readonly DiagnosticDescriptor SyncAsyncSemanticDriftRule = new(
        id: "KDT006",
        title: "Sync and async tool operation pairs should share the same protected-operation index",
        messageFormat: "Tool '{0}' declares sync/async method pair '{1}' and '{2}' with different AccessCheck indexes '{3}' and '{4}'",
        category: "KusDepot.ToolManifests",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/field[@name="HandwrittenManifestIndexMismatchRule"]/*'/>*/
    internal static readonly DiagnosticDescriptor HandwrittenManifestIndexMismatchRule = new(
        id: "KDT007",
        title: "Manifest descriptor index must match AccessCheck index",
        messageFormat: "Manifest for tool '{0}' declares method '{1}' with index '{2}', but the method's AccessCheck index is '{3}'",
        category: "KusDepot.ToolManifests",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/field[@name="HandwrittenManifestMethodNameMismatchRule"]/*'/>*/
    internal static readonly DiagnosticDescriptor HandwrittenManifestMethodNameMismatchRule = new(
        id: "KDT008",
        title: "Manifest descriptor must match a real tool operation method",
        messageFormat: "Manifest for tool '{0}' declares method '{1}', but no matching tool operation method could be found",
        category: "KusDepot.ToolManifests",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/field[@name="InvalidCompanionNamespaceRule"]/*'/>*/
    internal static readonly DiagnosticDescriptor InvalidCompanionNamespaceRule = new(
        id: "KDT009",
        title: "CompanionNamespace must be a valid namespace",
        messageFormat: "Tool '{0}' declares CompanionNamespace '{1}', but it is not a valid C# namespace",
        category: "KusDepot.ToolManifests",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/field[@name="InvalidAssemblyCompanionNamespaceRule"]/*'/>*/
    internal static readonly DiagnosticDescriptor InvalidAssemblyCompanionNamespaceRule = new(
        id: "KDT010",
        title: "Assembly CompanionNamespace must be a valid namespace",
        messageFormat: "Assembly declares ToolManifestAssemblyAttribute CompanionNamespace '{0}', but it is not a valid C# namespace",
        category: "KusDepot.ToolManifests",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/field[@name="InvalidCompanionTypeNameRule"]/*'/>*/
    internal static readonly DiagnosticDescriptor InvalidCompanionTypeNameRule = new(
        id: "KDT011",
        title: "CompanionTypeName must be a valid type name",
        messageFormat: "Tool '{0}' declares CompanionTypeName '{1}', but it is not a valid C# identifier",
        category: "KusDepot.ToolManifests",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /**<include file='ToolManifestDiagnostics.xml' path='ToolManifestDiagnostics/class[@name="ToolManifestDiagnostics"]/property[@name="All"]/*'/>*/
    internal static ImmutableArray<DiagnosticDescriptor> All { get; } =
    [
        InvalidToolTypeRule,
        UnsupportedGenericToolRule,
        AmbiguousMethodNameRule,
        CustomOperationIndexOutOfRangeRule,
        MissingToolSchemaIDRule,
        SyncAsyncSemanticDriftRule,
        HandwrittenManifestIndexMismatchRule,
        HandwrittenManifestMethodNameMismatchRule,
        InvalidCompanionNamespaceRule,
        InvalidAssemblyCompanionNamespaceRule,
        InvalidCompanionTypeNameRule
    ];
}
