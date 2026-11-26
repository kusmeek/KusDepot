namespace KusDepot.Exams.Tools;

[TestFixture]
public partial class ToolExam
{
    [Test]
    public void GetService_ResolvesFromDefaultScope()
    {
        using ITool tool = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,services) =>
        {
            services.AddScoped<TestScoped>();
            services.AddTransient<TestTransient>();
        }).Build();

        TestScoped? scoped = tool.GetService(typeof(TestScoped)) as TestScoped;
        TestScoped? scopedAgain = tool.GetRequiredService(typeof(TestScoped)) as TestScoped;
        TestTransient? transient = tool.GetService(typeof(TestTransient)) as TestTransient;
        TestTransient? transientAgain = tool.GetRequiredService(typeof(TestTransient)) as TestTransient;

        Check.That(scoped).IsNotNull();
        Check.That(scopedAgain).IsSameReferenceAs(scoped);
        Check.That(transient).IsNotNull();
        Check.That(transientAgain).Not.IsSameReferenceAs(transient);
        Check.That(tool.GetService(typeof(String))).IsNull();
        Check.ThatCode(() => tool.GetRequiredService(typeof(String))).Throws<InvalidOperationException>();
    }

    [Test]
    public void GetKeyedService_ResolvesRegisteredKeys()
    {
        const String key = "service-key";

        using ITool tool = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,services) =>
        {
            services.AddKeyedScoped<TestScoped>(key);
        }).Build();

        TestScoped? keyed = tool.GetKeyedService(typeof(TestScoped),key) as TestScoped;
        TestScoped? keyedAgain = ((IKeyedServiceProvider)tool).GetKeyedService(typeof(TestScoped),key) as TestScoped;

        Check.That(keyed).IsNotNull();
        Check.That(keyedAgain).IsSameReferenceAs(keyed);
        Check.That(tool.GetKeyedService(typeof(TestScoped),"missing")).IsNull();
        Check.ThatCode(() => tool.GetRequiredKeyedService(typeof(TestScoped),"missing")).Throws<InvalidOperationException>();
    }

    [Test]
    public void GetService_UsesDefaultScope()
    {
        using ITool tool = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,services) => services.AddScoped<TestScoped>()).Build();
        using ToolServiceScope namedScope = tool.CreateToolServiceScope("named")!;

        TestScoped? defaultInstance = tool.GetRequiredService(typeof(TestScoped)) as TestScoped;
        TestScoped? defaultScopeInstance = tool.GetToolServiceScope()!.ServiceProvider.GetRequiredService<TestScoped>();
        TestScoped namedInstance = namedScope.ServiceProvider.GetRequiredService<TestScoped>();

        Check.That(defaultInstance).IsSameReferenceAs(defaultScopeInstance);
        Check.That(namedInstance).Not.IsSameReferenceAs(defaultInstance);
    }

    [Test]
    public void ServiceResolution_FrameworkInterfacesForwardWithoutAccessKey()
    {
        using ITool tool = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,services) =>
        {
            services.AddSingleton<TestSingleton>();
            services.AddKeyedSingleton<TestSingleton>("service-key");
        }).Build();

        IServiceProvider services = tool;
        IKeyedServiceProvider keyedServices = tool;
        ISupportRequiredService requiredServices = tool;

        Check.That(services.GetService(typeof(TestSingleton))).IsNotNull();
        Check.That(keyedServices.GetKeyedService(typeof(TestSingleton),"service-key")).IsNotNull();
        Check.That(requiredServices.GetRequiredService(typeof(TestSingleton))).IsNotNull();
        Check.That(keyedServices.GetRequiredKeyedService(typeof(TestSingleton),"service-key")).IsNotNull();
    }

    [Test]
    public void ServiceResolution_AccessDeniedWhenLocked()
    {
        using Tool tool = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,services) => services.AddSingleton<TestSingleton>()).Build() as Tool
            ?? throw new InvalidOperationException();

        Check.That(tool.RegisterManager(KeyM)).IsTrue();
        Check.That(tool.Lock(KeyM)).IsTrue();

        AccessKey invalidKey = new CommandKey(Array.Empty<Byte>());
        IServiceProvider services = tool;
        IKeyedServiceProvider keyedServices = tool;
        ISupportRequiredService requiredServices = tool;

        Check.That(tool.GetService(typeof(TestSingleton),invalidKey)).IsNull();
        Check.That(tool.GetKeyedService(typeof(TestSingleton),"service-key",invalidKey)).IsNull();
        Check.ThatCode(() => tool.GetRequiredService(typeof(TestSingleton),invalidKey)).Throws<SecurityException>();
        Check.ThatCode(() => tool.GetRequiredKeyedService(typeof(TestSingleton),"service-key",invalidKey)).Throws<SecurityException>();
        Check.That(services.GetService(typeof(TestSingleton))).IsNull();
        Check.That(keyedServices.GetKeyedService(typeof(TestSingleton),"service-key")).IsNull();
        Check.ThatCode(() => requiredServices.GetRequiredService(typeof(TestSingleton))).Throws<SecurityException>();
        Check.ThatCode(() => keyedServices.GetRequiredKeyedService(typeof(TestSingleton),"service-key")).Throws<SecurityException>();
    }

    [Test]
    public void ServiceResolution_AuthorizedWhenLocked()
    {
        using Tool tool = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,services) =>
        {
            services.AddSingleton<TestSingleton>();
            services.AddKeyedSingleton<TestSingleton>("service-key");
        }).Build() as Tool ?? throw new InvalidOperationException();

        Check.That(tool.RegisterManager(KeyM)).IsTrue();
        ExecutiveKey? executiveKey = tool.RequestAccess(new ManagementRequest(KeyM!)) as ExecutiveKey;
        Check.That(executiveKey).IsNotNull();
        Check.That(tool.Lock(KeyM)).IsTrue();

        Check.That(tool.GetService(typeof(TestSingleton),executiveKey)).IsNotNull();
        Check.That(tool.GetKeyedService(typeof(TestSingleton),"service-key",executiveKey)).IsNotNull();
        Check.That(tool.GetRequiredService(typeof(TestSingleton),executiveKey)).IsNotNull();
        Check.That(tool.GetRequiredKeyedService(typeof(TestSingleton),"service-key",executiveKey)).IsNotNull();
    }
}
