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
}