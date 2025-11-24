namespace KusDepot.FabricExams.Data;

[TestFixture] [Parallelizable]
public class BlobExam
{

    public IBlob? Proxy;
    public DataItem? Item;
    public String? ConnectionString;
    public ActorId? ActorId;

    [OneTimeSetUp]
    public void Calibrate()
    {
        this.Item = new CodeItem(content:DataGenerator.GenerateUnicodeString(524288),id:Guid.NewGuid(),type:DataType.CS);
        this.ActorId = new(Guid.NewGuid()); this.Proxy = ActorProxy.Create<IBlob>(ActorId,ServiceLocators.BlobService);
        this.LoadSetup();
    }

    [OneTimeTearDown]
    public void Complete()
    {
        ActorServiceProxy.Create(ServiceLocators.BlobService,ActorId!).DeleteActorAsync(ActorId,CancellationToken.None).GetAwaiter().GetResult();
    }

    [Test] [Order(5)]
    public void Delete()
    {
        Check.That(this.Proxy!.Delete(this.ConnectionString,this.Item!.GetID().ToString(),null,null,null).Result).IsTrue();
    }

    [Test] [Order(3)]
    public void ExistsAfterStore()
    {
        Check.That(this.Proxy!.Exists(this.ConnectionString,this.Item!.GetID().ToString(),null,null,null).Result).IsEqualTo(true);
    }

    [Test] [Order(6)]
    public void ExistsAfterDelete()
    {
        Check.That(this.Proxy!.Exists(this.ConnectionString,this.Item!.GetID().ToString(),null,null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(1)]
    public void ExistsBeforeStore()
    {
        Check.That(this.Proxy!.Exists(this.ConnectionString,this.Item!.GetID().ToString(),null,null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(4)]
    public void Get()
    {
        CodeItem? _0;

        CodeItem.TryParse(this.Proxy!.Get(this.ConnectionString,this.Item!.GetID().ToString(),null,null,null).Result!.ToBase64FromByteArray(),null,out _0);

        Check.That(this.Item.Equals(_0)).IsTrue();
    }

    [Test] [Order(2)]
    public void Store()
    {
        Check.That(this.Proxy!.Store(this.ConnectionString,this.Item!.GetID().ToString(),this.Item.ToString().ToByteArrayFromBase64(),null,null).Result).IsTrue();

        Check.That(this.Proxy!.Store(this.ConnectionString,this.Item!.GetID().ToString(),String.Empty.ToByteArrayFromBase64(),null,null).Result).IsFalse();
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