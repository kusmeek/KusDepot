namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class KusDepotRegistryExam
{
    [Test] [Parallelizable]
    public void UpdateCommandRegistryExam()
    {
        Check.That(KusDepotRegistry.Commands?.Count).IsNull();

        var t  = ToolBuilderFactory.CreateBuilder()
                     .RegisterCommand("CD",new CommandDelegator("target"))
                     .ConfigureTool((x,t) => t.MaskCommandTypes(false))
                     .Build();

         Check.That(t.AddInstance()).IsTrue(); Check.That(t.MaskHostedServices(false)).IsTrue();

        var t1 = ToolBuilderFactory.CreateBuilder()
                     .RegisterCommand("CD",new CommandDelegator("target"))
                     .ConfigureTool((x,t) => t.MaskCommandTypes(false))
                     .RegisterCommand("TD",new ToolDelegator(t))
                     .Build();

        Check.That(t1.AddInstance()).IsTrue(); Check.That(t1.MaskHostedServices(false)).IsTrue();

        Check.That(KusDepotRegistry.Commands?.Count).IsNull();

        UpdateCommandRegistry();

        Check.That((Int32)KusDepotRegistry.Commands!["TD"]?.Count!).IsEqualTo(1);

        Check.That((Int32)KusDepotRegistry.Commands["CD"]?.Count!).IsEqualTo(2);

        var t3  = ToolBuilderFactory.CreateBuilder()
                     .RegisterCommand("CD",new CommandDelegator("target"))
                     .ConfigureTool((x,t) => t.MaskCommandTypes(false))
                     .Build();

        Check.That(t3.AddInstance()).IsTrue(); Check.That(t3.MaskHostedServices(false)).IsTrue();

        var t4 = ToolBuilderFactory.CreateBuilder()
                     .RegisterCommand("CD",new CommandDelegator("target"))
                     .ConfigureTool((x,t) => t.MaskCommandTypes(true))
                     .RegisterCommand("TD",new ToolDelegator(t))
                     .Build();

        Check.That(t4.AddInstance()).IsTrue(); Check.That(t4.MaskHostedServices(false)).IsTrue();

        UpdateCommandRegistry();

        Check.That((Int32)KusDepotRegistry.Commands["CD"]?.Count!).IsEqualTo(3);

        Check.That((Int32)KusDepotRegistry.Commands["TD"]?.Count!).IsEqualTo(1);
    }
}