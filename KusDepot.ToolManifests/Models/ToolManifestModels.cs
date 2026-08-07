namespace KusDepot.ToolManifests.Models;

/**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="MethodMetadata"]/main/*'/>*/
internal sealed class MethodMetadata
{
    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="MethodMetadata"]/constructor[@name="Constructor"]/*'/>*/
    internal MethodMetadata(String methodName , Int32 index , String description)
    {
        MethodName = methodName;
        Index = index;
        Description = description;
    }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="MethodMetadata"]/property[@name="MethodName"]/*'/>*/
    internal String MethodName { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="MethodMetadata"]/property[@name="Index"]/*'/>*/
    internal Int32 Index { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="MethodMetadata"]/property[@name="Description"]/*'/>*/
    internal String Description { get; }
}

/**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="OperationModel"]/main/*'/>*/
internal sealed class OperationModel
{
    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="OperationModel"]/constructor[@name="Constructor"]/*'/>*/
    internal OperationModel(String methodName , Int32 index , String description)
    {
        MethodName = methodName;
        Index = index;
        Description = description;
    }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="OperationModel"]/property[@name="MethodName"]/*'/>*/
    internal String MethodName { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="OperationModel"]/property[@name="Index"]/*'/>*/
    internal Int32 Index { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="OperationModel"]/property[@name="Description"]/*'/>*/
    internal String Description { get; }
}

/**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="HandwrittenManifestDescriptor"]/main/*'/>*/
internal sealed class HandwrittenManifestDescriptor
{
    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="HandwrittenManifestDescriptor"]/constructor[@name="Constructor"]/*'/>*/
    internal HandwrittenManifestDescriptor(String methodName , Int32 index , Location? methodNameLocation , Location? indexLocation)
    {
        MethodName = methodName;
        Index = index;
        MethodNameLocation = methodNameLocation;
        IndexLocation = indexLocation;
    }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="HandwrittenManifestDescriptor"]/property[@name="MethodName"]/*'/>*/
    internal String MethodName { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="HandwrittenManifestDescriptor"]/property[@name="Index"]/*'/>*/
    internal Int32 Index { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="HandwrittenManifestDescriptor"]/property[@name="MethodNameLocation"]/*'/>*/
    internal Location? MethodNameLocation { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="HandwrittenManifestDescriptor"]/property[@name="IndexLocation"]/*'/>*/
    internal Location? IndexLocation { get; }
}

/**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolModel"]/main/*'/>*/
internal sealed class ToolModel
{
    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolModel"]/constructor[@name="Constructor"]/*'/>*/
    internal ToolModel(
        String toolTypeName , String @namespace , String companionName , String companionQualifiedName , String toolTypeQualifiedName,
        String toolSchemaID , String? accessKeyRealmID , String toolSchemaVersion , ImmutableArray<OperationModel> operations)
    {
        ToolTypeName = toolTypeName;
        Namespace = @namespace;
        CompanionName = companionName;
        CompanionQualifiedName = companionQualifiedName;
        ToolTypeQualifiedName = toolTypeQualifiedName;
        ToolSchemaID = toolSchemaID;
        AccessKeyRealmID = accessKeyRealmID;
        ToolSchemaVersion = toolSchemaVersion;
        Operations = operations;
    }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolModel"]/property[@name="ToolTypeName"]/*'/>*/
    internal String ToolTypeName { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolModel"]/property[@name="Namespace"]/*'/>*/
    internal String Namespace { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolModel"]/property[@name="CompanionName"]/*'/>*/
    internal String CompanionName { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolModel"]/property[@name="CompanionQualifiedName"]/*'/>*/
    internal String CompanionQualifiedName { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolModel"]/property[@name="ToolTypeQualifiedName"]/*'/>*/
    internal String ToolTypeQualifiedName { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolModel"]/property[@name="ToolSchemaID"]/*'/>*/
    internal String ToolSchemaID { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolModel"]/property[@name="AccessKeyRealmID"]/*'/>*/
    internal String? AccessKeyRealmID { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolModel"]/property[@name="ToolSchemaVersion"]/*'/>*/
    internal String ToolSchemaVersion { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolModel"]/property[@name="Operations"]/*'/>*/
    internal ImmutableArray<OperationModel> Operations { get; }
}

/**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolManifestMetadata"]/main/*'/>*/
internal sealed class ToolManifestMetadata
{
    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolManifestMetadata"]/constructor[@name="Constructor"]/*'/>*/
    internal ToolManifestMetadata(String toolSchemaID , String? accessKeyRealmID , String toolSchemaVersion , String? companionNamespace , String? companionTypeName)
    {
        ToolSchemaID = toolSchemaID;
        AccessKeyRealmID = accessKeyRealmID;
        ToolSchemaVersion = toolSchemaVersion;
        CompanionNamespace = companionNamespace;
        CompanionTypeName = companionTypeName;
    }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolManifestMetadata"]/property[@name="ToolSchemaID"]/*'/>*/
    internal String ToolSchemaID { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolManifestMetadata"]/property[@name="AccessKeyRealmID"]/*'/>*/
    internal String? AccessKeyRealmID { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolManifestMetadata"]/property[@name="ToolSchemaVersion"]/*'/>*/
    internal String ToolSchemaVersion { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolManifestMetadata"]/property[@name="CompanionNamespace"]/*'/>*/
    internal String? CompanionNamespace { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="ToolManifestMetadata"]/property[@name="CompanionTypeName"]/*'/>*/
    internal String? CompanionTypeName { get; }
}

/**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="AssemblyManifestMetadata"]/main/*'/>*/
internal sealed class AssemblyManifestMetadata
{
    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="AssemblyManifestMetadata"]/constructor[@name="Constructor"]/*'/>*/
    internal AssemblyManifestMetadata(Boolean registerModuleInitializer , String? companionNamespace , String? companionTypeName)
    {
        RegisterModuleInitializer = registerModuleInitializer;
        CompanionNamespace = companionNamespace;
        CompanionTypeName = companionTypeName;
    }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="AssemblyManifestMetadata"]/property[@name="RegisterModuleInitializer"]/*'/>*/
    internal Boolean RegisterModuleInitializer { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="AssemblyManifestMetadata"]/property[@name="CompanionNamespace"]/*'/>*/
    internal String? CompanionNamespace { get; }

    /**<include file='ToolManifestModels.xml' path='ToolManifestModels/class[@name="AssemblyManifestMetadata"]/property[@name="CompanionTypeName"]/*'/>*/
    internal String? CompanionTypeName { get; }
}