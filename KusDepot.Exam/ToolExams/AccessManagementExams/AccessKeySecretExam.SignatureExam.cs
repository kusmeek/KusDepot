namespace KusDepot.Exams.Security;

public partial class AccessKeySecretExam
{
    private delegate Byte[] BuildSignedEnvelopeDelegate(
        ReadOnlySpan<Byte> ciphertext , ReadOnlySpan<Byte> nonce , ReadOnlySpan<Byte> tag , Guid signertoolid , Guid signeraccessmanagerid,
        AccessKeySignatureAlgorithm algorithm , AccessKeySignerKeyIdFormat signerkeyidformat , ReadOnlySpan<Byte> signerkeyid , ReadOnlySpan<Byte> signature);

    private sealed class FixedSignatureSigner(AccessKeySignatureInfo signinginfo , Byte[]? signedbytes = null) : IAccessKeySigner
    {
        public Int32 SignCalls { get; private set; }

        public AccessKeySignatureInfo GetSigningInfo(AccessKeySignatureSigningContext context) => signinginfo;

        public Boolean TrySign(AccessKeySignatureSigningContext context , Span<Byte> signature , out Int32 written)
        {
            SignCalls++;

            Byte[] material = signedbytes ?? SHA512.HashData(context.SignedRegion.Span).AsSpan(0,signinginfo.SignatureLength).ToArray();

            if(signature.Length < material.Length)
            {
                written = 0;
                return false;
            }

            material.CopyTo(signature);
            written = material.Length;
            return true;
        }
    }

    private static Byte[] BuildSignedEnvelope(Byte[] ciphertext , Guid signertoolid , Guid signeraccessmanagerid , Byte[]? signerkeyid = null , Byte[]? signature = null)
    {
        MethodInfo method = typeof(AccessKeySecret).GetMethod("BuildSignedEnvelope",BindingFlags.NonPublic | BindingFlags.Static,null,
            [typeof(ReadOnlySpan<Byte>),typeof(ReadOnlySpan<Byte>),typeof(ReadOnlySpan<Byte>),typeof(Guid),typeof(Guid),typeof(AccessKeySignatureAlgorithm),typeof(AccessKeySignerKeyIdFormat),typeof(ReadOnlySpan<Byte>),typeof(ReadOnlySpan<Byte>)],null)
            ?? throw new InvalidOperationException();

        BuildSignedEnvelopeDelegate buildSignedEnvelope = method.CreateDelegate<BuildSignedEnvelopeDelegate>();

        signerkeyid ??= SHA256.HashData(Encoding.UTF8.GetBytes("thumbprint"));
        signature ??= Encoding.UTF8.GetBytes("signature-bytes");

        return buildSignedEnvelope(
            ciphertext,
            ciphertext.AsSpan(46,12),
            ciphertext.AsSpan(ciphertext.Length - 16,16),
            signertoolid,
            signeraccessmanagerid,
            AccessKeySignatureAlgorithm.RsaPssSha512,
            AccessKeySignerKeyIdFormat.Thumbprint,
            signerkeyid,
            signature
        );
    }

    private static ReadOnlyMemory<Byte> ReadSignedRegion(Byte[] secret)
    {
        MethodInfo method = typeof(AccessKeySecret).GetMethod("TryReadSignedRegionArray",BindingFlags.NonPublic | BindingFlags.Static,null,[typeof(Byte[]),typeof(Byte[]).MakeByRefType()],null)
            ?? throw new InvalidOperationException();
        Object?[] args = [secret,Array.Empty<Byte>()];

        Boolean ok = (Boolean)method.Invoke(null,args)!;
        Check.That(ok).IsTrue();
        return ((Byte[])args[1]!).AsMemory();
    }

