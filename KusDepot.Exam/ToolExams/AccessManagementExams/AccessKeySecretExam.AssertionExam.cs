namespace KusDepot.Exams.Security;

partial class AccessKeySecretExam
{
    private static TokenAssertion CreateTokenAssertion(
        String tokentext,
        String? issuer = "https://issuer.example",
        String? subject = "subject-1",
        String? tenantid = "tenant-1",
        String? objectid = "object-1",
        String? clientid = "client-1")
    {
        return new()
        {
            AuthenticationContextClassReference = "urn:kdp:mfa",
            Audiences = ["aud-1","aud-2"],
            AuthenticationMethods = ["pwd","mfa"],
            AuthenticationTime = DateTimeOffset.UtcNow.AddMinutes(-1),
            AuthorizedParty = "client-azp",
            ClientID = clientid,
            Format = "jwt/v1",
            IssuedAt = DateTimeOffset.UtcNow.AddMinutes(-2),
            Issuer = issuer,
            JwtID = "jwt-id-1",
            NotAfter = DateTimeOffset.UtcNow.AddMinutes(5),
            NotBefore = DateTimeOffset.UtcNow.AddMinutes(-3),
            ObjectID = objectid,
            Scopes = ["scope.a","scope.b"],
            SessionID = "session-1",
            Subject = subject,
            TenantID = tenantid,
            Token = Encoding.UTF8.GetBytes(tokentext),
            TokenType = TokenKeyType.Jwt
        };
    }

    private Boolean CreateTokenWithAssertions(IEnumerable<Int32> ops , IEnumerable<AccessKeyAssertion>? assertions , out Byte[]? ct , out AccessKeyToken token)
    {
        return AccessKeySecret.TryCreate(new()
        {
            AccessKeyRealmID = String.Empty,
            Assertions = assertions,
            IssuerAccessManagerID = AccessManagerId,
            IssuerToolID = IssuerId,
            ManifestHash = String.Empty,
            Operations = ops,
            Subject = "assertions",
            ToolSchemaID = String.Empty
        },Key!,out ct,out token);
    }

    [Test]
    public void Create_WithAssertions_PersistsAndReadsAssertions()
    {
        TokenAssertion expected = CreateTokenAssertion("jwt-token-1");

        Boolean ok = CreateTokenWithAssertions(new[]{1,2},[expected],out Byte[]? ct,out _);

        Check.That(ok).IsTrue();
        Check.That(ct).IsNotNull();
        Check.That(AccessKeySecret.TryReadAssertions(ct!,Key!,String.Empty,String.Empty,out ImmutableArray<AccessKeyAssertion> assertions)).IsTrue();
        Check.That(assertions.Length).IsEqualTo(1);
        Check.That(assertions[0]).IsInstanceOf<TokenAssertion>();

        TokenAssertion actual = (TokenAssertion)assertions[0];
        Check.That(actual.AssertionType).IsEqualTo("token");
        Check.That(actual.Format).IsEqualTo(expected.Format);
        Check.That(actual.AuthenticationContextClassReference).IsEqualTo(expected.AuthenticationContextClassReference);
        Check.That(actual.Audiences.SequenceEqual(expected.Audiences)).IsTrue();
        Check.That(actual.Scopes.SequenceEqual(expected.Scopes)).IsTrue();
        Check.That(actual.AuthorizedParty).IsEqualTo(expected.AuthorizedParty);
        Check.That(actual.Issuer).IsEqualTo(expected.Issuer);
        Check.That(actual.JwtID).IsEqualTo(expected.JwtID);
        Check.That(actual.NotBefore).IsEqualTo(expected.NotBefore);
        Check.That(actual.SessionID).IsEqualTo(expected.SessionID);
        Check.That(actual.Subject).IsEqualTo(expected.Subject);
        Check.That(actual.TenantID).IsEqualTo(expected.TenantID);
        Check.That(actual.ObjectID).IsEqualTo(expected.ObjectID);
        Check.That(actual.ClientID).IsEqualTo(expected.ClientID);
        Check.That(actual.AuthenticationMethods.SequenceEqual(expected.AuthenticationMethods)).IsTrue();
        Check.That(Encoding.UTF8.GetString(actual.Token)).IsEqualTo("jwt-token-1");
    }

