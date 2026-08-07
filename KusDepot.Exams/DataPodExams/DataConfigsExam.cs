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

        this.AdminToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.AdminUserName,this.AdminUserPass).ExecuteAsync()).AccessToken;

        this.ReadToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync()).AccessToken;

        this.WriteToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync()).AccessToken;
    }

    [OneTimeTearDown]
    public async Task Complete() { await Host!.DisposeAsync(); }

    [Test] [Order(3)]
    public async Task GetAuthorizedReadSilo()
    {
        Check.That(await this.DataConfigs!.GetAuthorizedReadSilo(String.Empty,null,null)).IsNull();

        Check.That((await this.DataConfigs!.GetAuthorizedReadSilo(this.ReadToken!,null,null))!.CatalogName).IsEqualTo("PublicCatalog");
    }

    [Test] [Order(4)]
    public async Task GetAuthorizedWriteSilo()
    {
        Check.That(await this.DataConfigs!.GetAuthorizedWriteSilo(String.Empty,null,null)).IsNull();

        Check.That(await this.DataConfigs!.GetAuthorizedWriteSilo(this.ReadToken!,null,null)).IsNull();

        Check.That((await this.DataConfigs!.GetAuthorizedWriteSilo(this.WriteToken!,null,null))!.CatalogName).IsEqualTo("PublicCatalog");
    }

    [Test] [Order(2)]
    public async Task GetStorageSilos()
    {
        Check.That(await this.DataConfigs!.GetStorageSilos(String.Empty,null,null)).IsNull();

        HashSet<StorageSilo>? _ = await this.DataConfigs!.GetStorageSilos(this.AdminToken!,null,null);

        Check.That(_!.Count).IsEqualTo(4);

        Check.That(_.Any(_=>String.Equals(_.Name,"Private",StringComparison.Ordinal))).IsTrue();
    }

    [Test] [Order(5)]
    public async Task IsAdmin()
    {
        Check.That(await this.DataConfigs!.IsAdmin(this.ReadToken!,null,null)).IsFalse();

        Check.That(await this.DataConfigs!.IsAdmin(this.AdminToken!,null,null)).IsTrue();
    }

    [Test] [Order(1)]
    public async Task SetStorageSilos()
    {
        HashSet<StorageSilo>? s = await this.DataConfigs!.GetStorageSilos(this.AdminToken!,null,null);

        Check.That(s!.Count).IsEqualTo(1);

        Check.That(await this.DataConfigs.SetStorageSilos(this.SampleSilos(),String.Empty,null,null)).IsFalse();

        s.UnionWith(this.SampleSilos());

        Check.That(await this.DataConfigs.SetStorageSilos(s,this.AdminToken!,null,null)).IsTrue();

        Check.That((await this.DataConfigs.GetStorageSilos(this.AdminToken!,null,null))!.Count).IsEqualTo(4);

        Check.That((await this.DataConfigs.GetStorageSilos(this.AdminToken!,null,null))!.Any(s=>String.Equals(s.Name,"Private",StringComparison.Ordinal))).IsTrue();
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