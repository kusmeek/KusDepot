namespace KusDepot.FabricExams;

[TestFixture] [Parallelizable]
public class BlobExam
{

    public Tool? Tool;
    public IBlob? Proxy;
    public String? ConnectionString;

    [OneTimeSetUp]
    public void Calibrate()
    {
        CodeItem _1 = new CodeItem(DataGenerator.GenerateUnicodeString(524288),Guid.NewGuid(),null,null,null,DataType.CS);
        this.Tool = new Tool(); this.Tool.AddDataItems(new HashSet<DataItem>(){_1});
        this.Proxy = ActorProxy.Create<IBlob>(new ActorId(Guid.NewGuid()),ServiceLocators.BlobService);
        this.LoadSetup();
    }

    [Test] [Order(5)]
    public void Delete()
    {
        Check.That(this.Proxy!.Delete(this.ConnectionString,this.Tool!.GetID().ToString(),null,null,null).Result).IsTrue();
    }

    [Test] [Order(3)]
    public void ExistsAfterStore()
    {
        Check.That(this.Proxy!.Exists(this.ConnectionString,this.Tool!.GetID().ToString(),null,null,null).Result).IsEqualTo(true);
    }

    [Test] [Order(6)]
    public void ExistsAfterDelete()
    {
        Check.That(this.Proxy!.Exists(this.ConnectionString,this.Tool!.GetID().ToString(),null,null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(1)]
    public void ExistsBeforeStore()
    {
        Check.That(this.Proxy!.Exists(this.ConnectionString,this.Tool!.GetID().ToString(),null,null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(4)]
    public void Get()
    {
        Tool? _0;

        Tool.TryParse(this.Proxy!.Get(this.ConnectionString,this.Tool!.GetID().ToString(),null,null,null).Result!.ToBase64FromByteArray(),null,out _0);

        Check.That(this.Tool.Equals(_0)).IsTrue();
    }

    [Test] [Order(2)]
    public void Store()
    {
        Check.That(this.Proxy!.Store(this.ConnectionString,this.Tool!.GetID().ToString(),this.Tool.ToString().ToByteArrayFromBase64(),null,null).Result).IsTrue();

        Check.That(this.Proxy!.Store(this.ConnectionString,this.Tool!.GetID().ToString(),String.Empty.ToByteArrayFromBase64(),null,null).Result).IsFalse();
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