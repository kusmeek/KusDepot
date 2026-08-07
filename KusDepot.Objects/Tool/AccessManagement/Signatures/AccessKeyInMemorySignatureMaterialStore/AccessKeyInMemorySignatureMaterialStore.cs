namespace KusDepot.Security.Signatures;

/**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/main/*'/>*/
public class AccessKeyInMemorySignatureMaterialStore : IAccessKeySignatureMaterialStore
{
    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/field[@name="sync"]/*'/>*/
    private readonly Lock sync = new();

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/field[@name="currentsigningmaterial"]/*'/>*/
    private AccessKeySignatureSigningMaterial? currentsigningmaterial;

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/field[@name="verificationmaterials"]/*'/>*/
    private readonly Dictionary<String,AccessKeySignatureVerificationMaterial> verificationmaterials = new(StringComparer.OrdinalIgnoreCase);

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/method[@name="TryAddVerificationMaterial"]/*'/>*/
    public virtual Boolean TryAddVerificationMaterial(AccessKeySignatureVerificationMaterial material)
    {
        if(!TryNormalizeVerificationMaterial(in material,out AccessKeySignatureVerificationMaterial normalized,out String? key)) { return false; }

        lock(sync)
        {
            verificationmaterials[key!] = normalized;

            return true;
        }
    }

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/method[@name="TryClearCurrentSigningMaterial"]/*'/>*/
    public virtual Boolean TryClearCurrentSigningMaterial()
    {
        lock(sync)
        {
            Boolean hadmaterial = currentsigningmaterial is not null;

            currentsigningmaterial = null;

            return hadmaterial;
        }
    }

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/method[@name="TryGetCurrentSigningMaterial"]/*'/>*/
    public virtual Boolean TryGetCurrentSigningMaterial(out AccessKeySignatureSigningMaterial material)
    {
        lock(sync)
        {
            if(currentsigningmaterial is AccessKeySignatureSigningMaterial current)
            {
                material = current;

                return true;
            }
        }

        material = default;

        return false;
    }

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/method[@name="TryRemoveVerificationMaterial"]/*'/>*/
    public virtual Boolean TryRemoveVerificationMaterial(AccessKeySignerKeyIdFormat signerkeyidformat , ReadOnlyMemory<Byte> signerkeyid)
    {
        String? key = CreateVerificationKey(signerkeyidformat,signerkeyid.Span);

        if(String.IsNullOrEmpty(key)) { return false; }

        lock(sync)
        {
            return verificationmaterials.Remove(key);
        }
    }

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/method[@name="TryResolveSigningMaterial"]/*'/>*/
    public virtual Boolean TryResolveSigningMaterial(AccessKeySignatureSigningContext context , out AccessKeySignatureSigningMaterial material)
    {
        ArgumentNullException.ThrowIfNull(context.Logger);

        return TryGetCurrentSigningMaterial(out material);
    }

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/method[@name="TryResolveVerificationMaterial"]/*'/>*/
    public virtual Boolean TryResolveVerificationMaterial(AccessKeySignatureValidationContext context , out AccessKeySignatureVerificationMaterial material)
    {
        String? key = CreateVerificationKey(context.Signature.SignerKeyIDFormat,context.Signature.SignerKeyID.AsSpan());

        if(String.IsNullOrEmpty(key))
        {
            material = default;

            return false;
        }

        lock(sync)
        {
            return verificationmaterials.TryGetValue(key,out material);
        }
    }

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/method[@name="TrySetCurrentSigningMaterial"]/*'/>*/
    public virtual Boolean TrySetCurrentSigningMaterial(AccessKeySignatureSigningMaterial material)
    {
        if(!TryNormalizeSigningMaterial(in material,out AccessKeySignatureSigningMaterial normalized)) { return false; }

        lock(sync)
        {
            currentsigningmaterial = normalized;

            return true;
        }
    }

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/method[@name="CreateVerificationKey"]/*'/>*/
    private static String? CreateVerificationKey(AccessKeySignerKeyIdFormat signerkeyidformat , ReadOnlySpan<Byte> signerkeyid)
    {
        if(signerkeyid.IsEmpty) { return null; }

        return $"{(Byte)signerkeyidformat}:{Convert.ToHexString(signerkeyid)}";
    }

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/method[@name="NormalizeThumbprint"]/*'/>*/
    private static String? NormalizeThumbprint(X509Certificate2? certificate , String? thumbprint)
    {
        String value = String.IsNullOrWhiteSpace(thumbprint) ? certificate?.Thumbprint ?? String.Empty : thumbprint.Trim();

        if(String.IsNullOrWhiteSpace(value)) { return null; }

        return value.Replace(" ",String.Empty,Ordinal).ToUpperInvariant();
    }

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/method[@name="TryNormalizeSigningMaterial"]/*'/>*/
    private static Boolean TryNormalizeSigningMaterial(in AccessKeySignatureSigningMaterial material , out AccessKeySignatureSigningMaterial normalized)
    {
        normalized = default;

        if(material.Certificate is null) { return false; }

        String? thumbprint = NormalizeThumbprint(material.Certificate,material.Thumbprint);

        if(String.IsNullOrWhiteSpace(thumbprint)) { return false; }

        ImmutableArray<Byte> signerkeyid;

        try { signerkeyid = Convert.FromHexString(thumbprint).ToImmutableArray(); } catch ( FormatException ) { return false; }

        AccessKeySignatureInfo signatureinfo = material.SignatureInfo;

        Int32 signaturelength = signatureinfo.SignatureLength > 0 ? signatureinfo.SignatureLength : material.Certificate.GetRSAPublicKey()?.KeySize / 8 ?? 0;

        if(signaturelength <= 0) { return false; }

        if(signatureinfo.Algorithm is AccessKeySignatureAlgorithm.None) { signatureinfo = signatureinfo with { Algorithm = AccessKeySignatureAlgorithm.RsaPssSha512 }; }
        if(signatureinfo.SignerKeyIDFormat is not AccessKeySignerKeyIdFormat.Thumbprint) { signatureinfo = signatureinfo with { SignerKeyIDFormat = AccessKeySignerKeyIdFormat.Thumbprint }; }
        if(signatureinfo.SignerKeyID.IsDefaultOrEmpty || !signatureinfo.SignerKeyID.AsSpan().SequenceEqual(signerkeyid.AsSpan())) { signatureinfo = signatureinfo with { SignerKeyID = signerkeyid }; }
        if(signatureinfo.SignatureLength != signaturelength) { signatureinfo = signatureinfo with { SignatureLength = signaturelength }; }

        normalized = new()
        {
            Certificate = material.Certificate,
            SignatureInfo = signatureinfo,
            Thumbprint = thumbprint
        };

        return true;
    }

