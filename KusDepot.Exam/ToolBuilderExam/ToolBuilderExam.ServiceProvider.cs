namespace KusDepot.Exams.Tools;

[TestFixture]
public partial class ToolBuilderExam
{
    [Test]
    public void UseServiceProvider_ITool_ExternalOwnership_PreservesProvider()
    {
        using ITool provider = ToolBuilderFactory.CreateBuilder().Build();
        using ITool tool = ToolBuilderFactory.CreateBuilder()
            .UseServiceProvider(provider,ToolServiceProviderOwnership.External)
            .Build();

        ToolServiceProvider? serviceprovider = GetToolServiceProvider(tool);

        Check.That(serviceprovider).IsNotNull();
        Check.That(serviceprovider!.ServiceProvider).IsSameReferenceAs(provider);

        tool.Dispose();

        Check.That(provider.GetDisposed()).IsFalse();
    }

    [Test]
    public void UseServiceProvider_ITool_ToolOwnership_DisposesProvider()
    {
        ITool provider = ToolBuilderFactory.CreateBuilder().Build();
        ITool tool = ToolBuilderFactory.CreateBuilder()
            .UseServiceProvider(provider,ToolServiceProviderOwnership.Tool)
            .Build();

        ToolServiceProvider? serviceprovider = GetToolServiceProvider(tool);

        Check.That(serviceprovider).IsNotNull();
        Check.That(serviceprovider!.ServiceProvider).IsSameReferenceAs(provider);
        Check.That(provider.GetDisposed()).IsFalse();

        tool.Dispose();

        Check.That(provider.GetDisposed()).IsTrue();
    }

    private static ToolServiceProvider? GetToolServiceProvider(ITool tool)
        => typeof(Tool).GetField("ToolServiceProvider",BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(tool) as ToolServiceProvider;
}
