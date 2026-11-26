namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolDescriptorExam
{
    private sealed class ToolDescriptorTestTool : Tool {}

    [Test]
    public void ToolDescriptorRoundTrip()
    {
        ToolManifest manifest = ToolManifest.Create("KusDepot.Tools/DescriptorTool",null,"1.0",typeof(ToolDescriptorTestTool).FullName,
        [
            new ToolOperationDescriptor { Index = 7 , MethodName = nameof(ITool.GetToolDescriptor) , Description = "Gets the tool descriptor." }
        ]).ComputeManifestHash();

        var d = new ToolDescriptor()
        {
            ID = Guid.NewGuid(),
            Type = typeof(ToolDescriptorTestTool).FullName,
            Specifications = "specifications",
            ExtendedData = new(){ "ext-0" , "ext-1" },
            Manifest = manifest.ToString()
        };

        var p = ToolDescriptor.Parse(d.ToString());

        Check.That(p).IsNotNull();
        Check.That(p!.ID).IsEqualTo(d.ID);
        Check.That(p.Type).IsEqualTo(d.Type);
        Check.That(p.Specifications).IsEqualTo(d.Specifications);
        Check.That(p.ExtendedData).ContainsExactly(d.ExtendedData);
        Check.That(p.Manifest).IsEqualTo(d.Manifest);

        ToolManifest? parsedManifest = ToolManifest.Parse(p.Manifest!);
        Check.That(parsedManifest).IsNotNull();
        Check.That(parsedManifest!.ToolSchemaID).IsEqualTo(manifest.ToolSchemaID);
        Check.That(parsedManifest.ManifestHash).IsEqualTo(manifest.ManifestHash);
    }

    [Test]
    public void ToolDescriptorCreate()
    {
        ITool t = new ToolDescriptorTestTool();
        ToolManifest manifest = ToolManifest.Create("KusDepot.Tools/DescriptorTool",null,"1.0",typeof(ToolDescriptorTestTool).FullName,
        [
            new ToolOperationDescriptor { Index = 9 , MethodName = nameof(ITool.GetToolManifest) , Description = "Gets the tool manifest." }
        ]).ComputeManifestHash();

        var d = ToolDescriptor.Create(t,manifest);

        Check.That(d.ID).IsEqualTo(t.GetID());
        Check.That(d.Type).IsEqualTo(typeof(ToolDescriptorTestTool).FullName);
        Check.That(d.Manifest).IsEqualTo(manifest.ToString());
    }
}
