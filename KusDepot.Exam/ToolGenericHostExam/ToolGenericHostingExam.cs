namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolGenericHostingExam
{
    [Test]
    public async Task ToolGenericHosting()
    {
        IToolGenericHostBuilder tb = ToolBuilderFactory.CreateGenericHostBuilder();

        using IToolGenericHost _ = tb.BuildGenericHost();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(await _.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await _.StopHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(_.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);
    }

    [Test, Explicit, Category("Extended")]
    public async Task ToolGenericHostSelfHost()
    {
        using IToolGenericHost x = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost();

        using IToolGenericHost n = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(x);

        using IToolGenericHost t = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(n);

        using IToolGenericHost h = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(t);

        Check.That(h.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(x.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(n.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        await h.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token);

        Check.That(h.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(x.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(n.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await h.StopHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token);

        Check.That(h.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(x.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(n.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(h.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(x.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(n.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(t.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(x.GetLocked()).IsTrue(); Check.That(t.GetLocked()).IsTrue(); Check.That(n.GetLocked()).IsTrue();
    }

    [Test, Explicit, Category("Extended")]
    public async Task ToolGenericHostToolHost()
    {
        using IToolHost m = ((IToolHostBuilder)ToolBuilderFactory.CreateHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildHost();

        using IToolGenericHost x2 = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(m);

        using IToolGenericHost x = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(x2);

        using IToolGenericHost n = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(x);

        using IToolGenericHost t = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(n);

        using IToolGenericHost h = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(t);

        Check.That(m.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(h.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(x.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(n.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        await h.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token);

        Check.That(m.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(h.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(x.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(n.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await h.StopHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token);

        Check.That(m.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(h.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(x.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(n.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(m.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(h.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(x.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(n.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(t.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(x.GetLocked()).IsTrue(); Check.That(t.GetLocked()).IsTrue(); Check.That(n.GetLocked()).IsTrue(); Check.That(m.GetLocked()).IsTrue();
    }

    [Test, Explicit, Category("Extended")]
    public async Task ToolGenericHostWebHost()
    {
        using IToolWebHost w = ((IToolWebHostBuilder)ToolBuilderFactory.CreateWebHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).UseRandomLocalPorts().BuildWebHost();

        using IToolGenericHost x2 = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(w);

        using IToolGenericHost x = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(x2);

        using IToolGenericHost n = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(x);

        using IToolGenericHost t = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(n);

        using IToolGenericHost h = ((IToolGenericHostBuilder)ToolBuilderFactory.CreateGenericHostBuilder().ConfigureServices((_,s) => s.AddHostedService<Tool001>())).BuildGenericHost(t);

        Check.That(w.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(h.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(x.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(n.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        await h.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token);

        Check.That(w.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(h.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(x.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(n.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await h.StopHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token);

        Check.That(w.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(h.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(x.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(n.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(w.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(h.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(x.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(n.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(t.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(x.GetLocked()).IsTrue(); Check.That(t.GetLocked()).IsTrue(); Check.That(n.GetLocked()).IsTrue(); Check.That(w.GetLocked()).IsTrue();
    }

    [Test]
    public async Task ToolGenericHost()
    {
        IToolGenericHostBuilder _ = ToolBuilderFactory.CreateGenericHostBuilder();

        _.ConfigureServices((context,services) =>
        {
            services.AddSingletonWithInterfaces<Tool001>();
        });

        _.RegisterCommand<CommandTest>("Work");

        using IToolGenericHost __ = _.BuildGenericHost();

        await __.StartHostAsync();

        Check.That(__.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test]
    public async Task ToolGenericHostingAsync()
    {
        IToolGenericHostBuilder tb = ToolBuilderFactory.CreateGenericHostBuilder();

        using IToolGenericHost _ = await tb.BuildGenericHostAsync();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(await _.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await _.StopHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(10)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(_.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);
    }
}