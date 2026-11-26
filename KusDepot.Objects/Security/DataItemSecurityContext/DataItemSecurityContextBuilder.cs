namespace KusDepot.Security.Data;

/**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/main/*'/>*/
public sealed class DataItemSecurityContextBuilder
{
    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/field[@name="directory"]/*'/>*/
    private ISecurityIdentityDirectory? directory;

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/field[@name="knownobjects"]/*'/>*/
    private readonly Dictionary<Guid,DataSecurityObject> knownobjects = new();

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/field[@name="recipients"]/*'/>*/
    private readonly List<DataSecurityRecipient> recipients = [];

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/field[@name="localobject"]/*'/>*/
    private readonly DataSecurityObject localobject;

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/field[@name="signingobject"]/*'/>*/
    private DataSecurityObject? signingobject;

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/field[@name="purpose"]/*'/>*/
    private String? purpose;

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/field[@name="notbefore"]/*'/>*/
    private DateTimeOffset? notbefore;

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/field[@name="expiresat"]/*'/>*/
    private DateTimeOffset? expiresat;

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/field[@name="includeselfrecipient"]/*'/>*/
    private Boolean includeselfrecipient = true;

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/constructor[@name="Constructor"]/*'/>*/
    public DataItemSecurityContextBuilder(DataSecurityObject localobject)
    {
        ArgumentNullException.ThrowIfNull(localobject);

        this.localobject = localobject;

        this.knownobjects[localobject.ObjectId] = localobject;
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="Build"]/*'/>*/
    public DataItemSecurityContext Build()
    {
        List<DataSecurityRecipient> effectiverecipients = [..this.recipients];

        DataSecurityObject effectivesigningobject = this.signingobject ?? this.localobject;

        if(this.includeselfrecipient || effectiverecipients.Count == 0)
        {
            effectiverecipients.Add(new DataSecurityRecipient(this.localobject.Certificate,this.localobject.ObjectId));
        }

        this.knownobjects[effectivesigningobject.ObjectId] = effectivesigningobject;

        return new DataItemSecurityContext(
            this.localobject,
            effectivesigningobject,
            [..this.knownobjects.Values],
            [..effectiverecipients],
            this.directory,
            this.purpose,
            this.notbefore,
            this.expiresat,
            this.includeselfrecipient);
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="IncludeSelfRecipient"]/*'/>*/
    public DataItemSecurityContextBuilder IncludeSelfRecipient(Boolean value = true)
    {
        this.includeselfrecipient = value;

        return this;
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="WithSigningCertificate"]/*'/>*/
    public DataItemSecurityContextBuilder WithSigningCertificate(X509Certificate2 certificate , Guid? objectid = null , String? displayname = null)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        Guid signingobjectid = objectid ?? this.localobject.ObjectId;

        this.signingobject = new DataSecurityObject(signingobjectid,certificate,displayname);

        this.knownobjects[signingobjectid] = this.signingobject;

        return this;
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="WithSigningObjectGuid"]/*'/>*/
    public DataItemSecurityContextBuilder WithSigningObject(Guid objectid , X509Certificate2 certificate , String? displayname = null)
    {
        return this.WithSigningCertificate(certificate,objectid,displayname);
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="WithSigningObjectObject"]/*'/>*/
    public DataItemSecurityContextBuilder WithSigningObject(DataSecurityObject value)
    {
        ArgumentNullException.ThrowIfNull(value);

        this.signingobject = value;

        this.knownobjects[value.ObjectId] = value;

        return this;
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="WithKnownObject"]/*'/>*/
    public DataItemSecurityContextBuilder WithKnownObject(Guid objectid , X509Certificate2 certificate , String? displayname = null)
    {
        this.knownobjects[objectid] = new DataSecurityObject(objectid,certificate,displayname);

        return this;
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="WithKnownObjectObject"]/*'/>*/
    public DataItemSecurityContextBuilder WithKnownObject(DataSecurityObject value)
    {
        ArgumentNullException.ThrowIfNull(value);

        this.knownobjects[value.ObjectId] = value;

        return this;
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="WithPurpose"]/*'/>*/
    public DataItemSecurityContextBuilder WithPurpose(String? value)
    {
        this.purpose = String.IsNullOrWhiteSpace(value) ? null : value;

        return this;
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="WithRecipientObjectId"]/*'/>*/
    public DataItemSecurityContextBuilder WithRecipient(Guid objectid)
    {
        this.recipients.Add(new DataSecurityRecipient(objectid));

        return this;
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="WithRecipientCertificate"]/*'/>*/
    public DataItemSecurityContextBuilder WithRecipient(X509Certificate2 certificate , Guid? objectid = null)
    {
        this.recipients.Add(new DataSecurityRecipient(certificate,objectid));

        return this;
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="WithRecipients"]/*'/>*/
    public DataItemSecurityContextBuilder WithRecipients(IEnumerable<DataSecurityRecipient>? values)
    {
        if(values is null) { return this; }

        foreach(var value in values)
        {
            if(value is not null) { this.recipients.Add(value); }
        }

        return this;
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="WithDirectory"]/*'/>*/
    public DataItemSecurityContextBuilder WithDirectory(ISecurityIdentityDirectory? value)
    {
        this.directory = value;

        return this;
    }

    /**<include file='DataItemSecurityContextBuilder.xml' path='DataItemSecurityContextBuilder/class[@name="DataItemSecurityContextBuilder"]/method[@name="WithValidity"]/*'/>*/
    public DataItemSecurityContextBuilder WithValidity(DateTimeOffset? notbefore , DateTimeOffset? expiresat)
    {
        this.notbefore = notbefore;

        this.expiresat = expiresat;

        return this;
    }
}
