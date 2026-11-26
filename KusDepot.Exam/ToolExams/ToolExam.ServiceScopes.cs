namespace KusDepot.Exams.Tools;

[TestFixture]
public partial class ToolExam
{
    public class TestSingleton : IDisposable
    {
        public Guid ID = Guid.NewGuid(); public Boolean Disposed;
        public void Dispose() { Disposed = true; GC.SuppressFinalize(this); }
    }

    public class TestAsyncSingleton : IAsyncDisposable
    {
        public Guid ID = Guid.NewGuid(); public Boolean Disposed;
        public ValueTask DisposeAsync() { Disposed = true; GC.SuppressFinalize(this); return ValueTask.CompletedTask; }
    }

    public class TestScoped : IDisposable
    {
        public Guid ID = Guid.NewGuid(); public Boolean Disposed;
        public void Dispose() { Disposed = true; GC.SuppressFinalize(this); }
    }

    public class TestTransient : IDisposable
    {
        public Guid ID = Guid.NewGuid(); public Boolean Disposed;
        public void Dispose() { Disposed = true; GC.SuppressFinalize(this); }
    }

    public class TestAsyncScoped : IAsyncDisposable
    {
        public Guid ID = Guid.NewGuid(); public Boolean Disposed;
        public ValueTask DisposeAsync() { Disposed = true; GC.SuppressFinalize(this); return ValueTask.CompletedTask; }
    }

    [Test]
    public void ShareServiceProviderDispose()
    {
        ServiceCollection _0 = new();
        _0.AddSingleton<TestSingleton>();
        _0.AddScoped<TestScoped>();
        _0.AddScoped<TestAsyncScoped>();
        _0.AddTransient<TestTransient>();

        IServiceProvider _1 = _0.BuildServiceProvider(new ServiceProviderOptions(){ValidateScopes = true , ValidateOnBuild = true});

        IToolBuilder _2 = ToolBuilderFactory.CreateBuilder().UseServiceProvider(_1,ToolServiceProviderOwnership.External);
        IToolBuilder _3 = ToolBuilderFactory.CreateBuilder().UseServiceProvider(_1,ToolServiceProviderOwnership.External);

        ITool _4 = _2.Build();
        ITool _5 = _3.Build();

        TestSingleton? _6  = _1.GetService<TestSingleton>();
        TestSingleton? _7  = _4.GetToolServices()!.GetService<TestSingleton>();
        TestSingleton? _8  = _5.GetToolServices()!.GetService<TestSingleton>();

        Check.That(_6).IsSameReferenceAs(_7);
        Check.That(_6).IsSameReferenceAs(_8);
        Check.That(_6!.Disposed).IsFalse();

        TestScoped? _9  = _4.GetToolServices()!.GetService<TestScoped>();
        TestScoped? _10 = _4.GetToolServices()!.GetService<TestScoped>();
        TestScoped? _11 = _5.GetToolServices()!.GetService<TestScoped>();

        Check.That(_9).IsSameReferenceAs(_10);
        Check.That(_9).Not.IsSameReferenceAs(_11);
        Check.That(_9!.Disposed).IsFalse();
        Check.That(_11!.Disposed).IsFalse();

        TestTransient? _12 = _4.GetToolServices()!.GetService<TestTransient>();
        TestTransient? _13 = _4.GetToolServices()!.GetService<TestTransient>();
        TestTransient? _14 = _5.GetToolServices()!.GetService<TestTransient>();

        Check.That(_12).Not.IsSameReferenceAs(_13);
        Check.That(_12).Not.IsSameReferenceAs(_14);
        Check.That(_13).Not.IsSameReferenceAs(_14);
        Check.That(_12!.Disposed).IsFalse();
        Check.That(_13!.Disposed).IsFalse();
        Check.That(_14!.Disposed).IsFalse();

        _4.Dispose();

        Check.That(_4.GetDisposed()).IsTrue();
        Check.That(_9.Disposed).IsTrue();
        Check.That(_10!.Disposed).IsTrue();
        Check.That(_12.Disposed).IsTrue();
        Check.That(_13.Disposed).IsTrue();
        Check.That(_11.Disposed).IsFalse();
        Check.That(_14.Disposed).IsFalse();
        Check.That(_6.Disposed).IsFalse();

        _5.Dispose();

        Check.That(_5.GetDisposed()).IsTrue();
        Check.That(_11.Disposed).IsTrue();
        Check.That(_14.Disposed).IsTrue();
        Check.That(_6.Disposed).IsFalse();

        using IServiceScope _15 = _1.CreateScope();
        TestScoped? _16 = _15.ServiceProvider.GetService<TestScoped>();
        Check.That(_16!.Disposed).IsFalse();
        _15.Dispose();
        Check.That(_16.Disposed).IsTrue();
        (_1 as IDisposable)?.Dispose();
        Check.That(_6.Disposed).IsTrue();
    }

