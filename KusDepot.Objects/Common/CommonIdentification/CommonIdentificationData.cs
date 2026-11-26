namespace KusDepot;

/**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.CommonIdentificationData")]

public readonly record struct CommonIdentificationData
{
    /**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/property[@name="Version"]/*'/>*/
    [JsonPropertyName("Version")] [JsonRequired] [Id(0)]
    public Byte Version { get; init; }

    /**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/property[@name="Id"]/*'/>*/
    [JsonPropertyName("Id")] [JsonRequired] [Id(1)]
    public Guid Id { get; init; }

    /**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/property[@name="IssuerObjectId"]/*'/>*/
    [JsonPropertyName("IssuerObjectId")] [JsonRequired] [Id(2)]
    public Guid IssuerObjectId { get; init; }

    /**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/property[@name="SubjectObjectId"]/*'/>*/
    [JsonPropertyName("SubjectObjectId")] [JsonRequired] [Id(3)]
    public Guid? SubjectObjectId { get; init; }

    /**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/property[@name="CreatedAt"]/*'/>*/
    [JsonPropertyName("CreatedAt")] [JsonRequired] [Id(4)]
    public DateTimeOffset CreatedAt { get; init; }

    /**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/property[@name="NotBefore"]/*'/>*/
    [JsonPropertyName("NotBefore")] [JsonRequired] [Id(5)]
    public DateTimeOffset NotBefore { get; init; }

    /**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/property[@name="ExpiresAt"]/*'/>*/
    [JsonPropertyName("ExpiresAt")] [JsonRequired] [Id(6)]
    public DateTimeOffset? ExpiresAt { get; init; }

    /**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/property[@name="Purpose"]/*'/>*/
    [JsonPropertyName("Purpose")] [JsonRequired] [Id(7)]
    public String Purpose { get; init; }

    /**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/property[@name="Thumbprint"]/*'/>*/
    [JsonPropertyName("Thumbprint")] [JsonRequired] [Id(8)]
    public String Thumbprint { get; init; }

    /**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/method[@name="Create"]/*'/>*/
    public static CommonIdentificationData Create(Guid issuerobjectid , String purpose , String thumbprint , Guid? subjectobjectid = null,
    DateTimeOffset? createdat = null , DateTimeOffset? notbefore = null , DateTimeOffset? expiresat = null , Guid? id = null)
    {
        DateTimeOffset created = createdat ?? DateTimeOffset.UtcNow;

        return new()
        {
            Version = CommonIdentificationFactory.DataVersion,
            Id = id ?? Guid.NewGuid(),
            IssuerObjectId = issuerobjectid,
            SubjectObjectId = subjectobjectid,
            CreatedAt = created,
            NotBefore = notbefore ?? created,
            ExpiresAt = expiresat,
            Purpose = purpose,
            Thumbprint = thumbprint
        };
    }

    /**<include file='CommonIdentificationData.xml' path='CommonIdentificationData/record[@name="CommonIdentificationData"]/method[@name="CreateSecurityObject"]/*'/>*/
    public static CommonIdentificationData Create(DataSecurityObject issuer , String purpose , Guid? subjectobjectid = null,
        DateTimeOffset? createdat = null , DateTimeOffset? notbefore = null , DateTimeOffset? expiresat = null , Guid? id = null)
    {
        ArgumentNullException.ThrowIfNull(issuer);

        return Create(issuer.ObjectId,purpose,issuer.Thumbprint,subjectobjectid,createdat,notbefore,expiresat,id);
    }
}