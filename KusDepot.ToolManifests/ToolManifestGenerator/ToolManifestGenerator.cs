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

                context.AddSource($"{model.CompanionName}.g.cs",SourceText.From(GenerateToolSource(model),Encoding.UTF8));
            }
        }

        if(models.Count == 0) { return; }

        context.AddSource($"{SanitizeIdentifier(compilation.AssemblyName)}ToolManifests.g.cs",SourceText.From(GenerateAssemblySource(compilation,models,assemblyMetadata),Encoding.UTF8));
    }
}