    [Test]
    public void Create_WithAssertions_ReadClaims_ProjectsAssertionSummaries()
    {
        TokenAssertion expected = CreateTokenAssertion("jwt-token-2",issuer: "issuer-2",subject: "subject-2",tenantid: "tenant-2",objectid: "object-2",clientid: "client-2");

        Boolean ok = CreateTokenWithAssertions(new[]{3},[expected],out Byte[]? ct,out _);

        Check.That(ok).IsTrue();
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.AssertionSummaries.Length).IsEqualTo(1);
        Check.That(claims.AssertionSummaries[0]).IsInstanceOf<TokenAssertionSummary>();

        TokenAssertionSummary summary = (TokenAssertionSummary)claims.AssertionSummaries[0];
        Check.That(summary.AssertionType).IsEqualTo("token");
        Check.That(summary.Format).IsEqualTo(expected.Format);
        Check.That(summary.AuthenticationContextClassReference).IsEqualTo(expected.AuthenticationContextClassReference);
        Check.That(summary.Audiences.SequenceEqual(expected.Audiences)).IsTrue();
        Check.That(summary.Scopes.SequenceEqual(expected.Scopes)).IsTrue();
        Check.That(summary.AuthorizedParty).IsEqualTo(expected.AuthorizedParty);
        Check.That(summary.Issuer).IsEqualTo(expected.Issuer);
        Check.That(summary.JwtID).IsEqualTo(expected.JwtID);
        Check.That(summary.NotBefore).IsEqualTo(expected.NotBefore);
        Check.That(summary.SessionID).IsEqualTo(expected.SessionID);
        Check.That(summary.Subject).IsEqualTo(expected.Subject);
        Check.That(summary.TenantID).IsEqualTo(expected.TenantID);
        Check.That(summary.ObjectID).IsEqualTo(expected.ObjectID);
        Check.That(summary.ClientID).IsEqualTo(expected.ClientID);
        Check.That(summary.AuthenticationMethods.SequenceEqual(expected.AuthenticationMethods)).IsTrue();
    }

    [Test]
    public void Create_WithoutAssertions_TryReadAssertions_ReturnsEmpty()
    {
        Boolean ok = CreateTokenWithAssertions(new[]{4},null,out Byte[]? ct,out _);

        Check.That(ok).IsTrue();
        Check.That(AccessKeySecret.TryReadAssertions(ct!,Key!,String.Empty,String.Empty,out ImmutableArray<AccessKeyAssertion> assertions)).IsTrue();
        Check.That(assertions).IsEmpty();
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.AssertionSummaries).IsEmpty();
    }

    [Test]
    public void Create_WithMultipleAssertions_ReadsAllInOrder()
    {
        TokenAssertion first = CreateTokenAssertion("jwt-token-a",issuer: "issuer-a",subject: "subject-a");
        TokenAssertion second = CreateTokenAssertion("jwt-token-b",issuer: "issuer-b",subject: "subject-b");

        Boolean ok = CreateTokenWithAssertions(new[]{5},[first,second],out Byte[]? ct,out _);

        Check.That(ok).IsTrue();
        Check.That(AccessKeySecret.TryReadAssertions(ct!,Key!,String.Empty,String.Empty,out ImmutableArray<AccessKeyAssertion> assertions)).IsTrue();
        Check.That(assertions.Length).IsEqualTo(2);
        Check.That(((TokenAssertion)assertions[0]).Issuer).IsEqualTo("issuer-a");
        Check.That(((TokenAssertion)assertions[0]).Subject).IsEqualTo("subject-a");
        Check.That(((TokenAssertion)assertions[1]).Issuer).IsEqualTo("issuer-b");
        Check.That(((TokenAssertion)assertions[1]).Subject).IsEqualTo("subject-b");
    }
}
