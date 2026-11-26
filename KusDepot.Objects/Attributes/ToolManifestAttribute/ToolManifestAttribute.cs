namespace KusDepot;

/**<include file='ToolManifestAttribute.xml' path='ToolManifestAttribute/class[@name="ToolManifestAttribute"]/main/*'/>*/
[AttributeUsage(AttributeTargets.Class , AllowMultiple = false , Inherited = false)]
public sealed class ToolManifestAttribute : Attribute
{
    /**<include file='ToolManifestAttribute.xml' path='ToolManifestAttribute/class[@name="ToolManifestAttribute"]/property[@name="ToolSchemaID"]/*'/>*/
    public String? ToolSchemaID { get; init; }

    /**<include file='ToolManifestAttribute.xml' path='ToolManifestAttribute/class[@name="ToolManifestAttribute"]/property[@name="AccessKeyRealmID"]/*'/>*/
    public String? AccessKeyRealmID { get; init; }

    /**<include file='ToolManifestAttribute.xml' path='ToolManifestAttribute/class[@name="ToolManifestAttribute"]/property[@name="ToolSchemaVersion"]/*'/>*/
    public String? ToolSchemaVersion { get; init; }

    /**<include file='ToolManifestAttribute.xml' path='ToolManifestAttribute/class[@name="ToolManifestAttribute"]/property[@name="CompanionNamespace"]/*'/>*/
    public String? CompanionNamespace { get; init; }

    /**<include file='ToolManifestAttribute.xml' path='ToolManifestAttribute/class[@name="ToolManifestAttribute"]/property[@name="CompanionTypeName"]/*'/>*/
    public String? CompanionTypeName { get; init; }
}