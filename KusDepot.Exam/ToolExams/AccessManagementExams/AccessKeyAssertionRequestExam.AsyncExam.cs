namespace KusDepot.Exams.Security;

public partial class AccessKeyAssertionRequestExam
{
    private sealed class AsyncCapturingAssertionGenerator(params AccessKeyAssertion[] assertions) : IAsyncAccessKeyAssertionGenerator
    {
        public AccessKeyAssertionGeneratorContext? LastContext { get; private set; }
        public Int32 AsyncCalls { get; private set; }

        public async ValueTask<ImmutableArray<AccessKeyAssertion>> GenerateAsync(AccessKeyAssertionGeneratorContext context , CancellationToken cancel = default)
        {
            await Task.Yield();
            cancel.ThrowIfCancellationRequested();
            LastContext = context;
            AsyncCalls++;
            return assertions.ToImmutableArray();
        }
    }

    [Test]
    public async Task GenerateAccessKeyAsync_WithAsyncAssertionGenerator_PersistsAndRecoversAssertionSummaries()
    {
        Tool tool = new();
        TokenAssertion assertion = new()
        {
            AuthenticationContextClassReference = "urn:kdp:manager-async",
            Audiences = ["aud-manager-async"],
            AuthenticationMethods = ["pwd","mfa"],
            AuthenticationTime = DateTimeOffset.UtcNow.AddMinutes(-1),
            AuthorizedParty = "azp-manager-async",
            ClientID = "client-manager-async",
            Format = "jwt/v1",
            IssuedAt = DateTimeOffset.UtcNow.AddMinutes(-2),
            Issuer = "https://issuer.manager-async.example",
            JwtID = "jwt-manager-async",
            NotAfter = DateTimeOffset.UtcNow.AddMinutes(10),
            NotBefore = DateTimeOffset.UtcNow.AddMinutes(-3),
            ObjectID = "object-manager-async",
            Scopes = ["scope.manager.async"],
            SessionID = "sid-manager-async",
            Subject = "subject-manager-async",
            TenantID = "tenant-manager-async",
            Token = Encoding.UTF8.GetBytes("manager-jwt-async"),
            TokenType = TokenKeyType.Jwt
        };
        AsyncCapturingAssertionGenerator generator = new(assertion);
        AccessManagerOptions options = new(keyassertiongenerator: generator);
        AccessManager manager = new(options,tool);

        ClientKey? key = await manager.GenerateAccessKeyAsync<ClientKey>(AccessKeyIssueOptions.Create(subject: "manager-subject-async",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(key).IsNotNull();
        Check.That(generator.LastContext).IsNotNull();
        Check.That(generator.LastContext!.Value.Subject).IsEqualTo("manager-subject-async");
        Check.That(generator.LastContext!.Value.Request).IsNull();
        Check.That(generator.LastContext!.Value.RequestContext).IsNull();
        Check.That(generator.LastContext!.Value.RequestedKeyType).IsEqualTo(nameof(ClientKey));
        Check.That(generator.LastContext!.Value.AccessPolicyContext.Mode).IsEqualTo(AccessPolicyMode.Issue);

        Byte[] realmkey = GetRealmKey(manager);
        var identity = GetManifestIdentity(manager);
        Check.That(AccessKeySecret.TryReadClaims(key!.Key!,realmkey,identity.ToolSchemaID,identity.AccessKeyRealmID,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.AssertionSummaries.Length).IsEqualTo(1);
        Check.That(claims.AssertionSummaries[0]).IsInstanceOf<TokenAssertionSummary>();
        TokenAssertionSummary summary = (TokenAssertionSummary)claims.AssertionSummaries[0];
        Check.That(summary.AssertionType).IsEqualTo("token");
        Check.That(summary.AuthenticationContextClassReference).IsEqualTo("urn:kdp:manager-async");
        Check.That(summary.Format).IsEqualTo("jwt/v1");
        Check.That(summary.Issuer).IsEqualTo("https://issuer.manager-async.example");
        Check.That(summary.JwtID).IsEqualTo("jwt-manager-async");
        Check.That(summary.NotBefore).IsEqualTo(assertion.NotBefore);
        Check.That(summary.SessionID).IsEqualTo("sid-manager-async");
        Check.That(summary.Subject).IsEqualTo("subject-manager-async");
        Check.That(summary.Audiences.SequenceEqual(["aud-manager-async"])).IsTrue();
        Check.That(summary.AuthorizedParty).IsEqualTo("azp-manager-async");
        Check.That(summary.Scopes.SequenceEqual(["scope.manager.async"])).IsTrue();
        Check.That(summary.AuthenticationMethods.SequenceEqual(["pwd","mfa"])).IsTrue();
        Check.That(generator.AsyncCalls).IsEqualTo(1);
    }

    [Test]
    public async Task DefaultAccessKeyAssertionGenerator_GenerateAsync_Uses_Async_Child_Generators()
    {
        TokenAssertion assertion = new()
        {
            Format = "jwt/v1",
            Issuer = "https://generator.example",
            Subject = "subject-generator",
            Token = Encoding.UTF8.GetBytes("generator-jwt"),
            TokenType = TokenKeyType.Jwt
        };
        AsyncCapturingAssertionGenerator child = new(assertion);
        DefaultAccessKeyAssertionGenerator generator = new([child]);
        AccessKeyAssertionGeneratorContext context = new() { Subject = "subject-generator", RequestedKeyType = nameof(ClientKey) };

        ImmutableArray<AccessKeyAssertion> assertions = await generator.GenerateAsync(context);

        Check.That(assertions.Length).IsEqualTo(1);
        Check.That(assertions[0]).IsInstanceOf<TokenAssertion>();
        Check.That(child.AsyncCalls).IsEqualTo(1);
    }
}
