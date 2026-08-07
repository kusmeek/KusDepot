namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public partial class ToolManifestGeneratorExam
{
    [Test]
    public void Generator_Produces_PerTool_Companion_With_SharedIndex_Descriptors()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;
                using System.Threading.Tasks;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/SharedPulse", AccessKeyRealmID = "Exam-Realm", ToolSchemaVersion = "1.0")]
                internal sealed class SharedPulse : Tool
                {
                    [ToolOperation(Description = "Reads the pulse.")]
                    [AccessCheck(4600)]
                    public string ReadPulse() => string.Empty;

                    [ToolOperation(Description = "Reads the pulse asynchronously.")]
                    [AccessCheck(4600)]
                    public Task<string> ReadPulseAsync() => Task.FromResult(string.Empty);
                }
                """,
            assemblyName: "ToolManifestGeneratorPerToolExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"SharedPulseToolManifest.g.cs");

        Check.That(toolSource).Contains("internal static partial class SharedPulseToolManifest");
        Check.That(toolSource).Contains("MethodName = nameof(global::ExamSpace.SharedPulse.ReadPulse)");
        Check.That(toolSource).Contains("MethodName = nameof(global::ExamSpace.SharedPulse.ReadPulseAsync)");
        Check.That(Generator_Produces_PerTool_Companion_With_SharedIndex_DescriptorsRegex().Count(toolSource)).IsEqualTo(2);
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [GeneratedRegex("Index = 4600")]
    private static partial Regex Generator_Produces_PerTool_Companion_With_SharedIndex_DescriptorsRegex();

    [Test]
    public void Generator_Uses_CompanionNamespace_For_Generated_Companion_And_Assembly_Bootstrap()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace.Tools;

                [ToolManifest(ToolSchemaID = "Exam/CompanionNamespace", CompanionNamespace = "ExamSpace.Generated")]
                internal sealed class CustomNamespaceTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(5310)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "CompanionNamespaceGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"CustomNamespaceToolToolManifest.g.cs");
        String assemblySource = GetGeneratedSource(result,"CompanionNamespaceGeneratorExamToolManifests.g.cs");

        Check.That(toolSource).Contains("namespace ExamSpace.Generated;");
        Check.That(toolSource).Contains("typeof(global::ExamSpace.Tools.CustomNamespaceTool).FullName");
        Check.That(toolSource).Contains("nameof(global::ExamSpace.Tools.CustomNamespaceTool.ReadValue)");
        Check.That(assemblySource).Contains("global::ExamSpace.Generated.CustomNamespaceToolToolManifest.RegisterAndBind();");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Uses_CompanionTypeName_And_CompanionNamespace_For_Generated_Companion()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace.Tools;

                [ToolManifest(ToolSchemaID = "Exam/CompanionTypeName", CompanionNamespace = "ExamSpace.Generated", CompanionTypeName = "CustomToolManifestCompanion")]
                internal sealed class CustomNamedTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(5311)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "CompanionTypeNameGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"CustomToolManifestCompanion.g.cs");
        String assemblySource = GetGeneratedSource(result,"CompanionTypeNameGeneratorExamToolManifests.g.cs");

        Check.That(toolSource).Contains("namespace ExamSpace.Generated;");
        Check.That(toolSource).Contains("internal static partial class CustomToolManifestCompanion");
        Check.That(toolSource).Contains("typeof(global::ExamSpace.Tools.CustomNamedTool).FullName");
        Check.That(toolSource).Contains("nameof(global::ExamSpace.Tools.CustomNamedTool.ReadValue)");
        Check.That(assemblySource).Contains("global::ExamSpace.Generated.CustomToolManifestCompanion.RegisterAndBind();");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Produces_Companion_For_Tool_Derived_From_ToolHost()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/DerivedToolHost")]
                internal sealed class DerivedToolHost : ToolHost
                {
                    [ToolOperation(Description = "Reads a host value.")]
                    [AccessCheck(5301)]
                    public string ReadHostValue() => string.Empty;
                }
                """,
            assemblyName: "DerivedToolHostGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"DerivedToolHostToolManifest.g.cs");

        Check.That(toolSource).Contains("internal static partial class DerivedToolHostToolManifest");
        Check.That(toolSource).Contains("MethodName = nameof(global::ExamSpace.DerivedToolHost.ReadHostValue)");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Produces_Companion_For_Tool_Derived_From_ToolGenericHost()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/DerivedToolGenericHost")]
                internal sealed class DerivedToolGenericHost : ToolGenericHost
                {
                    [ToolOperation(Description = "Reads a generic host value.")]
                    [AccessCheck(5302)]
                    public string ReadGenericHostValue() => string.Empty;
                }
                """,
            assemblyName: "DerivedToolGenericHostGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"DerivedToolGenericHostToolManifest.g.cs");

        Check.That(toolSource).Contains("internal static partial class DerivedToolGenericHostToolManifest");
        Check.That(toolSource).Contains("MethodName = nameof(global::ExamSpace.DerivedToolGenericHost.ReadGenericHostValue)");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Produces_Companion_For_Tool_Derived_From_ToolWebHost()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/DerivedToolWebHost")]
                internal sealed class DerivedToolWebHost : ToolWebHost
                {
                    [ToolOperation(Description = "Reads a web host value.")]
                    [AccessCheck(5303)]
                    public string ReadWebHostValue() => string.Empty;
                }
                """,
            assemblyName: "DerivedToolWebHostGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"DerivedToolWebHostToolManifest.g.cs");

        Check.That(toolSource).Contains("internal static partial class DerivedToolWebHostToolManifest");
        Check.That(toolSource).Contains("MethodName = nameof(global::ExamSpace.DerivedToolWebHost.ReadWebHostValue)");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Produces_Companion_For_Tool_Derived_From_ToolAspireHost()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/DerivedToolAspireHost")]
                internal sealed class DerivedToolAspireHost : ToolAspireHost
                {
                    [ToolOperation(Description = "Reads an Aspire host value.")]
                    [AccessCheck(5304)]
                    public string ReadAspireHostValue() => string.Empty;
                }
                """,
            assemblyName: "DerivedToolAspireHostGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"DerivedToolAspireHostToolManifest.g.cs");

        Check.That(toolSource).Contains("internal static partial class DerivedToolAspireHostToolManifest");
        Check.That(toolSource).Contains("MethodName = nameof(global::ExamSpace.DerivedToolAspireHost.ReadAspireHostValue)");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Produces_Assembly_Bootstrap_For_Multiple_Tools()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/Alpha")]
                internal sealed class Alpha : Tool
                {
                    [ToolOperation(Description = "Reads alpha.")]
                    [AccessCheck(5001)]
                    public string ReadAlpha() => string.Empty;
                }

                [ToolManifest(ToolSchemaID = "Exam/Beta")]
                internal sealed class Beta : Tool
                {
                    [ToolOperation(Description = "Reads beta.")]
                    [AccessCheck(5002)]
                    public string ReadBeta() => string.Empty;
                }
                """,
            assemblyName: "MultiToolGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String assemblySource = GetGeneratedSource(result,"MultiToolGeneratorExamToolManifests.g.cs");

        Check.That(assemblySource).Contains("internal static partial class MultiToolGeneratorExamToolManifests");
        Check.That(assemblySource).Contains("global::ExamSpace.AlphaToolManifest.Register();");
        Check.That(assemblySource).Contains("global::ExamSpace.BetaToolManifest.RegisterAndBind();");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Emits_ModuleInitializer_When_Assembly_OptIn_Is_Enabled()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                [assembly: ToolManifestAssembly(RegisterModuleInitializer = true)]

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/ModuleInit")]
                internal sealed class ModuleInit : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(5100)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "ModuleInitGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String assemblySource = GetGeneratedSource(result,"ModuleInitGeneratorExamToolManifests.g.cs");

        Check.That(assemblySource).Contains("ModuleInitializer");
        Check.That(assemblySource).Contains("internal static void InitializeModule()");
        Check.That(assemblySource).Contains("RegisterAndBindAll();");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Uses_Assembly_CompanionNamespace_And_CompanionTypeName_For_Assembly_Bootstrap()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                [assembly: ToolManifestAssembly(CompanionNamespace = "ExamSpace.AssemblyGenerated", CompanionTypeName = "AssemblyBootstrap")]

                namespace ExamSpace.Tools;

                [ToolManifest(ToolSchemaID = "Exam/AssemblyOverride")]
                internal sealed class AssemblyOverrideTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(5101)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "AssemblyCompanionOverrideGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"AssemblyOverrideToolToolManifest.g.cs");
        String assemblySource = GetGeneratedSource(result,"AssemblyCompanionOverrideGeneratorExamToolManifests.g.cs");

        Check.That(toolSource).Contains("namespace ExamSpace.Tools;");
        Check.That(assemblySource).Contains("namespace ExamSpace.AssemblyGenerated;");
        Check.That(assemblySource).Contains("internal static partial class AssemblyBootstrap");
        Check.That(assemblySource).Contains("global::ExamSpace.Tools.AssemblyOverrideToolToolManifest.RegisterAndBind();");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Reports_InvalidToolType_Diagnostic()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;
                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/Invalid")]
                internal sealed class InvalidTool
                {
                }
                """,
            assemblyName: "InvalidToolGeneratorExam",
            out _);

        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT001");
    }

    [Test]
    public void Generator_Reports_GenericTool_Diagnostic()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;
                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/Generic")]
                internal sealed class GenericTool<T> : Tool
                {
                }
                """,
            assemblyName: "GenericToolGeneratorExam",
            out _);

        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT002");
    }

    [Test]
    public void Generator_Reports_Ambiguous_Method_Name_Diagnostic()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                internal abstract class BasePulseTool : Tool
                {
                    [ToolOperation(Description = "Reads a pulse.")]
                    [AccessCheck(6001)]
                    public string ReadPulse() => string.Empty;
                }

                [ToolManifest(ToolSchemaID = "Exam/Ambiguous")]
                internal sealed class AmbiguousPulseTool : BasePulseTool
                {
                    [ToolOperation(Description = "Reads a different pulse.")]
                    [AccessCheck(6002)]
                    public new string ReadPulse() => string.Empty;
                }
                """,
            assemblyName: "AmbiguousToolGeneratorExam",
            out _);

        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT003");
    }

    [Test]
    public void Generator_Reports_Custom_Operation_Index_Out_Of_Range_Diagnostic()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/OutOfRange")]
                internal sealed class OutOfRangeTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(255)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "OutOfRangeToolGeneratorExam",
            out _);

        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT004");
    }

    [Test]
    public void Generator_Reports_Missing_ToolSchemaID_Diagnostic()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest]
                internal sealed class MissingSchemaTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(5001)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "MissingSchemaToolGeneratorExam",
            out _);

        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT005");
    }

    [Test]
    public void Generator_Reports_Sync_Async_Semantic_Drift_Diagnostic()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;
                using System.Threading.Tasks;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/Drift")]
                internal sealed class DriftTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(6001)]
                    public string ReadValue() => string.Empty;

                    [ToolOperation(Description = "Reads a value asynchronously.")]
                    [AccessCheck(6002)]
                    public Task<string> ReadValueAsync() => Task.FromResult(string.Empty);
                }
                """,
            assemblyName: "DriftToolGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT006");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Reports_Handwritten_Manifest_Index_Mismatch_Diagnostic()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;
                using KusDepot.Security;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/ManualDirect")]
                internal sealed class ManualDirectTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(7001)]
                    public string ReadValue() => string.Empty;

                    public override ToolManifest? GetToolManifest(AccessKey? key = null)
                    {
                        return ToolManifest.Create(
                            "Exam/ManualDirect",
                            null,
                            "1.0",
                            typeof(ManualDirectTool).FullName,
                            [
                                new ToolOperationDescriptor { Index = 7002, MethodName = nameof(ReadValue), Description = "Reads a value." }
                            ]).ComputeManifestHash();
                    }
                }
                """,
            assemblyName: "ManualDirectToolGeneratorExam",
            out _);

        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT007");
    }

    [Test]
    public void Generator_Reports_Handwritten_Manifest_Method_Name_Mismatch_Diagnostic()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;
                using KusDepot.Security;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/ManualHelper")]
                internal sealed class ManualHelperTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(7101)]
                    public string ReadValue() => string.Empty;

                    public override ToolManifest? GetToolManifest(AccessKey? key = null) => CreateManualManifest();

                    private static ToolManifest CreateManualManifest()
                    {
                        return ToolManifest.Create(
                            "Exam/ManualHelper",
                            null,
                            "1.0",
                            typeof(ManualHelperTool).FullName,
                            [
                                new ToolOperationDescriptor { Index = 7101, MethodName = "ReadValueMissing", Description = "Reads a value." }
                            ]).ComputeManifestHash();
                    }
                }
                """,
            assemblyName: "ManualHelperToolGeneratorExam",
            out _);

        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT008");
    }

    [Test]
    public void Generator_Reports_Invalid_CompanionNamespace_Diagnostic()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/BadNamespace", CompanionNamespace = "ExamSpace..Generated")]
                internal sealed class InvalidCompanionNamespaceTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(5320)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "InvalidCompanionNamespaceGeneratorExam",
            out _);

        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT009");
    }

    [Test]
    public void Generator_Reports_Invalid_CompanionTypeName_Diagnostic()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/BadCompanionTypeName", CompanionTypeName = "Bad Companion Name")]
                internal sealed class InvalidCompanionTypeNameTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(5322)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "InvalidCompanionTypeNameGeneratorExam",
            out _);

        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT011");
    }

    [Test]
    public void Generator_Reports_Invalid_Assembly_CompanionNamespace_Diagnostic()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                [assembly: ToolManifestAssembly(CompanionNamespace = "ExamSpace..AssemblyGenerated")]

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/BadAssemblyNamespace")]
                internal sealed class InvalidAssemblyCompanionNamespaceTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(5321)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "InvalidAssemblyCompanionNamespaceGeneratorExam",
            out _);

        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT010");
    }

    [Test]
    public void Generator_Supports_Handwritten_Manifest_Companion_Helper_Pattern()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;
                using KusDepot.Security;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/ManualCompanion")]
                internal sealed class ManualCompanionTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(7201)]
                    public string ReadValue() => string.Empty;

                    public override ToolManifest? GetToolManifest(AccessKey? key = null) => ManualCompanionManifest.CreateManifest();
                }

                internal static class ManualCompanionManifest
                {
                    public static ToolManifest CreateManifest()
                    {
                        return ToolManifest.Create(
                            "Exam/ManualCompanion",
                            null,
                            "1.0",
                            typeof(ManualCompanionTool).FullName,
                            [
                                new ToolOperationDescriptor { Index = 7201, MethodName = nameof(ManualCompanionTool.ReadValue), Description = "Reads a value." }
                            ]).ComputeManifestHash();
                    }
                }
                """,
            assemblyName: "ManualCompanionToolGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);
        CheckCompilationHasNoErrors(outputCompilation);
    }

    private static void CheckCompilationHasNoErrors(Compilation compilation)
    {
        String[] errors = compilation.GetDiagnostics()
            .Where(static _ => _.Severity == DiagnosticSeverity.Error)
            .Select(static _ => _.ToString())
            .ToArray();

        Check.That(errors).IsEmpty();
    }

    private static CSharpCompilation CreateCompilation(String source , String assemblyName)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source,new CSharpParseOptions(LanguageVersion.Preview));

        HashSet<String> paths = ((String?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))?
            .Split(Path.PathSeparator,StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? [];

        foreach(String path in new[]
        {
            typeof(Object).Assembly.Location,
            typeof(Enumerable).Assembly.Location,
            typeof(Task).Assembly.Location,
            typeof(Tool).Assembly.Location,
            typeof(ToolManifestAttribute).Assembly.Location,
            typeof(ToolManifestGenerator).Assembly.Location
        })
        {
            if(String.IsNullOrWhiteSpace(path) is false)
            {
                _ = paths.Add(path);
            }
        }

        return CSharpCompilation.Create(
            assemblyName,
            [syntaxTree],
            paths.Where(File.Exists).Select(static _ => MetadataReference.CreateFromFile(_)),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,nullableContextOptions: NullableContextOptions.Enable));
    }

    private static String GetGeneratedSource(GeneratorRunResult result , String hintName)
    {
        String? source = result.GeneratedSources
            .Where(_ => String.Equals(_.HintName,hintName,StringComparison.Ordinal))
            .Select(_ => _.SourceText.ToString())
            .SingleOrDefault();

        Check.That(source).IsNotNull();
        return source!;
    }

    private static GeneratorRunResult RunGenerator(String source , String assemblyName , out Compilation outputCompilation)
    {
        CSharpCompilation compilation = CreateCompilation(source,assemblyName);
        IIncrementalGenerator generator = new ToolManifestGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            parseOptions: (CSharpParseOptions)compilation.SyntaxTrees.First().Options);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation,out outputCompilation,out ImmutableArray<Diagnostic> diagnostics);

        GeneratorDriverRunResult runResult = driver.GetRunResult();
        Check.That(runResult.Results.Length).IsEqualTo(1);
        return runResult.Results[0];
    }

}
