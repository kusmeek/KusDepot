namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class TokenKeyExam
{
    [Test]
    public void Equality_And_HashCode()
    {
        var id = Guid.NewGuid();
        var data = new Byte[]{7,8};
        var k1 = new TokenKey(data, id, TokenKeyType.Jwt);
        var k2 = new TokenKey(data.ToArray(), id, TokenKeyType.Jwt);
        Check.That(k1).IsEqualTo(k2);
        Check.That(k1.GetHashCode()).IsEqualTo(k2.GetHashCode());

        var k3 = new TokenKey(Array.Empty<Byte>(), type: TokenKeyType.Opaque);
        var k4 = new TokenKey(Array.Empty<Byte>(), type: TokenKeyType.Opaque);
        Check.That(k3).IsNotEqualTo(k4);
        Check.That(k3.GetHashCode()).IsNotEqualTo(k4.GetHashCode());
    }

    [Test]
    public void ParseAny_AccessKey_And_SecurityKey()
    {
        var k = new TokenKey(new Byte[]{5}, type: TokenKeyType.Jwt);
        var s = k.ToString();

        var ak = AccessKey.Parse(s);
        Check.That(ak).IsNotNull();
        Check.That(ak).IsInstanceOf<TokenKey>();

        var sk = SecurityKey.Parse(s);
        Check.That(sk).IsNotNull();
        Check.That(sk).IsInstanceOf<TokenKey>();
    }

    [Test]
    public void Serialize_Parse_TryParse_Roundtrip()
    {
        var k = new TokenKey(new Byte[]{1,2,3}, type: TokenKeyType.Saml2);
        var s = k.ToString();

        var kd = TokenKey.Parse(s);
        Check.That(kd).IsNotNull();
        Check.That(kd).IsEqualTo(k);

        var success = TokenKey.TryParse(s, null, out var kdp);
        Check.That(success).IsTrue();
        Check.That(kdp).IsEqualTo(k);
    }

    [Test]
    public void Type_Roundtrip()
    {
        var k = new TokenKey(new Byte[]{9,4,9}, type: TokenKeyType.Spnego);
        var s = k.ToString();

        var kd = TokenKey.Parse(s);
        Check.That(kd).IsNotNull();
        Check.That(kd!.Type).IsEqualTo(TokenKeyType.Spnego);
    }
}