    [Test]
    public async Task ShareServiceProviderDisposeAsync()
    {
        ServiceCollection _0 = new();
        _0.AddSingleton<TestSingleton>();
        _0.AddScoped<TestAsyncScoped>();
        _0.AddTransient<TestTransient>();

        IServiceProvider _1 = _0.BuildServiceProvider(new ServiceProviderOptions(){ValidateScopes = true , ValidateOnBuild = true});

        IToolBuilder _2 = ToolBuilderFactory.CreateBuilder().UseServiceProvider(_1,ToolServiceProviderOwnership.External);
        IToolBuilder _3 = ToolBuilderFactory.CreateBuilder().UseServiceProvider(_1,ToolServiceProviderOwnership.External);

        ITool _4 = _2.Build();
        ITool _5 = _3.Build();

        TestSingleton? _6 = _1.GetService<TestSingleton>();
        TestSingleton? _7 = _4.GetToolServices()!.GetService<TestSingleton>();
        TestSingleton? _8 = _5.GetToolServices()!.GetService<TestSingleton>();
        Check.That(_6).IsSameReferenceAs(_7);
        Check.That(_6).IsSameReferenceAs(_8);
        Check.That(_6!.Disposed).IsFalse();

        TestAsyncScoped? _9  = _4.GetToolServices()!.GetService<TestAsyncScoped>();
        TestAsyncScoped? _10 = _4.GetToolServices()!.GetService<TestAsyncScoped>();
        TestAsyncScoped? _11 = _5.GetToolServices()!.GetService<TestAsyncScoped>();
        Check.That(_9).IsSameReferenceAs(_10);
        Check.That(_9).Not.IsSameReferenceAs(_11);
        Check.That(_9!.Disposed).IsFalse();
        Check.That(_11!.Disposed).IsFalse();

        TestTransient? _12 = _4.GetToolServices()!.GetService<TestTransient>();
        TestTransient? _13 = _4.GetToolServices()!.GetService<TestTransient>();
        TestTransient? _14 = _5.GetToolServices()!.GetService<TestTransient>();
        Check.That(_12).Not.IsSameReferenceAs(_13);
        Check.That(_12).Not.IsSameReferenceAs(_14);
        Check.That(_13).Not.IsSameReferenceAs(_14);
        Check.That(_12!.Disposed).IsFalse();
        Check.That(_13!.Disposed).IsFalse();
        Check.That(_14!.Disposed).IsFalse();

        await _4.DisposeAsync();

        Check.That(_4.GetDisposed()).IsTrue();
        Check.That(_9.Disposed).IsTrue();
        Check.That(_10!.Disposed).IsTrue();
        Check.That(_12.Disposed).IsTrue();
        Check.That(_13.Disposed).IsTrue();
        Check.That(_11.Disposed).IsFalse();
        Check.That(_14.Disposed).IsFalse();
        Check.That(_6.Disposed).IsFalse();

        await _5.DisposeAsync();

        Check.That(_5.GetDisposed()).IsTrue();
        Check.That(_11.Disposed).IsTrue();
        Check.That(_14.Disposed).IsTrue();
        Check.That(_6.Disposed).IsFalse();

        AsyncServiceScope _15 = _1.CreateAsyncScope();
        TestAsyncScoped? _16 = _15.ServiceProvider.GetService<TestAsyncScoped>();
        Check.That(_16!).IsNotNull();
        Check.That(_16!.Disposed).IsFalse();
        await _15.DisposeAsync();
        Check.That(_16.Disposed).IsTrue();
        (_1 as IDisposable)?.Dispose();
        Check.That(_6.Disposed).IsTrue();
    }

