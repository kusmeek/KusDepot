namespace KusDepot.ToolManifests.Analyzers;

/**<include file='ToolManifestAnalyzer.xml' path='ToolManifestAnalyzer/class[@name="ToolManifestAnalyzer"]/main/*'/>*/
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ToolManifestAnalyzer : DiagnosticAnalyzer
{
    /**<include file='ToolManifestAnalyzer.xml' path='ToolManifestAnalyzer/class[@name="ToolManifestAnalyzer"]/property[@name="SupportedDiagnostics"]/*'/>*/
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ToolManifestDiagnostics.All;

    /**<include file='ToolManifestAnalyzer.xml' path='ToolManifestAnalyzer/class[@name="ToolManifestAnalyzer"]/method[@name="Initialize"]/*'/>*/
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(InitializeCompilation);
    }

    /**<include file='ToolManifestAnalyzer.xml' path='ToolManifestAnalyzer/class[@name="ToolManifestAnalyzer"]/method[@name="InitializeCompilation"]/*'/>*/
    private static void InitializeCompilation(CompilationStartAnalysisContext context)
    {
        Compilation compilation = context.Compilation;

        context.RegisterSymbolAction(
            symbolContext => AnalyzeNamedType(symbolContext,compilation),
            SymbolKind.NamedType);
    }

    /**<include file='ToolManifestAnalyzer.xml' path='ToolManifestAnalyzer/class[@name="ToolManifestAnalyzer"]/method[@name="AnalyzeNamedType"]/*'/>*/
    private static void AnalyzeNamedType(SymbolAnalysisContext context , Compilation compilation)
    {
        if(context.Symbol is not INamedTypeSymbol toolType || toolType.TypeKind is not TypeKind.Class)
        {
            return;
        }

        if(toolType.GetAttributes().Any(static _ => _.AttributeClass?.ToDisplayString() == ToolManifestConstants.ToolManifestAttributeFullName) is false)
        {
            return;
        }

        ToolManifestAnalysisResult analysis = ToolManifestAnalysisService.AnalyzeToolType(compilation,toolType);

        foreach(ToolManifestAnalysisDiagnostic diagnostic in analysis.Diagnostics)
        {
            context.ReportDiagnostic(Diagnostic.Create(diagnostic.Rule,diagnostic.Location,diagnostic.MessageArgs));
        }
    }
}
