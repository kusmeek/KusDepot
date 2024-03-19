namespace KusDepot.FabricExams;

[TestFixture]
public class ManagementExam
{
    public Ark? Ark;
    public IArkKeeper? Keeper;
    public IManagement? Proxy;
    public IDataConfigs? Config;
    public X509Certificate2? Cert;
    public HashSet<StorageSilo>? Silos;
    public HashSet<Guid>? Uni;

    public String? AdminUserName;
    public String? AdminUserPass;
    public String? AdminToken;
    public String? ClientID;
    public String? TenantID;
    public String? Authority;
    public String[]? Scope;
    public String? ConnectionString;

    [OneTimeSetUp]
    public void Calibrate()
    {
        Check.That(this.LoadSetup()).IsTrue();
        this.Ark = new Ark(); Check.That(this.Ark.AddUpdate(new Tool().GetDescriptor())).IsTrue();
        this.Keeper = ActorProxy.Create<IArkKeeper>(new ActorId("managementexam"),ServiceLocators.ArkKeeperService);
        Check.That(this.Keeper.StoreArk(Ark.GetBytes(this.Ark),null,null).Result).IsTrue();
        this.Proxy = ActorProxy.Create<IManagement>(new ActorId("managementexam"),ServiceLocators.ManagementService);
        this.Cert = new CertificateRequest("CN=SelfSigned",RSA.Create(4096), HashAlgorithmName.SHA512, RSASignaturePadding.Pss).CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1));
        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();
        this.AdminToken = _.AcquireTokenByUsernamePassword(this.Scope,this.AdminUserName,this.AdminUserPass).ExecuteAsync().Result.AccessToken;
    }

    [OneTimeTearDown]
    public void CleanUp()
    {
        IBlob b = ActorProxy.Create<IBlob>(new ActorId(Guid.NewGuid()),ServiceLocators.BlobService);

        foreach(String pfx in new String[]{"backup-arkkeeper","backup-dataconfigs","backup-universe"})
        {
            foreach(BlobContainerItem _ in new BlobServiceClient(this.ConnectionString).GetBlobContainers(prefix: pfx))
            {
                b.Delete(this.ConnectionString,_.Name,null,null,null).Wait();
            }
        }
    }

    [Test] [Order(1)]
    public void BackupArkKeeper()
    {
        Check.That(this.Proxy!.BackupArkKeeper(this.ConnectionString!,Utility.SerializeCertificate(this.Cert!)!,"managementexam",this.AdminToken!,null,null).Result).IsTrue();
    }

    [Test] [Order(2)]
    public void SimulateKeeperFail()
    {
        Check.That(this.Keeper!.StoreArk(Ark.GetBytes(new Ark()),null,null).Result).IsTrue();
    }

    [Test] [Order(3)]
    public void RestoreArkKeeper()
    {
        Check.That(this.Proxy!.RestoreArkKeeper(this.ConnectionString!,Utility.SerializeCertificate(this.Cert!)!,"managementexam",this.AdminToken!,null,null).Result).IsTrue();
        Check.That(Ark.GetBytes(this.Ark!).SequenceEqual(this.Keeper!.GetArk(null,null).Result!)).IsTrue();
    }

    [Test] [Order(7)]
    public void BackupDataConfigs()
    {
        Check.That(this.Proxy!.BackupDataConfigs(this.ConnectionString!,Utility.SerializeCertificate(this.Cert!)!,this.AdminToken!,null,null).Result).IsTrue();
    }

    [Test] [Order(8)]
    public void SimulateDataConfigsFail()
    {
        this.Config = ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService);
        this.Silos = this.Config!.GetStorageSilos(this.AdminToken!,null,null).Result;
        Check.That(this.Config!.SetStorageSilos(new HashSet<StorageSilo>(),this.AdminToken!,null,null).Result).IsTrue();
    }

    [Test] [Order(9)]
    public void RestoreDataConfigs()
    {
        Check.That(this.Proxy!.RestoreDataConfigs(this.ConnectionString!,Utility.SerializeCertificate(this.Cert!)!,this.AdminToken!,null,null).Result).IsTrue();
        Check.That(this.Config!.GetStorageSilos(this.AdminToken!,null,null).Result!.SetEquals(this.Silos!)).IsTrue();
    }

    [Test] [Order(4)]
    public void BackupUniverse()
    {
        this.Uni = new HashSet<Guid>(); this.Uni.UnionWith(Enumerable.Range(0,10000).Select(_=>Guid.NewGuid()));
        Check.That(ActorProxy.Create<IUniverse>(ActorIds.Universe,ServiceLocators.UniverseService).Reset(this.Uni,this.AdminToken!,null,null).Result).IsTrue();
        Check.That(this.Proxy!.BackupUniverse(this.ConnectionString!,Utility.SerializeCertificate(this.Cert!)!,this.AdminToken!,null,null).Result).IsTrue();
    }

    [Test] [Order(5)]
    public void SimulateUniverseFail()
    {
        Check.That(ActorProxy.Create<IUniverse>(ActorIds.Universe,ServiceLocators.UniverseService).Reset(new HashSet<Guid>(){},this.AdminToken!,null,null).Result).IsTrue();
    }

    [Test] [Order(6)]
    public void RestoreUniverse()
    {
        Check.That(this.Proxy!.RestoreUniverse(this.ConnectionString!,Utility.SerializeCertificate(this.Cert!)!,this.AdminToken!,null,null).Result).IsTrue();
        Check.That(ActorProxy.Create<IUniverse>(ActorIds.Universe,ServiceLocators.UniverseService).ListAll(this.AdminToken!,null,null).Result!.SetEquals(this.Uni!)!).IsTrue();
    }

    private Boolean LoadSetup()
    {
        try
        {
            XmlDocument d = new XmlDocument(); d.Load(Environment.CurrentDirectory + @"\EntraSetup.xml"); XmlNode? n = d.SelectSingleNode("EntraSetup");

            this.Authority        = n!.SelectSingleNode("Authority")!.InnerText;
            this.TenantID         = n.SelectSingleNode("TenantID")!.InnerText;
            this.ClientID         = n.SelectSingleNode("ClientID")!.InnerText;
            this.Scope            = new List<String>(){n.SelectSingleNode("Scope")!.InnerText}.ToArray();
            this.AdminUserName    = n.SelectSingleNode("AdminUserName")!.InnerText;
            this.AdminUserPass    = n.SelectSingleNode("AdminUserPass")!.InnerText;
            this.ConnectionString = n.SelectSingleNode("ConnectionString")!.InnerText;

            return true;
        }
        catch (Exception) { return false; }
    }
}