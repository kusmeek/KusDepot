namespace KusDepot.ToolManifests.Analysis;

/**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestNaming"]/main/*'/>*/
internal static class ToolManifestNaming
{
    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestNaming"]/method[@name="GetCompanionQualifiedName"]/*'/>*/
    internal static String GetCompanionQualifiedName(String? companionNamespace , String? companionTypeName , INamedTypeSymbol toolType)
    {
        String companionName = GetCompanionName(companionTypeName,toolType);

        String? resolvedNamespace = String.IsNullOrEmpty(companionNamespace)
            ? toolType.ContainingNamespace is { IsGlobalNamespace: false } toolNamespace ? toolNamespace.ToDisplayString() : String.Empty
            : companionNamespace;

        return String.IsNullOrEmpty(resolvedNamespace) ? $"global::{companionName}" : $"global::{resolvedNamespace}.{companionName}";
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestNaming"]/method[@name="GetCompanionName"]/*'/>*/
    internal static String GetCompanionName(String? companionTypeName , INamedTypeSymbol toolType)
        => String.IsNullOrWhiteSpace(companionTypeName) ? $"{toolType.Name}ToolManifest" : companionTypeName!;

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestNaming"]/method[@name="GetContainingTypeName"]/*'/>*/
    internal static String? GetContainingTypeName(INamedTypeSymbol toolType)
    {
        Stack<String> containingTypeNames = new();

        for(INamedTypeSymbol? containingType = toolType.ContainingType; containingType is not null; containingType = containingType.ContainingType)
        {
            containingTypeNames.Push(containingType.Name);
        }

        return containingTypeNames.Count == 0 ? null : String.Join(".",containingTypeNames);
    }
}