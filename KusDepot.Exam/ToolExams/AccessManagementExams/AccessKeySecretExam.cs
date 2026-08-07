namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public partial class AccessKeySecretExam
{
    private Byte[]? Key;
    private Byte[]? OtherKey;
    private Guid IssuerId;
    private Guid AccessManagerId;

    private static UInt128 ReadBlock(ReadOnlySpan<Byte> bitmap,Int32 block) => BinaryPrimitives.ReadUInt128BigEndian(bitmap.Slice(block * 16,16));

    private static Boolean IsBitSet(ReadOnlySpan<Byte> bitmap,Int32 op)
    {
        Int32 block = op / 128; Int32 bit = op % 128; UInt128 v = ReadBlock(bitmap,block); return ((v >> bit) & UInt128.One) == UInt128.One;
    }

    private static Int32 CountBits(ReadOnlySpan<Byte> bitmap)
    {
        Int32 count = 0; for(Int32 i =0;i<bitmap.Length / 16;i++){ UInt128 v = ReadBlock(bitmap,i); while(v!=UInt128.Zero){ v &= (v-1); count++; } } return count;
    }

    private static String CreateManifestHash(String input) => Convert.ToHexString(SHA512.HashData(Encoding.UTF8.GetBytes(input)));

    private static void SleepMillis(Int32 ms) => Thread.Sleep(ms);

    [OneTimeSetUp]
    public void Calibrate()
    {
        IssuerId = Guid.NewGuid();
        AccessManagerId = Guid.NewGuid();
        Key = new Byte[AccessKeySecret.SymmetricKeySize];
        OtherKey = new Byte[AccessKeySecret.SymmetricKeySize];
        RandomNumberGenerator.Fill(Key);
        RandomNumberGenerator.Fill(OtherKey);
    }

    private Boolean CreateToken(IEnumerable<Int32> ops , String? subject , TimeSpan? life , out Byte[]? ct , out AccessKeyToken token)
    {
        return AccessKeySecret.TryCreate(CreateRequest(ops,subject,life),Key!,out ct,out token);
    }

    private AccessKeySecretCreateRequest CreateRequest(IEnumerable<Int32> ops , String? subject , TimeSpan? life = null , IAccessKeySigner? signer = null)
    {
        return new()
        {
            AccessKeyRealmID = String.Empty,
            IssuerAccessManagerID = AccessManagerId,
            IssuerToolID = IssuerId,
            Lifetime = life,
            ManifestHash = String.Empty,
            Operations = ops,
            Signer = signer,
            Subject = subject ?? String.Empty,
            ToolSchemaID = String.Empty
        };
    }

    private Boolean ReadClaims(Byte[] ct , String? toolschemaid , String? accesskeyrealmid , out AccessKeyClaims claims)
    {
        return AccessKeySecret.TryReadClaims(ct,Key!,toolschemaid,accesskeyrealmid,out claims);
    }

    /* Creation / Round Trip */

    [Test]
    public void Create_NoOps_Succeeds()
    {
        Boolean ok = CreateToken(Array.Empty<Int32>(),"noops",null,out Byte[]? ct,out var tok); Check.That(ok).IsTrue(); Check.That(ct).IsNotNull();
        String tokHex = tok.ToString(); Check.That(tokHex.Length).IsEqualTo(128);
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.Subject).IsEqualTo("noops"); Check.That(claims.Expired).IsFalse(); Check.That(CountBits(claims.Permissions)).IsEqualTo(0); Check.That(claims.Token == tok).IsTrue();
    }

    [Test]
    public void Create_RequestOverload_WithoutSigner_Creates_Unsigned_Envelope()
    {
        AccessKeySecretCreateRequest request = CreateRequest(new[] { 2,4 },"request-unsigned",TimeSpan.FromMinutes(1));

        Boolean ok = AccessKeySecret.TryCreate(request,Key!,out Byte[]? ct,out AccessKeyToken token);

        Check.That(ok).IsTrue();
        Check.That(ct).IsNotNull();
        Check.That(AccessKeySecret.IsSigned(ct!)).IsFalse();
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.Subject).IsEqualTo("request-unsigned");
        Check.That(claims.Token == token).IsTrue();
        Check.That(IsBitSet(claims.Permissions,2)).IsTrue();
        Check.That(IsBitSet(claims.Permissions,4)).IsTrue();
    }

    [Test]
    public void Create_SparseIndices_Succeeds()
    {
        Int32[] ops = {0,127,128,8191,16383}; Boolean ok = CreateToken(ops,"sparse",null,out Byte[]? ct,out var tok); Check.That(ok).IsTrue();
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.Subject).IsEqualTo("sparse"); Check.That(claims.Expired).IsFalse(); Check.That(claims.Token == tok).IsTrue();
        foreach(var op in ops) { Check.That(IsBitSet(claims.Permissions,op)).IsTrue(); } Check.That(CountBits(claims.Permissions)).IsEqualTo(5);
    }

    [Test]
    public void Create_DuplicateIndices_Ignored()
    {
        Int32[] ops = {5,5,5,5}; Boolean ok = CreateToken(ops,"dups",null,out Byte[]? ct,out var tok); Check.That(ok).IsTrue();
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(IsBitSet(claims.Permissions,5)).IsTrue(); Check.That(CountBits(claims.Permissions)).IsEqualTo(1); Check.That(claims.Token == tok).IsTrue();
    }

    [Test]
    public void Create_InvalidIndex_Negative_Fails()
    {
        Boolean ok = CreateToken(new[]{-1},"bad",null,out _,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Create_InvalidIndex_TooLarge_Fails()
    {
        Boolean ok = CreateToken(new[]{16384},"bad",null,out _,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Create_KeyNull_Fails()
    {
        Boolean ok = AccessKeySecret.TryCreate(CreateRequest(Array.Empty<Int32>(),"x"),null!,out _,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Create_SubjectNull_TreatedAsEmpty()
    {
        Boolean ok = CreateToken(new[]{1},null,null,out Byte[]? ct,out _); Check.That(ok).IsTrue();
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.Subject).IsEqualTo(String.Empty); Check.That(IsBitSet(claims.Permissions,1)).IsTrue();
    }

    [Test]
    public void Create_WithSchemaIdentityRealmAudienceAndScopes_PopulatesPayloadMetadata()
    {
        String manifestHash = CreateManifestHash("HASH-123");
        Boolean ok = AccessKeySecret.TryCreate(new()
        {
            AccessKeyRealmID = "Realm-A",
            Audiences = ["aud-2","aud-1","aud-1"],
            IssuerAccessManagerID = AccessManagerId,
            IssuerToolID = IssuerId,
            Lifetime = TimeSpan.FromMinutes(1),
            ManifestHash = manifestHash,
            Operations = new[] { 1,129 },
            Scopes = ["scope.z","scope.a","scope.a"],
            Subject = "schema",
            ToolSchemaID = "KusDepot.Tools/Schema"
        },Key!,out Byte[]? ct,out _);
        Check.That(ok).IsTrue();

        Check.That(ReadClaims(ct!,"KusDepot.Tools/Schema","Realm-A",out AccessKeyClaims claims)).IsTrue();

        Check.That(claims.IssuerToolID).IsEqualTo(IssuerId);
        Check.That(claims.IssuerAccessManagerID).IsEqualTo(AccessManagerId);
        Check.That(claims.ToolSchemaID).IsEqualTo("KusDepot.Tools/Schema");
        Check.That(claims.AccessKeyRealmID).IsEqualTo("Realm-A");
        Check.That(claims.Audiences.SequenceEqual(["aud-1","aud-2"])).IsTrue();
        Check.That(claims.Scopes.SequenceEqual(["scope.a","scope.z"])).IsTrue();
        Check.That(claims.ManifestHash).IsEqualTo(manifestHash);

        Check.That(claims.Subject).IsEqualTo("schema");
        Check.That(IsBitSet(claims.Permissions,1)).IsTrue();
        Check.That(IsBitSet(claims.Permissions,129)).IsTrue();
    }

    [Test]
    public void Create_DefaultOverload_Uses_EmptySchemaIdentity()
    {
        Boolean ok = CreateToken(new[]{6},"defaultschema",null,out Byte[]? ct,out _); Check.That(ok).IsTrue();

        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();

        Check.That(claims.ToolSchemaID).IsEqualTo(String.Empty);
        Check.That(claims.AccessKeyRealmID).IsEqualTo(String.Empty);
        Check.That(claims.Audiences).IsEmpty();
        Check.That(claims.Scopes).IsEmpty();
        Check.That(claims.ManifestHash).IsEqualTo(String.Empty);
    }

    [Test]
    public void Create_WithUnicodeMetadata_RoundTrips()
    {
        String manifestHash = CreateManifestHash("日本語-manifest");
        Boolean ok = AccessKeySecret.TryCreate(new()
        {
            AccessKeyRealmID = "Realm-主",
            Audiences = ["aud-é"],
            IssuerAccessManagerID = AccessManagerId,
            IssuerToolID = IssuerId,
            Lifetime = TimeSpan.FromMinutes(1),
            ManifestHash = manifestHash,
            Operations = new[] { 4 },
            Scopes = ["scope-ß"],
            Subject = "sübject-主",
            ToolSchemaID = "KusDepot.Tools/Schéma"
        },Key!,out Byte[]? ct,out _);

        Check.That(ok).IsTrue();
        Check.That(ReadClaims(ct!,"KusDepot.Tools/Schéma","Realm-主",out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.Subject).IsEqualTo("sübject-主");
        Check.That(claims.ToolSchemaID).IsEqualTo("KusDepot.Tools/Schéma");
        Check.That(claims.AccessKeyRealmID).IsEqualTo("Realm-主");
        Check.That(claims.Audiences.SequenceEqual(["aud-é"])).IsTrue();
        Check.That(claims.Scopes.SequenceEqual(["scope-ß"])).IsTrue();
        Check.That(claims.ManifestHash).IsEqualTo(manifestHash);
    }

    /* Randomness / Uniqueness */

    [Test]
    public void CiphertextAndToken_Random()
    {
        Int32[] ops = {1,2,3}; String subject = "rand"; HashSet<String> ctHashes = new(); HashSet<String> tokenHex = new();
        for(Int32 i =0;i<15;i++)
        {
            Boolean ok = CreateToken(ops,subject,null,out Byte[]? ct,out var tok); Check.That(ok).IsTrue();
            String ctH = Convert.ToHexString(SHA256.HashData(ct!)); Check.That(ctHashes.Contains(ctH)).IsFalse(); ctHashes.Add(ctH);
            String tH = tok.ToString(); Check.That(tokenHex.Contains(tH)).IsFalse(); tokenHex.Add(tH);
        }
    }

    [Test]
    public void AccessKeyToken_Equality_Operators()
    {
        CreateToken(new[]{7},"eq",null,out Byte[]? ct,out var tokA); CreateToken(new[]{7},"eq",null,out Byte[]? ct2,out var tokB);
        Check.That(tokA == tokA).IsTrue(); Check.That(tokA != tokA).IsFalse(); Check.That(tokA == tokB).IsFalse(); Check.That(tokA != tokB).IsTrue();
        var clone = new AccessKeyToken(Convert.FromHexString(tokA.ToString())); Check.That(clone == tokA).IsTrue();
    }

    /* Parse Success & Permission Bits */

    [Test]
    public void Parse_PermissionBits_Correct()
    {
        Int32[] ops = {0,127,128,8191,16383}; CreateToken(ops,"perm",null,out Byte[]? ct,out _);
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.Subject).IsEqualTo("perm"); Check.That(claims.Expired).IsFalse(); Check.That(CountBits(claims.Permissions)).IsEqualTo(5);
        foreach(var op in ops){ Check.That(IsBitSet(claims.Permissions,op)).IsTrue(); }
    }

    [Test]
    public void Parse_NoExpiry_ExpiredFalse()
    {
        CreateToken(new[]{5},"noexp",null,out Byte[]? ct,out _);
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue(); Check.That(claims.Expired).IsFalse();
    }

    [Test]
    public void Parse_WithLifetime_Expires()
    {
        CreateToken(new[]{9},"short",TimeSpan.FromSeconds(5),out Byte[]? ct,out _);
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims1)).IsTrue(); Check.That(claims1.Expired).IsFalse();
        SleepMillis(5000);
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims2)).IsTrue(); Check.That(claims2.Expired).IsTrue();
    }

    /* AAD Binding */

    [Test]
    public void Parse_WrongSchema_Fails()
    {
        CreateToken(new[]{3},"issuer",null,out Byte[]? ct,out _);
        Boolean parseOk = AccessKeySecret.TryReadClaims(ct!,Key!,"wrong-schema",String.Empty,out _);
        Check.That(parseOk).IsFalse();
    }

    [Test]
    public void Parse_WrongKey_Fails()
    {
        CreateToken(new[]{11},"key",null,out Byte[]? ct,out _);
        Boolean parseOk = AccessKeySecret.TryReadClaims(ct!,OtherKey!,String.Empty,String.Empty,out _);
        Check.That(parseOk).IsFalse();
    }

    /* Integrity / Tamper Detection */

    private static Byte[] Flip(Byte[] data,Int32 index){ var m=(Byte[])data.Clone(); m[index]^=0xFF; return m; }

    [Test]
    public void Tamper_VersionByte_Fails()
    {
        CreateToken(new[]{1},"tv",null,out Byte[]? ct,out _); var mod = Flip(ct!,0);
        Boolean ok = AccessKeySecret.TryReadClaims(mod,Key!,String.Empty,String.Empty,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Tamper_NonceByte_Fails()
    {
        CreateToken(new[]{2},"nonce",null,out Byte[]? ct,out _); var mod = Flip(ct!,1);
        Boolean ok = AccessKeySecret.TryReadClaims(mod,Key!,String.Empty,String.Empty,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Tamper_CiphertextByte_Fails()
    {
        Int32[] ops = {0}; CreateToken(ops,"tpb",null,out Byte[]? ct,out _);
        Int32 mid = ct!.Length / 2; var mod = Flip(ct!,mid);
        Boolean ok = AccessKeySecret.TryReadClaims(mod,Key!,String.Empty,String.Empty,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Tamper_TagByte_Fails()
    {
        CreateToken(new[]{4},"tag",null,out Byte[]? ct,out _); var mod = Flip(ct!,ct!.Length-1);
        Boolean ok = AccessKeySecret.TryReadClaims(mod,Key!,String.Empty,String.Empty,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Truncation_Fails()
    {
        CreateToken(new[]{7},"trunc",null,out Byte[]? ct,out _);
        for(Int32 cut =1; cut<10 && cut<ct!.Length; cut++)
        {
            var tr = new Byte[ct.Length-cut]; Array.Copy(ct,tr,tr.Length);
            Boolean ok = AccessKeySecret.TryReadClaims(tr,Key!,String.Empty,String.Empty,out _); Check.That(ok).IsFalse();
        }
    }

    /* Boundary Bits */

    [Test]
    public void Permission_Index0_Set()
    {
        CreateToken(new[]{0},"b0",null,out Byte[]? ct,out _);
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(IsBitSet(claims.Permissions,0)).IsTrue();
        Check.That(CountBits(claims.Permissions)).IsEqualTo(1);
    }

    [Test]
    public void Permission_Index_Max_Set()
    {
        CreateToken(new[]{16383},"bMax",null,out Byte[]? ct,out _);
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(IsBitSet(claims.Permissions,16383)).IsTrue();
        Check.That(CountBits(claims.Permissions)).IsEqualTo(1);
    }

    /* Token Identity */

    [Test]
    public void TokenId_RoundTrips()
    {
        CreateToken(new[]{10},"token",null,out Byte[]? ct,out var issuedToken);
        Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.Subject).IsEqualTo("token");
        Check.That(claims.Permissions.Length).IsGreaterOrEqualThan(0);
        Check.That(claims.Expired).IsFalse();
        Check.That(claims.NotAfter).IsNull();
        Check.That(claims.IssuedAt).IsNotEqualTo(default(DateTimeOffset));
        Check.That(claims.Token).IsEqualTo(issuedToken);
    }

    /* Stress */

    [Test]
    public void Issue_Parse_Stress()
    {
        Int32 iterations = 100;
        
        for(Int32 i =0;i<iterations;i++)
        {
            Boolean ok = CreateToken(new[]{i%70},"stress",null,out Byte[]? ct,out var tok); Check.That(ok).IsTrue();

            Check.That(ReadClaims(ct!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue(); Check.That(claims.Expired).IsFalse(); Check.That(IsBitSet(claims.Permissions,i % 70)).IsTrue(); Check.That(claims.Token == tok).IsTrue();
        }
    }
}
