namespace KusDepot.DaprSidecar;

[NonParallelizable()]
public class DaprSidecarExam
{
    private IToolHost? host;

    [OneTimeSetUp]
    public async Task Calibrate()
    {
        Environment.SetEnvironmentVariable("DAPR_GRPC_PORT","10000");

        IToolWebHostBuilder t = ToolBuilderFactory.CreateWebHostBuilder();

        t.Builder.Services.AddActors(o => {});

        t.ConfigureWebApplication((a) =>
        {
            a.Urls.Add("http://localhost:26000");

            a.MapActorsHandlers();
        });

        host = t.BuildWebHost(); Check.That(await host.StartHostAsync()).IsTrue(); await Task.Delay(TimeSpan.FromSeconds(10));
    }

    [OneTimeTearDown]
    public async Task Complete() { await (host?.DisposeAsync().AsTask() ?? Task.CompletedTask); }

    [Test]
    [Order(1)]
    public async Task CheckHealth()
    {
        DaprClientBuilder? _0; DaprClient? _1;

        _0 = new(); _0.UseHttpEndpoint("http://localhost:11000");

        _1 = _0.Build(); Check.That(await _1.CheckOutboundHealthAsync()).IsTrue();
    }

    [Test]
    [Order(2)]
    public async Task StateManagement()
    {
        DaprClientBuilder? _0 = new(); _0.UseGrpcEndpoint("http://localhost:10000"); DaprClient? _1;

        DataSetItem ds = DataItemGenerator.CreateDataSet(12); String sc = "volatilememory"; String k = "ObjectID";

        _1 = _0.Build(); await _1.SaveStateAsync(sc,k,ds.ToString());

        Check.That(DataSetItem.Parse(await _1.GetStateAsync<String>(sc,k))).IsEqualTo(ds);
    }

    [Test]
    [Order(3)]
    [NonParallelizable()]
    public async Task WorkflowInvocation()
    {
        var b = Host.CreateDefaultBuilder().ConfigureServices(s =>
        {
            s.AddDaprWorkflow();
        }).Build();

        var wfclient = b.Services.GetRequiredService<DaprWorkflowClient>(); Guid detailid = Guid.NewGuid();

        var details = new CommandDetails().SetHandle("Workflow").SetID(detailid);

        await wfclient.ScheduleNewWorkflowAsync(name: nameof(ToolWorkflow),instanceId: detailid.ToString(),input: details.ToKusDepotCab());

        WorkflowState state = await wfclient.WaitForWorkflowStartAsync(instanceId:detailid.ToString());

        Check.That(state.RuntimeStatus).Equals(WorkflowRuntimeStatus.Running);

        state = await wfclient.WaitForWorkflowCompletionAsync(instanceId: detailid.ToString());

        Check.That(state.RuntimeStatus).Equals(WorkflowRuntimeStatus.Completed);

        var outcab = state.ReadOutputAs<KusDepotCab>(); var tout = outcab?.GetToolOutput();

        Check.That(Equals(tout?.ID,detailid)).IsTrue();

        Check.That(tout?.Data).IsInstanceOf<Guid>().Which.HasDifferentValueThan(Guid.Empty);

        Check.That(await wfclient.PurgeInstanceAsync(instanceId: detailid.ToString())).IsTrue();
    }

    [Test]
    [Order(4)]
    [NonParallelizable()]
    public async Task DataWorkflowInvocation()
    {
        var b = Host.CreateDefaultBuilder().ConfigureServices(s =>
        {
            s.AddDaprWorkflow();
        }).Build();

        var wfclient = b.Services.GetRequiredService<DaprWorkflowClient>(); Guid detailid = Guid.NewGuid();

        var details = new CommandDetails().SetHandle("Workflow").SetID(detailid);

        await wfclient.ScheduleNewWorkflowAsync(name: nameof(ToolDataWorkflow),instanceId: detailid.ToString(),input: details.ToKusDepotCab());

        WorkflowState state = await wfclient.WaitForWorkflowStartAsync(instanceId:detailid.ToString());

        Check.That(state.RuntimeStatus).Equals(WorkflowRuntimeStatus.Running);

        state = await wfclient.WaitForWorkflowCompletionAsync(instanceId: detailid.ToString());

        Check.That(state.RuntimeStatus).Equals(WorkflowRuntimeStatus.Completed);

        var outcab = state.ReadOutputAs<KusDepotCab>(); var tout = outcab?.GetToolOutput();

        Check.That(Equals(tout?.ID,detailid)).IsTrue();

        Check.That(tout?.Data).IsInstanceOf<DataSetItem>();

        Check.That(await wfclient.PurgeInstanceAsync(instanceId: detailid.ToString())).IsTrue();
    }
}