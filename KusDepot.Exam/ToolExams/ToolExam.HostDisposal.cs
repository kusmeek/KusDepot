namespace KusDepot.Exams.Tools;

public partial class ToolExam
{
    [Test]
    public async Task ToolHost_DisposalFailure_ContinuesCleanupAndDisposesLifetimeOnce()
    {
        ThrowingToolHostLifetime lifetime = new(throwOnDispose:true); ToolHost host = CreateDisposalToolHost(lifetime,ToolServiceProviderOwnership.Tool);
        using ToolServiceScope scope = host.CreateToolServiceScope("tracked") ?? throw new InvalidOperationException();
        CountingScopedService service = scope.ServiceProvider.GetRequiredService<CountingScopedService>();

        AggregateException? failure = null;

        try { host.Dispose(); }
        catch(AggregateException error) { failure = error; }

        Check.That(failure).IsNotNull(); Check.That(host.GetDisposed()).IsTrue();
        Check.That(lifetime.DisposeCount).IsEqualTo(1); Check.That(ReferenceEquals(lifetime.ToolHost,host)).IsTrue(); Check.That(service.DisposeCount).IsEqualTo(1);
        Check.ThatCode(host.Dispose).DoesNotThrow(); Check.That(lifetime.DisposeCount).IsEqualTo(1);
    }

    [Test]
    public async Task ToolHost_DisposeAsync_LifetimeFailureStillDisposesBaseResources()
    {
        ThrowingToolHostLifetime lifetime = new(throwOnDispose:true); ToolHost host = CreateDisposalToolHost(lifetime,ToolServiceProviderOwnership.Tool);
        ToolServiceScope scope = host.CreateToolServiceScope("tracked") ?? throw new InvalidOperationException();
        CountingScopedService service = scope.ServiceProvider.GetRequiredService<CountingScopedService>();
        AggregateException? failure = null;

        try { await host.DisposeAsync().ConfigureAwait(false); }
        catch(AggregateException error) { failure = error; }

        Check.That(failure).IsNotNull(); Check.That(host.GetDisposed()).IsTrue();
        Check.That(lifetime.DisposeCount).IsEqualTo(1); Check.That(ReferenceEquals(lifetime.ToolHost,host)).IsTrue(); Check.That(service.DisposeCount).IsEqualTo(1);
    }

    [Test]
    public void ToolHostDerived_DisposesLifetimeOnceAndContinuesWhenOwnedHostThrows()
    {
        ThrowingToolHostLifetime lifetime = new(); ThrowingDisposableHost app = new(throwOnDispose:true);
        ToolWebHost host = CreateDisposalWebHost(lifetime,app);
        AggregateException? failure = null;

        try { host.Dispose(); }
        catch(AggregateException error) { failure = error; }

        Check.That(failure).IsNotNull(); Check.That(host.GetDisposed()).IsTrue();
        Check.That(app.DisposeCount).IsEqualTo(1); Check.That(lifetime.DisposeCount).IsEqualTo(1); Check.That(lifetime.ToolHost).IsNull();
        Check.That(host.Urls).IsNull();
    }

    [Test]
    public void ToolGenericHost_DisposesNestedToolHostAndOwnLifetimeOnce()
    {
        ThrowingToolHostLifetime nestedLifetime = new(); ToolHost nestedHost = CreateDisposalToolHost(nestedLifetime,ToolServiceProviderOwnership.Tool);
        ThrowingToolHostLifetime outerLifetime = new(); ServiceCollection services = new(); services.AddSingleton<IToolHostLifetime>(outerLifetime);
        ToolGenericHost host = new(services:new ToolServiceProvider(services.BuildServiceProvider()),host:nestedHost);

        host.Dispose();

        Check.That(host.GetDisposed()).IsTrue(); Check.That(nestedHost.GetDisposed()).IsTrue();
        Check.That(nestedLifetime.DisposeCount).IsEqualTo(1); Check.That(outerLifetime.DisposeCount).IsEqualTo(1);
    }

