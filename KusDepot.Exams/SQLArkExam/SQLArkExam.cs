namespace KusDepot.SQLArkExam;

[TestFixture] [Parallelizable]
public class SQLArkExam
{
    public Tool? Tool;

    [OneTimeSetUp]
    public void Calibrate()
    {
        this.Tool = new Tool();
    }

    [Test]
    public void AddUpdate()
    {
        Check.That(new SQLArk().AddUpdate(this.Tool!.GetDescriptor()!)).IsTrue();
    }

    [Test]
    public void Exists()
    {
        Check.That(new SQLArk().Exists(this.Tool!.GetDescriptor()!)).IsTrue();
    }

    [Test]
    public void Remove()
    {
        Check.That(new SQLArk().Remove(this.Tool!.GetDescriptor()!)).IsTrue();
    }
}