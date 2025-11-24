namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AccessKeySecretExam
{
    private X509Certificate2? Cert;
    private X509Certificate2? OtherCert;
    private Guid IssuerId;
    private Guid AccessManagerId;

    private static Byte[] Rand(Int32 size) { Byte[] b = new Byte[size]; RandomNumberGenerator.Fill(b); return b; }

    private static UInt128 ReadBlock(ReadOnlySpan<Byte> bitmap,Int32 block) => BinaryPrimitives.ReadUInt128BigEndian(bitmap.Slice(block * 16,16));

    private static Boolean IsBitSet(ReadOnlySpan<Byte> bitmap,Int32 op)
    {
        Int32 block = op / 128; Int32 bit = op % 128; UInt128 v = ReadBlock(bitmap,block); return ((v >> bit) & UInt128.One) == UInt128.One;
    }

    private static Int32 CountBits(ReadOnlySpan<Byte> bitmap)
    {
        Int32 count = 0; for(Int32 i =0;i<bitmap.Length / 16;i++){ UInt128 v = ReadBlock(bitmap,i); while(v!=UInt128.Zero){ v &= (v-1); count++; } } return count;
    }

    private static void SleepMillis(Int32 ms) => Thread.Sleep(ms);

    [OneTimeSetUp]
    public void Calibrate()
    {
        IssuerId = Guid.NewGuid();
        AccessManagerId = Guid.NewGuid();
        Cert = CreateCertificate(IssuerId,"AccessKeySecretExam",2048,2,null);
        OtherCert = CreateCertificate(Guid.NewGuid(),"AccessKeySecretExamOther",2048,2,null);
        Check.That(Cert).IsNotNull();
        Check.That(OtherCert).IsNotNull();
    }

    private Boolean CreateToken(IEnumerable<Int32> ops , String? subject , TimeSpan? life , out Byte[]? ct , out AccessKeyToken token)
    {
        return AccessKeySecret.TryCreate(Cert!,IssuerId,AccessManagerId,subject ?? String.Empty,ops,life,out ct,out token);
    }

    private Boolean Parse(Byte[] ct , Guid issuer , Guid accessmanagerid , out String? subject , out ReadOnlyMemory<Byte> bitmap , out AccessKeyToken token , out Boolean expired)
    {
        return AccessKeySecret.TryParse(ct,Cert!,issuer,accessmanagerid,out subject,out bitmap,out DateTimeOffset issued,out DateTimeOffset? notAfter,out token,out expired);
    }

    /* Creation / Round Trip */

    [Test]
    public void Create_NoOps_Succeeds()
    {
        Boolean ok = CreateToken(Array.Empty<Int32>(),"noops",null,out Byte[]? ct,out var tok); Check.That(ok).IsTrue(); Check.That(ct).IsNotNull();
        String tokHex = tok.ToString(); Check.That(tokHex.Length).IsEqualTo(128);
        Boolean parseOk = Parse(ct!,IssuerId,AccessManagerId,out var subj,out var bitmap,out var parsedTok,out var expired);
        Check.That(parseOk).IsTrue(); Check.That(subj).IsEqualTo("noops"); Check.That(expired).IsFalse(); Check.That(CountBits(bitmap.Span)).IsEqualTo(0); Check.That(parsedTok == tok).IsTrue();
    }

    [Test]
    public void Create_SparseIndices_Succeeds()
    {
        Int32[] ops = {0,127,128,8191,16383}; Boolean ok = CreateToken(ops,"sparse",null,out Byte[]? ct,out var tok); Check.That(ok).IsTrue();
        Boolean parseOk = Parse(ct!,IssuerId,AccessManagerId,out var subj,out var bitmap,out var parsedTok,out var expired);
        Check.That(parseOk).IsTrue(); Check.That(subj).IsEqualTo("sparse"); Check.That(expired).IsFalse();
        foreach(var op in ops) { Check.That(IsBitSet(bitmap.Span,op)).IsTrue(); } Check.That(CountBits(bitmap.Span)).IsEqualTo(5);
    }

    [Test]
    public void Create_DuplicateIndices_Ignored()
    {
        Int32[] ops = {5,5,5,5}; Boolean ok = CreateToken(ops,"dups",null,out Byte[]? ct,out var tok); Check.That(ok).IsTrue();
        Boolean parseOk = Parse(ct!,IssuerId,AccessManagerId,out _,out var bitmap,out _,out _);
        Check.That(parseOk).IsTrue(); Check.That(IsBitSet(bitmap.Span,5)).IsTrue(); Check.That(CountBits(bitmap.Span)).IsEqualTo(1);
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
    public void Create_CertificateNull_Fails()
    {
        Boolean ok = AccessKeySecret.TryCreate(null!,IssuerId,AccessManagerId,"x",Array.Empty<Int32>(),null,out _,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Create_SubjectNull_TreatedAsEmpty()
    {
        Boolean ok = CreateToken(new[]{1},null,null,out Byte[]? ct,out _); Check.That(ok).IsTrue();
        Boolean parseOk = Parse(ct!,IssuerId,AccessManagerId,out var subj,out var bitmap,out _,out _);
        Check.That(parseOk).IsTrue(); Check.That(subj).IsEqualTo(String.Empty); Check.That(IsBitSet(bitmap.Span,1)).IsTrue();
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
        Boolean parseOk = Parse(ct!,IssuerId,AccessManagerId,out var subj,out var bitmap,out _,out var expired);
        Check.That(parseOk).IsTrue(); Check.That(subj).IsEqualTo("perm"); Check.That(expired).IsFalse(); Check.That(CountBits(bitmap.Span)).IsEqualTo(5);
        foreach(var op in ops){ Check.That(IsBitSet(bitmap.Span,op)).IsTrue(); }
    }

    [Test]
    public void Parse_NoExpiry_ExpiredFalse()
    {
        CreateToken(new[]{5},"noexp",null,out Byte[]? ct,out _);
        Boolean parseOk = Parse(ct!,IssuerId,AccessManagerId,out _,out _,out _,out var expired);
        Check.That(parseOk).IsTrue(); Check.That(expired).IsFalse();
    }

    [Test]
    public void Parse_WithLifetime_Expires()
    {
        CreateToken(new[]{9},"short",TimeSpan.FromSeconds(5),out Byte[]? ct,out _);
        Boolean parseOk1 = Parse(ct!,IssuerId,AccessManagerId,out _,out _,out _,out var exp1);
        Check.That(parseOk1).IsTrue(); Check.That(exp1).IsFalse();
        SleepMillis(5000);
        Boolean parseOk2 = Parse(ct!,IssuerId,AccessManagerId,out _,out _,out _,out var exp2);
        Check.That(parseOk2).IsTrue(); Check.That(exp2).IsTrue();
    }

    /* AAD / Issuer Binding */

    [Test]
    public void Parse_WrongIssuer_Fails()
    {
        CreateToken(new[]{3},"issuer",null,out Byte[]? ct,out _);
        Boolean parseOk = AccessKeySecret.TryParse(ct!,Cert!,Guid.NewGuid(),AccessManagerId,out _,out _,out _,out _,out _,out _);
        Check.That(parseOk).IsFalse();
    }

    [Test]
    public void Parse_WrongCertificate_Fails()
    {
        CreateToken(new[]{11},"cert",null,out Byte[]? ct,out _);
        Boolean parseOk = AccessKeySecret.TryParse(ct!,OtherCert!,IssuerId,AccessManagerId,out _,out _,out _,out _,out _,out _);
        Check.That(parseOk).IsFalse();
    }

    /* Integrity / Tamper Detection */

    private static Byte[] Flip(Byte[] data,Int32 index){ var m=(Byte[])data.Clone(); m[index]^=0xFF; return m; }

    [Test]
    public void Tamper_VersionByte_Fails()
    {
        CreateToken(new[]{1},"tv",null,out Byte[]? ct,out _); var mod = Flip(ct!,0);
        Boolean ok = Parse(mod,IssuerId,AccessManagerId,out _,out _,out _,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Tamper_SubjectLengthByte_Fails()
    {
        CreateToken(new[]{2},"tsl",null,out Byte[]? ct,out _); var mod = Flip(ct!,66);
        Boolean ok = Parse(mod,IssuerId,AccessManagerId,out _,out _,out _,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Tamper_PermissionBitmapByte_Fails()
    {
        Int32[] ops = {0}; CreateToken(ops,"tpb",null,out Byte[]? ct,out _);
        Parse(ct!,IssuerId,AccessManagerId,out var subj,out var bitmap,out _,out _);
        Int32 mid = ct!.Length/2; var mod = Flip(ct!,mid);
        Boolean ok = Parse(mod,IssuerId,AccessManagerId,out _,out _,out _,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Tamper_SaltByte_Fails()
    {
        CreateToken(new[]{4},"salt",null,out Byte[]? ct,out _);
        Int32 pos = Math.Max(0,ct!.Length - 40); var mod = Flip(ct!,pos);
        Boolean ok = Parse(mod,IssuerId,AccessManagerId,out _,out _,out _,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Tamper_AccessKeyTokenByte_Fails()
    {
        CreateToken(new[]{6},"hash",null,out Byte[]? ct,out _); var mod = Flip(ct!,ct!.Length-1);
        Boolean ok = Parse(mod,IssuerId,AccessManagerId,out _,out _,out _,out _); Check.That(ok).IsFalse();
    }

    [Test]
    public void Truncation_Fails()
    {
        CreateToken(new[]{7},"trunc",null,out Byte[]? ct,out _);
        for(Int32 cut =1; cut<10 && cut<ct!.Length; cut++)
        {
            var tr = new Byte[ct.Length-cut]; Array.Copy(ct,tr,tr.Length);
            Boolean ok = Parse(tr,IssuerId,AccessManagerId,out _,out _,out _,out _); Check.That(ok).IsFalse();
        }
    }

    /* Boundary Bits */

    [Test]
    public void Permission_Index0_Set()
    {
        CreateToken(new[]{0},"b0",null,out Byte[]? ct,out _);
        Parse(ct!,IssuerId,AccessManagerId,out _,out var bitmap,out _,out _);
        Check.That(IsBitSet(bitmap.Span,0)).IsTrue();
        Check.That(CountBits(bitmap.Span)).IsEqualTo(1);
    }

    [Test]
    public void Permission_Index_Max_Set()
    {
        CreateToken(new[]{16383},"bMax",null,out Byte[]? ct,out _);
        Parse(ct!,IssuerId,AccessManagerId,out _,out var bitmap,out _,out _);
        Check.That(IsBitSet(bitmap.Span,16383)).IsTrue();
        Check.That(CountBits(bitmap.Span)).IsEqualTo(1);
    }

    /* Hash Recompute Validation */

    [Test]
    public void TokenHash_Recompute_Matches()
    {
        CreateToken(new[]{10},"rehash",null,out Byte[]? ct,out _);
        Boolean parseOk = AccessKeySecret.TryParse(ct!,Cert!,IssuerId,AccessManagerId,out var subject,out var bitmap,out var issued,out var notAfter,out var token,out var expired); Check.That(parseOk).IsTrue();
        Span<Byte> aad = stackalloc Byte[1 + 16 + 16]; aad[0]=0x01; IssuerId.TryWriteBytes(aad[1..]); AccessManagerId.TryWriteBytes(aad.Slice(1 + 16, 16));
        Byte[] decrypted = ct!.Decrypt(Cert,aad.ToArray())!;
        Int32 tokenHashSize = 64; Int32 len = decrypted.Length - tokenHashSize; Byte[] comp = SHA512.HashData(decrypted.AsSpan(0,len));
        Check.That(Convert.ToHexString(comp)).IsEqualTo(token.ToString());
        decrypted.AsSpan().Clear();
    }

    /* Stress */

    [Test]
    public void Issue_Parse_Stress()
    {
        Int32 iterations = 100;
        
        for(Int32 i =0;i<iterations;i++)
        {
            Boolean ok = CreateToken(new[]{i%70},"stress",null,out Byte[]? ct,out var tok); Check.That(ok).IsTrue();
            
            Boolean parseOk = Parse(ct!,IssuerId,AccessManagerId,out _,out _,out var parsedTok,out var expired); Check.That(parseOk).IsTrue(); Check.That(expired).IsFalse(); Check.That(parsedTok == tok).IsTrue();
        }
    }
}
