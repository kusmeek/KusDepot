namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolManifestExam
{
    private sealed class ToolManifestTestTool : Tool {}

    [Test]
    public void ToolManifestCreate_FromTool_Uses_Defaults()
    {
        ITool tool = new ToolManifestTestTool();

        ToolManifest manifest = ToolManifest.Create(tool);

        Check.That(manifest.ToolSchemaID).IsEqualTo(typeof(ToolManifestTestTool).FullName);
        Check.That(manifest.AccessKeyRealmID).IsEqualTo(typeof(ToolManifestTestTool).FullName);
        Check.That(manifest.ToolSchemaVersion).IsEqualTo(String.Empty);
        Check.That(manifest.ToolType).IsEqualTo(typeof(ToolManifestTestTool).FullName);
        Check.That(manifest.Operations).IsEmpty();
        Check.That(manifest.ManifestHash).IsNull();
    }

    [Test]
    public void ToolManifestCreate_ExplicitValues_RoundTrip()
    {
        ToolManifest manifest = ToolManifest.Create("KusDepot.Tools/Manifest","Manifest-Realm","2.0",typeof(ToolManifestTestTool).FullName,
        [
            new ToolOperationDescriptor { Index = 1 , MethodName = nameof(ITool.GetToolDescriptor) , Description = "Gets the tool descriptor." },
            new ToolOperationDescriptor { Index = 9 , MethodName = nameof(ITool.GetToolManifest) , Description = "Gets the tool manifest." }
        ]).ComputeManifestHash();

        ToolManifest? parsed = ToolManifest.Parse(manifest.ToString());

        Check.That(parsed).IsNotNull();
        Check.That(parsed!.ToolSchemaID).IsEqualTo(manifest.ToolSchemaID);
        Check.That(parsed.AccessKeyRealmID).IsEqualTo(manifest.AccessKeyRealmID);
        Check.That(parsed.ToolSchemaVersion).IsEqualTo(manifest.ToolSchemaVersion);
        Check.That(parsed.ToolType).IsEqualTo(manifest.ToolType);
        Check.That(parsed.ManifestHash).IsEqualTo(manifest.ManifestHash);
        Check.That(parsed.Operations.Select(_ => _.MethodName)).ContainsExactly(manifest.Operations.Select(_ => _.MethodName));
    }

    [Test]
    public void ToolManifestCreate_Defaults_AccessKeyRealmID_To_ToolSchemaID()
    {
        ToolManifest manifest = ToolManifest.Create("KusDepot.Tools/Manifest",null,"1.0",typeof(ToolManifestTestTool).FullName,[]);

        Check.That(manifest.AccessKeyRealmID).IsEqualTo(manifest.ToolSchemaID);
    }

    [Test]
    public void ToolManifestComputeManifestHash_Uses_SchemaVersion()
    {
        ToolManifest v1 = ToolManifest.Create("KusDepot.Tools/Manifest",null,"1.0",typeof(ToolManifestTestTool).FullName,
        [
            new ToolOperationDescriptor { Index = 2 , MethodName = nameof(ITool.GetToolDescriptor) , Description = "Gets the tool descriptor." }
        ]).ComputeManifestHash();

        ToolManifest v2 = ToolManifest.Create("KusDepot.Tools/Manifest",null,"2.0",typeof(ToolManifestTestTool).FullName,
        [
            new ToolOperationDescriptor { Index = 2 , MethodName = nameof(ITool.GetToolDescriptor) , Description = "Gets the tool descriptor." }
        ]).ComputeManifestHash();

        Check.That(v1.ManifestHash).IsNotNull();
        Check.That(v2.ManifestHash).IsNotNull();
        Check.That(v1.ManifestHash).IsNotEqualTo(v2.ManifestHash);
    }

    [Test]
    public void ToolManifestComputeManifestHash_Is_Order_Stable()
    {
        ToolManifest left = ToolManifest.Create("KusDepot.Tools/Manifest",null,"1.0",typeof(ToolManifestTestTool).FullName,
        [
            new ToolOperationDescriptor { Index = 7 , MethodName = nameof(ITool.GetToolManifest) , Description = "Gets the tool manifest." },
            new ToolOperationDescriptor { Index = 3 , MethodName = nameof(ITool.GetToolDescriptor) , Description = "Gets the tool descriptor." }
        ]).ComputeManifestHash();

        ToolManifest right = ToolManifest.Create("KusDepot.Tools/Manifest",null,"1.0",typeof(ToolManifestTestTool).FullName,
        [
            new ToolOperationDescriptor { Index = 3 , MethodName = nameof(ITool.GetToolDescriptor) , Description = "Gets the tool descriptor." },
            new ToolOperationDescriptor { Index = 7 , MethodName = nameof(ITool.GetToolManifest) , Description = "Gets the tool manifest." }
        ]).ComputeManifestHash();

        Check.That(left.ManifestHash).IsEqualTo(right.ManifestHash);
    }

    [Test]
    public void ToolManifestComputeManifestHash_Does_Not_Overwrite_Existing_Value()
    {
        ToolManifest manifest = ToolManifest.Create("KusDepot.Tools/Manifest",null,"1.0",typeof(ToolManifestTestTool).FullName,
        [
            new ToolOperationDescriptor { Index = 4 , MethodName = nameof(ITool.GetToolDescriptor) , Description = "Gets the tool descriptor." }
        ]);

        manifest.ManifestHash = "PRESET";

        ToolManifest computed = manifest.ComputeManifestHash();

        Check.That(ReferenceEquals(computed,manifest)).IsTrue();
        Check.That(computed.ManifestHash).IsEqualTo("PRESET");
    }
}
