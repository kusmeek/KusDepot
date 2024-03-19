namespace KusDepot.FabricExams;

[TestFixture] [Parallelizable]
public class CoreCacheExam
{
    public String? ID;
    public Tool? Tool;
    public ICoreCache? Proxy;

    [OneTimeSetUp]
    public void Calibrate()
    {
        this.Tool = new Tool(new HashSet<DataItem>(){new TextItem(DataGenerator.GenerateUnicodeString(500000))});
        this.Proxy = ActorProxy.Create<ICoreCache>(ActorIds.CoreCache,ServiceLocators.CoreCacheService);
        this.ID = this.Tool!.GetID().ToString();
    }

    [Test] [Order(7)]
    public void CleanUp()
    {
        Check.That(this.Proxy!.CleanUp(null,null,null).Result).IsTrue();
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
    public void Get()
    {
        Tool _t = Tool.Parse(this.Proxy!.Get(this.ID!,null,null).Result!,null);

        Check.That(this.Tool!.Equals(_t)).IsTrue();
    }

    [Test] [Order(2)]
    public void Store()
    {
        Check.That(this.Proxy!.Store(this.ID,this.Tool!.ToString(),null,null).Result).IsTrue();
    }
}