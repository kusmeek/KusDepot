namespace KusDepot.FabricExams.Data;

[TestFixture]
public class ManagementExam
{
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
    public ActorId? ActorId;

    [OneTimeSetUp]
    public void Calibrate()
    {
        Check.That(this.LoadSetup()).IsTrue(); this.ActorId = new("managementexam");

        this.Proxy = ActorProxy.Create<IManagement>(ActorId,ServiceLocators.ManagementService); this.Cert = Utility.CreateCertificate(Guid.NewGuid(),"Test");
        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();
        this.AdminToken = _.AcquireTokenByUsernamePassword(this.Scope,this.AdminUserName,this.AdminUserPass).ExecuteAsync().Result.AccessToken;
    }

    [OneTimeTearDown]
    public void Complete()
    {
        ActorId a = new(Guid.NewGuid());

        IBlob b = ActorProxy.Create<IBlob>(a,ServiceLocators.BlobService);

        foreach(String pfx in new String[]{"backup-dataconfigs"})
        {
            foreach(BlobContainerItem _ in new BlobServiceClient(this.ConnectionString).GetBlobContainers(prefix: pfx))
            {
                b.Delete(this.ConnectionString,_.Name,null,null,null).Wait();
            }
        }

        ActorServiceProxy.Create(ServiceLocators.BlobService,a).DeleteActorAsync(a,CancellationToken.None).GetAwaiter().GetResult();
        ActorServiceProxy.Create(ServiceLocators.ManagementService,ActorId!).DeleteActorAsync(ActorId,CancellationToken.None).GetAwaiter().GetResult();
    }

    [Test] [Order(1)]
    public void BackupDataConfigs()
    {
        Check.That(this.Proxy!.BackupDataConfigs(this.ConnectionString!,Utility.SerializeCertificate(this.Cert!).ToBase64FromByteArray()!,this.AdminToken!,null,null).Result).IsTrue();
    }

    [Test] [Order(2)]
    public void SimulateDataConfigsFail()
    {
        this.Config = ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService);
        this.Silos = this.Config!.GetStorageSilos(this.AdminToken!,null,null).Result;
        Check.That(this.Config!.SetStorageSilos(new HashSet<StorageSilo>(),this.AdminToken!,null,null).Result).IsTrue();
    }

    [Test] [Order(3)]
    public void RestoreDataConfigs()
    {
        Check.That(this.Proxy!.RestoreDataConfigs(this.ConnectionString!,Utility.SerializeCertificate(this.Cert!).ToBase64FromByteArray()!,this.AdminToken!,null,null).Result).IsTrue();
        Check.That(this.Config!.GetStorageSilos(this.AdminToken!,null,null).Result!.SetEquals(this.Silos!)).IsTrue();
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