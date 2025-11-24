namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolWebHostBuilderExam
{
    [Test]
    public void BuildWebHost()
    {
        Check.That(ToolBuilderFactory.CreateWebHostBuilder().BuildWebHost().GetType().IsAssignableTo(typeof(IToolWebHost))).IsTrue();
    }

    [Test]
    public void BuildWebHostOpen()
    {
        Check.That(ToolBuilderFactory.CreateWebHostBuilder().BuildWebHost<ToolWebHost>().GetType().IsAssignableTo(typeof(IToolWebHost))).IsTrue();
    }

    [Test]
    public async Task BuildWebHostAsync()
    {
        Check.That((await ToolBuilderFactory.CreateWebHostBuilder().BuildWebHostAsync()).GetType().IsAssignableTo(typeof(IToolWebHost))).IsTrue();
    }

    [Test]
    public async Task BuildWebHostAsyncOpen()
    {
        Check.That((await ToolBuilderFactory.CreateWebHostBuilder().BuildWebHostAsync<ToolWebHost>()).GetType().IsAssignableTo(typeof(IToolWebHost))).IsTrue();
    }
}