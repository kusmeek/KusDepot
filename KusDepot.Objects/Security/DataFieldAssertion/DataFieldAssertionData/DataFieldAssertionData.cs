namespace KusDepot.Security;

/**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.DataFieldAssertionData")]

public readonly record struct DataFieldAssertionData
{
    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="Version"]/*'/>*/
    [JsonPropertyName("Version")] [JsonRequired] [Id(0)]
    public Byte Version { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="AssertionId"]/*'/>*/
    [JsonPropertyName("AssertionId")] [JsonRequired] [Id(1)]
    public Guid AssertionId { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="DataItemId"]/*'/>*/
    [JsonPropertyName("DataItemId")] [JsonRequired] [Id(2)]
    public Guid DataItemId { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="Field"]/*'/>*/
    [JsonPropertyName("Field")] [JsonRequired] [Id(3)]
    public String Field { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="FieldState"]/*'/>*/
    [JsonPropertyName("FieldState")] [JsonRequired] [Id(4)]
    public DataFieldState FieldState { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="HashAlgorithm"]/*'/>*/
    [JsonPropertyName("HashAlgorithm")] [JsonRequired] [Id(5)]
    public DataFieldHashAlgorithm HashAlgorithm { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="Hash"]/*'/>*/
    [JsonPropertyName("Hash")] [JsonRequired] [Id(6)]
    public Byte[] Hash { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="IssuerObjectId"]/*'/>*/
    [JsonPropertyName("IssuerObjectId")] [JsonRequired] [Id(7)]
    public Guid IssuerObjectId { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="SubjectObjectId"]/*'/>*/
    [JsonPropertyName("SubjectObjectId")] [JsonRequired] [Id(8)]
    public Guid? SubjectObjectId { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="CreatedAt"]/*'/>*/
    [JsonPropertyName("CreatedAt")] [JsonRequired] [Id(9)]
    public DateTimeOffset CreatedAt { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="NotBefore"]/*'/>*/
    [JsonPropertyName("NotBefore")] [JsonRequired] [Id(10)]
    public DateTimeOffset? NotBefore { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="ExpiresAt"]/*'/>*/
    [JsonPropertyName("ExpiresAt")] [JsonRequired] [Id(11)]
    public DateTimeOffset? ExpiresAt { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="Purpose"]/*'/>*/
    [JsonPropertyName("Purpose")] [JsonRequired] [Id(12)]
    public String? Purpose { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/property[@name="Thumbprint"]/*'/>*/
    [JsonPropertyName("Thumbprint")] [JsonRequired] [Id(13)]
    public String Thumbprint { get; init; }

    /**<include file='DataFieldAssertionData.xml' path='DataFieldAssertionData/struct[@name="DataFieldAssertionData"]/method[@name="Create"]/*'/>*/
    public static DataFieldAssertionData Create(Guid dataitemid , String field , DataFieldState fieldstate , DataFieldHashAlgorithm hashalgorithm , Byte[] hash , Guid issuerobjectid , String thumbprint , Guid? subjectobjectid = null , String? purpose = null,
        DateTimeOffset? createdat = null , DateTimeOffset? notbefore = null , DateTimeOffset? expiresat = null , Guid? assertionid = null)
    {
        DateTimeOffset created = createdat ?? DateTimeOffset.UtcNow;

        return new()
        {
            Version = DataFieldAssertionFactory.DataVersion,
            AssertionId = assertionid ?? Guid.NewGuid(),
            DataItemId = dataitemid,
            Field = field,
            FieldState = fieldstate,
            HashAlgorithm = hashalgorithm,
            Hash = (Byte[])hash.Clone(),
            IssuerObjectId = issuerobjectid,
            SubjectObjectId = subjectobjectid,
            CreatedAt = created,
            NotBefore = notbefore,
            ExpiresAt = expiresat,
            Purpose = purpose,
            Thumbprint = thumbprint
        };
    }
}