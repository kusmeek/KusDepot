namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class CommandDescriptorExam
{
    [Test]
    public void CommandDescriptorRoundTrip()
    {
        var d = new CommandDescriptor()
        {
            Type = typeof(CommandTest).FullName,
            Specifications = "specifications",
            ExtendedData = new(){ "ext-0" , "ext-1" },
            RegisteredHandles = new(){ "handle-0" , "handle-1" }
        };

        var p = CommandDescriptor.Parse(d.ToString());

        Check.That(p).IsNotNull();
        Check.That(p!.Type).IsEqualTo(d.Type);
        Check.That(p.Specifications).IsEqualTo(d.Specifications);
        Check.That(p.ExtendedData).ContainsExactly(d.ExtendedData);
        Check.That(p.RegisteredHandles).ContainsExactly(d.RegisteredHandles);
    }

    [Test]
    public void CommandDescriptorCreate()
    {
        ICommand c = new CommandTest();

        var d = CommandDescriptor.Create(c);

        Check.That(d.Type).IsEqualTo(typeof(CommandTest).FullName);
    }
}
