namespace KusDepot;

/**<include file='ToolManifestAssemblyAttribute.xml' path='ToolManifestAssemblyAttribute/class[@name="ToolManifestAssemblyAttribute"]/main/*'/>*/
[AttributeUsage(AttributeTargets.Assembly , AllowMultiple = false)]
public sealed class ToolManifestAssemblyAttribute : Attribute
{
    /**<include file='ToolManifestAssemblyAttribute.xml' path='ToolManifestAssemblyAttribute/class[@name="ToolManifestAssemblyAttribute"]/property[@name="RegisterModuleInitializer"]/*'/>*/
    public Boolean RegisterModuleInitializer { get; init; }

    /**<include file='ToolManifestAssemblyAttribute.xml' path='ToolManifestAssemblyAttribute/class[@name="ToolManifestAssemblyAttribute"]/property[@name="CompanionNamespace"]/*'/>*/
    public String? CompanionNamespace { get; init; }

    /**<include file='ToolManifestAssemblyAttribute.xml' path='ToolManifestAssemblyAttribute/class[@name="ToolManifestAssemblyAttribute"]/property[@name="CompanionTypeName"]/*'/>*/
    public String? CompanionTypeName { get; init; }
}