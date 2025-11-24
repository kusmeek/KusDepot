namespace KusDepot.FabricExams.Grains;

[TestFixture] [Parallelizable(ParallelScope.Self)]
public class ToolGrainExam
{
    private Guid? Oid;
    private AccessKey? Key;
    private ITestToolGrain? Grain;
    private IToolGenericHost? Host;

    [OneTimeSetUp]
    public async Task Calibrate()
    {
        IToolGenericHostBuilder tb = ToolBuilderFactory.CreateGenericHostBuilder();

        tb.Builder.UseOrleansClient((b) =>
        {
            b.UseLocalhostClustering(gatewayPort:30000,serviceId:"default",clusterId:"default"); b.UseTransactions();
        });

        Host = tb.BuildGenericHost(); await Host.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token);

        IGrainFactory? gf = Host.Services.GetService<IGrainFactory>();

        ITestToolGrain? tg = gf?.GetGrain<ITestToolGrain>(Guid.NewGuid().ToString());

        Check.That(tg).IsNotNull(); Grain = tg;
    }

    [OneTimeTearDown]
    public async Task Complete() { await Host!.DisposeAsync(); }

    [Test] [Order(1)]
    public async Task RequestAccess()
    {
        Key = (await this.Grain!.RequestAccess(new StandardRequest("RequestAccess"))) as ClientKey;

        Check.That(Key).IsNotNull();
    }

    [Test] [Order(2)]
    public async Task ExecuteCommand()
    {
        Oid = await this.Grain!.ExecuteCommand(new CommandDetails().SetHandle("CommandTest"),Key);

        Check.That(Oid).HasAValue().Which.HasDifferentValueThan(Guid.Empty);
    }

    [Test] [Order(3)]
    public async Task ExecuteCommandCab()
    {
        Oid = await this.Grain!.ExecuteCommandCab(new CommandDetails().SetHandle("CommandTest").ToKusDepotCab(),Key);

        Check.That(Oid).HasAValue().Which.HasDifferentValueThan(Guid.Empty);
    }

    [Test] [Order(4)]
    public async Task GetOutput()
    {
        Oid = await this.Grain!.ExecuteCommand(new CommandDetails().SetHandle("CommandTest"),Key);

        Check.That(await this.Grain!.GetOutput(Oid,Key)).IsNotNull().And.IsInstanceOf<Guid>().Which.HasDifferentValueThan(Guid.Empty);
    }

    [Test] [Order(5)]
    public async Task RevokeAccess()
    {
        Check.That(await this.Grain!.RevokeAccess(Key)).IsTrue();
    }
}