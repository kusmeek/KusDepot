namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class KeyMakerExam
{
    private X509Certificate2? cert;
    private Guid id1;
    private Guid id2;
    private Byte[]? key1;
    private Byte[]? key2;

    [OneTimeSetUp]
    public void Calibrate()
    {
        cert = CreateCertificate(Guid.NewGuid(),"KeyMakerExamCert");
        id1 = Guid.NewGuid(); id2 = Guid.NewGuid();
        key1 = RandomNumberGenerator.GetBytes(2048);
        key2 = RandomNumberGenerator.GetBytes(2048);
    }

    [OneTimeTearDown]
    public void Complete()
    {
        cert?.Dispose();
    }

    [Test]
    public void MissingInputs_ReturnsNull()
    {
        var b0 = new KeyMaker();
        Check.That(b0.AsServiceKey()).IsNull();
        Check.That(b0.AsOwnerKey()).IsNull();

        var b1 = new KeyMaker().WithKey(key1!);
        Check.That(b1.AsClientKey()).IsNull();

        var b2 = new KeyMaker().WithID(id1);
        Check.That(b2.AsServiceKey()).IsNull();
        Check.That(b2.AsOwnerKey()).IsNull();
    }

    [Test]
    public void ServiceKey_Construction_Succeeds()
    {
        var k = KeyMaker.Create().WithID(id1).WithKey(key1!).AsServiceKey();
        Check.That(k).IsNotNull();
        Check.That(k!.ID).IsEqualTo(id1);
        Check.That(k.Key).IsEqualTo(key1);
    }

    [Test]
    public void ClientKey_Construction_Succeeds()
    {
        var k = new KeyMaker().WithID(id1).WithKey(key1!).AsClientKey();
        Check.That(k).IsNotNull();
        Check.That(k!.ID).IsEqualTo(id1);
        Check.That(k.Key).IsEqualTo(key1);
    }

    [Test]
    public void CommandKey_Construction_Succeeds()
    {
        var k = new KeyMaker().WithID(id1).WithKey(key1!).AsCommandKey();
        Check.That(k).IsNotNull();
        Check.That(k!.ID).IsEqualTo(id1);
        Check.That(k.Key).IsEqualTo(key1);
    }

    [Test]
    public void ExecutiveKey_Construction_Succeeds()
    {
        var k = new KeyMaker().WithID(id1).WithKey(key1!).AsExecutiveKey();
        Check.That(k).IsNotNull();
        Check.That(k!.ID).IsEqualTo(id1);
        Check.That(k.Key).IsEqualTo(key1);
    }

    [Test]
    public void HostKey_Construction_Succeeds()
    {
        var k = new KeyMaker().WithID(id1).WithKey(key1!).AsHostKey();
        Check.That(k).IsNotNull();
        Check.That(k!.ID).IsEqualTo(id1);
        Check.That(k.Key).IsEqualTo(key1);
    }

    [Test]
    public void MyHostKey_Construction_Succeeds()
    {
        var k = new KeyMaker().WithID(id1).WithKey(key1!).AsMyHostKey();
        Check.That(k).IsNotNull();
        Check.That(k!.ID).IsEqualTo(id1);
        Check.That(k.Key).IsEqualTo(key1);
    }

    [Test]
    public void TokenKey_Construction_Succeeds_TypePreserved()
    {
        var k = new KeyMaker().WithID(id1).WithKey(key1!).AsTokenKey(TokenKeyType.Jwt);
        Check.That(k).IsNotNull();
        Check.That(k!.ID).IsEqualTo(id1);
        Check.That(k.TokenType).IsEqualTo(TokenKeyType.Jwt);
    }

    [Test]
    public void OwnerKey_Construction_Succeeds()
    {
        var k = new KeyMaker().WithID(id1).WithCertificateKey(cert!).AsOwnerKey();
        Check.That(k).IsNotNull();
        Check.That(k!.ID).IsEqualTo(id1);
    }

    [Test]
    public void ManagerKey_Construction_Succeeds()
    {
        var k = new KeyMaker().WithID(id1).WithCertificateKey(cert!).AsManagerKey();
        Check.That(k).IsNotNull();
        Check.That(k!.ID).IsEqualTo(id1);
    }

    [Test]
    public void Cleanup_AfterKeyMaterialization_SetsKeyToNull()
    {
        var b = new KeyMaker().WithID(id1).WithKey(key1!);
        var k = b.AsServiceKey();
        Check.That(k).IsNotNull();
        var keyField = GetPrivateField(b, "Key");
        Check.That(keyField).IsNull();
        var k2 = b.AsClientKey();
        Check.That(k2).IsNull();
    }

    [Test]
    public void Cleanup_AfterCertificateMaterialization_SetsCertificateToNull()
    {
        var b = new KeyMaker().WithID(id1).WithCertificateKey(cert!);
        var k = b.AsOwnerKey();
        Check.That(k).IsNotNull();
        var certField = GetPrivateField(b, "Certificate");
        Check.That(certField).IsNull();
        var k2 = b.AsManagerKey();
        Check.That(k2).IsNull();
    }

    [Test]
    public void SubsequentMaterializations_AfterCleanup_ReturnNull()
    {
        var b = new KeyMaker().WithID(id1).WithKey(key1!);
        Check.That(b.AsServiceKey()).IsNotNull();
        Check.That(b.AsServiceKey()).IsNull();
        Check.That(b.AsOwnerKey()).IsNull();
    }

    [Test]
    public void WithRandomKey_ProducesRandomMaterial()
    {
        var b1 = new KeyMaker().WithID(id1).WithRandomKey();
        var b2 = new KeyMaker().WithID(id2).WithRandomKey();

        var k1 = b1.AsServiceKey();
        var k2 = b2.AsServiceKey();

        Check.That(k1).IsNotNull();
        Check.That(k2).IsNotNull();
        Check.That(k1!.Key).IsNotNull();
        Check.That(k2!.Key).IsNotNull();
        Check.That(k1.Key).IsNotEqualTo(k2.Key);
    }

    private static Object? GetPrivateField(Object target,String name)
    {
        var t = target.GetType();
        var f = t.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        return f?.GetValue(target);
    }
}