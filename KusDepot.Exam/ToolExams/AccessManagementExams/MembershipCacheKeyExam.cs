namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class MembershipCacheKeyExam
{
    [Test]
    public void TryCreate_IdenticalCredentials_ProduceEqualKeys()
    {
        Byte[] credential = RandomNumberGenerator.GetBytes(128);

        Check.That(MembershipCacheKey.TryCreate(credential,out MembershipCacheKey first)).IsTrue();
        Check.That(MembershipCacheKey.TryCreate(credential,out MembershipCacheKey second)).IsTrue();

        Check.That(first == second).IsTrue();
        Check.That(first.Equals(second)).IsTrue();
        Check.That(first != second).IsFalse();
        Check.That(first.GetHashCode()).IsEqualTo(second.GetHashCode());
        Check.That(new Dictionary<MembershipCacheKey,String> { [first] = "cached" }.TryGetValue(second,out String? value)).IsTrue();
        Check.That(value).IsEqualTo("cached");
    }

    [Test]
    public void TryCreate_SameLengthCredentialsWithDifferentBytes_ProduceDifferentKeys()
    {
        Byte[] original = RandomNumberGenerator.GetBytes(128);
        Byte[] modified = (Byte[])original.Clone();
        modified[modified.Length / 2] ^= 0x01;

        Check.That(MembershipCacheKey.TryCreate(original,out MembershipCacheKey first)).IsTrue();
        Check.That(MembershipCacheKey.TryCreate(modified,out MembershipCacheKey second)).IsTrue();

        Check.That(first == second).IsFalse();
        Check.That(first != second).IsTrue();
        Check.That(first.Equals(second)).IsFalse();
    }

    [Test]
    public void TryCreate_EmptyCredential_Fails()
    {
        Check.That(MembershipCacheKey.TryCreate(ReadOnlySpan<Byte>.Empty,out MembershipCacheKey key)).IsFalse();
        Check.That(key).IsEqualTo(default(MembershipCacheKey));
    }
}
