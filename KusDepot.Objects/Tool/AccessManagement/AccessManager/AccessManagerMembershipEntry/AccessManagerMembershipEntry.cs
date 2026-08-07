namespace KusDepot.Security;

/**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.AccessManagerMembershipEntry")] [Immutable]

public sealed class AccessManagerMembershipEntry
{
    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="AccessKeyRealmID"]/*'/>*/
    [JsonPropertyName("AccessKeyRealmID")] [JsonRequired] [Id(0)]
    public String AccessKeyRealmID { get; set; } = String.Empty;

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="AssertionSummaries"]/*'/>*/
    [JsonPropertyName("AssertionSummaries")] [JsonRequired] [Id(1)]
    public ImmutableArray<AccessKeyAssertionSummary> AssertionSummaries { get; set; } = [];

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="Audiences"]/*'/>*/
    [JsonPropertyName("Audiences")] [JsonRequired] [Id(2)]
    public ImmutableArray<String> Audiences { get; set; } = [];

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="IssuedAt"]/*'/>*/
    [JsonPropertyName("IssuedAt")] [JsonRequired] [Id(3)]
    public DateTimeOffset IssuedAt { get; set; }

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="IssuerAccessManagerID"]/*'/>*/
    [JsonPropertyName("IssuerAccessManagerID")] [JsonRequired] [Id(4)]
    public Guid IssuerAccessManagerID { get; set; }

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="IssuerToolID"]/*'/>*/
    [JsonPropertyName("IssuerToolID")] [JsonRequired] [Id(5)]
    public Guid IssuerToolID { get; set; }

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="ManifestHash"]/*'/>*/
    [JsonPropertyName("ManifestHash")] [JsonRequired] [Id(6)]
    public String ManifestHash { get; set; } = String.Empty;

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="NotAfter"]/*'/>*/
    [JsonPropertyName("NotAfter")] [JsonRequired] [Id(7)]
    public DateTimeOffset? NotAfter { get; set; }

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="Permissions"]/*'/>*/
    [JsonPropertyName("Permissions")] [JsonRequired] [Id(8)]
    public Byte[] Permissions
    {
        get => permissions.CloneByteArray();

        set => permissions = value?.CloneByteArray() ?? Array.Empty<Byte>();
    }

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/field[@name="permissions"]/*'/>*/
    [NonSerialized]
    private Byte[] permissions = Array.Empty<Byte>();

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="Scopes"]/*'/>*/
    [JsonPropertyName("Scopes")] [JsonRequired] [Id(9)]
    public ImmutableArray<String> Scopes { get; set; } = [];

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="Subject"]/*'/>*/
    [JsonPropertyName("Subject")] [JsonRequired] [Id(10)]
    public String Subject { get; set; } = String.Empty;

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="Token"]/*'/>*/
    [JsonPropertyName("Token")] [JsonRequired] [Id(11)]
    public AccessKeyToken Token { get; set; }

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/property[@name="ToolSchemaID"]/*'/>*/
    [JsonPropertyName("ToolSchemaID")] [JsonRequired] [Id(12)]
    public String ToolSchemaID { get; set; } = String.Empty;

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/method[@name="Clone"]/*'/>*/
    public AccessManagerMembershipEntry Clone()
    {
        return new()
        {
            AccessKeyRealmID = this.AccessKeyRealmID,
            AssertionSummaries = this.AssertionSummaries.IsDefaultOrEmpty ? [] : this.AssertionSummaries.Select(_ => _.Copy()).ToImmutableArray(),
            Audiences = this.Audiences,
            IssuedAt = this.IssuedAt,
            IssuerAccessManagerID = this.IssuerAccessManagerID,
            IssuerToolID = this.IssuerToolID,
            ManifestHash = this.ManifestHash,
            NotAfter = this.NotAfter,
            Permissions = this.Permissions,
            Scopes = this.Scopes,
            Subject = this.Subject,
            Token = this.Token,
            ToolSchemaID = this.ToolSchemaID
        };
    }

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/method[@name="Create"]/*'/>*/
    public static AccessManagerMembershipEntry Create(MembershipToken membership)
    {
        return new()
        {
            AccessKeyRealmID = membership.AccessKeyRealmID,
            AssertionSummaries = membership.AssertionSummaries.IsDefaultOrEmpty ? [] : membership.AssertionSummaries.Select(_ => _.Copy()).ToImmutableArray(),
            Audiences = membership.Audiences,
            IssuedAt = membership.IssuedAt,
            IssuerAccessManagerID = membership.IssuerAccessManagerID,
            IssuerToolID = membership.IssuerToolID,
            ManifestHash = membership.ManifestHash,
            NotAfter = membership.NotAfter,
            Permissions = membership.ToClaims(membership.IssuedAt).Permissions,
            Scopes = membership.Scopes,
            Subject = membership.Subject,
            Token = membership.Token,
            ToolSchemaID = membership.ToolSchemaID
        };
    }

    /**<include file='AccessManagerMembershipEntry.xml' path='AccessManagerMembershipEntry/class[@name="AccessManagerMembershipEntry"]/method[@name="ToMembershipToken"]/*'/>*/
    public MembershipToken ToMembershipToken()
    {
        return new(
            this.Subject,
            this.Token,
            this.Permissions,
            this.IssuedAt,
            this.NotAfter,
            this.ToolSchemaID,
            this.ManifestHash,
            this.AccessKeyRealmID,
            this.IssuerToolID,
            this.IssuerAccessManagerID,
            this.Audiences,
            this.Scopes,
            this.AssertionSummaries.IsDefaultOrEmpty ? [] : this.AssertionSummaries.Select(_ => _.Copy()).ToImmutableArray());
    }
}