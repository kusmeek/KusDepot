namespace KusDepot.AI;

public sealed partial class ToolPod
{
    internal static class Descriptions
    {
        public const String ToolPodClass = "MCP tool pod for loading assemblies, creating CLR objects, invoking members, and tracking live object references across calls.";
        public const String SetWorkingDirectory = "Set the base directory used to resolve local assembly load paths. Paths on the system drive are rejected.";
        public const String SetWorkingDirectoryPath = "Absolute directory path used for subsequent local assembly loads.";

        public const String LoadAssembly = "Load a .NET assembly file from the active working directory into the ToolPod load context.";
        public const String LoadAssemblyFileName = "Assembly file name relative to the active working directory, for example `Labspace.dll`.";
        public const String ListAssemblies = "List AppDomain assemblies, ToolPod-context assemblies, and explicitly loaded ToolPod assembly paths.";
        public const String ListTypes = "List exported public types from a loaded ToolPod-context assembly by name. Returns type full names, base types, and whether each type is abstract, an interface, or an enum.";
        public const String ListTypesAssemblyName = "Simple name of a loaded ToolPod-context assembly, for example `Labspace`. Matched case-insensitively against loaded assembly names.";
        public const String UnloadAssemblies = "Unload the ToolPod load context, clear tracked objects and aliases, and reset loaded assembly state.";

        public const String Create = "Create and track a live CLR object by type name. Supply an alias to force the result into tracked reference mode. Pass tracked objects into constructors using Kind=Reference with the object's id or alias in RefId. Complex results are registered and returned as ToolPod references. Use Describe on the result to inspect available properties and methods.";
        public const String CreateRequest = "Object creation request including type name, optional alias, and constructor arguments.";

        public const String Describe = "Describe an object by id or alias, returning its ToolPod reference plus public instance property and method names.";
        public const String DescribeIdOrAlias = "Object id or alias.";

        public const String Invoke = "Invoke an instance method on an object. Complex results are registered and returned as ToolPod references.";
        public const String InvokeRequest = "Invocation request including target id or alias, method name, and arguments.";
        public const String InvokeAlias = "Optional alias to assign if the invocation result is registered as a tracked object.";

        public const String InvokeStatic = "Invoke a public static method on a type by full type name. The type is resolved from loaded ToolPod-context assemblies and the default load context. Complex results are registered and returned as ToolPod references.";
        public const String InvokeStaticRequest = "Static invocation request including full type name, method name, and arguments.";
        public const String InvokeStaticAlias = "Optional alias to assign if the invocation result is registered as a tracked object.";

        public const String InvokeStaticRequestType = "Full type name of the target type. For custom or loaded-assembly types, prefer an assembly-qualified or partially assembly-qualified name.";
        public const String InvokeStaticRequestMethod = "Public static method name to invoke.";
        public const String InvokeStaticRequestArguments = "Method arguments. Each entry is resolved by Kind: Value parses Data+Type, Reference resolves a tracked object from RefId.";

        public const String ListObjects = "List all currently tracked ToolPod object references.";
        public const String SetAlias = "Assigns or updates an alias for an object. Fails if the alias is already actively bound to a different object.";
        public const String SetAliasIdOrAlias = "The exact ID of the existing object, or its current alias to rename from.";
        public const String SetAliasNewAlias = "The new alias to apply.";
        public const String Remove = "Remove an object by id or alias.";
        public const String RemoveIdOrAlias = "Object id or alias.";

        public const String GetProperty = "Get a public instance property from an object by id or alias.";
        public const String GetPropertyIdOrAlias = "Object id or alias.";
        public const String GetPropertyName = "Public property name.";
        public const String GetPropertyAlias = "Optional alias to assign if the property value is registered as a tracked object.";

        public const String SetProperty = "Set a public instance property on an object by id or alias.";
        public const String SetPropertyIdOrAlias = "Object id or alias.";
        public const String SetPropertyName = "Public property name.";
        public const String SetPropertyArgument = "Value or tracked-object reference to assign.";

        public const String Status = "Get internal server status logs, loaded assemblies, and tracked objects for debugging.";

        public const String ArgumentKind = "Resolution mode. Value: parse from Data and Type. Reference: resolve a tracked object from RefId.";
        public const String ArgumentKindValue = "Parse a CLR value from Data using Type for type resolution.";
        public const String ArgumentKindReference = "Resolve a live tracked object by id or alias from RefId.";
        public const String ArgumentData = "Serialized value payload. For primitive types, pass the textual representation directly. For System.String, pass raw text — not a JSON-quoted string. Ignored when Kind is Reference.";
        public const String ArgumentRefId = "Tracked object id or alias. Used when Kind is Reference. Ignored when Kind is Value.";

        public const String CreateRequestType = "Target type to instantiate. Use a full type name. For custom or loaded-assembly types, prefer an assembly-qualified or partially assembly-qualified name.";
        public const String CreateRequestAlias = "Optional alias for the created object. When supplied, the result is always tracked as a live reference regardless of type.";
        public const String CreateRequestArguments = "Constructor arguments. Each entry is resolved by Kind: Value parses Data+Type, Reference resolves a tracked object from RefId.";

