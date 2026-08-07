namespace KusDepot.Exams.Tools;

internal static class ToolBuilderFactoryExamHelpers
{
    internal static readonly Guid ToolId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    internal static readonly Guid ToolHostId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    internal static readonly Guid ToolGenericHostId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    internal static readonly Guid ToolWebHostId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    internal static readonly Guid ToolAspireHostId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    internal static FactoryTestTool CreateTool(IAccessManager? accessmanager , ToolServiceProvider? services , IConfiguration? configuration)
        => new(accessmanager:accessmanager,services:services,configuration:configuration);

    internal static FactoryTestToolHost CreateToolHost(IAccessManager? accessmanager , ToolServiceProvider? services , IConfiguration? configuration , Guid? lifeid , IToolHostLifetime? lifetime)
        => new(accessmanager:accessmanager,services:services,configuration:configuration,lifeid:lifeid,lifetime:lifetime);

    internal static FactoryTestToolGenericHost CreateToolGenericHost(IAccessManager? accessmanager , ToolServiceProvider? services , IConfiguration? configuration , IHost? host , Guid? lifeid , IToolHostLifetime? lifetime , ILoggerFactory? logger)
        => new(accessmanager:accessmanager,services:services,configuration:configuration,host:host,lifeid:lifeid,lifetime:lifetime,logger:logger);

    internal static FactoryTestToolWebHost CreateToolWebHost(IAccessManager? accessmanager , ToolServiceProvider? services , IConfiguration? configuration , Guid? lifeid , IToolHostLifetime? lifetime , ILoggerFactory? logger , ICollection<String>? urls , IHost? webapplication)
        => new(accessmanager:accessmanager,services:services,configuration:configuration,lifeid:lifeid,lifetime:lifetime,logger:logger,urls:urls,webapplication:webapplication);

    internal static FactoryTestToolAspireHost CreateToolAspireHost(IAccessManager? accessmanager , ToolServiceProvider? services , IConfiguration? configuration , Guid? lifeid , IToolHostLifetime? lifetime , ILoggerFactory? logger , IHost? aspireapplication)
        => new(accessmanager:accessmanager,services:services,configuration:configuration,lifeid:lifeid,lifetime:lifetime,logger:logger,aspireapplication:aspireapplication);

    internal static void ConfigureToolId(ToolBuilderContext context , ITool tool) => _ = tool.SetID(ToolId);

    internal static void ConfigureToolHostId(ToolBuilderContext context , ITool tool) => _ = tool.SetID(ToolHostId);

    internal static void ConfigureToolGenericHostId(ToolBuilderContext context , ITool tool) => _ = tool.SetID(ToolGenericHostId);

    internal static void ConfigureToolWebHostId(ToolBuilderContext context , ITool tool) => _ = tool.SetID(ToolWebHostId);

