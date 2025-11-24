namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolBuilderExam
{
    [Test]
    public void Build()
    {
        Check.That(ToolBuilderFactory.CreateBuilder().Build().GetType().IsAssignableTo(typeof(ITool))).IsTrue();
    }

    [Test]
    public void BuildOpen()
    {
        Check.That(ToolBuilderFactory.CreateBuilder().Build<Tool>().GetType().IsAssignableTo(typeof(ITool))).IsTrue();
    }

    [Test]
    public async Task BuildAsync()
    {
        Check.That((await ToolBuilderFactory.CreateBuilder().BuildAsync()).GetType().IsAssignableTo(typeof(ITool))).IsTrue();
    }

    [Test]
    public async Task BuildAsyncOpen()
    {
        Check.That((await ToolBuilderFactory.CreateBuilder().BuildAsync<Tool>()).GetType().IsAssignableTo(typeof(ITool))).IsTrue();
    }

    [Test]
    public async Task RegisterCommandInstance()
    {
        var _ = ToolBuilderFactory.CreateBuilder().RegisterCommand("Work",new CommandTestR()).Build();

        Check.That(await _.Activate()).IsTrue();

        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).IsNotNull();
    }

    [Test]
    public async Task RegisterCommandType() 
    {
        var _ = ToolBuilderFactory.CreateBuilder().RegisterCommand<CommandTestR>("Work").Build();

        Check.That(await _.Activate()).IsTrue();

        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).IsNotNull();

        Check.That(await _.Deactivate()).IsTrue();

        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).IsNull();
    }
}