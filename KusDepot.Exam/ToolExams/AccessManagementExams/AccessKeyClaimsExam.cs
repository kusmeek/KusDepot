namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class AccessKeyClaimsExam
{
    private const Int32 BitsPerBlock = 128;
    private const Int32 BytesPerBlock = 16;

    private static AccessKeyClaims CreateClaims(Int32[]? operations = null , ImmutableArray<String> audiences = default , ImmutableArray<String> scopes = default , DateTimeOffset? notafter = null , Guid? issuertoolid = null , Guid? issueraccessmanagerid = null)
    {
        Byte[] permissions = CreatePermissions(operations ?? []);

        return new()
        {
            AccessKeyRealmID = "Realm-A",
            Audiences = audiences.IsDefault ? ["aud-a","aud-b"] : audiences,
            Expired = false,
            IssuedAt = DateTimeOffset.UtcNow,
            IssuerAccessManagerID = issueraccessmanagerid ?? Guid.Parse("22222222-2222-2222-2222-222222222222"),
            IssuerToolID = issuertoolid ?? Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ManifestHash = "HASH-123",
            NotAfter = notafter,
            Permissions = permissions,
            Scopes = scopes.IsDefault ? ["scope.a","scope.b"] : scopes,
            Subject = "subject-a",
            Token = default,
            ToolSchemaID = "KusDepot.Tools/Schema"
        };
    }

    private static Byte[] CreatePermissions(Int32[] operations)
    {
        if(operations.Length == 0) { return []; }

        Int32 maxop = operations.Max();
        Int32 blockcount = (maxop / BitsPerBlock) + 1;
        Byte[] permissions = new Byte[blockcount * BytesPerBlock];

        foreach(Int32 operation in operations)
        {
            Int32 blockindex = operation / BitsPerBlock;
            Int32 bit = operation % BitsPerBlock;
            Int32 offset = blockindex * BytesPerBlock;
            UInt128 block = BinaryPrimitives.ReadUInt128BigEndian(permissions.AsSpan(offset,BytesPerBlock));
            block |= UInt128.One << bit;
            BinaryPrimitives.WriteUInt128BigEndian(permissions.AsSpan(offset,BytesPerBlock),block);
        }

        return permissions;
    }

    [Test]
    public void AccessKeyClaimsAllows_Returns_True_For_Configured_Operation()
    {
        AccessKeyClaims claims = CreateClaims([1,129]);

        Check.That(claims.Allows(129)).IsTrue();
    }

    [Test]
    public void AccessKeyClaimsAllows_Returns_False_For_Unconfigured_Operation()
    {
        AccessKeyClaims claims = CreateClaims([1,129]);

        Check.That(claims.Allows(5)).IsFalse();
    }

    [Test]
    public void AccessKeyClaimsHasAudience_Returns_True_For_Exact_Match()
    {
        AccessKeyClaims claims = CreateClaims();

        Check.That(claims.HasAudience("aud-b")).IsTrue();
    }

    [Test]
    public void AccessKeyClaimsHasScope_Returns_True_For_Exact_Match()
    {
        AccessKeyClaims claims = CreateClaims();

        Check.That(claims.HasScope("scope.b")).IsTrue();
    }

    [Test]
    public void AccessKeyClaimsIsForRealm_Returns_True_For_Exact_Match()
    {
        AccessKeyClaims claims = CreateClaims();

        Check.That(claims.IsForRealm("Realm-A")).IsTrue();
    }

    [Test]
    public void AccessKeyClaimsIsForSchema_Returns_True_For_Exact_Match()
    {
        AccessKeyClaims claims = CreateClaims();

        Check.That(claims.IsForSchema("KusDepot.Tools/Schema")).IsTrue();
    }

    [Test]
    public void AccessKeyClaimsIsIssuedBy_Returns_True_When_Both_Issuers_Match()
    {
        Guid issuertoolid = Guid.Parse("11111111-1111-1111-1111-111111111111");
        Guid issueraccessmanagerid = Guid.Parse("22222222-2222-2222-2222-222222222222");
        AccessKeyClaims claims = CreateClaims(issuertoolid: issuertoolid,issueraccessmanagerid: issueraccessmanagerid);

        Check.That(claims.IsIssuedBy(issuertoolid,issueraccessmanagerid)).IsTrue();
    }

    [Test]
    public void AccessKeyClaimsIsIssuedBy_Returns_False_When_No_Issuer_Is_Supplied()
    {
        AccessKeyClaims claims = CreateClaims();

        Check.That(claims.IsIssuedBy()).IsFalse();
    }

    [Test]
    public void AccessKeyClaimsIsExpiredAt_Returns_True_When_Instant_Is_After_NotAfter()
    {
        DateTimeOffset notafter = DateTimeOffset.UtcNow.AddMinutes(5);
        AccessKeyClaims claims = CreateClaims(notafter: notafter);

        Check.That(claims.IsExpiredAt(notafter.AddSeconds(1))).IsTrue();
    }
}
