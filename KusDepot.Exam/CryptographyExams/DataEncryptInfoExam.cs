namespace KusDepot.Exams;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class DataEncryptInfoExam
{
    private X509Certificate2? CertA;
    private X509Certificate2? CertB;

    [OneTimeSetUp]
    public void Calibrate()
    {
        CertA = Utility.CreateCertificate(Guid.NewGuid(), "DataEncryptInfoExamA", 2048, 2, null);
        CertB = Utility.CreateCertificate(Guid.NewGuid(), "DataEncryptInfoExamB", 2048, 2, null);
        Check.That(CertA).IsNotNull();
        Check.That(CertB).IsNotNull();
    }

    [Test]
    public void Construct_FromCertificate()
    {
        var info = new DataEncryptInfo(CertA!);
        Check.That(info.PublicKey).IsNotNull().And.Not.Equals(String.Empty);
        Check.That(info.Serial).IsEqualTo(CertA!.SerialNumber);
        Check.That(info.ThumbPrint).IsEqualTo(CertA.Thumbprint);
    }

    [Test]
    public void Construct_FromManagementKey()
    {
        var key = new ManagerKey(CertB!);
        var info = new DataEncryptInfo(key);
        Check.That(info.ManagementKeyID).HasAValue();
        Check.That(info.PublicKey).IsNotNull().And.Not.Equals(String.Empty);
        Check.That(info.Serial).IsEqualTo(CertB!.SerialNumber);
        Check.That(info.ThumbPrint).IsEqualTo(CertB.Thumbprint);
    }

    [Test]
    public void Construct_FromValues()
    {
        var pk = "ABC123";
        var serial = "001122";
        var thumb = "AABBCC";
        var info = new DataEncryptInfo(Guid.NewGuid(),pk, serial, thumb);
        Check.That(info.ManagementKeyID).HasAValue();
        Check.That(info.PublicKey).IsEqualTo(pk);
        Check.That(info.Serial).IsEqualTo(serial);
        Check.That(info.ThumbPrint).IsEqualTo(thumb);
    }

    [Test]
    public void Clone_CreatesDeepCopy()
    {
        var original = new DataEncryptInfo(CertA!);
        var clone = original.Clone();
        Check.That(clone).IsNotNull();
        Check.That(clone).Not.IsSameReferenceAs(original);
        Check.That(clone.PublicKey).IsEqualTo(original.PublicKey);
        Check.That(clone.Serial).IsEqualTo(original.Serial);
        Check.That(clone.ThumbPrint).IsEqualTo(original.ThumbPrint);
    }
}