    [Test]
    public void ToolAspireHost_DisposesApplicationAndLifetimeOnce()
    {
        ThrowingToolHostLifetime lifetime = new(); ThrowingDisposableHost app = new();
        ToolAspireHost host = CreateDisposalAspireHost(lifetime,app);

        host.Dispose();

        Check.That(host.GetDisposed()).IsTrue(); Check.That(app.DisposeCount).IsEqualTo(1);
        Check.That(lifetime.DisposeCount).IsEqualTo(1); Check.That(lifetime.ToolHost).IsNull();
    }

    [Test]
    public void ToolHostLifetime_BindIsSingleUseAndNotificationsRequireLifeKey()
    {
        ToolHostLifetime lifetime = new();
        using ToolHost host = new();
        Guid lifeKey = lifetime.Bind(host);
        CancellationToken starting = lifetime.ApplicationStarting;

        Check.That(lifetime.ToolHost).IsSameReferenceAs(host);
        Check.That(lifetime.LifeID).IsEqualTo(host.GetID());
        Check.That(lifeKey).IsNotEqualTo(Guid.Empty);
        Check.That(lifetime is IDisposable).IsFalse();

        lifetime.NotifyStarting(Guid.NewGuid());
        Check.That(starting.IsCancellationRequested).IsFalse();
        lifetime.NotifyStarting(lifeKey);
        Check.That(starting.IsCancellationRequested).IsTrue();
        Check.ThatCode(() => lifetime.Bind(host)).Throws<InvalidOperationException>();

        lifetime.Dispose(Guid.NewGuid());
        Check.That(lifetime.ToolHost).IsSameReferenceAs(host);
        lifetime.Dispose(lifeKey);
        Check.That(lifetime.ToolHost).IsNull();
        lifetime.Dispose(lifeKey);
        Check.That(lifetime.ToolHost).IsNull();
    }

    [Test]
    public void ToolHost_UsesDefaultLifetimeWhenServiceProviderIsExternallyOwned()
    {
        ToolHostLifetime externalLifetime = new(); ServiceCollection services = new(); services.AddSingleton<IToolHostLifetime>(externalLifetime);
        using InspectableToolHost host = new(services:new ToolServiceProvider(services.BuildServiceProvider(),ToolServiceProviderOwnership.External));

        Check.That(ReferenceEquals(host.ExposedLifetime,externalLifetime)).IsFalse();
        Check.That(host.ExposedLifetime).IsInstanceOf<ToolHostLifetime>();
    }

    [Test]
    public async Task ToolHostBuilder_CreatesAndBindsOwnedLifetimeForSyncAndAsyncBuilds()
    {
        IToolHostBuilder builder = ToolBuilderFactory.CreateHostBuilder();
        builder.ConfigureTool(ToolBuilderFactoryExamHelpers.ConfigureToolHostId);
        InspectableToolHost host = (InspectableToolHost)builder.BuildHost<InspectableToolHost>();

        ToolHostLifetime lifetime = host.ExposedLifetime as ToolHostLifetime ?? throw new InvalidOperationException();
        Check.That(lifetime.ToolHost).IsSameReferenceAs(host);
        Check.That(lifetime.LifeID).IsEqualTo(ToolBuilderFactoryExamHelpers.ToolHostId);
        Check.That(await host.StartingAsync(CancellationToken.None,(AccessKey?)null)).IsTrue();
        Check.That(lifetime.ApplicationStarting.IsCancellationRequested).IsTrue();
        host.Dispose();

        IToolHostBuilder asyncBuilder = ToolBuilderFactory.CreateHostBuilder();
        InspectableToolHost asyncHost = (InspectableToolHost)await asyncBuilder.BuildHostAsync<InspectableToolHost>().ConfigureAwait(false);
        ToolHostLifetime asyncLifetime = asyncHost.ExposedLifetime as ToolHostLifetime ?? throw new InvalidOperationException();
        Check.That(asyncLifetime.ToolHost).IsSameReferenceAs(asyncHost);
        Check.That(asyncLifetime.LifeID).IsEqualTo(asyncHost.GetID());
        await asyncHost.DisposeAsync().ConfigureAwait(false);
    }

