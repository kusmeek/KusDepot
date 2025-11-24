namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolAspireHostBuilderExam
{
    [Test]
    public void BuildAspireHost()
    {
        Check.That(ToolBuilderFactory.CreateAspireHostBuilder().BuildAspireHost().GetType().IsAssignableTo(typeof(IToolAspireHost))).IsTrue();
    }

    [Test]
    public void BuildAspireHostOpen()
    {
        Check.That(ToolBuilderFactory.CreateAspireHostBuilder().BuildAspireHost<ToolAspireHost>().GetType().IsAssignableTo(typeof(IToolAspireHost))).IsTrue();
    }

    [Test]
    public async Task BuildAspireHostAsync()
    {
        Check.That((await ToolBuilderFactory.CreateAspireHostBuilder().BuildAspireHostAsync()).GetType().IsAssignableTo(typeof(IToolAspireHost))).IsTrue();
    }

    [Test]
    public async Task BuildAspireHostAsyncOpen()
    {
        Check.That((await ToolBuilderFactory.CreateAspireHostBuilder().BuildAspireHostAsync<ToolAspireHost>()).GetType().IsAssignableTo(typeof(IToolAspireHost))).IsTrue();
    }
}