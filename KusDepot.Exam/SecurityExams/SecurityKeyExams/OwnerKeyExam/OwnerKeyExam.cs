namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class OwnerKeyExam
{
    [Test]
    public void Equality_And_HashCode()
    {
        var id = Guid.NewGuid();
        var data = new Byte[]{7,8};
        var k1 = new OwnerKey(data, id);
        var k2 = new OwnerKey(data.ToArray(), id);
        Check.That(k1).IsEqualTo(k2);
        Check.That(k1.GetHashCode()).IsEqualTo(k2.GetHashCode());

        var cert = CreateCertificate(Guid.NewGuid(),"Test")!;
        var k5 = new OwnerKey(cert, id);
        var k6 = new OwnerKey(cert, id);
        Check.That(k5).IsEqualTo(k6);
        Check.That(k5.GetHashCode()).IsEqualTo(k6.GetHashCode());

        var k3 = new OwnerKey(Array.Empty<Byte>());
        var k4 = new OwnerKey(Array.Empty<Byte>());
        Check.That(k3).IsNotEqualTo(k4);
        Check.That(k3.GetHashCode()).IsNotEqualTo(k4.GetHashCode());
    }

    [Test]
    public void KeyWeb_Roundtrip()
    {
        var cert = Utility.CreateCertificate(Guid.NewGuid(),"Test");
        var k = new OwnerKey(cert!);
        var dto = k.ToOwnerKeyWeb();
        var back = dto.ToOwnerKey();
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(dto.ToSecurityKey()).IsNotNull();
    }

    [Test]
    public void ParseAny_SecurityKey()
    {
        var cert = Utility.CreateCertificate(Guid.NewGuid(),"Test");
        var k = new OwnerKey(cert!);
        var s = k.ToString();

        var sk = SecurityKey.Parse(s);
        Check.That(sk).IsNotNull();
        Check.That(sk).IsInstanceOf<OwnerKey>();

        var mk = ManagementKey.Parse(s);
        Check.That(mk).IsNotNull();
        Check.That(mk).IsInstanceOf<OwnerKey>();
    }

    [Test]
    public void Serialize_Parse_TryParse_Roundtrip()
    {
        var cert = Utility.CreateCertificate(Guid.NewGuid(),"Test");
        var k = new OwnerKey(cert!);
        var s = k.ToString();

        var kd = OwnerKey.Parse(s);
        Check.That(kd).IsNotNull();
        Check.That(kd!.Key).IsNotNull();
        Check.That(kd!.ID).IsNotNull();

        var success = OwnerKey.TryParse(s, null, out var kdp);
        Check.That(success).IsTrue();
        Check.That(kdp).IsNotNull();
    }
}