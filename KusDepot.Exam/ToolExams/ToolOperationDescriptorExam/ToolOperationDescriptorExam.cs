namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolOperationDescriptorExam
{
    [Test]
    public void ToolOperationDescriptorRoundTrip()
    {
        var descriptor = new ToolOperationDescriptor
        {
            Index = 11,
            MethodName = nameof(ITool.GetToolDescriptor),
            Description = "Gets the tool descriptor."
        };

        var parsed = ToolOperationDescriptor.Parse(descriptor.ToString());

        Check.That(parsed).IsNotNull();
        Check.That(parsed!.Index).IsEqualTo(descriptor.Index);
        Check.That(parsed.MethodName).IsEqualTo(descriptor.MethodName);
        Check.That(parsed.Description).IsEqualTo(descriptor.Description);
    }

    [Test]
    public void ToolOperationDescriptorEquals_Uses_Reference_Equality()
    {
        var descriptor = new ToolOperationDescriptor { Index = 5 , MethodName = "Op" , Description = "Operation." };
        var sameValues = new ToolOperationDescriptor { Index = 5 , MethodName = "Op" , Description = "Operation." };

        Check.That(descriptor.Equals(descriptor)).IsTrue();
        Check.That(descriptor.Equals(sameValues)).IsFalse();
        Check.That(descriptor.GetHashCode()).IsEqualTo(RuntimeHelpers.GetHashCode(descriptor));
    }
}
