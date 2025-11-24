namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public partial class UtilityExam
{
    private static Byte[] RandomBytes(Int32 size) { Byte[] b = new Byte[size]; RandomNumberGenerator.Fill(b); return b; }

    private X509Certificate2? CertA; private X509Certificate2? CertB;

    [OneTimeSetUp]
    public void Calibrate()
    {
        CertA = CreateCertificate(Guid.NewGuid(),"UtilityExamA",2048,2,null);
        CertB = CreateCertificate(Guid.NewGuid(),"UtilityExamB",2048,2,null);
        Check.That(CertA).IsNotNull(); Check.That(CertB).IsNotNull();
    }

    [Test]
    public void CloneByteArray_Basic()
    {
        Byte[]? original = RandomBytes(128);
        Byte[]? cloned = original.CloneByteArray();
        Check.That(cloned).IsNotNull();
        Check.That(cloned).Not.IsSameReferenceAs(original);
        Check.That(cloned!.SequenceEqual(original!)).IsTrue();

        Byte[]? nullOriginal = null;
        Byte[]? clonedFromNull = nullOriginal.CloneByteArray();
        Check.That(clonedFromNull).IsNotNull().And.IsEmpty();

        Byte[] emptyOriginal = Array.Empty<Byte>();
        Byte[] clonedFromEmpty = emptyOriginal.CloneByteArray();
        Check.That(clonedFromEmpty).IsNotNull().And.IsEmpty();
    }

    [Test]
    public void IsNullOrEmptyGuid_Basic()
    {
        Guid? nullGuid = null;
        Check.That(IsNullOrEmpty(nullGuid)).IsTrue();

        Guid emptyGuid = Guid.Empty;
        Check.That(IsNullOrEmpty(emptyGuid)).IsTrue();

        Guid validGuid = Guid.NewGuid();
        Check.That(IsNullOrEmpty(validGuid)).IsFalse();
    }

    [Test]
    public void ReadAllText_Basic()
    {
        String? content = "Hello from ReadAllText test!";
        String? path = Path.GetTempFileName() + ".txt";
        File.WriteAllText(path, content);

        try
        {
            String? readContent = ReadAllText(path);
            Check.That(readContent).IsEqualTo(content);
        }
        finally
        {
            if(File.Exists(path)) { File.Delete(path); }
        }
    }
}