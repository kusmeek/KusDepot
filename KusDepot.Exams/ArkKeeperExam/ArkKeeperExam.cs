namespace KusDepot.FabricExams;

[TestFixture] [Parallelizable]
public class ArkKeeperExam
{
    public Byte[]? ArkExam;
    public Guid? ID;
    public Tool? Tool;
    public IArkKeeper? Proxy;

    [OneTimeSetUp]
    public void Calibrate()
    {
        this.Proxy = ActorProxy.Create<IArkKeeper>(new ActorId("ArkKeeperExam"),ServiceLocators.ArkKeeperService);
    }

    [Test] [Order(4)]
    public void AddUpdate()
    {
        this.Tool = new Tool();

        Check.That(this.Proxy!.AddUpdate(this.Tool.GetDescriptor(),null,null).Result).IsTrue();
        Check.That(this.Proxy.Exists(this.Tool.GetDescriptor(),null,null).Result).IsEqualTo(true);
    }

    [Test] [Order(3)]
    public void Exists()
    {
        Check.That(this.Proxy!.Exists(new Descriptor(){ID = this.ID},null,null).Result).IsEqualTo(true);
        Check.That(this.Proxy!.Exists(new Descriptor(){ID = Guid.NewGuid()},null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(2)]
    public void GetArk()
    {
        Check.That(this.ArkExam!.SequenceEqual(this.Proxy!.GetArk(null,null).Result!)).IsTrue();
    }

    [Test] [Order(5)]
    public void Remove()
    {
        Check.That(this.Proxy!.Remove(this.Tool!.GetDescriptor(),null,null).Result).IsTrue();
        Check.That(this.Proxy!.Exists(this.Tool.GetDescriptor(),null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(6)]
    public void RemoveID()
    {
        Check.That(this.Proxy!.RemoveID(this.ID!,null,null).Result).IsTrue();
        Check.That(this.Proxy!.Exists(new Descriptor(){ID = this.ID},null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(1)]
    public void StoreArk()
    {
        Ark _0 = new Ark();
        Tool _1 = new Tool();
        Tool _2 = new Tool();
        BinaryItem _10 = new BinaryItem(); this.ID = _10.GetID();

        Check.That(_0.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_10.ToString())).IsTrue();

        this.ArkExam = Ark.GetBytes(_0);

        Check.That(this.Proxy!.StoreArk(this.ArkExam,null,null).Result).IsTrue();
    }
}