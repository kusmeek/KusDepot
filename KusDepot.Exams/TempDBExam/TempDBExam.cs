namespace KusDepot.FabricExams;

[TestFixture] [Parallelizable]
public class TempDBExam
{
    public Tool? Tool;
    public ITempDB? Proxy;

    [OneTimeSetUp]
    public void Calibrate()
    {
        Byte[] _0 = new Byte[33554432]; RandomNumberGenerator.Create().GetBytes(_0);

        Tool _1 = new Tool(); _1.AddDataItems(new HashSet<DataItem>(){new BinaryItem(_0)}); this.Tool = _1;

        this.Proxy = ActorProxy.Create<ITempDB>(new ActorId(Guid.NewGuid()),ServiceLocators.TempDBService);
    }

    [Test] [Order(5)]
    public void Delete()
    {
        Check.That(this.Proxy!.Delete(this.Tool!.GetID().ToString(),null,null).Result).IsTrue();
        Check.That(this.Proxy.Get(this.Tool.GetID().ToString(),null,null).Result).IsNull();
    }

    [Test] [Order(6)]
    public void ExistsAfterDelete()
    {
        Check.That(this.Proxy!.Exists(this.Tool!.GetID().ToString(),null,null).Result is true).IsFalse();
    }

    [Test] [Order(3)]
    public void ExistsAfterStore()
    {
        Check.That(this.Proxy!.Exists(this.Tool!.GetID().ToString(),null,null).Result is true).IsTrue();
    }

    [Test] [Order(1)]
    public void ExistsBeforeStore()
    {
        Check.That(this.Proxy!.Exists(this.Tool!.GetID().ToString(),null,null).Result is true).IsFalse();
    }

    [Test] [Order(4)]
    public void Get()
    {
        Check.That(this.Tool!.ToString().ToByteArrayFromBase64().SequenceEqual(this.Proxy!.Get(this.Tool.GetID().ToString(),null,null).Result!)).IsTrue();
    }

    [Test] [Order(2)]
    public void Store()
    {
        Check.That(this.Proxy!.Store(this.Tool!.GetID()!.ToString(),this.Tool.ToString().ToByteArrayFromBase64(),null,null).Result).IsTrue();
    }
}