    internal static void ConfigureToolAspireHostId(ToolBuilderContext context , ITool tool) => _ = tool.SetID(ToolAspireHostId);
}

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolBuilderFactoryExam
{
    [Test]
    public void BuildOpenFactory()
    {
        var _ = ToolBuilderFactory.CreateBuilder()
            .ConfigureTool(ToolBuilderFactoryExamHelpers.ConfigureToolId)
            .Build<FactoryTestTool>(ToolBuilderFactoryExamHelpers.CreateTool);

        Check.That(_).IsNotNull();
        Check.That(_.GetType()).IsEqualTo(typeof(FactoryTestTool));
        Check.That(_.GetType().IsAssignableTo(typeof(ITool))).IsTrue();
        Check.That(_.GetID()).IsEqualTo(ToolBuilderFactoryExamHelpers.ToolId);
    }

    [Test]
    public async Task BuildAsyncOpenFactory()
    {
        var _ = await ToolBuilderFactory.CreateBuilder()
            .RegisterCommand("Work",new CommandTestR())
            .EnableAllCommands()
            .ConfigureTool(ToolBuilderFactoryExamHelpers.ConfigureToolId)
            .BuildAsync<FactoryTestTool>(ToolBuilderFactoryExamHelpers.CreateTool);

        Check.That(_).IsNotNull();
        Check.That(_.GetType()).IsEqualTo(typeof(FactoryTestTool));
        Check.That(_.GetID()).IsEqualTo(ToolBuilderFactoryExamHelpers.ToolId);
        Check.That(await _.Activate()).IsTrue();
        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).IsNotNull();
    }

    [Test]
    public void BuildHostOpenFactory()
    {
        var builder = ToolBuilderFactory.CreateHostBuilder();

        builder.ConfigureTool(ToolBuilderFactoryExamHelpers.ConfigureToolHostId);

        var _ = builder.BuildHost<FactoryTestToolHost>(ToolBuilderFactoryExamHelpers.CreateToolHost);

        Check.That(_).IsNotNull();
        Check.That(_.GetType()).IsEqualTo(typeof(FactoryTestToolHost));
        Check.That(_.GetType().IsAssignableTo(typeof(IToolHost))).IsTrue();
        Check.That(_.GetID()).IsEqualTo(ToolBuilderFactoryExamHelpers.ToolHostId);
    }

    [Test]
    public async Task BuildHostAsyncOpenFactory()
    {
        var builder = ToolBuilderFactory.CreateHostBuilder();

        builder.ConfigureTool(ToolBuilderFactoryExamHelpers.ConfigureToolHostId);

        var _ = await builder.BuildHostAsync<FactoryTestToolHost>(ToolBuilderFactoryExamHelpers.CreateToolHost);

        Check.That(_).IsNotNull();
        Check.That(_.GetType()).IsEqualTo(typeof(FactoryTestToolHost));
        Check.That(_.GetID()).IsEqualTo(ToolBuilderFactoryExamHelpers.ToolHostId);
        Check.That(await _.Activate()).IsTrue();
        await _.Deactivate();
        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public void BuildGenericHostOpenFactory()
    {
        var builder = ToolBuilderFactory.CreateGenericHostBuilder();

        builder.ConfigureTool(ToolBuilderFactoryExamHelpers.ConfigureToolGenericHostId);

        var _ = builder.BuildGenericHost<FactoryTestToolGenericHost>(ToolBuilderFactoryExamHelpers.CreateToolGenericHost);

        Check.That(_).IsNotNull();
        Check.That(_.GetType()).IsEqualTo(typeof(FactoryTestToolGenericHost));
        Check.That(_.GetType().IsAssignableTo(typeof(IToolGenericHost))).IsTrue();
        Check.That(_.GetID()).IsEqualTo(ToolBuilderFactoryExamHelpers.ToolGenericHostId);
    }

    [Test]
    public async Task BuildGenericHostAsyncOpenFactory()
    {
        var builder = ToolBuilderFactory.CreateGenericHostBuilder();

        builder.ConfigureTool(ToolBuilderFactoryExamHelpers.ConfigureToolGenericHostId);

        var _ = await builder.BuildGenericHostAsync<FactoryTestToolGenericHost>(ToolBuilderFactoryExamHelpers.CreateToolGenericHost);

        Check.That(_).IsNotNull();
        Check.That(_.GetType()).IsEqualTo(typeof(FactoryTestToolGenericHost));
        Check.That(_.GetID()).IsEqualTo(ToolBuilderFactoryExamHelpers.ToolGenericHostId);
        Check.That(await _.Activate()).IsTrue();
        await _.Deactivate();
        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public void BuildWebHostOpenFactory()
    {
        var builder = ToolBuilderFactory.CreateWebHostBuilder()
            .UseSlimBuilder(args:null);

        builder.ConfigureTool(ToolBuilderFactoryExamHelpers.ConfigureToolWebHostId);

        var _ = builder.BuildWebHost<FactoryTestToolWebHost>(ToolBuilderFactoryExamHelpers.CreateToolWebHost);

        Check.That(_).IsNotNull();
        Check.That(_.GetType()).IsEqualTo(typeof(FactoryTestToolWebHost));
        Check.That(_.GetType().IsAssignableTo(typeof(IToolWebHost))).IsTrue();
        Check.That(_.GetID()).IsEqualTo(ToolBuilderFactoryExamHelpers.ToolWebHostId);
    }

    [Test]
    public async Task BuildWebHostAsyncOpenFactory()
    {
        var builder = ToolBuilderFactory.CreateWebHostBuilder()
            .UseSlimBuilder(args:null);

        builder.ConfigureTool(ToolBuilderFactoryExamHelpers.ConfigureToolWebHostId);

        var _ = await builder.BuildWebHostAsync<FactoryTestToolWebHost>(ToolBuilderFactoryExamHelpers.CreateToolWebHost);

        Check.That(_).IsNotNull();
        Check.That(_.GetType()).IsEqualTo(typeof(FactoryTestToolWebHost));
        Check.That(_.GetID()).IsEqualTo(ToolBuilderFactoryExamHelpers.ToolWebHostId);
        Check.That(await _.Activate()).IsTrue();
        await _.Deactivate();
        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public void BuildAspireHostOpenFactory()
    {
        var builder = ToolBuilderFactory.CreateAspireHostBuilder();

        builder.ConfigureTool(ToolBuilderFactoryExamHelpers.ConfigureToolAspireHostId);

        var _ = builder.BuildAspireHost<FactoryTestToolAspireHost>(ToolBuilderFactoryExamHelpers.CreateToolAspireHost);

        Check.That(_).IsNotNull();
        Check.That(_.GetType()).IsEqualTo(typeof(FactoryTestToolAspireHost));
        Check.That(_.GetType().IsAssignableTo(typeof(IToolAspireHost))).IsTrue();
        Check.That(_.GetID()).IsEqualTo(ToolBuilderFactoryExamHelpers.ToolAspireHostId);
    }

    [Test]
    public async Task BuildAspireHostAsyncOpenFactory()
    {
        var builder = ToolBuilderFactory.CreateAspireHostBuilder();

        builder.ConfigureTool(ToolBuilderFactoryExamHelpers.ConfigureToolAspireHostId);

        var _ = await builder.BuildAspireHostAsync<FactoryTestToolAspireHost>(ToolBuilderFactoryExamHelpers.CreateToolAspireHost);

        Check.That(_).IsNotNull();
        Check.That(_.GetType()).IsEqualTo(typeof(FactoryTestToolAspireHost));
        Check.That(_.GetID()).IsEqualTo(ToolBuilderFactoryExamHelpers.ToolAspireHostId);
        Check.That(await _.Activate()).IsTrue();
        await _.Deactivate();
        Check.That(_.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }
}
