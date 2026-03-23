namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolDescriptorExam
{
    private sealed class ToolDescriptorTestTool : Tool {}

    [Test]
    public void ToolDescriptorRoundTrip()
    {
        var d = new ToolDescriptor()
        {
            ID = Guid.NewGuid(),
            Type = typeof(ToolDescriptorTestTool).FullName,
            Specifications = "specifications",
            ExtendedData = new(){ "ext-0" , "ext-1" }
        };

        var p = ToolDescriptor.Parse(d.ToString());

        Check.That(p).IsNotNull();
        Check.That(p!.ID).IsEqualTo(d.ID);
        Check.That(p.Type).IsEqualTo(d.Type);
        Check.That(p.Specifications).IsEqualTo(d.Specifications);
        Check.That(p.ExtendedData).ContainsExactly(d.ExtendedData);
    }

    [Test]
    public void ToolDescriptorCreate()
    {
        ITool t = new ToolDescriptorTestTool();

        var d = ToolDescriptor.Create(t);

        Check.That(d.ID).IsEqualTo(t.GetID());
        Check.That(d.Type).IsEqualTo(typeof(ToolDescriptorTestTool).FullName);
    }
}
