namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolGenericHostBuilderExam
{
    [Test]
    public void BuildGenericHost()
    {
        Check.That(ToolBuilderFactory.CreateGenericHostBuilder().BuildGenericHost().GetType().IsAssignableTo(typeof(IToolGenericHost))).IsTrue();
    }

    [Test]
    public void BuildGenericHostOpen()
    {
        Check.That(ToolBuilderFactory.CreateGenericHostBuilder().BuildGenericHost<ToolGenericHost>().GetType().IsAssignableTo(typeof(IToolGenericHost))).IsTrue();
    }

    [Test]
    public async Task BuildGenericHostAsync()
    {
        Check.That((await ToolBuilderFactory.CreateGenericHostBuilder().BuildGenericHostAsync()).GetType().IsAssignableTo(typeof(IToolGenericHost))).IsTrue();
    }

    [Test]
    public async Task BuildGenericHostAsyncOpen()
    {
        Check.That((await ToolBuilderFactory.CreateGenericHostBuilder().BuildGenericHostAsync<ToolGenericHost>()).GetType().IsAssignableTo(typeof(IToolGenericHost))).IsTrue();
    }
}