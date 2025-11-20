namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolWebHostingExam
{
    [Test] [NonParallelizable]
    public async Task ToolWebHosting()
    {
        IToolWebHostBuilder tb = ToolBuilderFactory.CreateWebHostBuilder();

        tb.RegisterTool<Tool001>();

        using IToolWebHost _ = tb.UseRandomLocalPorts().BuildWebHost();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(await _.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await _.StopHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(_.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);
    }

    [Test] [NonParallelizable]
    public async Task ToolWebHost()
    {
        IToolWebHostBuilder _ = ToolBuilderFactory.CreateWebHostBuilder();

        _.ConfigureServices((context,services) => { services.AddSingletonWithInterfaces<Tool001>(); });

        _.RegisterCommand<CommandTest>("Work");

        using IToolWebHost __ = _.UseRandomLocalPorts().BuildWebHost();

        await __.StartHostAsync();

        Check.That(__.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test] [NonParallelizable]
    public async Task ToolWebHostingAsync()
    {
        IToolWebHostBuilder _ = ToolBuilderFactory.CreateWebHostBuilder();

        _.ConfigureServices((context,services) => { services.AddSingletonWithInterfaces<Tool001>(); });

        _.RegisterCommand<CommandTest>("Work");

        using IToolWebHost __ = await _.UseRandomLocalPorts().BuildWebHostAsync();

        await __.StartHostAsync();

        Check.That(__.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }
}