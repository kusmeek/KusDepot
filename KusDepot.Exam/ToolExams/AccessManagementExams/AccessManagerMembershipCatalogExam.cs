namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.None)]
public class AccessManagerMembershipCatalogExam
{
    private AccessManagerMembershipCatalog? Catalog;
    private ToolManifestIdentity? Identity;

    [OneTimeSetUp]
    public void SetUp()
    {
        this.Identity = ToolManifestIdentity.Create(ToolManifest.Create("KusDepot.Tools/TestSchema","Test-Realm","1.0.0",typeof(Tool).FullName,[]).ComputeManifestHash());
        AccessManagerMembershipEntry entry = new()
        {
            AccessKeyRealmID = this.Identity.AccessKeyRealmID,
            AssertionSummaries = [new TokenAssertionSummary { AssertionType = "token", Format = "jwt/v1", Issuer = "https://catalog.example" }],
            Audiences = ["aud-a"],
            IssuedAt = DateTimeOffset.UtcNow,
            IssuerAccessManagerID = Guid.NewGuid(),
            IssuerToolID = Guid.NewGuid(),
            ManifestHash = this.Identity.ManifestHash,
            NotAfter = DateTimeOffset.UtcNow.AddHours(1),
            Permissions = [1,2,3],
            Scopes = ["scope-a"],
            Subject = "subject-a",
            Token = new AccessKeyToken(SHA512.HashData(Encoding.UTF8.GetBytes("subject-a"))),
            ToolSchemaID = this.Identity.ToolSchemaID
        };

        this.Catalog = new AccessManagerMembershipCatalog
        {
            ToolManifestIdentity = this.Identity,
            Entries = new Dictionary<String,ImmutableArray<AccessManagerMembershipEntry>>
            {
                { "subject-a", [entry] }
            }
        };
    }

    [Test] [Order(1)]
    public void SerializeAndDeserialize()
    {
        Byte[]? serialized = this.Catalog!.Serialize();
        Check.That(serialized).IsNotNull().And.Not.IsEmpty();

        AccessManagerMembershipCatalog? deserialized = AccessManagerMembershipCatalog.Deserialize(serialized);
        Check.That(deserialized).IsNotNull();
        Check.That(deserialized!.ToolManifestIdentity).IsNotNull();
        Check.That(deserialized.ToolManifestIdentity!.ToolSchemaID).IsEqualTo(this.Identity!.ToolSchemaID);
        Check.That(deserialized.Entries).IsNotNull();
        Check.That(deserialized.Entries!.ContainsKey("subject-a")).IsTrue();
        Check.That(deserialized.Entries["subject-a"].Length).IsEqualTo(1);
        Check.That(Enumerable.SequenceEqual(deserialized.Entries["subject-a"][0].Permissions,new Byte[] { 1,2,3 })).IsTrue();
        Check.That(deserialized.Entries["subject-a"][0].AssertionSummaries.Length).IsEqualTo(1);
        Check.That(((TokenAssertionSummary)deserialized.Entries["subject-a"][0].AssertionSummaries[0]).Issuer).IsEqualTo("https://catalog.example");
    }

    [Test] [Order(2)]
    public void Clone()
    {
        AccessManagerMembershipCatalog clone = this.Catalog!.Clone();
        AccessManagerMembershipEntry updatedEntry = this.Catalog.Entries!["subject-a"][0].Clone();
        Byte[] permissions = updatedEntry.Permissions;
        permissions[0] = 9;
        updatedEntry.Permissions = permissions;
        this.Catalog.Entries = new Dictionary<String,ImmutableArray<AccessManagerMembershipEntry>> { { "subject-a" , [updatedEntry] } };
        this.Catalog.Clear();

        Check.That(clone.ToolManifestIdentity).IsNotNull();
        Check.That(clone.ToolManifestIdentity!.ToolSchemaID).IsEqualTo(this.Identity!.ToolSchemaID);
        Check.That(clone.Entries).IsNotNull();
        Check.That(Enumerable.SequenceEqual(clone.Entries!["subject-a"][0].Permissions,new Byte[] { 1,2,3 })).IsTrue();
        Check.That(clone.Entries["subject-a"][0].AssertionSummaries.Length).IsEqualTo(1);
        Check.That(((TokenAssertionSummary)clone.Entries["subject-a"][0].AssertionSummaries[0]).Issuer).IsEqualTo("https://catalog.example");
    }

    [Test] [Order(3)]
    public void Clear()
    {
        this.Catalog!.Clear();

        Check.That(this.Catalog.ToolManifestIdentity).IsNull();
        Check.That(this.Catalog.Entries).IsNull();
    }
}
