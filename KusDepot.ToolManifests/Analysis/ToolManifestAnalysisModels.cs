namespace KusDepot.ToolManifests.Analysis;

/**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisDiagnostic"]/main/*'/>*/
internal sealed class ToolManifestAnalysisDiagnostic
{
    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisDiagnostic"]/constructor[@name="Constructor"]/*'/>*/
    internal ToolManifestAnalysisDiagnostic(DiagnosticDescriptor rule , Location? location , params Object?[] messageArgs)
    {
        Rule = rule;
        Location = location;
        MessageArgs = messageArgs;
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisDiagnostic"]/property[@name="Rule"]/*'/>*/
    internal DiagnosticDescriptor Rule { get; }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisDiagnostic"]/property[@name="Location"]/*'/>*/
    internal Location? Location { get; }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisDiagnostic"]/property[@name="MessageArgs"]/*'/>*/
    internal Object?[] MessageArgs { get; }
}

/**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisResult"]/main/*'/>*/
internal sealed class ToolManifestAnalysisResult
{
    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisResult"]/constructor[@name="Constructor"]/*'/>*/
    internal ToolManifestAnalysisResult(ToolModel? model , IEnumerable<ToolManifestAnalysisDiagnostic>? diagnostics = null)
    {
        Model = model;
        Diagnostics = diagnostics?.ToImmutableArray() ?? [];
    }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisResult"]/property[@name="Model"]/*'/>*/
    internal ToolModel? Model { get; }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisResult"]/property[@name="Diagnostics"]/*'/>*/
    internal ImmutableArray<ToolManifestAnalysisDiagnostic> Diagnostics { get; }

    /**<include file='ToolManifestAnalysis.xml' path='ToolManifestAnalysis/class[@name="ToolManifestAnalysisResult"]/property[@name="CanGenerate"]/*'/>*/
    internal Boolean CanGenerate => Model is not null && Diagnostics.All(static _ => _.Rule.DefaultSeverity != DiagnosticSeverity.Error);
}