    private static ReadOnlyMemory<Byte> ReadSignatureBytes(Byte[] secret)
    {
        MethodInfo method = typeof(AccessKeySecret).GetMethod("TryReadSignatureBytesArray",BindingFlags.NonPublic | BindingFlags.Static,null,[typeof(Byte[]),typeof(Byte[]).MakeByRefType()],null)
            ?? throw new InvalidOperationException();
        Object?[] args = [secret,Array.Empty<Byte>()];

        Boolean ok = (Boolean)method.Invoke(null,args)!;
        Check.That(ok).IsTrue();
        return ((Byte[])args[1]!).AsMemory();
    }

    [Test]
    public void IsSigned_Returns_False_For_Unsigned_Envelope()
    {
        CreateToken(new[] { 1,2,3 },"unsigned",null,out Byte[]? ct,out _);

        Boolean signed = AccessKeySecret.IsSigned(ct!);

        Check.That(signed).IsFalse();
    }

    [Test]
    public void TryReadSignature_Returns_Unsigned_Descriptor_For_Unsigned_Envelope()
    {
        CreateToken(new[] { 5 },"unsigned-descriptor",null,out Byte[]? ct,out _);

        Boolean ok = AccessKeySecret.TryReadSignature(ct!,out AccessKeySignatureDescriptor signature);

        Check.That(ok).IsTrue();
        Check.That(signature.Signed).IsFalse();
        Check.That(signature.Algorithm).IsEqualTo(AccessKeySignatureAlgorithm.None);
        Check.That(signature.SignerKeyIDFormat).IsEqualTo(AccessKeySignerKeyIdFormat.None);
        Check.That(signature.SignerToolID).IsEqualTo(Guid.Empty);
        Check.That(signature.SignerAccessManagerID).IsEqualTo(Guid.Empty);
        Check.That(signature.SignerKeyID.IsDefaultOrEmpty).IsTrue();
    }

    [Test]
    public void TryReadSignature_Fails_When_Signed_Flag_Is_Set_On_Unsigned_Envelope()
    {
        CreateToken(new[] { 7 },"bad-flags",null,out Byte[]? ct,out _);
        Byte[] tampered = ct!.ToArray();
        tampered[1] = 0x01;

        Boolean ok = AccessKeySecret.TryReadSignature(tampered,out _);

        Check.That(ok).IsFalse();
    }

    [Test]
    public void TryReadSignature_Fails_When_Unsigned_Envelope_Declares_Algorithm()
    {
        CreateToken(new[] { 9 },"bad-algorithm",null,out Byte[]? ct,out _);
        Byte[] tampered = ct!.ToArray();
        tampered[2] = (Byte)AccessKeySignatureAlgorithm.RsaPssSha512;

        Boolean ok = AccessKeySecret.TryReadSignature(tampered,out _);

        Check.That(ok).IsFalse();
    }

    [Test]
    public void TryReadSignature_Fails_When_Ciphertext_Length_Is_Invalid()
    {
        CreateToken(new[] { 11 },"bad-ciphertext",null,out Byte[]? ct,out _);
        Byte[] tampered = ct!.ToArray();
        tampered[42] = 0x00;
        tampered[43] = 0x00;
        tampered[44] = 0x00;
        tampered[45] = 0x01;

        Boolean ok = AccessKeySecret.TryReadSignature(tampered,out _);

        Check.That(ok).IsFalse();
    }

    [Test]
    public void IsSigned_Returns_True_For_Signed_Envelope()
    {
        CreateToken(new[] { 13 },"signed",null,out Byte[]? ct,out _);
        Byte[] signed = BuildSignedEnvelope(ct!,IssuerId,AccessManagerId);

        Boolean result = AccessKeySecret.IsSigned(signed);

        Check.That(result).IsTrue();
    }

