namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolHostingExam
{
    [Test]
    public async Task IsHosting()
    {
        IToolBuilder tb = ToolBuilderFactory.CreateBuilder();

        tb.RegisterTool<Tool001>();

        ToolHost1 _ = tb.Build<ToolHost1>();

        Check.That(await _.StartHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await _.StopHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token)).IsTrue();

        Check.That(_.MaskHostedServices(false)).IsTrue();

        Check.That(_.IsHosting<Tool001>()).IsTrue();

        Check.That(_.IsHosting<Tool000>()).IsFalse();

        Check.That(_.IsHosting(typeof(Tool001))).IsTrue();

        Check.That(_.IsHosting(typeof(Tool000))).IsFalse();
    }

    [Test]
    public async Task ToolHosting()
    {
        IToolBuilder tb = ToolBuilderFactory.CreateBuilder();

        tb.RegisterTool<Tool001>();

        ToolHost1 _ = tb.Build<ToolHost1>();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(await _.StartHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await _.StopHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(_.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(_.MaskHostedServices(false)).IsTrue();

        Check.That(_.IsHosting<Tool001>()).IsTrue();

        Check.That(_.IsHosting<Tool000>()).IsFalse();
    }

    [Test]
    public async Task GenericHost()
    {
        var _ = Host.CreateEmptyApplicationBuilder(null); _.Services.AddHostedService<Tool001>();

        await _.Build().RunAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);
    }

    [Test]
    public async Task SelfHost()
    {
        using ITool t = ToolBuilderFactory.CreateBuilder().ConfigureServices((_,services) => services.AddHostedService<Tool001>()).Build();

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        await t.StartHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await t.StopHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(t.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);
    }

    [Test]
    public async Task ToolHost()
    {
        IToolHostBuilder _ = ToolBuilderFactory.CreateHostBuilder();

        _.ConfigureServices((context,services) =>
        {
            services.AddSingletonWithInterfaces<Tool002>();
        });

        _.RegisterCommand<CommandTest>("Work");

        IToolHost __ = _.BuildHost();

        Check.That(__.AddInstance()).IsTrue(); Check.That(__.MaskHostedServices(false)).IsTrue();

        await __.StartHostAsync();

        Check.That(__.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test]
    public async Task ToolHosts()
    {
        var tb = ToolBuilderFactory.CreateBuilder();

        using var th = ToolBuilderFactory.CreateHostBuilder().BuildHost();

        using var tgh = ToolBuilderFactory.CreateGenericHostBuilder().BuildGenericHost();

        using var twh = ToolBuilderFactory.CreateWebHostBuilder().BuildWebHost();

        tb.ConfigureServices((x,s) => 
        {
            s.AddSingletonWithInterfaces(th);
            s.AddSingletonWithInterfaces(tgh);
            s.AddSingletonWithInterfaces(twh);
        });

        var t = tb.Build();

        await t.StartHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(th.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(tgh.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(twh.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await t.StopHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(th.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(tgh.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(twh.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }
}