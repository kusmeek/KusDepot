namespace KusDepot.Security;

/**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.AccessManagerSnapshotEntry")] [Immutable]

public sealed record class AccessManagerSnapshotEntry
{
    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="AccessKeyRealmID"]/*'/>*/
    [JsonPropertyName("AccessKeyRealmID")] [JsonRequired] [Id(0)]
    public String AccessKeyRealmID { get; init; } = String.Empty;

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="AssertionSummaries"]/*'/>*/
    [JsonPropertyName("AssertionSummaries")] [JsonRequired] [Id(1)]
    public ImmutableArray<AccessKeyAssertionSummary> AssertionSummaries { get; init; } = [];

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="Audiences"]/*'/>*/
    [JsonPropertyName("Audiences")] [JsonRequired] [Id(2)]
    public ImmutableArray<String> Audiences { get; init; } = [];

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="Expired"]/*'/>*/
    [JsonPropertyName("Expired")] [JsonRequired] [Id(3)]
    public Boolean Expired { get; init; }

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="IssuedAt"]/*'/>*/
    [JsonPropertyName("IssuedAt")] [JsonRequired] [Id(4)]
    public DateTimeOffset IssuedAt { get; init; }

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="IssuerAccessManagerID"]/*'/>*/
    [JsonPropertyName("IssuerAccessManagerID")] [JsonRequired] [Id(5)]
    public Guid IssuerAccessManagerID { get; init; }

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="IssuerToolID"]/*'/>*/
    [JsonPropertyName("IssuerToolID")] [JsonRequired] [Id(6)]
    public Guid IssuerToolID { get; init; }

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="ManifestHash"]/*'/>*/
    [JsonPropertyName("ManifestHash")] [JsonRequired] [Id(7)]
    public String ManifestHash { get; init; } = String.Empty;

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="NotAfter"]/*'/>*/
    [JsonPropertyName("NotAfter")] [JsonRequired] [Id(8)]
    public DateTimeOffset? NotAfter { get; init; }

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="Permissions"]/*'/>*/
    [JsonPropertyName("Permissions")] [JsonRequired] [Id(9)]
    public Byte[] Permissions { get; init; } = Array.Empty<Byte>();

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="Scopes"]/*'/>*/
    [JsonPropertyName("Scopes")] [JsonRequired] [Id(10)]
    public ImmutableArray<String> Scopes { get; init; } = [];

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="Subject"]/*'/>*/
    [JsonPropertyName("Subject")] [JsonRequired] [Id(11)]
    public String Subject { get; init; } = String.Empty;

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/property[@name="ToolSchemaID"]/*'/>*/
    [JsonPropertyName("ToolSchemaID")] [JsonRequired] [Id(12)]
    public String ToolSchemaID { get; init; } = String.Empty;

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/method[@name="Allows"]/*'/>*/
    public Boolean Allows(Int32 operation)
    {
        if(operation < 0) { return false; }

        Int32 bit = operation % AccessKeySecret.BitsPerBlock;
        Int32 blockindex = operation / AccessKeySecret.BitsPerBlock;
        Int32 blockoffset = blockindex * AccessKeySecret.BytesPerBlock;

        if(blockoffset + AccessKeySecret.BytesPerBlock > this.Permissions.Length) { return false; }

        UInt128 block = ReadUInt128BigEndian(this.Permissions.AsSpan(blockoffset,AccessKeySecret.BytesPerBlock));

        return ((block >> bit) & UInt128.One) == UInt128.One;
    }

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/method[@name="Copy"]/*'/>*/
    public AccessManagerSnapshotEntry Copy() => this with { AssertionSummaries = this.AssertionSummaries.IsDefaultOrEmpty ? [] : this.AssertionSummaries.Select(_ => _.Copy()).ToImmutableArray() , Permissions = this.Permissions.CloneByteArray() };

    /**<include file='AccessManagerSnapshotEntry.xml' path='AccessManagerSnapshotEntry/record[@name="AccessManagerSnapshotEntry"]/method[@name="Create"]/*'/>*/
    public static AccessManagerSnapshotEntry Create(AccessKeyClaims claims)
    {
        return new()
        {
            AccessKeyRealmID = claims.AccessKeyRealmID,
            AssertionSummaries = claims.AssertionSummaries.IsDefaultOrEmpty ? [] : claims.AssertionSummaries.Select(_ => _.Copy()).ToImmutableArray(),
            Audiences = claims.Audiences,
            Expired = claims.Expired,
            IssuedAt = claims.IssuedAt,
            IssuerAccessManagerID = claims.IssuerAccessManagerID,
            IssuerToolID = claims.IssuerToolID,
            ManifestHash = claims.ManifestHash,
            NotAfter = claims.NotAfter,
            Permissions = claims.Permissions.CloneByteArray(),
            Scopes = claims.Scopes,
            Subject = claims.Subject,
            ToolSchemaID = claims.ToolSchemaID
        };
    }
}