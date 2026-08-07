namespace KusDepot.Security.Data;

/**<include file='DataItemSecurityContext.xml' path='DataItemSecurityContext/class[@name="DataItemSecurityContext"]/main/*'/>*/
public sealed record class DataItemSecurityContext
{
    /**<include file='DataItemSecurityContext.xml' path='DataItemSecurityContext/class[@name="DataItemSecurityContext"]/constructor[@name="Constructor"]/*'/>*/
    public DataItemSecurityContext(
        DataSecurityObject localobject,
        DataSecurityObject? signingobject,
        ImmutableArray<DataSecurityObject> knownobjects,
        ImmutableArray<DataSecurityRecipient> recipients,
        ISecurityIdentityDirectory? directory = null,
        String? purpose = null,
        DateTimeOffset? notbefore = null,
        DateTimeOffset? expiresat = null,
        Boolean includeselfrecipient = true)
    {
        ArgumentNullException.ThrowIfNull(localobject);

        this.LocalObject = localobject;
        this.SigningObject = signingobject ?? localobject;
        this.KnownObjects = knownobjects.IsDefault ? [] : knownobjects;
        this.Recipients = recipients.IsDefault ? [] : recipients;
        this.Directory = directory;
        this.Purpose = String.IsNullOrWhiteSpace(purpose) ? null : purpose;
        this.NotBefore = notbefore;
        this.ExpiresAt = expiresat;
        this.IncludeSelfRecipient = includeselfrecipient;
    }

    /**<include file='DataItemSecurityContext.xml' path='DataItemSecurityContext/class[@name="DataItemSecurityContext"]/property[@name="LocalObject"]/*'/>*/
    public DataSecurityObject LocalObject { get; init; }

    /**<include file='DataItemSecurityContext.xml' path='DataItemSecurityContext/class[@name="DataItemSecurityContext"]/property[@name="SigningObject"]/*'/>*/
    public DataSecurityObject SigningObject { get; init; }

    /**<include file='DataItemSecurityContext.xml' path='DataItemSecurityContext/class[@name="DataItemSecurityContext"]/property[@name="KnownObjects"]/*'/>*/
    public ImmutableArray<DataSecurityObject> KnownObjects { get; init; } = [];

    /**<include file='DataItemSecurityContext.xml' path='DataItemSecurityContext/class[@name="DataItemSecurityContext"]/property[@name="Recipients"]/*'/>*/
    public ImmutableArray<DataSecurityRecipient> Recipients { get; init; } = [];

    /**<include file='DataItemSecurityContext.xml' path='DataItemSecurityContext/class[@name="DataItemSecurityContext"]/property[@name="Directory"]/*'/>*/
    public ISecurityIdentityDirectory? Directory { get; init; }

    /**<include file='DataItemSecurityContext.xml' path='DataItemSecurityContext/class[@name="DataItemSecurityContext"]/property[@name="Purpose"]/*'/>*/
    public String? Purpose { get; init; }

    /**<include file='DataItemSecurityContext.xml' path='DataItemSecurityContext/class[@name="DataItemSecurityContext"]/property[@name="NotBefore"]/*'/>*/
    public DateTimeOffset? NotBefore { get; init; }

    /**<include file='DataItemSecurityContext.xml' path='DataItemSecurityContext/class[@name="DataItemSecurityContext"]/property[@name="ExpiresAt"]/*'/>*/
    public DateTimeOffset? ExpiresAt { get; init; }

    /**<include file='DataItemSecurityContext.xml' path='DataItemSecurityContext/class[@name="DataItemSecurityContext"]/property[@name="IncludeSelfRecipient"]/*'/>*/
    public Boolean IncludeSelfRecipient { get; init; }
}