namespace KusDepot.Exams.Tools;

[TestFixture]
public partial class ToolExam
{
    public class TestSingleton : IDisposable
    {
        public Guid ID = Guid.NewGuid(); public Boolean Disposed;
        public void Dispose() { Disposed = true; GC.SuppressFinalize(this); }
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

        IToolBuilder _2 = ToolBuilderFactory.CreateBuilder().UseServiceProvider(_1);
        IToolBuilder _3 = ToolBuilderFactory.CreateBuilder().UseServiceProvider(_1);

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

        IToolBuilder _2 = ToolBuilderFactory.CreateBuilder().UseServiceProvider(_1);
        IToolBuilder _3 = ToolBuilderFactory.CreateBuilder().UseServiceProvider(_1);

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
}