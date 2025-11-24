namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class CommandKeyExam
{
    [Test]
    public void Equality_And_HashCode()
    {
        var id = Guid.NewGuid();
        var data = new Byte[]{7,8};
        var k1 = new CommandKey(data, id);
        var k2 = new CommandKey(data.ToArray(), id);
        Check.That(k1).IsEqualTo(k2);
        Check.That(k1.GetHashCode()).IsEqualTo(k2.GetHashCode());

        var k3 = new CommandKey(Array.Empty<Byte>());
        var k4 = new CommandKey(Array.Empty<Byte>());
        Check.That(k3).IsNotEqualTo(k4);
        Check.That(k3.GetHashCode()).IsNotEqualTo(k4.GetHashCode());
    }

    [Test]
    public void KeyWeb_Roundtrip()
    {
        var id = Guid.NewGuid();
        var k = new CommandKey(new Byte[]{1,2,3}, id);
        var dto = k.ToCommandKeyWeb();
        var back = dto.ToCommandKey();
        Check.That(back).IsNotNull();
        Check.That(back).IsEqualTo(k);
        Check.That(dto.ToSecurityKey()).IsEqualTo(k);
    }

    [Test]
    public void ParseAny_AccessKey_And_SecurityKey()
    {
        var k = new CommandKey(new Byte[]{5});
        var s = k.ToString();

        var ak = AccessKey.Parse(s);
        Check.That(ak).IsNotNull();
        Check.That(ak).IsInstanceOf<CommandKey>();

        var sk = SecurityKey.Parse(s);
        Check.That(sk).IsNotNull();
        Check.That(sk).IsInstanceOf<CommandKey>();
    }

    [Test]
    public void Serialize_Parse_TryParse_Roundtrip()
    {
        var k = new CommandKey(new Byte[]{1,2,3});
        var s = k.ToString();

        var kd = CommandKey.Parse(s);
        Check.That(kd).IsNotNull();
        Check.That(kd).IsEqualTo(k);

        var success = CommandKey.TryParse(s, null, out var kdp);
        Check.That(success).IsTrue();
        Check.That(kdp).IsEqualTo(k);
    }
}