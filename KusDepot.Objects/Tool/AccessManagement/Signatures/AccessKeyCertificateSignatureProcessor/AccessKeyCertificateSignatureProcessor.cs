namespace KusDepot.Security.Signatures;

/**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/main/*'/>*/
public class AccessKeyCertificateSignatureProcessor : IAccessKeySignatureProcessor , IAccessKeySignatureMaterialStore
{
    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/field[@name="materialresolver"]/*'/>*/
    private readonly IAccessKeySignatureMaterialResolver materialresolver;

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/field[@name="materialstore"]/*'/>*/
    private readonly IAccessKeySignatureMaterialStore? materialstore;

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/constructor[@name="ConstructorStore"]/*'/>*/
    public AccessKeyCertificateSignatureProcessor(AccessKeyInMemorySignatureMaterialStore? materialstore = null)
    {
        materialstore ??= new(); this.materialresolver = materialstore; this.materialstore = materialstore;
    }

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/constructor[@name="Constructor"]/*'/>*/
    public AccessKeyCertificateSignatureProcessor(IAccessKeySignatureMaterialResolver materialresolver , IAccessKeySignatureMaterialStore materialstore)
    {
        ArgumentNullException.ThrowIfNull(materialresolver); ArgumentNullException.ThrowIfNull(materialstore);

        this.materialresolver = materialresolver; this.materialstore = materialstore;
    }

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/method[@name="GetSigningInfo"]/*'/>*/
    public virtual AccessKeySignatureInfo GetSigningInfo(AccessKeySignatureSigningContext context)
    {
        return materialresolver.TryResolveSigningMaterial(context,out AccessKeySignatureSigningMaterial material)
            ? material.SignatureInfo
            : default;
    }

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/method[@name="TryAddVerificationMaterial"]/*'/>*/
    public virtual Boolean TryAddVerificationMaterial(AccessKeySignatureVerificationMaterial material)
    {
        return materialstore?.TryAddVerificationMaterial(material) is true;
    }

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/method[@name="TryClearCurrentSigningMaterial"]/*'/>*/
    public virtual Boolean TryClearCurrentSigningMaterial()
    {
        return materialstore?.TryClearCurrentSigningMaterial() is true;
    }

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/method[@name="TryGetCurrentSigningMaterial"]/*'/>*/
    public virtual Boolean TryGetCurrentSigningMaterial(out AccessKeySignatureSigningMaterial material)
    {
        if(materialstore?.TryGetCurrentSigningMaterial(out material) is true) { return true; }

        material = default;

        return false;
    }

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/method[@name="TryRemoveVerificationMaterial"]/*'/>*/
    public virtual Boolean TryRemoveVerificationMaterial(AccessKeySignerKeyIdFormat signerkeyidformat , ReadOnlyMemory<Byte> signerkeyid)
    {
        return materialstore?.TryRemoveVerificationMaterial(signerkeyidformat,signerkeyid) is true;
    }

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/method[@name="TryResolveSigningMaterial"]/*'/>*/
    public virtual Boolean TryResolveSigningMaterial(AccessKeySignatureSigningContext context , out AccessKeySignatureSigningMaterial material)
    {
        return materialresolver.TryResolveSigningMaterial(context,out material);
    }

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/method[@name="TryResolveVerificationMaterial"]/*'/>*/
    public virtual Boolean TryResolveVerificationMaterial(AccessKeySignatureValidationContext context , out AccessKeySignatureVerificationMaterial material)
    {
        return materialresolver.TryResolveVerificationMaterial(context,out material);
    }

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/method[@name="TrySetCurrentSigningMaterial"]/*'/>*/
    public virtual Boolean TrySetCurrentSigningMaterial(AccessKeySignatureSigningMaterial material)
    {
        return materialstore?.TrySetCurrentSigningMaterial(material) is true;
    }

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/method[@name="TrySign"]/*'/>*/
    public virtual Boolean TrySign(AccessKeySignatureSigningContext context , Span<Byte> signature , out Int32 written)
    {
        written = 0;

        if(!materialresolver.TryResolveSigningMaterial(context,out AccessKeySignatureSigningMaterial material) || material.Certificate is null)
        {
            return false;
        }

        if(material.SignatureInfo.Algorithm is not AccessKeySignatureAlgorithm.RsaPssSha512) { return false; }

        using RSA? rsa = material.Certificate.GetRSAPrivateKey(); if(rsa is null) { return false; }

        Byte[] signed = rsa.SignData(context.SignedRegion.Span,HashAlgorithmName.SHA512,RSASignaturePadding.Pss);

        if(material.SignatureInfo.SignatureLength != signed.Length || signature.Length < signed.Length) { return false; }

        signed.CopyTo(signature); written = signed.Length;

        return true;
    }

    /**<include file='AccessKeyCertificateSignatureProcessor.xml' path='AccessKeyCertificateSignatureProcessor/class[@name="AccessKeyCertificateSignatureProcessor"]/method[@name="Verify"]/*'/>*/
    public virtual SignatureValidation Verify(AccessKeySignatureValidationContext context)
    {
        SignatureValidation validation = new()
        {
            Algorithm = context.Signature.Algorithm,
            CertificateResolved = false,
            ChainTrusted = false,
            Signed = context.Signature.Signed,
            SignerAccessManagerID = context.Signature.SignerAccessManagerID,
            SignerKeyID = context.Signature.SignerKeyID,
            SignerKeyIDFormat = context.Signature.SignerKeyIDFormat,
            SignerToolID = context.Signature.SignerToolID,
            SignerTrusted = false,
            VerificationAttempted = true
        };

        if(context.Signature.SignerKeyIDFormat is not AccessKeySignerKeyIdFormat.Thumbprint)
        {
            return validation with { FailureReason = "Unsupported signer key identifier format.", SignatureValid = false };
        }

        if(context.Signature.Algorithm is not AccessKeySignatureAlgorithm.RsaPssSha512)
        {
            return validation with { FailureReason = "Unsupported signature algorithm.", SignatureValid = false };
        }

        if(!materialresolver.TryResolveVerificationMaterial(context,out AccessKeySignatureVerificationMaterial material) || material.Certificate is null)
        {
            return validation with { FailureReason = "Signer certificate could not be resolved by thumbprint.", SignatureValid = false };
        }

        String thumbprint = String.IsNullOrWhiteSpace(material.Thumbprint) ? material.Certificate.Thumbprint ?? String.Empty : material.Thumbprint;

        validation = validation with { CertificateResolved = true, CertificateThumbprint = thumbprint };

        using RSA? rsa = material.Certificate.GetRSAPublicKey();

        if(rsa is null)
        {
            return validation with { FailureReason = "Resolved certificate does not expose an RSA public key.", SignatureValid = false };
        }

        Boolean valid = rsa.VerifyData(context.SignedRegion.Span,context.SignatureBytes.Span,HashAlgorithmName.SHA512,RSASignaturePadding.Pss);

        return valid
            ? validation with { FailureReason = null, SignatureValid = true }
            : validation with { FailureReason = "Signature verification failed.", SignatureValid = false };
    }
}
