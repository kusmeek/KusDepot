namespace KusDepot.DataPodExams;

[TestFixture] [NonParallelizable]
public class DataConfigsExam
{
    private IToolGenericHost? Host;
    private DataPodServices.DataConfigs.IDataConfigs? DataConfigs;

    public String? AdminUserName;
    public String? AdminUserPass;
    public String? AdminToken;
    public String? ReadUserName;
    public String? ReadUserPass;
    public String? ReadToken;
    public String? WriteUserName;
    public String? WriteUserPass;
    public String? WriteToken;

    public String? ClientID;
    public String? TenantID;
    public String? Authority;
    public String[]? Scope;


    [OneTimeSetUp]
    public async Task Calibrate()
    {
        Check.That(this.LoadSetup()).IsTrue();

        IToolGenericHostBuilder tb = ToolBuilderFactory.CreateGenericHostBuilder();

        tb.Builder.UseOrleansClient((b) =>
        {
            b.UseLocalhostClustering(gatewayPort:30003,serviceId:"default",clusterId:"default"); b.UseTransactions();
        });

        Host = await tb.BuildGenericHostAsync(); await Host.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token);

        IGrainFactory? gf = Host.Services.GetService<IGrainFactory>();

        DataPodServices.DataConfigs.IDataConfigs? tg = gf?.GetGrain<DataPodServices.DataConfigs.IDataConfigs>(Guid.NewGuid().ToString());

        Check.That(tg).IsNotNull(); DataConfigs = tg;

        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();

        this.AdminToken = _.AcquireTokenByUsernamePassword(this.Scope,this.AdminUserName,this.AdminUserPass).ExecuteAsync().Result.AccessToken;

        this.ReadToken = _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync().Result.AccessToken;

        this.WriteToken = _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync().Result.AccessToken;
    }

    [OneTimeTearDown]
    public async Task Complete() { await Host!.DisposeAsync(); }

    [Test] [Order(3)]
    public void GetAuthorizedReadSilo()
    {
        Check.That(this.DataConfigs!.GetAuthorizedReadSilo(String.Empty,null,null).Result).IsNull();

        Check.That(this.DataConfigs!.GetAuthorizedReadSilo(this.ReadToken!,null,null).Result!.CatalogName).IsEqualTo("PublicCatalog");
    }

    [Test] [Order(4)]
    public void GetAuthorizedWriteSilo()
    {
        Check.That(this.DataConfigs!.GetAuthorizedWriteSilo(String.Empty,null,null).Result).IsNull();

        Check.That(this.DataConfigs!.GetAuthorizedWriteSilo(this.ReadToken!,null,null).Result).IsNull();

        Check.That(this.DataConfigs!.GetAuthorizedWriteSilo(this.WriteToken!,null,null).Result!.CatalogName).IsEqualTo("PublicCatalog");
    }

    [Test] [Order(2)]
    public void GetStorageSilos()
    {
        Check.That(this.DataConfigs!.GetStorageSilos(String.Empty,null,null).Result).IsNull();

        HashSet<StorageSilo>? _ = this.DataConfigs!.GetStorageSilos(this.AdminToken!,null,null).Result;

        Check.That(_!.Count).IsEqualTo(4);

        Check.That(_.Any(_=>String.Equals(_.Name,"Private",StringComparison.Ordinal))).IsTrue();
    }

    [Test] [Order(1)]
    public void SetStorageSilos()
    {
        HashSet<StorageSilo>? i = this.DataConfigs!.GetStorageSilos(this.AdminToken!,null,null).Result;

        Check.That(i!.Count).IsEqualTo(1);

        Check.That(this.DataConfigs.SetStorageSilos(this.SampleSilos(),String.Empty,null,null).Result).IsFalse();

        i.UnionWith(this.SampleSilos());

        Check.That(this.DataConfigs.SetStorageSilos(i,this.AdminToken!,null,null).Result).IsTrue();

        Check.That(this.DataConfigs.GetStorageSilos(this.AdminToken!,null,null).Result!.Count).IsEqualTo(4);

        Check.That(this.DataConfigs.GetStorageSilos(this.AdminToken!,null,null).Result!.Any(s=>String.Equals(s.Name,"Private",StringComparison.Ordinal))).IsTrue();
    }

    private HashSet<StorageSilo> SampleSilos()
    {
        return new HashSet<StorageSilo>() { new StorageSilo() { Name = "Silo2" , ConnectionString = "connection1" } , new StorageSilo() { Name = "Silo3" } ,
            new StorageSilo() { Name = "Private" , AppClientID = this.ClientID! , TenantID = this.TenantID! , CatalogName = "PrivateCatalog" }
        };
    }


    private Boolean LoadSetup()
    {
        try
        {
            XmlDocument d = new XmlDocument(); d.Load(Environment.CurrentDirectory + @"\EntraSetup.xml"); XmlNode? n = d.SelectSingleNode("EntraSetup");

            this.Authority     = n!.SelectSingleNode("Authority")!.InnerText;
            this.TenantID      = n.SelectSingleNode("TenantID")!.InnerText;
            this.ClientID      = n.SelectSingleNode("ClientID")!.InnerText;
            this.Scope         = new List<String>(){n.SelectSingleNode("Scope")!.InnerText}.ToArray();
            this.AdminUserName = n.SelectSingleNode("AdminUserName")!.InnerText;
            this.AdminUserPass = n.SelectSingleNode("AdminUserPass")!.InnerText;
            this.ReadUserName  = n.SelectSingleNode("ReadUserName")!.InnerText;
            this.ReadUserPass  = n.SelectSingleNode("ReadUserPass")!.InnerText;
            this.WriteUserName = n.SelectSingleNode("WriteUserName")!.InnerText;
            this.WriteUserPass = n.SelectSingleNode("WriteUserPass")!.InnerText;

            return true;
        }
        catch (Exception) { return false; }
    }
}