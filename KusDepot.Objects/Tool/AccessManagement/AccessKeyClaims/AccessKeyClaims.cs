namespace KusDepot.Security;

/**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.AccessKeyClaims")] [Immutable]

public sealed record class AccessKeyClaims
{
    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="AccessKeyRealmID"]/*'/>*/
    [JsonPropertyName("AccessKeyRealmID")] [JsonRequired] [Id(0)]
    public String AccessKeyRealmID { get; init; } = String.Empty;

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="AssertionSummaries"]/*'/>*/
    [JsonPropertyName("AssertionSummaries")] [JsonRequired] [Id(1)]
    public ImmutableArray<AccessKeyAssertionSummary> AssertionSummaries { get; init; } = [];

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="Audiences"]/*'/>*/
    [JsonPropertyName("Audiences")] [JsonRequired] [Id(2)]
    public ImmutableArray<String> Audiences { get; init; } = [];

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="Expired"]/*'/>*/
    [JsonPropertyName("Expired")] [JsonRequired] [Id(3)]
    public Boolean Expired { get; init; }

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="IssuedAt"]/*'/>*/
    [JsonPropertyName("IssuedAt")] [JsonRequired] [Id(4)]
    public DateTimeOffset IssuedAt { get; init; }

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="IssuerAccessManagerID"]/*'/>*/
    [JsonPropertyName("IssuerAccessManagerID")] [JsonRequired] [Id(5)]
    public Guid IssuerAccessManagerID { get; init; }

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="IssuerToolID"]/*'/>*/
    [JsonPropertyName("IssuerToolID")] [JsonRequired] [Id(6)]
    public Guid IssuerToolID { get; init; }

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="ManifestHash"]/*'/>*/
    [JsonPropertyName("ManifestHash")] [JsonRequired] [Id(7)]
    public String ManifestHash { get; init; } = String.Empty;

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="NotAfter"]/*'/>*/
    [JsonPropertyName("NotAfter")] [JsonRequired] [Id(8)]
    public DateTimeOffset? NotAfter { get; init; }

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="Permissions"]/*'/>*/
    [JsonPropertyName("Permissions")] [JsonRequired] [Id(9)]
    public Byte[] Permissions { get; init; } = Array.Empty<Byte>();

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="Scopes"]/*'/>*/
    [JsonPropertyName("Scopes")] [JsonRequired] [Id(10)]
    public ImmutableArray<String> Scopes { get; init; } = [];

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="Subject"]/*'/>*/
    [JsonPropertyName("Subject")] [JsonRequired] [Id(11)]
    public String Subject { get; init; } = String.Empty;

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="Token"]/*'/>*/
    [JsonPropertyName("Token")] [JsonRequired] [Id(12)]
    public AccessKeyToken Token { get; init; }

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/property[@name="ToolSchemaID"]/*'/>*/
    [JsonPropertyName("ToolSchemaID")] [JsonRequired] [Id(13)]
    public String ToolSchemaID { get; init; } = String.Empty;

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/method[@name="Allows"]/*'/>*/
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

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/method[@name="Copy"]/*'/>*/
    public AccessKeyClaims Copy() => this with { Permissions = this.Permissions.CloneByteArray() };

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/method[@name="HasAudience"]/*'/>*/
    public Boolean HasAudience(String? audience)
    {
        return String.IsNullOrWhiteSpace(audience) is false && this.Audiences.Contains(audience,StringComparer.Ordinal);
    }

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/method[@name="HasScope"]/*'/>*/
    public Boolean HasScope(String? scope)
    {
        return String.IsNullOrWhiteSpace(scope) is false && this.Scopes.Contains(scope,StringComparer.Ordinal);
    }

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/method[@name="IsExpiredAt"]/*'/>*/
    public Boolean IsExpiredAt(DateTimeOffset when)
    {
        return this.NotAfter is not null && when > this.NotAfter.Value;
    }

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/method[@name="IsForRealm"]/*'/>*/
    public Boolean IsForRealm(String? accesskeyrealmid)
    {
        return String.IsNullOrWhiteSpace(accesskeyrealmid) is false && String.Equals(this.AccessKeyRealmID,accesskeyrealmid,Ordinal);
    }

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/method[@name="IsForSchema"]/*'/>*/
    public Boolean IsForSchema(String? toolschemaid)
    {
        return String.IsNullOrWhiteSpace(toolschemaid) is false && String.Equals(this.ToolSchemaID,toolschemaid,Ordinal);
    }

    /**<include file='AccessKeyClaims.xml' path='AccessKeyClaims/record[@name="AccessKeyClaims"]/method[@name="IsIssuedBy"]/*'/>*/
    public Boolean IsIssuedBy(Guid? issuertoolid = null , Guid? issueraccessmanagerid = null)
    {
        if(issuertoolid is null && issueraccessmanagerid is null) { return false; }

        if(issuertoolid.HasValue && issuertoolid.Value != Guid.Empty && this.IssuerToolID != issuertoolid.Value) { return false; }

        if(issueraccessmanagerid.HasValue && issueraccessmanagerid.Value != Guid.Empty && this.IssuerAccessManagerID != issueraccessmanagerid.Value) { return false; }

        return (issuertoolid.HasValue && issuertoolid.Value != Guid.Empty) || (issueraccessmanagerid.HasValue && issueraccessmanagerid.Value != Guid.Empty);
    }
}