    [Test]
    public void TryReadSignature_Returns_Signed_Descriptor_For_Signed_Envelope()
    {
        CreateToken(new[] { 15 },"signed-descriptor",null,out Byte[]? ct,out _);
        Byte[] signerkeyid = SHA256.HashData(Encoding.UTF8.GetBytes("thumbprint-signed"));
        Byte[] signaturebytes = Encoding.UTF8.GetBytes("signed-envelope-signature");
        Byte[] signed = BuildSignedEnvelope(ct!,IssuerId,AccessManagerId,signerkeyid,signaturebytes);

        Boolean ok = AccessKeySecret.TryReadSignature(signed,out AccessKeySignatureDescriptor signature);

        Check.That(ok).IsTrue();
        Check.That(signature.Signed).IsTrue();
        Check.That(signature.Algorithm).IsEqualTo(AccessKeySignatureAlgorithm.RsaPssSha512);
        Check.That(signature.SignerKeyIDFormat).IsEqualTo(AccessKeySignerKeyIdFormat.Thumbprint);
        Check.That(signature.SignerToolID).IsEqualTo(IssuerId);
        Check.That(signature.SignerAccessManagerID).IsEqualTo(AccessManagerId);
        Check.That(signature.SignerKeyID.SequenceEqual(signerkeyid)).IsTrue();
    }

    [Test]
    public void SignedRegion_Excludes_Signature_Bytes_For_Signed_Envelope()
    {
        CreateToken(new[] { 17 },"signed-range",null,out Byte[]? ct,out _);
        Byte[] signaturebytes = Encoding.UTF8.GetBytes("range-signature");
        Byte[] signed = BuildSignedEnvelope(ct!,IssuerId,AccessManagerId,signature: signaturebytes);

        ReadOnlyMemory<Byte> signedregion = ReadSignedRegion(signed);

        Check.That(signedregion.Length).IsEqualTo(signed.Length - signaturebytes.Length);
        Check.That(signedregion.Span.SequenceEqual(signed.AsSpan(0,signed.Length - signaturebytes.Length))).IsTrue();
    }

    [Test]
    public void SignatureBytes_Return_Suffix_For_Signed_Envelope()
    {
        CreateToken(new[] { 19 },"signature-suffix",null,out Byte[]? ct,out _);
        Byte[] signaturebytes = Encoding.UTF8.GetBytes("suffix-signature");
        Byte[] signed = BuildSignedEnvelope(ct!,IssuerId,AccessManagerId,signature: signaturebytes);

        ReadOnlyMemory<Byte> resolved = ReadSignatureBytes(signed);

        Check.That(resolved.Span.SequenceEqual(signaturebytes)).IsTrue();
    }

