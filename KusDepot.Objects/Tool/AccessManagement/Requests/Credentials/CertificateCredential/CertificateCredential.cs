namespace KusDepot.Security.Requests;

/**<include file='CertificateCredential.xml' path='CertificateCredential/class[@name="CertificateCredential"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.Requests.CertificateCredential")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public record class CertificateCredential : AccessCredential
{
    /**<include file='CertificateCredential.xml' path='CertificateCredential/class[@name="CertificateCredential"]/property[@name="Certificate"]/*'/>*/
    [JsonPropertyName("Certificate")] [JsonRequired] [Id(0)]
    public Byte[] Certificate
    {
        get => certificate.CloneByteArray();

        set => certificate ??= value?.CloneByteArray() ?? Array.Empty<Byte>();
    }

    [NonSerialized] [IgnoreDataMember]
    private Byte[] certificate = Array.Empty<Byte>();

    /**<include file='CertificateCredential.xml' path='CertificateCredential/class[@name="CertificateCredential"]/method[@name="Create"]/*'/>*/
    public static CertificateCredential Create(X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        return new() { Certificate = SerializeCertificate(certificate) ?? throw new SecurityException() };
    }

    /**<include file='CertificateCredential.xml' path='CertificateCredential/class[@name="CertificateCredential"]/method[@name="TryGetCertificate"]/*'/>*/
    public Boolean TryGetCertificate([NotNullWhen(true)] out X509Certificate2? certificate)
    {
        try
        {
            if(this.certificate.Length == 0) { certificate = null; return false; }

            certificate = DeserializeCertificate(this.certificate)!;

            return certificate is not null;
        }
        catch
        {
            certificate = null;

            return false;
        }
    }

    /**<include file='CertificateCredential.xml' path='CertificateCredential/class[@name="CertificateCredential"]/method[@name="Equals"]/*'/>*/
    public virtual Boolean Equals(CertificateCredential? other)
    {
        return ReferenceEquals(this,other) || other is not null && this.certificate.AsSpan().SequenceEqual(other.certificate.AsSpan());
    }

    /**<include file='CertificateCredential.xml' path='CertificateCredential/class[@name="CertificateCredential"]/method[@name="GetHashCode"]/*'/>*/
    public override Int32 GetHashCode()
    {
        var _ = new HashCode(); _.AddBytes(this.certificate); return _.ToHashCode();
    }
}