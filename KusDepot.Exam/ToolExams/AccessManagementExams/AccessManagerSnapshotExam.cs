namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AccessManagerSnapshotExam
{
    private static TokenAssertion CreateTokenAssertion(String issuer)
    {
        return new()
        {
            Audiences = ["aud-snapshot"],
            AuthenticationMethods = ["pwd"],
            AuthenticationTime = DateTimeOffset.UtcNow.AddMinutes(-1),
            Format = "jwt/v1",
            IssuedAt = DateTimeOffset.UtcNow.AddMinutes(-2),
            Issuer = issuer,
            NotAfter = DateTimeOffset.UtcNow.AddMinutes(10),
            Scopes = ["scope.snapshot"],
            Subject = "snapshot-subject",
            Token = Encoding.UTF8.GetBytes($"snapshot-{issuer}"),
            TokenType = TokenKeyType.Jwt
        };
    }

    [Test]
    public void QuerySnapshot_Filters_By_Subject_And_Operation()
    {
        var tool = new Tool();
        var manager = new AccessManager(tool);

        ClientKey? matching = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-a",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        ClientKey? other = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-b",operations: ImmutableArray.Create(ProtectedOperation.GetOutput)));
        AccessManagerSnapshot? snapshot = manager.QuerySnapshot(AccessManagerQuery.Create(subjects:["subject-a"],protectedoperations:[ProtectedOperation.GetOutputIDs]));

        Check.That(matching).IsNotNull();
        Check.That(other).IsNotNull();
        Check.That(snapshot).IsNotNull();
        Check.That(snapshot!.ToolManifestIdentity).IsNotNull();
        Check.That(snapshot.AccessKeys).IsNotNull();
        Check.That(snapshot.AccessKeys!.Keys).Contains("subject-a");
        Check.That(snapshot.AccessKeys.Count).IsEqualTo(1);
        Check.That(snapshot.AccessKeys["subject-a"].Length).IsEqualTo(1);
    }

    [Test]
    public void QuerySnapshot_Projects_Audit_Safe_Entries_Without_Token_Identity()
    {
        var tool = new Tool();
        var manager = new AccessManager(tool);

        ClientKey? clientKey = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-a",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        AccessManagerSnapshot? snapshot = manager.QuerySnapshot(AccessManagerQuery.Create(subjects:["subject-a"]));
        AccessManagerMembershipCatalog? memberships = manager.QueryMembershipCatalog(AccessManagerQuery.Create(subjects:["subject-a"]));

        Check.That(tool).IsNotNull();
        Check.That(clientKey).IsNotNull();
        Check.That(snapshot).IsNotNull();
        Check.That(memberships).IsNotNull();
        Check.That(memberships!.ToolManifestIdentity).IsNotNull();
        Check.That(memberships.ToolManifestIdentity!.ToolSchemaID).IsEqualTo(snapshot!.ToolManifestIdentity!.ToolSchemaID);
        Check.That(memberships.ToolManifestIdentity.AccessKeyRealmID).IsEqualTo(snapshot.ToolManifestIdentity.AccessKeyRealmID);
        Check.That(memberships.ToolManifestIdentity.ManifestHash).IsEqualTo(snapshot.ToolManifestIdentity.ManifestHash);
        Check.That(memberships.Entries).IsNotNull();
        Check.That(snapshot.AccessKeys).IsNotNull();
        Check.That(snapshot.AccessKeys!["subject-a"][0].Subject).IsEqualTo(memberships.Entries!["subject-a"][0].Subject);
        Check.That(snapshot.AccessKeys["subject-a"][0].ManifestHash).IsEqualTo(memberships.Entries["subject-a"][0].ManifestHash);
        Check.That(snapshot.AccessKeys["subject-a"][0].AccessKeyRealmID).IsEqualTo(memberships.Entries["subject-a"][0].AccessKeyRealmID);
        Check.That(typeof(AccessManagerSnapshotEntry).GetProperty("Token")).IsNull();
    }

    [Test]
    public void QuerySnapshot_Projects_AssertionSummaries()
    {
        var tool = new Tool();
        var generator = new DefaultAccessKeyAssertionGenerator([new SnapshotAssertionGenerator(CreateTokenAssertion("https://snapshot.example"))]);
        var manager = new AccessManager(new AccessManagerOptions(keyassertiongenerator: generator),tool);

        ClientKey? clientKey = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-a",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        AccessManagerSnapshot? snapshot = manager.QuerySnapshot(AccessManagerQuery.Create(subjects:["subject-a"]));

        Check.That(clientKey).IsNotNull();
        Check.That(snapshot).IsNotNull();
        Check.That(snapshot!.AccessKeys).IsNotNull();
        Check.That(snapshot.AccessKeys!["subject-a"][0].AssertionSummaries.Length).IsEqualTo(1);
        Check.That(snapshot.AccessKeys["subject-a"][0].AssertionSummaries[0]).IsInstanceOf<TokenAssertionSummary>();

        TokenAssertionSummary summary = (TokenAssertionSummary)snapshot.AccessKeys["subject-a"][0].AssertionSummaries[0];
        Check.That(summary.AssertionType).IsEqualTo("token");
        Check.That(summary.Format).IsEqualTo("jwt/v1");
        Check.That(summary.Issuer).IsEqualTo("https://snapshot.example");
    }

    private sealed class SnapshotAssertionGenerator(params AccessKeyAssertion[] assertions) : IAccessKeyAssertionGenerator
    {
        public ImmutableArray<AccessKeyAssertion> Generate(in AccessKeyAssertionGeneratorContext context)
        {
            return assertions.ToImmutableArray();
        }
    }
}
