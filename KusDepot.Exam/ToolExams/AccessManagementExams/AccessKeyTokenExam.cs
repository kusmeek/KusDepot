namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AccessKeyTokenExam
{
    private static Byte[] Rand32()
    {
        Byte[] b = new Byte[32]; RandomNumberGenerator.Fill(b); return b;
    }

    [Test]
    public void Construct_And_ToString_HexLength()
    {
        var raw = Rand32(); var tok = new AccessKeyToken(raw);
        String hex = tok.ToString();
        Check.That(hex).IsNotNull();
        Check.That(hex.Length).IsEqualTo(64);
    }

    [Test]
    public void Equality_SameBytes()
    {
        var raw = Rand32(); var a = new AccessKeyToken(raw); var b = new AccessKeyToken(raw);
        Check.That(a == b).IsTrue();
        Check.That(a.Equals(b)).IsTrue();
        Check.That(a != b).IsFalse();
        Check.That(a.GetHashCode()).IsEqualTo(b.GetHashCode());
    }

    [Test]
    public void Inequality_DifferentBytes()
    {
        var a = new AccessKeyToken(Rand32()); var b = new AccessKeyToken(Rand32());
        Check.That(a == b).IsFalse();
        Check.That(a != b).IsTrue();
    }

    [Test]
    public void Clone_From_Hex_String()
    {
        var original = new AccessKeyToken(Rand32());
        String hex = original.ToString();
        var clone = new AccessKeyToken(Convert.FromHexString(hex));
        Check.That(clone == original).IsTrue();
        Check.That(clone.ToString()).IsEqualTo(hex);
    }

    [Test]
    public void MultipleRandomTokens_Unique()
    {
        HashSet<String> seen = new();
        for(Int32 i=0;i<25;i++)
        {
            var t = new AccessKeyToken(Rand32());
            String hex = t.ToString();
            Check.That(seen.Contains(hex)).IsFalse();
            seen.Add(hex);
        }
    }

    [Test]
    public void HashCode_Consistency()
    {
        var raw = Rand32(); var tok = new AccessKeyToken(raw);
        Int32 h1 = tok.GetHashCode(); Int32 h2 = tok.GetHashCode();
        Check.That(h1).IsEqualTo(h2);
    }

    [Test]
    public void Clear_ResetsToken()
    {
        var raw = Rand32();
        var token = new AccessKeyToken(raw);

        Check.That(token.ToString()).IsNotEqualTo(default(AccessKeyToken).ToString());

        token.Clear();

        Byte[] clearedBytes = Convert.FromHexString(token.ToString());
        Check.That(clearedBytes.All(b => b == 0)).IsTrue();
    }

    [Test]
    public void Clone_CreatesEqualInstance()
    {
        var original = new AccessKeyToken(Rand32());
        var clone = original.Clone();

        Check.That(clone).IsEqualTo(original);
        Check.That(clone.ToString()).IsEqualTo(original.ToString());
    }
}