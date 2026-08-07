namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AccessManagerMembershipEntryExam
{
    [Test]
    public void Create_Maps_Runtime_Membership()
    {
        AccessKeyToken token = new(SHA512.HashData(Encoding.UTF8.GetBytes("subject-a")));
        MembershipToken membership = new("subject-a",token,[1,2,3],DateTimeOffset.UtcNow,DateTimeOffset.UtcNow.AddHours(1),"KusDepot.Tools/TestSchema","HASH-A","Realm-A",Guid.NewGuid(),Guid.NewGuid(),["aud-a"],["scope-a"]);

        AccessManagerMembershipEntry entry = AccessManagerMembershipEntry.Create(membership);

        Check.That(entry.Subject).IsEqualTo("subject-a");
        Check.That(entry.Token).IsEqualTo(token);
        Check.That(System.Linq.Enumerable.SequenceEqual(entry.Permissions,new Byte[] { 1,2,3 })).IsTrue();
        Check.That(entry.Audiences.SequenceEqual(["aud-a"])).IsTrue();
        Check.That(entry.Scopes.SequenceEqual(["scope-a"])).IsTrue();
    }

    [Test]
    public void Clone_Is_Defensive_For_Permissions()
    {
        AccessManagerMembershipEntry entry = new()
        {
            Subject = "subject-a",
            Token = new AccessKeyToken(SHA512.HashData(Encoding.UTF8.GetBytes("subject-a"))),
            Permissions = [1,2,3]
        };

        AccessManagerMembershipEntry clone = entry.Clone();
        Byte[] permissions = entry.Permissions;
        permissions[0] = 9;
        entry.Permissions = permissions;

        Check.That(System.Linq.Enumerable.SequenceEqual(clone.Permissions,new Byte[] { 1,2,3 })).IsTrue();
    }

    [Test]
    public void ToMembershipToken_RoundTrips_Entry_Data()
    {
        Guid issuerToolId = Guid.NewGuid();
        Guid issuerAccessManagerId = Guid.NewGuid();
        DateTimeOffset issuedAt = DateTimeOffset.UtcNow;
        DateTimeOffset? notAfter = issuedAt.AddHours(1);
        AccessManagerMembershipEntry entry = new()
        {
            AccessKeyRealmID = "Realm-A",
            Audiences = ["aud-a"],
            IssuedAt = issuedAt,
            IssuerAccessManagerID = issuerAccessManagerId,
            IssuerToolID = issuerToolId,
            ManifestHash = "HASH-A",
            NotAfter = notAfter,
            Permissions = [1,2,3],
            Scopes = ["scope-a"],
            Subject = "subject-a",
            Token = new AccessKeyToken(SHA512.HashData(Encoding.UTF8.GetBytes("subject-a"))),
            ToolSchemaID = "KusDepot.Tools/TestSchema"
        };

        MembershipToken membership = entry.ToMembershipToken();

        Check.That(membership.Subject).IsEqualTo(entry.Subject);
        Check.That(membership.Token).IsEqualTo(entry.Token);
        Check.That(membership.AccessKeyRealmID).IsEqualTo(entry.AccessKeyRealmID);
        Check.That(membership.ManifestHash).IsEqualTo(entry.ManifestHash);
        Check.That(membership.ToolSchemaID).IsEqualTo(entry.ToolSchemaID);
        Check.That(membership.IssuedAt).IsEqualTo(issuedAt);
        Check.That(membership.NotAfter).IsEqualTo(notAfter);
        Check.That(membership.IssuerToolID).IsEqualTo(issuerToolId);
        Check.That(membership.IssuerAccessManagerID).IsEqualTo(issuerAccessManagerId);
        Check.That(membership.Audiences.SequenceEqual(["aud-a"])).IsTrue();
        Check.That(membership.Scopes.SequenceEqual(["scope-a"])).IsTrue();
        Check.That(System.Linq.Enumerable.SequenceEqual(AccessManagerMembershipEntry.Create(membership).Permissions,new Byte[] { 1,2,3 })).IsTrue();
    }
}