    [Test]
    public void ToolGenericHostBuilder_LinksOwnedLifetimeAndConnectsBuiltHost()
    {
        IToolGenericHostBuilder builder = ToolBuilderFactory.CreateGenericHostBuilder();
        builder.LinkLifetime(); builder.ConnectWithApplication();
        IToolGenericHost host = builder.BuildGenericHost();

        using(host as IDisposable ?? throw new InvalidOperationException())
        {
            IToolHostLifetime? linkedLifetime = host.Services.GetService<IToolHostLifetime>();
            IToolConnect? connection = host.Services.GetService<IToolConnect>();

            Check.That(linkedLifetime?.ToolHost).IsSameReferenceAs(host);
            Check.That(connection).IsNotNull();
            Check.That(connection?.ToolGenericHost).IsSameReferenceAs(host);
        }
    }

    [Test]
    public void ToolGenericHostBuilder_ExternalToolServicesDoNotLinkLifetimeOrConnection()
    {
        ToolHostLifetime externalLifetime = new(); ServiceCollection services = new(); services.AddSingleton<IToolHostLifetime>(externalLifetime);
        using ServiceProvider externalProvider = services.BuildServiceProvider();
        IToolGenericHostBuilder builder = ToolBuilderFactory.CreateGenericHostBuilder();
        builder.UseServiceProvider(externalProvider,ToolServiceProviderOwnership.External);
        builder.LinkLifetime(); builder.ConnectWithApplication();
        IToolGenericHost host = builder.BuildGenericHost();

        using(host as IDisposable ?? throw new InvalidOperationException())
        {
            Check.That(host.Services.GetService<IToolHostLifetime>()).IsNull();
            Check.That(host.Services.GetService<IToolConnect>()).IsNull();
            Check.That(externalLifetime.ToolHost).IsNull();
        }
    }

    [Test]
    public void ToolHostConsoleLifetime_BindsHostAndAcquiresScopedHostKey()
    {
        ToolHostConsoleLifetime lifetime = new();
        using ToolHost host = new();
        Guid lifeKey = lifetime.Bind(host);

        Check.That(lifeKey).IsNotEqualTo(Guid.Empty);
        Check.That(lifetime is IDisposable).IsFalse();
        Check.That(lifetime.ToolHost).IsSameReferenceAs(host);
        Check.That(lifetime.LifeID).IsEqualTo(host.GetID());

        lifetime.Dispose(Guid.NewGuid());
        Check.That(lifetime.ToolHost).IsSameReferenceAs(host);
        lifetime.Dispose(lifeKey);
        Check.That(lifetime.ToolHost).IsNull();
    }

    [Test]
    public void ToolHostBuilder_UseConsoleLifetimeBindsAndRequestsOnlyHostStopAndDispose()
    {
        CapturingHostRequestAccessManager accessmanager = new();
        IToolHostBuilder builder = ToolBuilderFactory.CreateHostBuilder();
        builder.UseAccessManager(accessmanager); builder.UseConsoleLifetime();
        InspectableToolHost host = (InspectableToolHost)builder.BuildHost<InspectableToolHost>();

        try
        {
            IToolHostLifetime? lifetime = host.ExposedLifetime;
            Check.That(lifetime).IsInstanceOf<ToolHostConsoleLifetime>();
            Check.That(lifetime?.ToolHost).IsSameReferenceAs(host);
            Check.That(accessmanager.CapturedHostRequest).IsNotNull();
            Check.That(accessmanager.CapturedHostRequest!.Operations!.Value.SequenceEqual([ProtectedOperation.StopHostAsync,ProtectedOperation.Dispose])).IsTrue();
        }
        finally { host.Dispose(); }
    }

