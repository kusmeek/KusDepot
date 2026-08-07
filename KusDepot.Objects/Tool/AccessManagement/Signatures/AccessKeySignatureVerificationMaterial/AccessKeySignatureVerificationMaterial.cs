namespace KusDepot.Security.Signatures;

/**<include file='AccessKeySignatureVerificationMaterial.xml' path='AccessKeySignatureVerificationMaterial/struct[@name="AccessKeySignatureVerificationMaterial"]/main/*'/>*/
public readonly record struct AccessKeySignatureVerificationMaterial
{
    /**<include file='AccessKeySignatureVerificationMaterial.xml' path='AccessKeySignatureVerificationMaterial/struct[@name="AccessKeySignatureVerificationMaterial"]/property[@name="Certificate"]/*'/>*/
    public X509Certificate2? Certificate { get; init; }

    /**<include file='AccessKeySignatureVerificationMaterial.xml' path='AccessKeySignatureVerificationMaterial/struct[@name="AccessKeySignatureVerificationMaterial"]/property[@name="SignerKeyID"]/*'/>*/
    public ImmutableArray<Byte> SignerKeyID { get; init; } = [];

    /**<include file='AccessKeySignatureVerificationMaterial.xml' path='AccessKeySignatureVerificationMaterial/struct[@name="AccessKeySignatureVerificationMaterial"]/property[@name="SignerKeyIDFormat"]/*'/>*/
    public AccessKeySignerKeyIdFormat SignerKeyIDFormat { get; init; }

    /**<include file='AccessKeySignatureVerificationMaterial.xml' path='AccessKeySignatureVerificationMaterial/struct[@name="AccessKeySignatureVerificationMaterial"]/property[@name="Thumbprint"]/*'/>*/
    public String Thumbprint { get; init; } = String.Empty;

    /**<include file='AccessKeySignatureVerificationMaterial.xml' path='AccessKeySignatureVerificationMaterial/struct[@name="AccessKeySignatureVerificationMaterial"]/constructor[@name="Constructor"]/*'/>*/
    public AccessKeySignatureVerificationMaterial() {}

    /**<include file='AccessKeySignatureVerificationMaterial.xml' path='AccessKeySignatureVerificationMaterial/struct[@name="AccessKeySignatureVerificationMaterial"]/method[@name="Create"]/*'/>*/
    public static AccessKeySignatureVerificationMaterial Create(X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        String thumbprint = (certificate.Thumbprint ?? String.Empty).Replace(" ",String.Empty,Ordinal).ToUpperInvariant();

        if(String.IsNullOrWhiteSpace(thumbprint)) { throw new ArgumentException(null,nameof(certificate)); }

        Byte[] signerkeyid;

        try { signerkeyid = Convert.FromHexString(thumbprint); }

        catch ( FormatException _ ) { throw new ArgumentException(null,nameof(certificate),_); }

        using RSA? rsa = certificate.GetRSAPublicKey(); if(rsa is null || rsa.KeySize <= 0) { throw new ArgumentException(null,nameof(certificate)); }

        return new()
        {
            Certificate = certificate,
            SignerKeyID = signerkeyid.ToImmutableArray(),
            SignerKeyIDFormat = AccessKeySignerKeyIdFormat.Thumbprint,
            Thumbprint = thumbprint
        };
    }
}