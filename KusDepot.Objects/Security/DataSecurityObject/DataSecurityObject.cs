namespace KusDepot.Security;

/**<include file='DataSecurityObject.xml' path='DataSecurityObject/class[@name="DataSecurityObject"]/main/*'/>*/
public sealed class DataSecurityObject
{
    /**<include file='DataSecurityObject.xml' path='DataSecurityObject/class[@name="DataSecurityObject"]/constructor[@name="Constructor"]/*'/>*/
    public DataSecurityObject(Guid objectid , X509Certificate2 certificate , String? displayname = null)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        if(objectid == Guid.Empty) { throw new ArgumentException(EmptyObjectIdMessage,nameof(objectid)); }

        if(String.IsNullOrWhiteSpace(certificate.Thumbprint)) { throw new ArgumentException(MissingThumbprintMessage,nameof(certificate)); }

        this.ObjectId = objectid;
        this.Certificate = certificate;
        this.Thumbprint = certificate.Thumbprint!;
        this.DisplayName = String.IsNullOrWhiteSpace(displayname) ? null : displayname;
    }

    /**<include file='DataSecurityObject.xml' path='DataSecurityObject/class[@name="DataSecurityObject"]/property[@name="ObjectId"]/*'/>*/
    public Guid ObjectId { get; init; }

    /**<include file='DataSecurityObject.xml' path='DataSecurityObject/class[@name="DataSecurityObject"]/property[@name="Certificate"]/*'/>*/
    public X509Certificate2 Certificate { get; init; }

    /**<include file='DataSecurityObject.xml' path='DataSecurityObject/class[@name="DataSecurityObject"]/property[@name="Thumbprint"]/*'/>*/
    public String Thumbprint { get; init; }

    /**<include file='DataSecurityObject.xml' path='DataSecurityObject/class[@name="DataSecurityObject"]/property[@name="DisplayName"]/*'/>*/
    public String? DisplayName { get; init; }

    private const String EmptyObjectIdMessage = "Object identifier cannot be empty.";

    private const String MissingThumbprintMessage = "Certificate thumbprint is required.";
}