    [Test]
    public void ToolOwnedServiceProviderDispose()
    {
        ServiceCollection _0 = new();
        _0.AddSingleton<TestSingleton>();

        IServiceProvider _1 = _0.BuildServiceProvider();
        IToolBuilder _2 = ToolBuilderFactory.CreateBuilder().UseServiceProvider(_1,ToolServiceProviderOwnership.Tool);
        ITool _3 = _2.Build();
        TestSingleton? _4 = _1.GetService<TestSingleton>();

        Check.That(_4).IsSameReferenceAs(_3.GetToolServices()!.GetService<TestSingleton>());
        Check.That(_4!.Disposed).IsFalse();

        _3.Dispose();

        Check.That(_3.GetDisposed()).IsTrue();
        Check.That(_4.Disposed).IsTrue();
    }

    [Test]
    public async Task ToolOwnedServiceProviderDisposeAsync()
    {
        ServiceCollection _0 = new();
        _0.AddSingleton<TestAsyncSingleton>();

        IServiceProvider _1 = _0.BuildServiceProvider();
        IToolBuilder _2 = ToolBuilderFactory.CreateBuilder().UseServiceProvider(_1,ToolServiceProviderOwnership.Tool);
        ITool _3 = _2.Build();
        TestAsyncSingleton? _4 = _1.GetService<TestAsyncSingleton>();

        Check.That(_4).IsSameReferenceAs(_3.GetToolServices()!.GetService<TestAsyncSingleton>());
        Check.That(_4!.Disposed).IsFalse();

        await _3.DisposeAsync();

        Check.That(_3.GetDisposed()).IsTrue();
        Check.That(_4.Disposed).IsTrue();
    }