    [Test]
    public void TryCreateSigned_Creates_Signed_Envelope_Using_Signer()
    {
        Byte[] signerkeyid = SHA256.HashData(Encoding.UTF8.GetBytes("signer-thumbprint"));
        AccessKeySignatureInfo signinginfo = new()
        {
            Algorithm = AccessKeySignatureAlgorithm.RsaPssSha512,
            SignerKeyID = signerkeyid.ToImmutableArray(),
            SignerKeyIDFormat = AccessKeySignerKeyIdFormat.Thumbprint,
            SignatureLength = 64
        };
        FixedSignatureSigner signer = new(signinginfo);

        Boolean ok = AccessKeySecret.TryCreate(CreateRequest(new[] { 3,5 },"signed-create",TimeSpan.FromMinutes(1),signer),Key!,out Byte[]? secret,out AccessKeyToken token);

        Check.That(ok).IsTrue();
        Check.That(secret).IsNotNull();
        Check.That(signer.SignCalls).IsEqualTo(1);
        Check.That(AccessKeySecret.IsSigned(secret!)).IsTrue();
        Check.That(AccessKeySecret.TryReadSignature(secret!,out AccessKeySignatureDescriptor signature)).IsTrue();
        Check.That(signature.Signed).IsTrue();
        Check.That(signature.Algorithm).IsEqualTo(AccessKeySignatureAlgorithm.RsaPssSha512);
        Check.That(signature.SignerKeyIDFormat).IsEqualTo(AccessKeySignerKeyIdFormat.Thumbprint);
        Check.That(signature.SignerToolID).IsEqualTo(IssuerId);
        Check.That(signature.SignerAccessManagerID).IsEqualTo(AccessManagerId);
        Check.That(signature.SignerKeyID.SequenceEqual(signerkeyid)).IsTrue();
        Check.That(ReadClaims(secret!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.Token == token).IsTrue();
        Check.That(claims.Subject).IsEqualTo("signed-create");
    }

    [Test]
    public void TryCreate_RequestOverload_WithSigner_Creates_Signed_Envelope()
    {
        Byte[] signerkeyid = SHA256.HashData(Encoding.UTF8.GetBytes("request-signer-thumbprint"));
        AccessKeySignatureInfo signinginfo = new()
        {
            Algorithm = AccessKeySignatureAlgorithm.RsaPssSha512,
            SignerKeyID = signerkeyid.ToImmutableArray(),
            SignerKeyIDFormat = AccessKeySignerKeyIdFormat.Thumbprint,
            SignatureLength = 64
        };
        FixedSignatureSigner signer = new(signinginfo);
        AccessKeySecretCreateRequest request = CreateRequest(new[] { 8 },"request-signed",TimeSpan.FromMinutes(1),signer);

        Boolean ok = AccessKeySecret.TryCreate(request,Key!,out Byte[]? secret,out AccessKeyToken token);

        Check.That(ok).IsTrue();
        Check.That(secret).IsNotNull();
        Check.That(signer.SignCalls).IsEqualTo(1);
        Check.That(AccessKeySecret.IsSigned(secret!)).IsTrue();
        Check.That(AccessKeySecret.TryReadSignature(secret!,out AccessKeySignatureDescriptor signature)).IsTrue();
        Check.That(signature.Signed).IsTrue();
        Check.That(signature.SignerKeyID.SequenceEqual(signerkeyid)).IsTrue();
        Check.That(ReadClaims(secret!,String.Empty,String.Empty,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.Token == token).IsTrue();
        Check.That(claims.Subject).IsEqualTo("request-signed");
    }

    [Test]
    public void TryCreateSigned_Fails_When_SigningInfo_Is_Invalid()
    {
        AccessKeySignatureInfo signinginfo = new()
        {
            Algorithm = AccessKeySignatureAlgorithm.None,
            SignerKeyID = [],
            SignerKeyIDFormat = AccessKeySignerKeyIdFormat.None,
            SignatureLength = 0
        };
        FixedSignatureSigner signer = new(signinginfo);

        Boolean ok = AccessKeySecret.TryCreate(CreateRequest(new[] { 1 },"signed-invalid",null,signer),Key!,out Byte[]? secret,out _);

        Check.That(ok).IsFalse();
        Check.That(secret).IsNull();
        Check.That(signer.SignCalls).IsEqualTo(0);
    }

    [Test]
    public void TryReadClaims_Fails_When_Signed_Envelope_SignerToolId_Differs_From_Inner_IssuerToolId()
    {
        CreateToken(new[] { 21 },"mismatch-tool",null,out Byte[]? ct,out _);
        Byte[] signed = BuildSignedEnvelope(ct!,Guid.NewGuid(),AccessManagerId);

        Boolean ok = AccessKeySecret.TryReadClaims(signed,Key!,String.Empty,String.Empty,out _);

        Check.That(ok).IsFalse();
    }

    [Test]
    public void TryReadAssertions_Fails_When_Signed_Envelope_SignerAccessManagerId_Differs_From_Inner_IssuerAccessManagerId()
    {
        CreateToken(new[] { 23 },"mismatch-manager",null,out Byte[]? ct,out _);
        Byte[] signed = BuildSignedEnvelope(ct!,IssuerId,Guid.NewGuid());

        Boolean ok = AccessKeySecret.TryReadAssertions(signed,Key!,String.Empty,String.Empty,out _);

        Check.That(ok).IsFalse();
    }
}
