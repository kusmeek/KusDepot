namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AccessKeyCertificateSignatureProcessorExam
{
    private sealed class CaptureAuthorizeSignaturePolicyEngine : IAccessPolicyEngine
    {
        public SignatureValidation AuthorizeSignatureValidation { get; private set; }

        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            if(context.Mode == AccessPolicyMode.Authorize)
            {
                AuthorizeSignatureValidation = context.SignatureValidation;
            }

            return AccessPolicyDecision.Allow();
        }
    }

    private static AccessKeySignatureSigningMaterial CreateSigningMaterial(X509Certificate2 certificate , AccessKeySignatureInfo? signatureinfo = null)
    {
        signatureinfo ??= new AccessKeySignatureInfo
        {
            Algorithm = AccessKeySignatureAlgorithm.RsaPssSha512,
            SignerKeyID = Convert.FromHexString(certificate.Thumbprint ?? String.Empty).ToImmutableArray(),
            SignerKeyIDFormat = AccessKeySignerKeyIdFormat.Thumbprint,
            SignatureLength = certificate.GetRSAPublicKey()?.KeySize / 8 ?? 0
        };

        return new()
        {
            Certificate = certificate,
            SignatureInfo = signatureinfo.Value,
            Thumbprint = certificate.Thumbprint ?? String.Empty
        };
    }

    private static AccessKeySignatureVerificationMaterial CreateVerificationMaterial(X509Certificate2 certificate)
    {
        return new()
        {
            Certificate = certificate,
            SignerKeyID = Convert.FromHexString(certificate.Thumbprint ?? String.Empty).ToImmutableArray(),
            SignerKeyIDFormat = AccessKeySignerKeyIdFormat.Thumbprint,
            Thumbprint = certificate.Thumbprint ?? String.Empty
        };
    }

    private static AccessKeySignatureValidationContext CreateValidationContext(X509Certificate2 certificate , Byte[] signedregion , Byte[] signaturebytes , AccessKeySignerKeyIdFormat signerkeyidformat = AccessKeySignerKeyIdFormat.Thumbprint)
    {
        return new()
        {
            AccessKeyRealmID = "realm-a",
            Logger = NullLogger.Instance,
            ManifestHash = Convert.ToHexString(SHA512.HashData(Encoding.UTF8.GetBytes("manifest-a"))),
            Signature = new()
            {
                Algorithm = AccessKeySignatureAlgorithm.RsaPssSha512,
                Signed = true,
                SignerAccessManagerID = Guid.NewGuid(),
                SignerKeyID = Convert.FromHexString(certificate.Thumbprint ?? String.Empty).ToImmutableArray(),
                SignerKeyIDFormat = signerkeyidformat,
                SignerToolID = Guid.NewGuid()
            },
            SignatureBytes = signaturebytes,
            SignedRegion = signedregion,
            ToolSchemaID = "KusDepot.Tools/Schema"
        };
    }

    [Test]
    public void Verify_WithMatchingThumbprintAndSignature_ReturnsValidFacts()
    {
        using X509Certificate2 certificate = CreateCertificate(Guid.NewGuid(),"ProcessorVerify",2048,2,null)!;
        AccessKeyCertificateSignatureProcessor processor = new();
        Byte[] signedregion = Encoding.UTF8.GetBytes("signed-region");
        Byte[] signaturebytes;

        using(RSA rsa = certificate.GetRSAPrivateKey()!)
        {
            signaturebytes = rsa.SignData(signedregion,HashAlgorithmName.SHA512,RSASignaturePadding.Pss);
        }

        Check.That(processor.TryAddVerificationMaterial(CreateVerificationMaterial(certificate))).IsTrue();

        SignatureValidation validation = processor.Verify(CreateValidationContext(certificate,signedregion,signaturebytes));

        Check.That(validation.VerificationAttempted).IsTrue();
        Check.That(validation.SignatureValid).IsTrue();
        Check.That(validation.CertificateResolved).IsTrue();
        Check.That(validation.CertificateThumbprint).IsEqualTo(certificate.Thumbprint);
        Check.That(validation.SignerTrusted).IsFalse();
        Check.That(validation.ChainTrusted).IsFalse();
        Check.That(validation.FailureReason).IsNull();
    }

    [Test]
    public void Verify_WithoutRegisteredVerificationMaterial_ReturnsUnresolvedFacts()
    {
        using X509Certificate2 certificate = CreateCertificate(Guid.NewGuid(),"ProcessorUnresolved",2048,2,null)!;
        AccessKeyCertificateSignatureProcessor processor = new();
        Byte[] signedregion = Encoding.UTF8.GetBytes("signed-region");
        Byte[] signaturebytes;

        using(RSA rsa = certificate.GetRSAPrivateKey()!)
        {
            signaturebytes = rsa.SignData(signedregion,HashAlgorithmName.SHA512,RSASignaturePadding.Pss);
        }

        SignatureValidation validation = processor.Verify(CreateValidationContext(certificate,signedregion,signaturebytes));

        Check.That(validation.VerificationAttempted).IsTrue();
        Check.That(validation.SignatureValid).IsFalse();
        Check.That(validation.CertificateResolved).IsFalse();
        Check.That(validation.CertificateThumbprint).IsNull();
        Check.That(validation.FailureReason).IsEqualTo("Signer certificate could not be resolved by thumbprint.");
    }

    [Test]
    public void Verify_WithTamperedSignature_ReturnsInvalidFacts()
    {
        using X509Certificate2 certificate = CreateCertificate(Guid.NewGuid(),"ProcessorTampered",2048,2,null)!;
        AccessKeyCertificateSignatureProcessor processor = new();
        Byte[] signedregion = Encoding.UTF8.GetBytes("signed-region");
        Byte[] signaturebytes;

        using(RSA rsa = certificate.GetRSAPrivateKey()!)
        {
            signaturebytes = rsa.SignData(signedregion,HashAlgorithmName.SHA512,RSASignaturePadding.Pss);
        }

        signaturebytes[^1] ^= 0x5A;
        Check.That(processor.TryAddVerificationMaterial(CreateVerificationMaterial(certificate))).IsTrue();

        SignatureValidation validation = processor.Verify(CreateValidationContext(certificate,signedregion,signaturebytes));

        Check.That(validation.VerificationAttempted).IsTrue();
        Check.That(validation.SignatureValid).IsFalse();
        Check.That(validation.CertificateResolved).IsTrue();
        Check.That(validation.CertificateThumbprint).IsEqualTo(certificate.Thumbprint);
        Check.That(validation.FailureReason).IsEqualTo("Signature verification failed.");
    }

    [Test]
    public void Verify_WithUnsupportedKeyIdFormat_ReturnsUnsupportedFacts()
    {
        using X509Certificate2 certificate = CreateCertificate(Guid.NewGuid(),"ProcessorUnsupported",2048,2,null)!;
        AccessKeyCertificateSignatureProcessor processor = new();
        Byte[] signedregion = Encoding.UTF8.GetBytes("signed-region");
        Byte[] signaturebytes;

        using(RSA rsa = certificate.GetRSAPrivateKey()!)
        {
            signaturebytes = rsa.SignData(signedregion,HashAlgorithmName.SHA512,RSASignaturePadding.Pss);
        }

        SignatureValidation validation = processor.Verify(CreateValidationContext(certificate,signedregion,signaturebytes,AccessKeySignerKeyIdFormat.None));

        Check.That(validation.VerificationAttempted).IsTrue();
        Check.That(validation.SignatureValid).IsFalse();
        Check.That(validation.CertificateResolved).IsFalse();
        Check.That(validation.FailureReason).IsEqualTo("Unsupported signer key identifier format.");
    }

    [Test]
    public void Authorize_Populates_SignatureValidation_When_CertificateProcessor_Is_Configured()
    {
        using X509Certificate2 certificate = CreateCertificate(Guid.NewGuid(),"ProcessorAccessManager",2048,2,null)!;
        CaptureAuthorizeSignaturePolicyEngine policyengine = new();
        AccessKeyCertificateSignatureProcessor processor = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: policyengine,signatureprocessor: processor),new Tool());

        Check.That(processor.TrySetCurrentSigningMaterial(CreateSigningMaterial(certificate))).IsTrue();
        Check.That(processor.TryAddVerificationMaterial(CreateVerificationMaterial(certificate))).IsTrue();

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "processor-authorize",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsTrue();
        Check.That(policyengine.AuthorizeSignatureValidation.Signed).IsTrue();
        Check.That(policyengine.AuthorizeSignatureValidation.VerificationAttempted).IsTrue();
        Check.That(policyengine.AuthorizeSignatureValidation.SignatureValid).IsTrue();
        Check.That(policyengine.AuthorizeSignatureValidation.CertificateResolved).IsTrue();
        Check.That(policyengine.AuthorizeSignatureValidation.CertificateThumbprint).IsEqualTo(certificate.Thumbprint);
        Check.That(policyengine.AuthorizeSignatureValidation.SignerTrusted).IsFalse();
        Check.That(policyengine.AuthorizeSignatureValidation.ChainTrusted).IsFalse();
        Check.That(policyengine.AuthorizeSignatureValidation.FailureReason).IsNull();
    }
}
