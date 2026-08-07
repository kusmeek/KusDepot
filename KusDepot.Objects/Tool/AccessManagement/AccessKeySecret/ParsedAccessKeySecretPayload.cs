namespace KusDepot.Security;

/**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/main/*'/>*/
internal sealed class ParsedAccessKeySecretPayload
{
    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="AccessKeyRealmID"]/*'/>*/
    public String AccessKeyRealmID { get; init; } = String.Empty;

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="Assertions"]/*'/>*/
    public ImmutableArray<AccessKeyAssertion> Assertions { get; init; } = [];

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="AssertionSummaries"]/*'/>*/
    public ImmutableArray<AccessKeyAssertionSummary> AssertionSummaries { get; init; } = [];

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="Audiences"]/*'/>*/
    public ImmutableArray<String> Audiences { get; init; } = [];

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="IssuedAt"]/*'/>*/
    public DateTimeOffset IssuedAt { get; init; }

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="IssuerAccessManagerID"]/*'/>*/
    public Guid IssuerAccessManagerID { get; init; }

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="IssuerToolID"]/*'/>*/
    public Guid IssuerToolID { get; init; }

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="ManifestHash"]/*'/>*/
    public String ManifestHash { get; init; } = String.Empty;

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="NotAfter"]/*'/>*/
    public DateTimeOffset? NotAfter { get; init; }

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="Permissions"]/*'/>*/
    public Byte[] Permissions { get; init; } = Array.Empty<Byte>();

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="Scopes"]/*'/>*/
    public ImmutableArray<String> Scopes { get; init; } = [];

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="Subject"]/*'/>*/
    public String Subject { get; init; } = String.Empty;

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="Token"]/*'/>*/
    public AccessKeyToken Token { get; init; }

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/property[@name="ToolSchemaID"]/*'/>*/
    public String ToolSchemaID { get; init; } = String.Empty;

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/method[@name="ToClaims"]/*'/>*/
    public AccessKeyClaims ToClaims(DateTimeOffset evaluatedat)
    {
        return new()
        {
            AccessKeyRealmID = this.AccessKeyRealmID,
            AssertionSummaries = this.AssertionSummaries,
            Audiences = this.Audiences,
            Expired = this.NotAfter is not null && evaluatedat > this.NotAfter.Value,
            IssuedAt = this.IssuedAt,
            IssuerAccessManagerID = this.IssuerAccessManagerID,
            IssuerToolID = this.IssuerToolID,
            ManifestHash = this.ManifestHash,
            NotAfter = this.NotAfter,
            Permissions = this.Permissions.CloneByteArray(),
            Scopes = this.Scopes,
            Subject = this.Subject,
            Token = this.Token,
            ToolSchemaID = this.ToolSchemaID
        };
    }

    /**<include file='ParsedAccessKeySecretPayload.xml' path='ParsedAccessKeySecretPayload/class[@name="ParsedAccessKeySecretPayload"]/method[@name="ToMembershipToken"]/*'/>*/
    public MembershipToken ToMembershipToken()
    {
        return new(
            this.Subject,
            this.Token,
            this.Permissions.CloneByteArray(),
            this.IssuedAt,
            this.NotAfter,
            this.ToolSchemaID,
            this.ManifestHash,
            this.AccessKeyRealmID,
            this.IssuerToolID,
            this.IssuerAccessManagerID,
            this.Audiences,
            this.Scopes,
            this.AssertionSummaries);
    }
}