namespace KusDepot.ToolManifests.Analysis;

/**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestNaming"]/main/*'/>*/
internal static class ToolManifestNaming
{
    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestNaming"]/method[@name="GetCompanionQualifiedName"]/*'/>*/
    internal static String GetCompanionQualifiedName(String? companionNamespace , String? companionTypeName , INamedTypeSymbol toolType)
    {
        String companionName = GetCompanionName(companionTypeName,toolType);
        String? resolvedNamespace = String.IsNullOrEmpty(companionNamespace)
            ? toolType.ContainingNamespace?.ToDisplayString() ?? String.Empty
            : companionNamespace;

        return String.IsNullOrEmpty(resolvedNamespace) ? companionName : $"global::{resolvedNamespace}.{companionName}";
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestNaming"]/method[@name="GetCompanionName"]/*'/>*/
    internal static String GetCompanionName(String? companionTypeName , INamedTypeSymbol toolType)
        => String.IsNullOrWhiteSpace(companionTypeName) ? $"{toolType.Name}ToolManifest" : companionTypeName!;
}