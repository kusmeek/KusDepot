namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolHostExam
{
    [Test] [Parallelizable]
    public async Task ToolHosting()
    {
        IToolHostBuilder tb = ToolBuilderFactory.CreateHostBuilder();

        tb.RegisterTool<Tool001>();

        IToolHost _ = tb.BuildHost();

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

    [Test] [Parallelizable]
    public async Task ToolSelfHost()
    {
        using IToolHost t = ((IToolHostBuilder)ToolBuilderFactory.CreateHostBuilder().ConfigureServices((_,services) => services.AddHostedService<ToolHost>())).BuildHost();

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        await t.StartHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await t.StopHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(t.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);
    }

    [Test] [Parallelizable]
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

    [Test] [Parallelizable]
    public async Task ToolHostBackgroundService()
    {
        IToolHostBuilder _ = ToolBuilderFactory.CreateHostBuilder();

        _.ConfigureServices((context,services) =>
        {
            services.AddSingleton<IHostedService,BackgroundServce006>();
        });

        _.RegisterCommand<CommandTest>("Work");

        IToolHost __ = _.BuildHost();

        Check.That(__.MaskHostedServices(false)).IsTrue();

        await __.StartHostAsync();

        Check.That(__.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test] [Parallelizable]
    public async Task ToolHostingAsync()
    {
        IToolHostBuilder tb = ToolBuilderFactory.CreateHostBuilder();

        tb.RegisterTool<Tool001>();

        IToolHost _ = await tb.BuildHostAsync();

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

    [Test] [Parallelizable]
    public async Task ToolSelfHostAsync()
    {
        using IToolHost t = await ((IToolHostBuilder)ToolBuilderFactory.CreateHostBuilder().ConfigureServices((_,services) => services.AddHostedService<ToolHost>())).BuildHostAsync();

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        await t.StartHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        await t.StopHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(t.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);
    }

    [Test] [Parallelizable]
    public async Task ToolHostAsync_Test()
    {
        IToolHostBuilder _ = ToolBuilderFactory.CreateHostBuilder();

        _.ConfigureServices((context,services) =>
        {
            services.AddSingletonWithInterfaces<Tool002>();
        });

        _.RegisterCommand<CommandTest>("Work");

        IToolHost __ = await _.BuildHostAsync(); 

        Check.That(__.AddInstance()).IsTrue(); Check.That(__.MaskHostedServices(false)).IsTrue();

        await __.StartHostAsync();

        Check.That(__.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test] [Parallelizable]
    public async Task ToolHostBackgroundServiceAsync()
    {
        IToolHostBuilder _ = ToolBuilderFactory.CreateHostBuilder();

        _.ConfigureServices((context,services) =>
        {
            services.AddSingleton<IHostedService,BackgroundServce006>();
        });

        _.RegisterCommand<CommandTest>("Work");

        IToolHost __ = await _.BuildHostAsync();

        Check.That(__.MaskHostedServices(false)).IsTrue();

        await __.StartHostAsync();

        Check.That(__.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }
}