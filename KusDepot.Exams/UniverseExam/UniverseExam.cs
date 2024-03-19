namespace KusDepot.FabricExams;

[TestFixture] [Parallelizable]
public class UniverseExam
{
    public Guid? ID;
    public IUniverse? Proxy;
    public HashSet<Guid>? Set;

    [OneTimeSetUp]
    public void Calibrate()
    {
        this.ID = Guid.NewGuid();
        this.Proxy = ActorProxy.Create<IUniverse>(new ActorId("UniverseExam"),ServiceLocators.UniverseService);
    }

    [Test] [Order(3)]
    public void Add()
    {
        Check.That(this.Proxy!.Add(this.ID,null,null).Result).IsTrue();
    }

    [Test] [Order(4)]
    public void ExistsAfterAdd()
    {
        Check.That(this.Proxy!.Exists(this.ID,null,null).Result).IsEqualTo(true);
    }

    [Test] [Order(7)]
    public void ExistsAfterRemove()
    {
        Check.That(this.Proxy!.Exists(this.ID,null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(1)]
    public void ExistsBeforeAdd()
    {
        Check.That(this.Proxy!.Exists(this.ID,null,null).Result).IsEqualTo(false);
    }

    [Test] [Order(2)]
    public void GetSizeBeforeAdd()
    {
        Check.That(this.Proxy!.GetSize(null,null).Result).Equals(0);
    }

    [Test] [Order(5)]
    public void GetSizeAfterAdd()
    {
        Check.That(this.Proxy!.GetSize(null,null).Result).Equals(1);
    }

    [Test] [Order(8)]
    public void GetSizeAfterRemove()
    {
        Check.That(this.Proxy!.GetSize(null,null).Result).Equals(0);
    }

    [Test] [Order(6)]
    public void Remove()
    {
        Check.That(this.Proxy!.Remove(this.ID,null,null).Result).IsTrue();
    }
}