namespace KusDepot.ToolManifests.Analyzers;

/**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/main/*'/>*/
[Generator(LanguageNames.CSharp)]
public sealed partial class ToolManifestGenerator : IIncrementalGenerator
{
    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="Initialize"]/*'/>*/
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<INamedTypeSymbol?> toolTypes = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node,_) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (syntaxContext,_) => ToolManifestSymbolAnalysis.GetToolTypeForGeneration(syntaxContext))
            .Where(static symbol => symbol is not null);

        IncrementalValueProvider<(Compilation compilation , ImmutableArray<INamedTypeSymbol?> tools)> compilationAndTools = context.CompilationProvider.Combine(toolTypes.Collect());

        context.RegisterSourceOutput(compilationAndTools,static (sourceContext,source) => Execute(source.compilation,source.tools,sourceContext));
    }

    /**<include file='ToolManifestGenerator.xml' path='ToolManifestGenerator/class[@name="ToolManifestGenerator"]/method[@name="Execute"]/*'/>*/
    private static void Execute(Compilation compilation , ImmutableArray<INamedTypeSymbol?> toolTypes , SourceProductionContext context)
    {
        if(toolTypes.IsDefaultOrEmpty) { return; }

        List<ToolManifestAnalysisDiagnostic> assemblyDiagnostics = [];

        AssemblyManifestMetadata assemblyMetadata = ToolManifestSymbolAnalysis.CreateAssemblyManifestMetadata(compilation.Assembly,assemblyDiagnostics);

        foreach(ToolManifestAnalysisDiagnostic diagnostic in assemblyDiagnostics)
        {
            context.ReportDiagnostic(Diagnostic.Create(diagnostic.Rule,diagnostic.Location,diagnostic.MessageArgs));
        }

        List<ToolModel> models = [];

        foreach(INamedTypeSymbol toolType in toolTypes.Distinct(SymbolEqualityComparer.Default).OfType<INamedTypeSymbol>())
        {
            ToolManifestAnalysisResult analysis = ToolManifestAnalysisService.AnalyzeToolType(compilation,toolType);

            foreach(ToolManifestAnalysisDiagnostic diagnostic in analysis.Diagnostics)
            {
                context.ReportDiagnostic(Diagnostic.Create(diagnostic.Rule,diagnostic.Location,diagnostic.MessageArgs));
            }

            if(analysis.CanGenerate is false || analysis.Model is null)
            {
                continue;
            }

            ToolModel model = analysis.Model;

            if(model is not null)
            {
                models.Add(model);

            }
        }

        if(models.Count == 0) { return; }

        HashSet<ToolModel> duplicateModels = new(
            models
                .GroupBy(static _ => _.ToolSchemaID,StringComparer.Ordinal)
                .Where(static _ => _.Skip(1).Any())
                .SelectMany(static _ => _));

        foreach(ToolModel model in duplicateModels)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                ToolManifestDiagnostics.DuplicateToolSchemaIDRule,
                model.ToolLocation,
                model.ToolTypeQualifiedName,
                model.ToolSchemaID));
        }

        models = models.Where(model => duplicateModels.Contains(model) is false).ToList();

        if(models.Count == 0) { return; }

        ResolveNestedCompanionNameCollisions(models);

        Boolean emitAssemblyBootstrap = ValidateCompanionIdentities(compilation,assemblyMetadata,models,context);

        String assemblyHintName = $"{SanitizeIdentifier(compilation.AssemblyName)}ToolManifests.g.cs";

        List<ToolModel> orderedModels = models
            .OrderBy(_ => _.CompanionQualifiedName,StringComparer.Ordinal)
            .ThenBy(_ => _.ToolTypeQualifiedName,StringComparer.Ordinal)
            .ToList();

        HashSet<ToolModel> primaryHintModels = [];

        foreach(IGrouping<String,ToolModel> group in orderedModels.GroupBy(_ => _.CompanionName,StringComparer.Ordinal))
        {
            ToolModel firstModel = group.First();

            String primaryHintName = $"{firstModel.CompanionName}.g.cs";

            if(String.Equals(primaryHintName,assemblyHintName,StringComparison.Ordinal) is false)
            {
                _ = primaryHintModels.Add(firstModel);
            }
        }

        HashSet<String> hintNames = new(StringComparer.Ordinal) { assemblyHintName };

        foreach(ToolModel model in primaryHintModels)
        {
            _ = hintNames.Add($"{model.CompanionName}.g.cs");
        }

        foreach(ToolModel model in orderedModels)
        {
            String hintName;

            if(primaryHintModels.Contains(model))
            {
                hintName = $"{model.CompanionName}.g.cs";
            }
            else
            {
                String fallbackBase = SanitizeIdentifier($"{model.Namespace}_{model.CompanionName}_{model.ToolTypeQualifiedName}");

                hintName = $"{fallbackBase}.g.cs";

                Int32 suffix = 2;

                while(hintNames.Add(hintName) is false)
                {
                    hintName = $"{fallbackBase}_{suffix++}.g.cs";
                }
            }

            context.AddSource(hintName,SourceText.From(GenerateToolSource(model),Encoding.UTF8));
        }

        if(emitAssemblyBootstrap)
        {
            context.AddSource(assemblyHintName,SourceText.From(GenerateAssemblySource(compilation,models,assemblyMetadata),Encoding.UTF8));
        }
    }
}