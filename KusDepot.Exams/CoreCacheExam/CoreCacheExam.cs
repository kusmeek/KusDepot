namespace KusDepot.FabricExams.Data;

[TestFixture] [Parallelizable]
public class CoreCacheExam
{
    public String? ID;
    public DataItem? Item;
    public ICoreCache? Proxy;
    public ActorId? ActorId;
    public ManagementKey? Key;

    [OneTimeSetUp]
    public void Calibrate()
    {
        this.Item = new DataSetItem(content:new HashSet<DataItem>(){new TextItem(DataGenerator.GenerateUnicodeString(500000))});
        this.Key = this.Item.CreateManagementKey("test"); Check.That(this.Item.SignData("HashCode",this.Key)).IsNotNull();
        this.ActorId = new("CoreCacheExam"); this.Proxy = ActorProxy.Create<ICoreCache>(ActorId,ServiceLocators.CoreCacheService);
        this.ID = this.Item!.GetID().ToString();
    }

    [OneTimeTearDown]
    public void Complete()
    {
        ActorServiceProxy.Create(ServiceLocators.CoreCacheService,ActorId!).DeleteActorAsync(ActorId,CancellationToken.None).GetAwaiter().GetResult();
    }

    [Test] [Order(5)]
    public void Delete()
    {
        Check.That(this.Proxy!.Delete(this.ID!,null,null).Result).IsTrue();
    }

    [Test] [Order(6)]
    public void ExistsAfterDelete()
    {
        Check.That(this.Proxy!.Exists(this.ID!,null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(3)]
    public void ExistsAfterStore()
    {
        Check.That(this.Proxy!.Exists(this.ID!,null,null).Result).IsEqualTo(true);
    }

    [Test] [Order(1)]
    public void ExistsBeforeStore()
    {
        Check.That(this.Proxy!.Exists(this.ID!,null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(4)]
    public async Task Get()
    {
        DataItem? _d = DataItem.Parse(this.Proxy!.Get(this.ID!,null,null).Result!,null);

        Check.That(await _d!.VerifyData("HashCode",this.Key!)).IsTrue();

        Check.That(this.Item!.Equals(_d)).IsTrue();
    }

    [Test] [Order(2)]
    public void Store()
    {
        Check.That(this.Proxy!.Store(this.ID,this.Item!.ToString(),null,null).Result).IsTrue();
    }
}