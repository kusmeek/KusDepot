namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class CommandRegistryExam
{
    [Test] [Parallelizable]
    public void UpdateCommandRegistryExam()
    {
        Check.That(CommandRegistry.Commands?.Count).IsNull();

        var t  = ToolBuilderFactory.CreateBuilder()
                     .RegisterCommand("CD",new CommandDelegator("target"))
                     .ConfigureTool((x,t) => t.UnMaskCommandTypes())
                     .Build();

        Check.That(t.AddInstance()).IsTrue(); Check.That(t.UnMaskHostedServices()).IsTrue();

        var t1 = ToolBuilderFactory.CreateBuilder()
                     .RegisterCommand("CD",new CommandDelegator("target"))
                     .ConfigureTool((x,t) => t.UnMaskCommandTypes())
                     .RegisterCommand("TD",new ToolDelegator(t))
                     .Build();

        Check.That(t1.AddInstance()).IsTrue(); Check.That(t1.UnMaskHostedServices()).IsTrue();

        Check.That(CommandRegistry.Commands?.Count).IsNull();

        UpdateCommandRegistry();

        Check.That((Int32)CommandRegistry.Commands!["TD"]?.Count!).IsEqualTo(1);

        Check.That((Int32)CommandRegistry.Commands["CD"]?.Count!).IsEqualTo(2);

        var t3  = ToolBuilderFactory.CreateBuilder()
                     .RegisterCommand("CD",new CommandDelegator("target"))
                     .ConfigureTool((x,t) => t.UnMaskCommandTypes())
                     .Build();

        Check.That(t3.AddInstance()).IsTrue(); Check.That(t3.UnMaskHostedServices()).IsTrue();

        var t4 = ToolBuilderFactory.CreateBuilder()
                     .RegisterCommand("CD",new CommandDelegator("target"))
                     .ConfigureTool((x,t) => t.MaskCommandTypes())
                     .RegisterCommand("TD",new ToolDelegator(t))
                     .Build();

        Check.That(t4.AddInstance()).IsTrue(); Check.That(t4.UnMaskHostedServices()).IsTrue();

        UpdateCommandRegistry();

        Check.That((Int32)CommandRegistry.Commands["CD"]?.Count!).IsEqualTo(3);

        Check.That((Int32)CommandRegistry.Commands["TD"]?.Count!).IsEqualTo(1);
    }
}