    [Test]
    public void ToolWebHost_ManagedApplicationLoggerOverridesToolServicesFallbackForLifetime()
    {
        ToolHostLifetime lifetime = new();
        CapturingLoggerFactory fallbackLoggerFactory = new(); CapturingLoggerFactory applicationLoggerFactory = new();
        ServiceCollection toolServices = new(); toolServices.AddSingleton<IToolHostLifetime>(lifetime); toolServices.AddSingleton<ILoggerFactory>(fallbackLoggerFactory);
        ServiceCollection applicationServices = new(); applicationServices.AddSingleton<ILoggerFactory>(applicationLoggerFactory);
        ThrowingDisposableHost application = new(services:applicationServices.BuildServiceProvider());
        ToolWebHost host = new(services:new ToolServiceProvider(toolServices.BuildServiceProvider()),webapplication:application);
        PropertyInfo logger = typeof(ToolHostLifetime).GetProperty("Logger",BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException();

        Check.That(logger.GetValue(lifetime)).IsSameReferenceAs(applicationLoggerFactory.Logger);
        Check.That(ReferenceEquals(logger.GetValue(lifetime),fallbackLoggerFactory.Logger)).IsFalse();
        host.Dispose();
    }

    private static ToolHost CreateDisposalToolHost(IToolHostLifetime lifetime , ToolServiceProviderOwnership ownership = ToolServiceProviderOwnership.External)
    {
        ServiceCollection services = new(); services.AddScoped<CountingScopedService>(); services.AddSingleton(lifetime);
        IServiceProvider provider = services.BuildServiceProvider();
        return new ToolHost(services:new ToolServiceProvider(provider,ownership));
    }

    private static ToolWebHost CreateDisposalWebHost(IToolHostLifetime lifetime , IHost app)
    {
        ServiceCollection services = new(); services.AddSingleton(lifetime);
        return new ToolWebHost(services:new ToolServiceProvider(services.BuildServiceProvider(),ToolServiceProviderOwnership.Tool),webapplication:app);
    }

    private static ToolAspireHost CreateDisposalAspireHost(IToolHostLifetime lifetime , IHost app)
    {
        ServiceCollection services = new(); services.AddSingleton(lifetime);
        return new ToolAspireHost(services:new ToolServiceProvider(services.BuildServiceProvider(),ToolServiceProviderOwnership.Tool),aspireapplication:app);
    }
}

internal sealed class ThrowingToolHostLifetime(Boolean throwOnDispose = false) : ToolHostLifetime
{
    private Int32 disposeCount;

    public Int32 DisposeCount => Volatile.Read(ref this.disposeCount);

    public override void Dispose(Guid lifekey)
    {
        _ = Interlocked.Increment(ref this.disposeCount);
        if(throwOnDispose) { throw new InvalidOperationException(nameof(ThrowingToolHostLifetime)); }
        base.Dispose(lifekey);
    }
}

internal sealed class InspectableToolHost : ToolHost
{
    public InspectableToolHost(){}

    public InspectableToolHost(IAccessManager? accessmanager , IEnumerable<DataItem>? data , Guid? id , ToolServiceProvider? services , Dictionary<String,ICommand>? commands , IConfiguration? configuration , ILoggerFactory? logger)
        : base(accessmanager,data,id,services,commands,configuration,logger){}

    public InspectableToolHost(ToolServiceProvider? services) : base(services:services){}

    public IToolHostLifetime? ExposedLifetime => Lifetime;
}

internal sealed class ThrowingDisposableHost(Boolean throwOnDispose = false , IServiceProvider? services = null) : IHost
{
    private Int32 disposeCount;

    public Int32 DisposeCount => Volatile.Read(ref this.disposeCount);

    public IServiceProvider Services { get; } = services ?? new ServiceCollection().BuildServiceProvider();

    public void Dispose()
    {
        _ = Interlocked.Increment(ref this.disposeCount);
        if(throwOnDispose) { throw new InvalidOperationException(nameof(ThrowingDisposableHost)); }
    }

    public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal sealed class CapturingLoggerFactory : ILoggerFactory
{
    public ILogger Logger { get; } = LoggerHelpers.CreateILogger() ?? NullLogger.Instance;

    public ILogger CreateLogger(String categoryName) => this.Logger;

    public void AddProvider(ILoggerProvider provider) {}

    public void Dispose() {}
}

internal sealed class CapturingHostRequestAccessManager : AccessManager
{
    public HostRequest? CapturedHostRequest { get; private set; }

    protected override AccessPolicyRequestContext CreateRequestPolicyContext(AccessRequest request , ITool tool)
    {
        if(request is HostRequest hostrequest) { CapturedHostRequest = hostrequest; }

        return base.CreateRequestPolicyContext(request,tool);
    }
}