    [Test]
    public void GetToolServiceScope_Default()
    {
        IToolBuilder _0 = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,s) =>
        {
            s.AddScoped<TestScoped>();
        });

        using ITool _1 = _0.Build();

        using ToolServiceScope _2 = _1.GetToolServiceScope()!;
        TestScoped? _3 = _2.ServiceProvider.GetService<TestScoped>();
        TestScoped? _4 = _2.ServiceProvider.GetService<TestScoped>();

        Check.That(_2).IsNotNull();
        Check.That(_2.IsDefault).IsTrue();
        Check.That(_2.Name).IsEqualTo("default");
        Check.That(_3).IsNotNull();
        Check.That(_3).IsSameReferenceAs(_4);
    }

    [Test]
    public void CreateToolServiceScope_CreatesNamedScope()
    {
        IToolBuilder _0 = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,s) =>
        {
            s.AddScoped<TestScoped>();
        });

        using ITool _1 = _0.Build();
        using ToolServiceScope _2 = _1.GetToolServiceScope()!;
        using ToolServiceScope _3 = _1.CreateToolServiceScope("alpha")!;

        TestScoped? _4 = _2.ServiceProvider.GetService<TestScoped>();
        TestScoped? _5 = _3.ServiceProvider.GetService<TestScoped>();

        Check.That(_3).IsNotNull();
        Check.That(_3.IsDefault).IsFalse();
        Check.That(_3.Name).IsEqualTo("alpha");
        Check.That(_4).IsNotNull();
        Check.That(_5).IsNotNull();
        Check.That(_4).Not.IsSameReferenceAs(_5);
    }

    [Test]
    public void GetToolServiceScope_ReturnsExistingNamedScope()
    {
        IToolBuilder _0 = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,s) =>
        {
            s.AddScoped<TestScoped>();
        });

        using ITool _1 = _0.Build();
        using ToolServiceScope _2 = _1.CreateToolServiceScope("alpha")!;
        using ToolServiceScope _3 = _1.GetToolServiceScope("alpha")!;

        TestScoped? _4 = _2.ServiceProvider.GetService<TestScoped>();
        TestScoped? _5 = _3.ServiceProvider.GetService<TestScoped>();

        Check.That(_2).IsNotNull();
        Check.That(_3).IsNotNull();
        Check.That(_3.Name).IsEqualTo(_2.Name);
        Check.That(_3.IsDefault).IsFalse();
        Check.That(_3.ServiceProvider).IsSameReferenceAs(_2.ServiceProvider);
        Check.That(_4).IsSameReferenceAs(_5);
    }

    [Test]
    public void GetToolServiceScopeNames_ReturnsDefaultAndCreatedNames()
    {
        using ITool _0 = ToolBuilderFactory.CreateBuilder().Build();
        using ToolServiceScope _1 = _0.CreateToolServiceScope("alpha")!;
        using ToolServiceScope _2 = _0.CreateToolServiceScope("beta")!;

        IEnumerable<String>? _3 = _0.GetToolServiceScopeNames();

        Check.That(_3).IsNotNull();
        Check.That(_3!).ContainsExactly("alpha","beta","default");
    }

    [Test]
    public void CreateToolServiceScope_RejectsDuplicateName()
    {
        using ITool _0 = ToolBuilderFactory.CreateBuilder().Build();
        using ToolServiceScope _1 = _0.CreateToolServiceScope("alpha")!;

        Check.That(_0.CreateToolServiceScope("alpha")).IsNull();
    }

    [Test]
    public void GetToolServiceScope_UnknownNameReturnsNull()
    {
        using ITool _0 = ToolBuilderFactory.CreateBuilder().Build();

        Check.That(_0.GetToolServiceScope("missing")).IsNull();
    }

    [Test]
    public void ToolServiceScope_Dispose_ReleasesHandleOnly()
    {
        IToolBuilder _0 = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,s) =>
        {
            s.AddScoped<TestScoped>();
        });

        using ITool _1 = _0.Build();
        ToolServiceScope? _2 = _1.CreateToolServiceScope("alpha");
        using ToolServiceScope _3 = _1.GetToolServiceScope("alpha")!;

        _2!.Dispose();

        TestScoped? _4 = _3.ServiceProvider.GetService<TestScoped>();

        Check.That(_4).IsNotNull();
        Check.ThatCode(() => _2.ServiceProvider.GetService<TestScoped>()).Throws<ObjectDisposedException>();
    }

    [Test]
    public void Tool_Dispose_DisposesAllNamedScopes()
    {
        IToolBuilder _0 = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,s) =>
        {
            s.AddScoped<TestScoped>();
        });

        ITool _1 = _0.Build();
        ToolServiceScope _2 = _1.GetToolServiceScope()!;
        ToolServiceScope _3 = _1.CreateToolServiceScope("alpha")!;

        TestScoped? _4 = _2.ServiceProvider.GetService<TestScoped>();
        TestScoped? _5 = _3.ServiceProvider.GetService<TestScoped>();

        _1.Dispose();

        Check.That(_4).IsNotNull();
        Check.That(_5).IsNotNull();
        Check.That(_4!.Disposed).IsTrue();
        Check.That(_5!.Disposed).IsTrue();
    }

    [Test]
    public async Task Tool_DisposeAsync_DisposesAllNamedScopes()
    {
        IToolBuilder _0 = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,s) =>
        {
            s.AddScoped<TestAsyncScoped>();
        });

        ITool _1 = _0.Build();
        ToolServiceScope _2 = _1.GetToolServiceScope()!;
        ToolServiceScope _3 = _1.CreateToolServiceScope("alpha")!;

        TestAsyncScoped? _4 = _2.ServiceProvider.GetService<TestAsyncScoped>();
        TestAsyncScoped? _5 = _3.ServiceProvider.GetService<TestAsyncScoped>();

        await _1.DisposeAsync();

        Check.That(_4).IsNotNull();
        Check.That(_5).IsNotNull();
        Check.That(_4!.Disposed).IsTrue();
        Check.That(_5!.Disposed).IsTrue();
    }

    [Test]
    public void GetToolServiceScope_AccessDeniedWhenLocked()
    {
        using Tool _0 = new();

        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetToolServiceScope(key:new CommandKey(Array.Empty<Byte>()))).IsNull();
        Check.That(_0.CreateToolServiceScope("alpha",new CommandKey(Array.Empty<Byte>()))).IsNull();
        Check.That(_0.GetToolServiceScopeNames(new CommandKey(Array.Empty<Byte>()))).IsNull();
    }
}