namespace KusDepot.FabricExams.Data;

[TestFixture] [Parallelizable]
public class DataConfigsExam
{
    public IDataConfigs? Proxy;
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
    public ActorId? ActorId;

    [OneTimeSetUp]
    public void Calibrate()
    {
        Check.That(this.LoadSetup()).IsTrue();

        this.ActorId = new("DataConfigsExam"); this.Proxy = ActorProxy.Create<IDataConfigs>(ActorId,ServiceLocators.DataConfigsService);

        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();

        this.AdminToken = _.AcquireTokenByUsernamePassword(this.Scope,this.AdminUserName,this.AdminUserPass).ExecuteAsync().Result.AccessToken;

        this.ReadToken = _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync().Result.AccessToken;

        this.WriteToken = _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync().Result.AccessToken;
    }

    [OneTimeTearDown]
    public void Complete()
    {
        ActorServiceProxy.Create(ServiceLocators.DataConfigsService,ActorId!).DeleteActorAsync(ActorId,CancellationToken.None).GetAwaiter().GetResult();
    }

    [Test] [Order(3)]
    public void GetAuthorizedReadSilo()
    {
        Check.That(this.Proxy!.GetAuthorizedReadSilo(String.Empty,null,null).Result).IsNull();

        Check.That(this.Proxy!.GetAuthorizedReadSilo(this.ReadToken!,null,null).Result!.CatalogName).IsEqualTo("PublicCatalog");
    }

    [Test] [Order(4)]
    public void GetAuthorizedWriteSilo()
    {
        Check.That(this.Proxy!.GetAuthorizedWriteSilo(String.Empty,null,null).Result).IsNull();

        Check.That(this.Proxy!.GetAuthorizedWriteSilo(this.ReadToken!,null,null).Result).IsNull();

        Check.That(this.Proxy!.GetAuthorizedWriteSilo(this.WriteToken!,null,null).Result!.CatalogName).IsEqualTo("PublicCatalog");
    }

    [Test] [Order(2)]
    public void GetStorageSilos()
    {
        Check.That(this.Proxy!.GetStorageSilos(String.Empty,null,null).Result).IsNull();

        HashSet<StorageSilo>? _ = this.Proxy!.GetStorageSilos(this.AdminToken!,null,null).Result;

        Check.That(_!.Count).IsEqualTo(4);

        Check.That(_.Any(_=>String.Equals(_.Name,"Private",StringComparison.Ordinal))).IsTrue();
    }

    [Test] [Order(1)]
    public void SetStorageSilos()
    {
        HashSet<StorageSilo>? i = this.Proxy!.GetStorageSilos(this.AdminToken!,null,null).Result;

        Check.That(i!.Count).IsEqualTo(1);

        Check.That(this.Proxy.SetStorageSilos(this.SampleSilos(),String.Empty,null,null).Result).IsFalse();

        i.UnionWith(this.SampleSilos());

        Check.That(this.Proxy.SetStorageSilos(i,this.AdminToken!,null,null).Result).IsTrue();

        Check.That(this.Proxy.GetStorageSilos(this.AdminToken!,null,null).Result!.Count).IsEqualTo(4);

        Check.That(this.Proxy.GetStorageSilos(this.AdminToken!,null,null).Result!.Any(s=>String.Equals(s.Name,"Private",StringComparison.Ordinal))).IsTrue();
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