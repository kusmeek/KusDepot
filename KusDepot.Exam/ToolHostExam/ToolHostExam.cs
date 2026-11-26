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

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await _.StartHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await _.StopHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(_.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(_.UnMaskHostedServices()).IsTrue();

        Check.That(_.IsHosting<Tool001>()).IsTrue();

        Check.That(_.IsHosting<Tool000>()).IsFalse();
    }

    [Test] [Parallelizable]
    public async Task ToolSelfHost()
    {
        using IToolHost t = ((IToolHostBuilder)ToolBuilderFactory.CreateHostBuilder().ConfigureToolServices((_,services) => services.AddHostedService<ToolHost>())).BuildHost();

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

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

        _.ConfigureToolServices((context,services) =>
        {
            services.AddSingletonWithInterfaces<Tool002>();
        });

        _.RegisterCommand<CommandTest>("Work");

        IToolHost __ = _.BuildHost(); 

        Check.That(__.AddInstance()).IsTrue(); Check.That(__.UnMaskHostedServices()).IsTrue();

        await __.StartHostAsync();

        Check.That(__.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test] [Parallelizable]
    public async Task ToolHostBackgroundService()
    {
        IToolHostBuilder _ = ToolBuilderFactory.CreateHostBuilder();

        _.ConfigureToolServices((context,services) =>
        {
            services.AddSingleton<IHostedService,BackgroundServce006>();
        });

        _.RegisterCommand<CommandTest>("Work");

        IToolHost __ = _.BuildHost();

        Check.That(__.UnMaskHostedServices()).IsTrue();

        await __.StartHostAsync();

        Check.That(__.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test] [Parallelizable]
    public async Task ToolHostingAsync()
    {
        IToolHostBuilder tb = ToolBuilderFactory.CreateHostBuilder();

        tb.RegisterTool<Tool001>();

        IToolHost _ = await tb.BuildHostAsync();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await _.StartHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await _.StopHostAsync(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token)).IsTrue();

        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(_.GetLifeCycleState()).IsNotEqualTo(LifeCycleState.Error);

        Check.That(_.UnMaskHostedServices()).IsTrue();

        Check.That(_.IsHosting<Tool001>()).IsTrue();

        Check.That(_.IsHosting<Tool000>()).IsFalse();
    }

    [Test] [Parallelizable]
    public async Task ToolSelfHostAsync()
    {
        using IToolHost t = await ((IToolHostBuilder)ToolBuilderFactory.CreateHostBuilder().ConfigureToolServices((_,services) => services.AddHostedService<ToolHost>())).BuildHostAsync();

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

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

        _.ConfigureToolServices((context,services) =>
        {
            services.AddSingletonWithInterfaces<Tool002>();
        });

        _.RegisterCommand<CommandTest>("Work");

        IToolHost __ = await _.BuildHostAsync(); 

        Check.That(__.AddInstance()).IsTrue(); Check.That(__.UnMaskHostedServices()).IsTrue();

        await __.StartHostAsync();

        Check.That(__.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test] [Parallelizable]
    public async Task ToolHostBackgroundServiceAsync()
    {
        IToolHostBuilder _ = ToolBuilderFactory.CreateHostBuilder();

        _.ConfigureToolServices((context,services) =>
        {
            services.AddSingleton<IHostedService,BackgroundServce006>();
        });

        _.RegisterCommand<CommandTest>("Work");

        IToolHost __ = await _.BuildHostAsync();

        Check.That(__.UnMaskHostedServices()).IsTrue();

        await __.StartHostAsync();

        Check.That(__.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test] [Parallelizable]
    public void Run_ReturnsFalseWhenStartFails()
    {
        using TestRunToolHost host = new(false,true);

        Check.That(host.Run()).IsFalse();
        Check.That(host.StopCalled).IsFalse();
    }

    [Test] [Parallelizable]
    public async Task RunAsync_ReturnsFalseWhenStartFails()
    {
        using CancellationTokenSource cancel = new();
        using TestRunToolHost host = new(false,true,cancel);

        Check.That(await host.RunAsync(cancel:cancel.Token)).IsFalse();
        Check.That(host.StopCalled).IsFalse();
    }

    [TestCase(true)] [TestCase(false)] [Parallelizable]
    public async Task RunAsync_ReturnsStopResultAfterCancellation(Boolean stopresult)
    {
        TimeSpan timeout = TimeSpan.FromSeconds(5);
        using CancellationTokenSource cancel = new();
        using TestRunToolHost host = new(true,stopresult,cancel);

        Check.That(await host.RunAsync(timeout:timeout,cancel:cancel.Token)).IsEqualTo(stopresult);
        Check.That(host.StopCalled).IsTrue();
        Check.That(host.StartTimeout).IsEqualTo(timeout);
        Check.That(host.StopTimeout).IsEqualTo(timeout);
    }

    [Test] [Parallelizable]
    public async Task RunAsync_ReturnsTrueWhenHostStopsExternally()
    {
        using TestRunToolHost host = new(true,false);
        Task<Boolean> run = host.RunAsync();

        host.SignalStopped();

        Check.That(await run).IsTrue();
        Check.That(host.StopCalled).IsFalse();
    }

    private sealed class TestRunToolHost : ToolHost
    {
        private readonly Boolean StartResult;
        private readonly Boolean StopResult;
        private readonly CancellationTokenSource? CancelOnStart;
        private readonly TestRunToolLifetime RunLifetime = new();

        public Boolean StopCalled { get; private set; }
        public TimeSpan? StartTimeout { get; private set; }
        public TimeSpan? StopTimeout { get; private set; }

        public TestRunToolHost(Boolean startresult , Boolean stopresult , CancellationTokenSource? cancelonstart = null)
        {
            this.StartResult = startresult; this.StopResult = stopresult; this.CancelOnStart = cancelonstart; this.Lifetime = this.RunLifetime;
        }

        public void SignalStopped() { this.RunLifetime.SignalStopped(); }

        public override Task<Boolean> StartHostAsync(CancellationToken? cancel = null , TimeSpan? timeout = null , AccessKey? key = null)
        {
            this.StartTimeout = timeout; this.CancelOnStart?.Cancel(); return Task.FromResult(this.StartResult);
        }

        public override Task<Boolean> StopHostAsync(CancellationToken? cancel = null , TimeSpan? timeout = null , AccessKey? key = null)
        {
            this.StopCalled = true; this.StopTimeout = timeout; return Task.FromResult(this.StopResult);
        }
    }

    private sealed class TestRunToolLifetime : IToolHostLifetime
    {
        private readonly CancellationTokenSource StoppedSource = new();

        public CancellationToken ApplicationStarting => CancellationToken.None;
        public CancellationToken ApplicationStarted => CancellationToken.None;
        public CancellationToken ApplicationStopping => CancellationToken.None;
        public CancellationToken ApplicationStopped => this.StoppedSource.Token;
        public Guid? LifeID => null;
        public IToolHost? ToolHost => null;

        public Guid Bind(IToolHost toolhost) { return Guid.Empty; }
        public void Dispose(Guid lifekey) { this.StoppedSource.Dispose(); }
        public void NotifyStarting(Guid lifekey) {}
        public void NotifyStarted(Guid lifekey) {}
        public void NotifyStopping(Guid lifekey) {}
        public void NotifyStopped(Guid lifekey) { this.SignalStopped(); }
        public void StopApplication() {}
        public void SignalStopped() { this.StoppedSource.Cancel(); }
    }
}