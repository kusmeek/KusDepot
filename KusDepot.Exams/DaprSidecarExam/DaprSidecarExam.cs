using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.DurableTask.Client.Grpc;

namespace KusDepot.FabricExams.DaprSidecar;

[NonParallelizable()]
public class DaprSidecarExam
{
    private IToolHost? actors;

    [OneTimeSetUp]
    public async Task Calibrate()
    {
        IToolWebHostBuilder t = ToolBuilderFactory.CreateWebHostBuilder();

        t.Builder.Services.AddActors(o =>
        {
            o.Actors.RegisterActor<DaprToolActor>();

            o.Actors.RegisterActor<DaprTestToolActor>();

            o.ReentrancyConfig = new ActorReentrancyConfig()
            {
                Enabled = true , MaxStackDepth = 32,
            };
        });

        t.ConfigureWebApplication((a) =>
        {
            a.MapPost("/toolsvc", () => Task.FromResult( JsonSerializer.Serialize(Guid.NewGuid())));

            a.Urls.Add("http://localhost:26000");

            a.MapActorsHandlers();
        });

        actors = t.BuildWebHost(); Check.That(await actors.StartHostAsync()).IsTrue(); await Task.Delay(TimeSpan.FromSeconds(10));
    }

    [OneTimeTearDown]
    public async Task Complete() { await (actors?.DisposeAsync().AsTask() ?? Task.CompletedTask); }

    [Test] [Order(1)]
    public async Task CheckHealth()
    {
        DaprClientBuilder? _0; DaprClient? _1;

        _0 = new(); _0.UseHttpEndpoint("http://localhost:11000");

        _1 = _0.Build(); Check.That( await _1.CheckOutboundHealthAsync() ).IsTrue();
    }

    [Test] [Order(2)]
    public async Task StateManagement()
    {
        DaprClientBuilder? _0 = new(); _0.UseGrpcEndpoint("http://localhost:10000"); DaprClient? _1;

        DataSetItem ds = DataItemGenerator.CreateDataSet(12); String sc = "volatilememory"; String k = "ObjectID";

        _1 = _0.Build(); await _1.SaveStateAsync(sc,k,ds.ToString());

        Check.That( DataSetItem.Parse(await _1.GetStateAsync<String>(sc,k))).IsEqualTo(ds);
    }

    [Test] [Order(3)]
    [NonParallelizable()]
    public async Task ServiceInvocation()
    {
        var _ = new DaprClientBuilder().UseHttpEndpoint("http://localhost:11000").Build();

        Check.That( await _.InvokeMethodAsync<Guid?>("toolworkflow","toolsvc") ).HasAValue();
    }

    [Test] [Order(4)]
    [NonParallelizable()]
    public async Task ActorInvocation()
    {
        IDaprToolActor __ = Dapr.Actors.Client.ActorProxy.Create<IDaprToolActor>(
                        new Dapr.Actors.ActorId(Guid.NewGuid().ToString()),"DaprTestToolActor",new ActorProxyOptions() { HttpEndpoint = "http://localhost:11000" });

        ClientKey? k = (await __.RequestAccess(new StandardRequest("RequestAccess"))) as ClientKey; Check.That(k).IsNotNull();

        Guid? o = await __.ExecuteCommand(new CommandDetails().SetHandle("CommandTest"),k); Check.That(o).HasAValue().Which.HasDifferentValueThan(Guid.Empty);

        o = await __.ExecuteCommandCab(new CommandDetails().SetHandle("CommandTest").ToKusDepotCab(),k); Check.That(o).HasAValue().Which.HasDifferentValueThan(Guid.Empty);

        Check.That(await __.GetOutput(o,k)).IsNotNull().And.IsInstanceOf<Guid>().Which.HasDifferentValueThan(Guid.Empty);

        Check.That(await __.RevokeAccess(k!)).IsTrue();

        Check.That(await __.ExecuteCommand(new CommandDetails().SetHandle("CommandTest"),k)).IsNull();
    }

    [Test] [Order(5)]
    [NonParallelizable()]
    public async Task WorkflowInvocation()
    {
        var dcopt = new GrpcDurableTaskClientOptions() { Channel = GrpcChannel.ForAddress("http://localhost:10000/") };

        var gdtc = new GrpcDurableTaskClient("gdtc",dcopt,new SerilogLoggerFactory(Log.Logger).CreateLogger("ToolWorkflow"));

        var wfclient = new DaprWorkflowClient(gdtc); Guid detailid = Guid.NewGuid();

        var details = new CommandDetails().SetHandle("Workflow").SetID(detailid);

        await wfclient.ScheduleNewWorkflowAsync(name:nameof(ToolWorkflow),instanceId:detailid.ToString(),input:details.ToKusDepotCab());

        WorkflowState state = await wfclient.WaitForWorkflowStartAsync(instanceId:detailid.ToString());

        Check.That(state.RuntimeStatus).Equals(WorkflowRuntimeStatus.Running);

        state = await wfclient.WaitForWorkflowCompletionAsync(instanceId:detailid.ToString());

        Check.That(state.RuntimeStatus).Equals(WorkflowRuntimeStatus.Completed);

        var outcab = state.ReadOutputAs<KusDepotCab>(); var tout = outcab?.GetToolOutput();

        Check.That(Equals(tout?.ID,detailid)).IsTrue();

        Check.That(tout?.Data).IsInstanceOf<Guid>().Which.HasDifferentValueThan(Guid.Empty);

        Check.That( await wfclient.PurgeInstanceAsync(instanceId:detailid.ToString()) ).IsTrue();
    }

    [Test] [Order(6)]
    [NonParallelizable()]
    public async Task DataWorkflowInvocation()
    {
        var dcopt = new GrpcDurableTaskClientOptions() { Channel = GrpcChannel.ForAddress("http://localhost:10000/") };

        var gdtc = new GrpcDurableTaskClient("gdtc",dcopt,new SerilogLoggerFactory(Log.Logger).CreateLogger("d"));

        var wfclient = new DaprWorkflowClient(gdtc); Guid detailid = Guid.NewGuid();

        var details = new CommandDetails().SetHandle("Workflow").SetID(detailid);

        await wfclient.ScheduleNewWorkflowAsync(name:nameof(ToolDataWorkflow),instanceId:detailid.ToString(),input:details.ToKusDepotCab());

        WorkflowState state = await wfclient.WaitForWorkflowStartAsync(instanceId:detailid.ToString());

        Check.That(state.RuntimeStatus).Equals(WorkflowRuntimeStatus.Running);

        state = await wfclient.WaitForWorkflowCompletionAsync(instanceId:detailid.ToString());

        Check.That(state.RuntimeStatus).Equals(WorkflowRuntimeStatus.Completed);

        var outcab = state.ReadOutputAs<KusDepotCab>(); var tout = outcab?.GetToolOutput();

        Check.That(Equals(tout?.ID,detailid)).IsTrue();

        Check.That(tout?.Data).IsInstanceOf<DataSetItem>();

        Check.That( await wfclient.PurgeInstanceAsync(instanceId:detailid.ToString()) ).IsTrue();
    }
}