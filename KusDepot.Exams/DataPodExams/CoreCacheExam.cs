namespace KusDepot.DataPodExams;

[TestFixture] [NonParallelizable]
public class CoreCacheExam
{
    public String? ID;
    public DataItem? Item;
    public ManagementKey? Key;
    private IToolGenericHost? Host;
    public String? ConnectionString;
    private DataPodServices.CoreCache.ICoreCache? CoreCache;

    [OneTimeSetUp]
    public async Task Calibrate()
    {
        IToolGenericHostBuilder tb = ToolBuilderFactory.CreateGenericHostBuilder();

        tb.Builder.UseOrleansClient((b) =>
        {
            b.UseLocalhostClustering(gatewayPort:30003,serviceId:"default",clusterId:"default"); b.UseTransactions();
        });

        Host = await tb.BuildGenericHostAsync(); await Host.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token);

        IGrainFactory? gf = Host.Services.GetService<IGrainFactory>();

        DataPodServices.CoreCache.ICoreCache? tg = gf?.GetGrain<DataPodServices.CoreCache.ICoreCache>(Guid.NewGuid().ToString());

        Check.That(tg).IsNotNull(); CoreCache = tg;

        this.Item = new DataSetItem(content:new HashSet<DataItem>(){new TextItem(DataGenerator.GenerateUnicodeString(500000))});
        this.Key = this.Item.CreateManagementKey("test"); Check.That(this.Item.SignData("HashCode",this.Key)).IsNotNull();
        this.ID = this.Item!.GetID().ToString();

        this.LoadSetup();
    }

    [OneTimeTearDown]
    public async Task Complete() { await Host!.DisposeAsync(); }

    [Test] [Order(5)]
    public void Delete()
    {
        Check.That(this.CoreCache!.Delete(this.ID!,null,null).Result).IsTrue();
    }

    [Test] [Order(6)]
    public void ExistsAfterDelete()
    {
        Check.That(this.CoreCache!.Exists(this.ID!,null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(3)]
    public void ExistsAfterStore()
    {
        Check.That(this.CoreCache!.Exists(this.ID!,null,null).Result).IsEqualTo(true);
    }

    [Test] [Order(1)]
    public void ExistsBeforeStore()
    {
        Check.That(this.CoreCache!.Exists(this.ID!,null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(4)]
    public async Task Get()
    {
        DataItem? _d = DataItem.Parse(this.CoreCache!.Get(this.ID!,null,null).Result!,null);

        Check.That(await _d!.VerifyData("HashCode",this.Key!)).IsTrue();

        Check.That(this.Item!.Equals(_d)).IsTrue();
    }

    [Test] [Order(2)]
    public void Store()
    {
        Check.That(this.CoreCache!.Store(this.ID,this.Item!.ToString(),null,null).Result).IsTrue();
    }

    private Boolean LoadSetup()
    {
        try
        {
            XmlDocument d = new XmlDocument(); d.Load(Environment.CurrentDirectory + @"\EntraSetup.xml"); XmlNode? n = d.SelectSingleNode("EntraSetup");

            this.ConnectionString = n!.SelectSingleNode("ConnectionString")!.InnerText;

            return true;
        }
        catch (Exception) { return false; }
    }
}