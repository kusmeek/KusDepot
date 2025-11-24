namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolHostBuilderExam
{
    [Test]
    public void BuildHost()
    {
        Check.That(ToolBuilderFactory.CreateHostBuilder().BuildHost().GetType().IsAssignableTo(typeof(IToolHost))).IsTrue();
    }

    [Test]
    public void BuildHostOpen()
    {
        Check.That(ToolBuilderFactory.CreateHostBuilder().BuildHost<ToolHost>().GetType().IsAssignableTo(typeof(IToolHost))).IsTrue();
    }

    [Test]
    public async Task BuildHostAsync()
    {
        Check.That((await ToolBuilderFactory.CreateHostBuilder().BuildHostAsync()).GetType().IsAssignableTo(typeof(IToolHost))).IsTrue();
    }

    [Test]
    public async Task BuildHostAsyncOpen()
    {
        Check.That((await ToolBuilderFactory.CreateHostBuilder().BuildHostAsync<ToolHost>()).GetType().IsAssignableTo(typeof(IToolHost))).IsTrue();
    }
}