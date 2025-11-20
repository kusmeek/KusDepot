namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ManagerKeyExam
{
    [Test]
    public void Equality_And_HashCode()
    {
        var id = Guid.NewGuid();
        var data = SerializeCertificate(CreateCertificate(Guid.NewGuid(),"Test"))!;
        var k1 = new ManagerKey(data, id);
        var k2 = new ManagerKey(data.ToArray(), id);
        Check.That(k1).IsEqualTo(k2);
        Check.That(k1.GetHashCode()).IsEqualTo(k2.GetHashCode());

        var cert = CreateCertificate(Guid.NewGuid(),"Test")!;
        var k5 = new ManagerKey(cert, id);
        var k6 = new ManagerKey(cert, id);
        Check.That(k5).IsEqualTo(k6);
        Check.That(k5.GetHashCode()).IsEqualTo(k6.GetHashCode());

        var k3 = new ManagerKey(Array.Empty<Byte>());
        var k4 = new ManagerKey(Array.Empty<Byte>());
        Check.That(k3).IsNotEqualTo(k4);
        Check.That(k3.GetHashCode()).IsNotEqualTo(k4.GetHashCode());
    }

    [Test]
    public void KeyWeb_Roundtrip()
    {
        var cert = CreateCertificate(Guid.NewGuid(),"Test");
        var k = new ManagerKey(cert!);
        var dto = k.ToManagerKeyWeb();
        var back = dto.ToManagerKey();
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(dto.ToSecurityKey()).IsNotNull();
    }

    [Test]
    public void ParseAny_SecurityKey()
    {
        var cert = CreateCertificate(Guid.NewGuid(),"Test");
        var k = new ManagerKey(cert!);
        var s = k.ToString();

        var sk = SecurityKey.Parse(s);
        Check.That(sk).IsNotNull();
        Check.That(sk).IsInstanceOf<ManagerKey>();

        var mk = ManagementKey.Parse(s);
        Check.That(mk).IsNotNull();
        Check.That(mk).IsInstanceOf<ManagerKey>();
    }

    [Test]
    public void Serialize_Parse_TryParse_Roundtrip()
    {
        var cert = CreateCertificate(Guid.NewGuid(),"Test");
        var k = new ManagerKey(cert!);
        var s = k.ToString();

        var kd = ManagerKey.Parse(s);
        Check.That(kd).IsNotNull();
        Check.That(kd!.Key).IsNotNull();
        Check.That(kd!.ID).IsNotNull();

        var success = ManagerKey.TryParse(s, null, out var kdp);
        Check.That(success).IsTrue();
        Check.That(kdp).IsNotNull();
    }
}