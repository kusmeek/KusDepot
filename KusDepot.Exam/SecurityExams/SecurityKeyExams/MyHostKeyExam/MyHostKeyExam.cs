namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class MyHostKeyExam
{
    [Test]
    public void Equality_And_HashCode()
    {
        var id = Guid.NewGuid();
        var data = new Byte[]{7,8};
        var k1 = new MyHostKey(data, id);
        var k2 = new MyHostKey(data.ToArray(), id);
        Check.That(k1).IsEqualTo(k2);
        Check.That(k1.GetHashCode()).IsEqualTo(k2.GetHashCode());

        var k3 = new MyHostKey(Array.Empty<Byte>());
        var k4 = new MyHostKey(Array.Empty<Byte>());
        Check.That(k3).IsNotEqualTo(k4);
        Check.That(k3.GetHashCode()).IsNotEqualTo(k4.GetHashCode());
    }

    [Test]
    public void ParseAny_AccessKey_And_SecurityKey()
    {
        var k = new MyHostKey(new Byte[]{5});
        var s = k.ToString();

        var ak = AccessKey.Parse(s);
        Check.That(ak).IsNotNull();
        Check.That(ak).IsInstanceOf<MyHostKey>();

        var sk = SecurityKey.Parse(s);
        Check.That(sk).IsNotNull();
        Check.That(sk).IsInstanceOf<MyHostKey>();
    }

    [Test]
    public void Serialize_Parse_TryParse_Roundtrip()
    {
        var k = new MyHostKey(new Byte[]{1,2,3});
        var s = k.ToString();

        var kd = MyHostKey.Parse(s);
        Check.That(kd).IsNotNull();
        Check.That(kd).IsEqualTo(k);

        var success = MyHostKey.TryParse(s, null, out var kdp);
        Check.That(success).IsTrue();
        Check.That(kdp).IsEqualTo(k);
    }
}