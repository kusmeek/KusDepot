namespace KusDepot.Security.Signatures;

/**<include file='AccessKeySignatureSigningMaterial.xml' path='AccessKeySignatureSigningMaterial/struct[@name="AccessKeySignatureSigningMaterial"]/main/*'/>*/
public readonly record struct AccessKeySignatureSigningMaterial
{
    /**<include file='AccessKeySignatureSigningMaterial.xml' path='AccessKeySignatureSigningMaterial/struct[@name="AccessKeySignatureSigningMaterial"]/property[@name="Certificate"]/*'/>*/
    public X509Certificate2? Certificate { get; init; }

    /**<include file='AccessKeySignatureSigningMaterial.xml' path='AccessKeySignatureSigningMaterial/struct[@name="AccessKeySignatureSigningMaterial"]/property[@name="SignatureInfo"]/*'/>*/
    public AccessKeySignatureInfo SignatureInfo { get; init; }

    /**<include file='AccessKeySignatureSigningMaterial.xml' path='AccessKeySignatureSigningMaterial/struct[@name="AccessKeySignatureSigningMaterial"]/property[@name="Thumbprint"]/*'/>*/
    public String Thumbprint { get; init; } = String.Empty;

    /**<include file='AccessKeySignatureSigningMaterial.xml' path='AccessKeySignatureSigningMaterial/struct[@name="AccessKeySignatureSigningMaterial"]/constructor[@name="Constructor"]/*'/>*/
    public AccessKeySignatureSigningMaterial() {}

    /**<include file='AccessKeySignatureSigningMaterial.xml' path='AccessKeySignatureSigningMaterial/struct[@name="AccessKeySignatureSigningMaterial"]/method[@name="Create"]/*'/>*/
    public static AccessKeySignatureSigningMaterial Create(X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        String thumbprint = (certificate.Thumbprint ?? String.Empty).Replace(" ",String.Empty,Ordinal).ToUpperInvariant();

        if(String.IsNullOrWhiteSpace(thumbprint)) { throw new ArgumentException(null,nameof(certificate)); }

        Byte[] signerkeyid;

        try { signerkeyid = Convert.FromHexString(thumbprint); }

        catch ( FormatException _ ) { throw new ArgumentException(null,nameof(certificate),_); }

        using RSA? rsa = certificate.GetRSAPrivateKey(); if(rsa is null || rsa.KeySize <= 0) { throw new ArgumentException(null,nameof(certificate)); }

        return new()
        {
            Certificate = certificate,
            SignatureInfo = new()
            {
                Algorithm = AccessKeySignatureAlgorithm.RsaPssSha512,
                SignerKeyID = signerkeyid.ToImmutableArray(),
                SignerKeyIDFormat = AccessKeySignerKeyIdFormat.Thumbprint,
                SignatureLength = rsa.KeySize / 8
            },
            Thumbprint = thumbprint
        };
    }
}