        public const String InvokeRequestTargetId = "Tracked object id or alias of the invocation target.";
        public const String InvokeRequestMethod = "Public instance method name to invoke.";
        public const String InvokeRequestArguments = "Method arguments. Each entry is resolved by Kind: Value parses Data+Type, Reference resolves a tracked object from RefId.";

        public const String ResultSuccess = "True if the operation completed without error.";
        public const String ResultKind = "Discriminator for the result payload shape: Void, Value, Reference, ToolValue, or Error.";
        public const String ResultData = "Serialized inline payload for Value results. Null for other result kinds.";
        public const String ResultType = "Full type name of the result value, reference, or ToolValue payload. Null for Void and Error results.";
        public const String ResultReference = "Tracked object reference for Reference results. Null for other result kinds.";
        public const String ResultToolValue = "ToolValue transport payload for ToolValue results. Null for other result kinds.";
        public const String ResultError = "Error message for Error results. Null for successful results.";

        public const String ResultKindVoid = "Operation completed with no return value.";
        public const String ResultKindValue = "Inline serialized scalar or JSON payload returned in Data.";
        public const String ResultKindReference = "Live tracked object returned in Reference.";
        public const String ResultKindToolValue = "Serialized ToolValue transport payload returned in ToolValue.";
        public const String ResultKindError = "Operation failed. Error contains the message.";
    }

    private static class LogStrings
    {
        public const String AssemblyLoadFailed = "ToolPod assembly load failed.";
        public const String ListAssembliesFailed = "ToolPod list assemblies failed.";
        public const String ListTypesFailed = "ToolPod list types failed.";
        public const String AssembliesUnloadFailed = "ToolPod assemblies unload failed.";
        public const String ArgumentBindingFailed = "ToolPod argument binding failed.";
        public const String CreateFailed = "ToolPod create failed.";
        public const String SetWorkingDirectoryFailed = "ToolPod set working directory failed.";
        public const String DescribeFailed = "ToolPod describe failed.";
        public const String InvokeFailed = "ToolPod invoke failed.";
        public const String InvokeStaticFailed = "ToolPod invoke static failed.";
        public const String ListObjectsFailed = "ToolPod list objects failed.";
        public const String SetAliasFailed = "ToolPod set alias failed.";
        public const String RemoveObjectFailed = "ToolPod remove object failed.";
        public const String GetPropertyFailed = "ToolPod get property failed.";
        public const String SetPropertyFailed = "ToolPod set property failed.";
        public const String ResultNormalizationFailed = "ToolPod result normalization failed.";
        public const String StatusFailed = "ToolPod status failed.";
    }

    private static class ErrorStrings
    {
        public const String AssemblyFileNameRequired = "Assembly file name is required.";
        public const String AssemblyNameRequired = "Assembly name is required.";
        public const String AssemblyNotFoundFormat = "No loaded ToolPod-context assembly matched name '{0}'.";
        public const String CreateRequestRequired = "Create request is required.";
        public const String CreateRequestTypeRequired = "Create request type is required.";
        public const String PathRequired = "Path is required.";
        public const String SystemDriveNotAllowed = "Targeting any path on the system drive is not allowed.";
        public const String InvokeRequestRequired = "Invoke request is required.";
        public const String InvokeTargetIdRequired = "Invoke request target id is required.";
        public const String InvokeMethodRequired = "Invoke request method is required.";
        public const String InvokeStaticRequestRequired = "Static invoke request is required.";
        public const String InvokeStaticTypeRequired = "Static invoke request type is required.";
        public const String InvokeStaticMethodRequired = "Static invoke request method is required.";
        public const String InvokeStaticTypeNotFoundFormat = "Could not resolve type '{0}' for static invocation.";
        public const String ObjectNotFound = "Object was not found.";
        public const String ObjectIdOrAliasRequired = "Object id or alias is required.";
        public const String NewAliasRequired = "New alias is required.";
        public const String SetAliasFailed = "Failed to set alias. It may be in use by another object, or the target was not found.";
        public const String AliasSetNoReference = "Alias set but reference could not be resolved.";
        public const String RemoveObjectFailed = "Failed to remove tracked object.";
        public const String ObjectIdRequired = "Object id is required.";
        public const String PropertyNameRequired = "Property name is required.";
        public const String ReferenceCreationFailed = "Reference creation failed.";
        public const String ToolValueCreationFailed = "ToolValue creation failed.";
        public const String AssemblyFileNotFoundFormat = "Assembly file does not exist at path: {0}";
        public const String BindConstructorArgumentFailedFormat = "Failed to bind constructor argument at index {0}.";
        public const String CreateTypeFailedFormat = "Failed to create type '{0}'.";
        public const String SetPropertyErrorFormat = "Failed to set property '{0}'.";
    }
}