    /**<include file='AccessKeyInMemorySignatureMaterialStore.xml' path='AccessKeyInMemorySignatureMaterialStore/class[@name="AccessKeyInMemorySignatureMaterialStore"]/method[@name="TryNormalizeVerificationMaterial"]/*'/>*/
    private static Boolean TryNormalizeVerificationMaterial(in AccessKeySignatureVerificationMaterial material , out AccessKeySignatureVerificationMaterial normalized , out String? key)
    {
        normalized = default; key = null; if(material.Certificate is null) { return false; }

        String? thumbprint = NormalizeThumbprint(material.Certificate,material.Thumbprint); if(String.IsNullOrWhiteSpace(thumbprint)) { return false; }

        ImmutableArray<Byte> signerkeyid = material.SignerKeyID;

        if(material.SignerKeyIDFormat is AccessKeySignerKeyIdFormat.Thumbprint && signerkeyid.IsDefaultOrEmpty)
        {
            try { signerkeyid = Convert.FromHexString(thumbprint).ToImmutableArray(); } catch ( FormatException ) { return false; }
        }

        if(signerkeyid.IsDefaultOrEmpty) { return false; }

        key = CreateVerificationKey(material.SignerKeyIDFormat,signerkeyid.AsSpan()); if(String.IsNullOrEmpty(key)) { return false; }

        normalized = new()
        {
            Certificate = material.Certificate,
            SignerKeyID = signerkeyid,
            SignerKeyIDFormat = material.SignerKeyIDFormat,
            Thumbprint = thumbprint
        };

        return true;
    }
}
