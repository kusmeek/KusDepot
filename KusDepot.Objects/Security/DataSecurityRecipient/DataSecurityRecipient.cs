namespace KusDepot.Security;

/**<include file='DataSecurityRecipient.xml' path='DataSecurityRecipient/class[@name="DataSecurityRecipient"]/main/*'/>*/
public sealed class DataSecurityRecipient
{
    /**<include file='DataSecurityRecipient.xml' path='DataSecurityRecipient/class[@name="DataSecurityRecipient"]/constructor[@name="ConstructorObject"]/*'/>*/
    public DataSecurityRecipient(Guid objectid)
    {
        if(objectid == Guid.Empty) { throw new ArgumentException(EmptyObjectIdMessage,nameof(objectid)); }

        this.ObjectId = objectid;
    }

    /**<include file='DataSecurityRecipient.xml' path='DataSecurityRecipient/class[@name="DataSecurityRecipient"]/constructor[@name="ConstructorCertificate"]/*'/>*/
    public DataSecurityRecipient(X509Certificate2 certificate , Guid? objectid = null)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        if(objectid == Guid.Empty) { throw new ArgumentException(EmptyObjectIdMessage,nameof(objectid)); }

        if(String.IsNullOrWhiteSpace(certificate.Thumbprint)) { throw new ArgumentException(MissingThumbprintMessage,nameof(certificate)); }

        this.ObjectId = objectid;
        this.Certificate = certificate;
        this.Thumbprint = certificate.Thumbprint!;
        this.PublicKeyHash = SHA256.HashData(certificate.GetPublicKey());
    }

    /**<include file='DataSecurityRecipient.xml' path='DataSecurityRecipient/class[@name="DataSecurityRecipient"]/property[@name="ObjectId"]/*'/>*/
    public Guid? ObjectId { get; init; }

    /**<include file='DataSecurityRecipient.xml' path='DataSecurityRecipient/class[@name="DataSecurityRecipient"]/property[@name="Thumbprint"]/*'/>*/
    public String? Thumbprint { get; init; }

    /**<include file='DataSecurityRecipient.xml' path='DataSecurityRecipient/class[@name="DataSecurityRecipient"]/property[@name="PublicKeyHash"]/*'/>*/
    public Byte[]? PublicKeyHash { get; init; }

    /**<include file='DataSecurityRecipient.xml' path='DataSecurityRecipient/class[@name="DataSecurityRecipient"]/property[@name="Certificate"]/*'/>*/
    public X509Certificate2? Certificate { get; init; }

    private const String EmptyObjectIdMessage = "Recipient object identifier cannot be empty.";

    private const String MissingThumbprintMessage = "Recipient certificate thumbprint is required.";
}