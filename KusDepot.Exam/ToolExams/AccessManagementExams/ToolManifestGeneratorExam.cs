namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public partial class ToolManifestGeneratorExam
{
    [Test]
    public void Generator_Reports_Duplicate_ToolSchemaID_And_Suppresses_Duplicates()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace.First
                {
                    [ToolManifest(ToolSchemaID = "Exam/Duplicate")]
                    internal sealed class FirstDuplicateTool : Tool
                    {
                        [ToolOperation]
                        [AccessCheck(5400)]
                        public string ReadValue() => string.Empty;
                    }
                }

                namespace ExamSpace.Second
                {
                    [ToolManifest(ToolSchemaID = "Exam/Duplicate")]
                    internal sealed class SecondDuplicateTool : Tool
                    {
                        [ToolOperation]
                        [AccessCheck(5401)]
                        public string ReadValue() => string.Empty;
                    }
                }

                namespace ExamSpace.Unique
                {
                    [ToolManifest(ToolSchemaID = "Exam/Unique")]
                    internal sealed class UniqueTool : Tool
                    {
                        [ToolOperation]
                        [AccessCheck(5402)]
                        public string ReadValue() => string.Empty;
                    }
                }
                """,
            assemblyName: "DuplicateSchemaGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Select(static _ => _.Id).Where(static _ => _ == "KDT012").Count()).IsEqualTo(2);
        Check.That(result.GeneratedSources.Any(_ => _.HintName.Contains("FirstDuplicateTool"))).IsFalse();
        Check.That(result.GeneratedSources.Any(_ => _.HintName.Contains("SecondDuplicateTool"))).IsFalse();
        Check.That(result.GeneratedSources.Any(_ => _.SourceText.ToString().Contains("UniqueToolToolManifest"))).IsTrue();
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Uses_Unique_Hints_When_Sanitized_Fallbacks_Collide()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace.A
                {
                    [ToolManifest(ToolSchemaID = "Exam/HintCollision/First")]
                    internal sealed class Same : Tool
                    {
                        [ToolOperation]
                        [AccessCheck(5409)]
                        public string ReadValue() => string.Empty;
                    }
                }

                namespace ExamSpace.B.C
                {
                    [ToolManifest(ToolSchemaID = "Exam/HintCollision/Second")]
                    internal sealed class Same : Tool
                    {
                        [ToolOperation]
                        [AccessCheck(5410)]
                        public string ReadValue() => string.Empty;
                    }
                }

                namespace ExamSpace.BC
                {
                    [ToolManifest(ToolSchemaID = "Exam/HintCollision/Third")]
                    internal sealed class Same : Tool
                    {
                        [ToolOperation]
                        [AccessCheck(5411)]
                        public string ReadValue() => string.Empty;
                    }
                }
                """,
            assemblyName: "HintFallbackCollisionExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);
        Check.That(result.Exception).IsNull();
        Check.That(result.GeneratedSources.Length).IsEqualTo(4);
        Check.That(result.GeneratedSources.Select(static _ => _.HintName).Distinct(StringComparer.Ordinal).Count()).IsEqualTo(4);
        Check.That(result.GeneratedSources.Count(_ => _.SourceText.ToString().Contains("partial class SameToolManifest"))).IsEqualTo(3);
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Reserves_Assembly_Bootstrap_Hint_From_PerTool_Collision()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/BootstrapHintCollision", CompanionTypeName = "BootstrapHintCollisionExamToolManifests")]
                internal sealed class BootstrapHintCollisionTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5412)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "BootstrapHintCollisionExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);
        Check.That(result.Exception).IsNull();
        Check.That(result.GeneratedSources.Length).IsEqualTo(2);
        Check.That(result.GeneratedSources.Select(static _ => _.HintName).Distinct(StringComparer.Ordinal).Count()).IsEqualTo(2);
        Check.That(result.GeneratedSources.Any(_ => String.Equals(_.HintName,"BootstrapHintCollisionExamToolManifests.g.cs",StringComparison.Ordinal))).IsTrue();
        Check.That(result.GeneratedSources.Count(_ => _.SourceText.ToString().Contains("partial class BootstrapHintCollisionExamToolManifests"))).IsEqualTo(2);
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Preserves_Unique_Nested_Tool_Default_Companion_Name()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                internal sealed class OuterA
                {
                    [ToolManifest(ToolSchemaID = "Exam/Nested/OuterA")]
                    internal sealed class Sample : Tool
                    {
                        [ToolOperation]
                        [AccessCheck(5413)]
                        public string ReadValue() => string.Empty;
                    }
                }
                """,
            assemblyName: "UniqueNestedCompanionIdentityExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);
        Check.That(result.GeneratedSources.Length).IsEqualTo(2);
        Check.That(GetGeneratedSource(result,"SampleToolManifest.g.cs")).Contains("internal static partial class SampleToolManifest");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Uses_Readable_Hashed_Names_For_Colliding_Nested_Default_Companions()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                internal sealed class OuterA
                {
                    [ToolManifest(ToolSchemaID = "Exam/NestedCollision/OuterA")]
                    internal sealed class Sample : Tool
                    {
                        [ToolOperation]
                        [AccessCheck(5414)]
                        public string ReadValue() => string.Empty;
                    }
                }

                internal sealed class OuterB
                {
                    [ToolManifest(ToolSchemaID = "Exam/NestedCollision/OuterB")]
                    internal sealed class Sample : Tool
                    {
                        [ToolOperation]
                        [AccessCheck(5420)]
                        public string ReadValue() => string.Empty;
                    }
                }
                """,
            assemblyName: "NestedCompanionIdentityExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);
        Check.That(result.GeneratedSources.Length).IsEqualTo(3);
        Check.That(result.GeneratedSources.Select(static _ => _.HintName).Distinct(StringComparer.Ordinal).Count()).IsEqualTo(3);
        Check.That(result.GeneratedSources.Any(_ => OuterA().IsMatch(_.SourceText.ToString()))).IsTrue();
        Check.That(result.GeneratedSources.Any(_ => OuterB().IsMatch(_.SourceText.ToString()))).IsTrue();
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [GeneratedRegex(@"partial class OuterA_Sample_[0-9A-F]{8}ToolManifest")]
    private static partial Regex OuterA();
    [GeneratedRegex(@"partial class OuterB_Sample_[0-9A-F]{8}ToolManifest")]
    private static partial Regex OuterB();

    [Test]
    public void Generator_Reports_And_Suppresses_Conflicting_Explicit_Companion_Overrides()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/OverrideCollision/First", CompanionTypeName = "SharedCompanion")]
                internal sealed class FirstTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5415)]
                    public string ReadValue() => string.Empty;
                }

                [ToolManifest(ToolSchemaID = "Exam/OverrideCollision/Second", CompanionTypeName = "SharedCompanion")]
                internal sealed class SecondTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5416)]
                    public string ReadValue() => string.Empty;
                }

                [ToolManifest(ToolSchemaID = "Exam/OverrideCollision/Unique")]
                internal sealed class UniqueTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5417)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "ExplicitCompanionCollisionExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Count(static _ => _.Id == "KDT013")).IsEqualTo(2);
        Check.That(result.GeneratedSources.Length).IsEqualTo(2);
        Check.That(result.GeneratedSources.Any(_ => _.SourceText.ToString().Contains("SharedCompanion"))).IsFalse();
        Check.That(GetGeneratedSource(result,"ExplicitCompanionCollisionExamToolManifests.g.cs")).Contains("global::ExamSpace.UniqueToolToolManifest.RegisterAndBind() is false");
        Check.That(GetGeneratedSource(result,"ExplicitCompanionCollisionExamToolManifests.g.cs").Contains("SharedCompanion",StringComparison.Ordinal)).IsFalse();
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Reports_And_Suppresses_Companion_That_Conflicts_With_Assembly_Bootstrap()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                [assembly: ToolManifestAssembly(CompanionNamespace = "ExamSpace.Generated", CompanionTypeName = "SharedType")]

                namespace ExamSpace.Generated;

                [ToolManifest(ToolSchemaID = "Exam/BootstrapTypeCollision", CompanionNamespace = "ExamSpace.Generated", CompanionTypeName = "SharedType")]
                internal sealed class CollisionTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5418)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "BootstrapTypeCollisionExam",
            out Compilation outputCompilation);

        Check.That(result.Exception).IsNull();
        Check.That(result.Diagnostics.Select(static _ => _.Id)).Contains("KDT013");
        Check.That(result.GeneratedSources.Length).IsEqualTo(1);
        Check.That(result.Diagnostics.Count(static _ => _.Id == "KDT013")).IsEqualTo(1);
        Check.That(result.GeneratedSources[0].SourceText.ToString()).Contains("internal static partial class SharedType");
        Check.That(result.GeneratedSources[0].SourceText.ToString()).Contains("RegisterAndBindAll");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Extends_Compatible_Handwritten_Partial_Companion()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/PartialCompanion", CompanionTypeName = "PartialCompanion")]
                internal sealed class PartialCompanionTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5419)]
                    public string ReadValue() => string.Empty;
                }

                internal static partial class PartialCompanion
                {
                    public static string Marker => "existing";
                }
                """,
            assemblyName: "PartialCompanionExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);
        Check.That(result.GeneratedSources.Length).IsEqualTo(2);
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Allows_Generic_Sibling_With_Companion_Name()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/GenericSibling")]
                internal sealed class GenericSiblingTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5421)]
                    public string ReadValue() => string.Empty;
                }

                internal class GenericSiblingToolToolManifest<T> { }
                """,
            assemblyName: "GenericCompanionSiblingExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);
        Check.That(result.GeneratedSources.Length).IsEqualTo(2);
        Check.That(GetGeneratedSource(result,"GenericSiblingToolToolManifest.g.cs")).Contains("internal static partial class GenericSiblingToolToolManifest");
        CheckCompilationHasNoErrors(outputCompilation);
    }

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
        Check.That(toolSource).Contains("MethodName = \"ReadPulse\"");
        Check.That(toolSource).Contains("MethodName = \"ReadPulseAsync\"");
        Check.That(Generator_Produces_PerTool_Companion_With_SharedIndex_DescriptorsRegex().Count(toolSource)).IsEqualTo(2);
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Reports_Conflicting_Descriptions_For_SameName_SameIndex_Overloads_And_Continues()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/ConflictingDescriptions")]
                internal sealed class ConflictingDescriptionsTool : Tool
                {
                    [ToolOperation(Description = "Reads text.")]
                    [AccessCheck(6003)]
                    public string Read() => string.Empty;

                    [ToolOperation(Description = "Reads a number.")]
                    [AccessCheck(6003)]
                    public string Read(int value) => value.ToString();
                }

                [ToolManifest(ToolSchemaID = "Exam/DescriptionConflictControl")]
                internal sealed class DescriptionConflictControlTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(6004)]
                    public string Read() => string.Empty;
                }
                """,
            assemblyName: "ConflictingOperationDescriptionGeneratorExam",
            out Compilation outputCompilation);

        Diagnostic diagnostic = result.Diagnostics.Single(static _ => _.Id == "KDT015");
        Check.That(diagnostic.GetMessage()).Contains("Read");
        Check.That(diagnostic.GetMessage()).Contains("Reads a number.");
        Check.That(diagnostic.GetMessage()).Contains("Reads text.");
        Check.That(result.GeneratedSources.Any(_ => _.SourceText.ToString().Contains("partial class ConflictingDescriptionsToolToolManifest"))).IsFalse();
        Check.That(result.GeneratedSources.Any(_ => _.SourceText.ToString().Contains("partial class DescriptionConflictControlToolToolManifest"))).IsTrue();
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Collapses_SameDescription_SameName_SameIndex_Overloads()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/SameDescriptionOverloads")]
                internal sealed class SameDescriptionOverloadsTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(6005)]
                    public string Read() => string.Empty;

                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(6005)]
                    public string Read(int value) => value.ToString();
                }
                """,
            assemblyName: "SameOperationDescriptionGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"SameDescriptionOverloadsToolToolManifest.g.cs");

        Check.That(toolSource.Split("Index = 6005",StringSplitOptions.None).Length - 1).IsEqualTo(1);
        Check.That(toolSource).Contains("MethodName = \"Read\"");
        Check.That(toolSource).Contains("Description = \"Reads a value.\"");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Emits_Keyword_Operation_Name_As_Manifest_String()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/KeywordOperation")]
                internal sealed class KeywordOperationTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5421)]
                    public void @event() { }
                }
                """,
            assemblyName: "KeywordOperationGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);
        String toolSource = GetGeneratedSource(result,"KeywordOperationToolToolManifest.g.cs");
        Check.That(toolSource).Contains("MethodName = \"event\"");
        Check.That(toolSource.Contains("nameof(global::ExamSpace.KeywordOperationTool.event)",StringComparison.Ordinal)).IsFalse();
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [TestCase("public")]
    [TestCase("internal")]
    [TestCase("protected internal")]
    public void Generator_Allows_Assembly_Accessible_Operation_Methods(String accessibility)
    {
        GeneratorRunResult result = RunGenerator(
            source: $$"""
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/AccessibleOperation/{{accessibility.Replace(" ",String.Empty,StringComparison.Ordinal)}}")]
                internal sealed class AccessibleOperationTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5422)]
                    {{accessibility}} void Read() { }
                }
                """,
            assemblyName: "AccessibleOperationGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Select(static _ => _.Id).Contains("KDT014")).IsFalse();
        Check.That(result.GeneratedSources.Any(_ => _.SourceText.ToString().Contains("AccessibleOperationToolToolManifest"))).IsTrue();
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Emits_Operation_Metadata_Without_Referencing_Protected_Methods()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                internal abstract class ProtectedOperationBase : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5423)]
                    protected void Read() { }
                }

                [ToolManifest(ToolSchemaID = "Exam/OperationAccess")]
                internal sealed class OperationAccessTool : ProtectedOperationBase
                {
                }
                """,
            assemblyName: "OperationAccessGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Select(static _ => _.Id).Contains("KDT014")).IsFalse();
        Check.That(result.GeneratedSources.Any(_ => _.SourceText.ToString().Contains("MethodName = \"Read\""))).IsTrue();
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [TestCase("private")]
    [TestCase("protected")]
    [TestCase("private protected")]
    public void Generator_Reports_Inaccessible_Tool_Or_Containing_Type_And_Continues(String accessibility)
    {
        GeneratorRunResult result = RunGenerator(
            source: $$"""
                using KusDepot;

                namespace ExamSpace;

                internal sealed class Outer
                {
                    {{accessibility}} sealed class Container
                    {
                        [ToolManifest(ToolSchemaID = "Exam/InaccessibleTool")]
                        internal sealed class InaccessibleTool : Tool
                        {
                            [ToolOperation]
                            [AccessCheck(5424)]
                            public void Read() { }
                        }
                    }
                }

                [ToolManifest(ToolSchemaID = "Exam/AccessibleTool")]
                internal sealed class AccessibleTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5425)]
                    public void Read() { }
                }
                """,
            assemblyName: "InaccessibleToolGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Count(static _ => _.Id == "KDT014")).IsEqualTo(1);
        Check.That(result.Diagnostics.Single(static _ => _.Id == "KDT014").GetMessage()).Contains("InaccessibleTool");
        Check.That(result.GeneratedSources.Any(_ => _.SourceText.ToString().Contains("AccessibleToolToolManifest"))).IsTrue();
        Check.That(result.GeneratedSources.Any(_ => _.SourceText.ToString().Contains("InaccessibleToolToolManifest"))).IsFalse();
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [TestCase("internal")]
    [TestCase("protected internal")]
    public void Generator_Allows_Assembly_Accessible_Nested_Containing_Types(String accessibility)
    {
        GeneratorRunResult result = RunGenerator(
            source: $$"""
                using KusDepot;

                namespace ExamSpace;

                internal class Outer
                {
                    {{accessibility}} class Container
                    {
                        [ToolManifest(ToolSchemaID = "Exam/AccessibleNestedType")]
                        internal sealed class NestedTool : Tool
                        {
                            [ToolOperation]
                            [AccessCheck(5426)]
                            public void Read() { }
                        }
                    }
                }
                """,
            assemblyName: "AccessibleNestedTypeGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Select(static _ => _.Id).Contains("KDT014")).IsFalse();
        Check.That(result.GeneratedSources.Any(_ => _.SourceText.ToString().Contains("NestedToolToolManifest"))).IsTrue();
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [GeneratedRegex("Index = 4600")]
    private static partial Regex Generator_Produces_PerTool_Companion_With_SharedIndex_DescriptorsRegex();

    [Test]
    public void Generator_Escapes_String_Literals_With_Control_Characters()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/Quote\\\"Schema", AccessKeyRealmID = "Exam\\Realm", ToolSchemaVersion = "1.0\\n")]
                internal sealed class EscapedStringsTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.\\n\\t\\\"quoted\\\"")]
                    [AccessCheck(4601)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "EscapedStringsGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);
        Check.That(GetGeneratedSource(result,"EscapedStringsToolToolManifest.g.cs")).Contains("\\n");
        CheckCompilationHasNoErrors(outputCompilation);
    }

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
        Check.That(toolSource).Contains("MethodName = \"ReadValue\"");
        Check.That(assemblySource).Contains("global::ExamSpace.Generated.CustomNamespaceToolToolManifest.RegisterAndBind() is false");
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
        Check.That(toolSource).Contains("MethodName = \"ReadValue\"");
        Check.That(assemblySource).Contains("global::ExamSpace.Generated.CustomToolManifestCompanion.RegisterAndBind() is false");
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
        Check.That(toolSource).Contains("MethodName = \"ReadHostValue\"");
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
        Check.That(toolSource).Contains("MethodName = \"ReadGenericHostValue\"");
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
        Check.That(toolSource).Contains("MethodName = \"ReadWebHostValue\"");
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
        Check.That(toolSource).Contains("MethodName = \"ReadAspireHostValue\"");
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
        Check.That(assemblySource).Contains("global::ExamSpace.BetaToolManifest.RegisterAndBind() is false");
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
        Check.That(assemblySource).Contains("KusDepotLog.Error");
        Check.That(assemblySource).Contains("ExamSpace.ModuleInit");
        Check.That(assemblySource).Contains("Exam/ModuleInit");
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
        Check.That(assemblySource).Contains("global::ExamSpace.Tools.AssemblyOverrideToolToolManifest.RegisterAndBind() is false");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Uses_Global_Qualification_For_Global_Namespace_Tool_Companion()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                [ToolManifest(ToolSchemaID = "Exam/GlobalNamespace")]
                internal sealed class GlobalNamespaceTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5102)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "GlobalNamespaceGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"GlobalNamespaceToolToolManifest.g.cs");
        String assemblySource = GetGeneratedSource(result,"GlobalNamespaceGeneratorExamToolManifests.g.cs");

        Check.That(toolSource).Contains("internal static partial class GlobalNamespaceToolToolManifest");
        Check.That(assemblySource).Contains("global::GlobalNamespaceToolToolManifest.RegisterAndBind() is false");
        Check.That(assemblySource).DoesNotContain("<global namespace>");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Uses_Custom_Global_Companion_Name_Without_Namespace_Override()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                [ToolManifest(ToolSchemaID = "Exam/GlobalNamespaceCustomName", CompanionTypeName = "CustomGlobalCompanion")]
                internal sealed class GlobalNamespaceCustomNameTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5103)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "GlobalNamespaceCustomNameGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"CustomGlobalCompanion.g.cs");
        String assemblySource = GetGeneratedSource(result,"GlobalNamespaceCustomNameGeneratorExamToolManifests.g.cs");

        Check.That(toolSource).Contains("internal static partial class CustomGlobalCompanion");
        Check.That(assemblySource).Contains("global::CustomGlobalCompanion.RegisterAndBind() is false");
        CheckCompilationHasNoErrors(outputCompilation);
    }

    [Test]
    public void Generator_Uses_Explicit_Companion_Namespace_For_Global_Tool_With_Module_Initializer()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                [assembly: ToolManifestAssembly(CompanionNamespace = "ExamSpace.Bootstrap", CompanionTypeName = "GlobalToolBootstrap", RegisterModuleInitializer = true)]

                [ToolManifest(ToolSchemaID = "Exam/GlobalNamespaceExplicitNamespace", CompanionNamespace = "ExamSpace.Generated", CompanionTypeName = "ExplicitGlobalCompanion")]
                internal sealed class GlobalNamespaceExplicitNamespaceTool : Tool
                {
                    [ToolOperation]
                    [AccessCheck(5104)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "GlobalNamespaceExplicitNamespaceGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);

        String toolSource = GetGeneratedSource(result,"ExplicitGlobalCompanion.g.cs");
        String assemblySource = GetGeneratedSource(result,"GlobalNamespaceExplicitNamespaceGeneratorExamToolManifests.g.cs");

        Check.That(toolSource).Contains("namespace ExamSpace.Generated;");
        Check.That(assemblySource).Contains("namespace ExamSpace.Bootstrap;");
        Check.That(assemblySource).Contains("global::ExamSpace.Generated.ExplicitGlobalCompanion.RegisterAndBind() is false");
        Check.That(assemblySource).Contains("global::System.Runtime.CompilerServices.ModuleInitializer");
        Check.That(assemblySource).Contains("internal static void InitializeModule()");
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
    public void Generator_Reports_Keyword_CompanionNamespace_And_TypeName_Diagnostics()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/KeywordNames", CompanionNamespace = "ExamSpace.class", CompanionTypeName = "namespace")]
                internal sealed class KeywordNamesTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(5323)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "KeywordNamesGeneratorExam",
            out _);

        String[] diagnosticIds = result.Diagnostics.Select(static _ => _.Id).ToArray();
        Check.That(diagnosticIds).Contains("KDT009");
        Check.That(diagnosticIds).Contains("KDT011");
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
    public void Generator_Sanitizes_Keyword_Assembly_Type_Name()
    {
        GeneratorRunResult result = RunGenerator(
            source: """
                using KusDepot;

                [assembly: ToolManifestAssembly(CompanionTypeName = "class")]

                namespace ExamSpace;

                [ToolManifest(ToolSchemaID = "Exam/KeywordAssemblyName")]
                internal sealed class KeywordAssemblyNameTool : Tool
                {
                    [ToolOperation(Description = "Reads a value.")]
                    [AccessCheck(5324)]
                    public string ReadValue() => string.Empty;
                }
                """,
            assemblyName: "KeywordAssemblyNameGeneratorExam",
            out Compilation outputCompilation);

        Check.That(result.Diagnostics.Length).IsEqualTo(0);
        String assemblySource = GetGeneratedSource(result,"KeywordAssemblyNameGeneratorExamToolManifests.g.cs");
        Check.That(assemblySource).Contains("_class");
        CheckCompilationHasNoErrors(outputCompilation);
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
