namespace KusDepot.FabricExams;

[TestFixture] [Parallelizable]
public class WatchExam
{
    public IWatch? Proxy;

    [OneTimeSetUp]
    public void Calibrate()
    {
        this.Proxy = ActorProxy.Create<IWatch>(new ActorId(Guid.NewGuid()),ServiceLocators.WatchService);
    }

    [Test]
    public void GetElapsed()
    {
        Check.That(this.Proxy!.Reset().Result).IsTrue();
        Check.That(this.Proxy.GetStarted().Result).IsNull();
        Check.That(this.Proxy.Start().Result).IsTrue();
        Check.That(this.Proxy.GetStarted().Result).IsNotNull();
        Check.That(this.Proxy.GetStopped().Result).IsNull();
        Check.That(this.Proxy.Stop().Result).IsTrue();
        Check.That(this.Proxy.GetStopped().Result).IsNotNull();
        Check.That(this.Proxy.GetElapsed().Result!.Value).IsInstanceOf<TimeSpan>();
        Check.That(this.Proxy.GetElapsed().Result!.Value.TotalMilliseconds).IsStrictlyPositive();
    }

    [Test]
    public void GetStarted()
    {
        Check.That(this.Proxy!.Reset().Result).IsTrue();
        Check.That(this.Proxy.GetStarted().Result).IsNull();
        Check.That(this.Proxy.Start().Result).IsTrue();
        Check.That(this.Proxy.GetStarted().Result).IsNotNull();
    }

    [Test]
    public void GetStopped()
    {
        Check.That(this.Proxy!.Reset().Result).IsTrue();
        Check.That(this.Proxy.GetStarted().Result).IsNull();
        Check.That(this.Proxy.Start().Result).IsTrue();
        Check.That(this.Proxy.GetStarted().Result).IsNotNull();
        Check.That(this.Proxy.GetStopped().Result).IsNull();
        Check.That(this.Proxy.Stop().Result).IsTrue();
        Check.That(this.Proxy.GetStopped().Result).IsNotNull();
        Check.That(this.Proxy.GetStopped().Result - this.Proxy.GetStarted().Result).IsInstanceOf<TimeSpan?>();
    }

    [Test]
    public void GetTime()
    {
        DateTimeOffset _0;
        Check.That(DateTimeOffset.TryParse(this.Proxy!.GetTime().Result.ToString(CultureInfo.InvariantCulture),CultureInfo.InvariantCulture,out _0)).IsTrue();
    }

    [Test]
    public void Start()
    {
        Check.That(this.Proxy!.Reset().Result).IsTrue();
        Check.That(this.Proxy.GetStarted().Result).IsNull();
        Check.That(this.Proxy.Start().Result).IsTrue();
        Check.That(this.Proxy.GetStarted().Result).IsNotNull();
    }

    [Test]
    public void Stop()
    {
        Check.That(this.Proxy!.Reset().Result).IsTrue();
        Check.That(this.Proxy.GetStarted().Result).IsNull();
        Check.That(this.Proxy.Start().Result).IsTrue();
        Check.That(this.Proxy.GetStarted().Result).IsNotNull();
        Check.That(this.Proxy.GetStopped().Result).IsNull();
        Check.That(this.Proxy.Stop().Result).IsTrue();
        Check.That(this.Proxy.GetStopped().Result).IsNotNull();
    }
}