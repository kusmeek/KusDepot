namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ExecutiveKeyExam
{
    [Test]
    public void Equality_And_HashCode()
    {
        var id = Guid.NewGuid();
        var data = new Byte[]{7,8};
        var k1 = new ExecutiveKey(data, id);
        var k2 = new ExecutiveKey(data.ToArray(), id);
        Check.That(k1).IsEqualTo(k2);
        Check.That(k1.GetHashCode()).IsEqualTo(k2.GetHashCode());

        var k3 = new ExecutiveKey(Array.Empty<Byte>());
        var k4 = new ExecutiveKey(Array.Empty<Byte>());
        Check.That(k3).IsNotEqualTo(k4);
        Check.That(k3.GetHashCode()).IsNotEqualTo(k4.GetHashCode());
    }

    [Test]
    public void KeyWeb_Roundtrip()
    {
        var id = Guid.NewGuid();
        var k = new ExecutiveKey(new Byte[]{1,2,3}, id);
        var dto = k.ToExecutiveKeyWeb();
        var back = dto.ToExecutiveKey();
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(dto.ToSecurityKey()).IsEqualTo(k);
    }

    [Test]
    public void ParseAny_AccessKey_And_SecurityKey()
    {
        var k = new ExecutiveKey(new Byte[]{5});
        var s = k.ToString();

        var ak = AccessKey.Parse(s);
        Check.That(ak).IsNotNull();
        Check.That(ak).IsInstanceOf<ExecutiveKey>();

        var sk = SecurityKey.Parse(s);
        Check.That(sk).IsNotNull();
        Check.That(sk).IsInstanceOf<ExecutiveKey>();
    }

    [Test]
    public void Serialize_Parse_TryParse_Roundtrip()
    {
        var k = new ExecutiveKey(new Byte[]{1,2,3});
        var s = k.ToString();

        var kd = ExecutiveKey.Parse(s);
        Check.That(kd).IsNotNull();
        Check.That(kd).IsEqualTo(k);

        var success = ExecutiveKey.TryParse(s, null, out var kdp);
        Check.That(success).IsTrue();
        Check.That(kdp).IsEqualTo(k);
    }
}