namespace KusDepot.ToolManifests.Analysis;

/**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisService"]/main/*'/>*/
internal static class ToolManifestAnalysisService
{
    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisService"]/method[@name="AnalyzeToolType"]/*'/>*/
    internal static ToolManifestAnalysisResult AnalyzeToolType(Compilation compilation , INamedTypeSymbol toolType)
    {
        List<ToolManifestAnalysisDiagnostic> diagnostics = [];

        if(toolType.IsGenericType)
        {
            diagnostics.Add(CreateDiagnostic(ToolManifestDiagnostics.UnsupportedGenericToolRule,toolType.Locations.FirstOrDefault(),toolType.ToDisplayString()));

            return new(null,diagnostics);
        }

        if(ToolManifestSymbolAnalysis.InheritsFromTool(toolType) is false)
        {
            diagnostics.Add(CreateDiagnostic(ToolManifestDiagnostics.InvalidToolTypeRule,toolType.Locations.FirstOrDefault(),toolType.ToDisplayString()));

            return new(null,diagnostics);
        }

        ToolManifestMetadata? metadata = ToolManifestSymbolAnalysis.CreateToolManifestMetadata(toolType,diagnostics);

        if(metadata is null)
        {
            return new(null,diagnostics);
        }

        List<(IMethodSymbol Method,MethodMetadata Metadata)> methodEntries = [];

        foreach(IMethodSymbol method in ToolManifestSymbolAnalysis.GetToolOperationMethods(toolType))
        {
            MethodMetadata? methodMetadata = ToolManifestSymbolAnalysis.CreateMethodMetadata(method); if(methodMetadata is null) { continue; }

            if(methodMetadata.Index < ToolManifestConstants.CustomOperationIndexMin || methodMetadata.Index > ToolManifestConstants.CustomOperationIndexMax)
            {
                diagnostics.Add(CreateDiagnostic(ToolManifestDiagnostics.CustomOperationIndexOutOfRangeRule,method.Locations.FirstOrDefault(),toolType.ToDisplayString(),methodMetadata.Index,method.Name));

                return new(null,diagnostics);
            }

            methodEntries.Add((method,methodMetadata));
        }

        List<MethodMetadata> methods = methodEntries.Select(static _ => _.Metadata).ToList();

        foreach(IGrouping<String,MethodMetadata> group in methods.GroupBy(_ => _.MethodName,StringComparer.Ordinal))
        {
            if(group.Select(_ => _.Index).Distinct().Skip(1).Any())
            {
                diagnostics.Add(CreateDiagnostic(ToolManifestDiagnostics.AmbiguousMethodNameRule,toolType.Locations.FirstOrDefault(),toolType.ToDisplayString(),group.Key));

                return new(null,diagnostics);
            }
        }

        Dictionary<String,(IMethodSymbol Method,MethodMetadata Metadata)> methodsByName = methodEntries
            .GroupBy(static _ => _.Method.Name,StringComparer.Ordinal)
            .ToDictionary(static _ => _.Key,static _ => _.First(),StringComparer.Ordinal);

        foreach((IMethodSymbol method,MethodMetadata methodMetadata) in methodEntries)
        {
            if(ToolManifestSymbolAnalysis.IsAsyncOperationMethod(method) is false) { continue; }

            String syncMethodName = method.Name.Substring(0,method.Name.Length - "Async".Length);

            if(String.IsNullOrEmpty(syncMethodName) || methodsByName.TryGetValue(syncMethodName,out (IMethodSymbol Method,MethodMetadata Metadata) syncMethod) is false) { continue; }
            if(syncMethod.Metadata.Index == methodMetadata.Index) { continue; }

            diagnostics.Add(CreateDiagnostic(
                ToolManifestDiagnostics.SyncAsyncSemanticDriftRule,
                method.Locations.FirstOrDefault(),
                toolType.ToDisplayString(),
                syncMethod.Metadata.MethodName,
                methodMetadata.MethodName,
                syncMethod.Metadata.Index,
                methodMetadata.Index));
        }

        ToolManifestManualManifestAnalysis.ValidateHandwrittenManifestDescriptors(
            compilation,
            toolType,
            methodsByName.ToDictionary(static _ => _.Key,static _ => _.Value.Metadata,StringComparer.Ordinal),
            diagnostics);

        ImmutableArray<OperationModel> operations = methods
            .GroupBy(_ => new { _.MethodName,_.Index,_.Description })
            .Select(_ => new OperationModel(_.Key.MethodName,_.Key.Index,_.Key.Description))
            .OrderBy(_ => _.Index)
            .ThenBy(_ => _.MethodName,StringComparer.Ordinal)
            .ToImmutableArray();

        ToolModel model = new(
            toolType.Name,
            String.IsNullOrEmpty(metadata.CompanionNamespace)
                ? toolType.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : String.Empty
                : metadata.CompanionNamespace!,
            ToolManifestNaming.GetCompanionName(metadata.CompanionTypeName,toolType),
            ToolManifestNaming.GetCompanionQualifiedName(metadata.CompanionNamespace,metadata.CompanionTypeName,toolType),
            toolType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            metadata.ToolSchemaID,
            metadata.AccessKeyRealmID,
            metadata.ToolSchemaVersion,
            operations);

        return new(model,diagnostics);
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisService"]/method[@name="CreateDiagnostic"]/*'/>*/
    private static ToolManifestAnalysisDiagnostic CreateDiagnostic(DiagnosticDescriptor rule , Location? location , params Object?[] args)
    {
        return new(rule,location,args);
    }
}
