namespace KusDepot.Security;

/**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/main/*'/>*/
public sealed record class AccessKeySecretCreateRequest
{
    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="AccessKeyRealmID"]/*'/>*/
    public String? AccessKeyRealmID { get; init; }

    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="Assertions"]/*'/>*/
    public IEnumerable<AccessKeyAssertion>? Assertions { get; init; }

    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="Audiences"]/*'/>*/
    public IEnumerable<String>? Audiences { get; init; }

    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="IssuerAccessManagerID"]/*'/>*/
    public Guid IssuerAccessManagerID { get; init; }

    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="IssuerToolID"]/*'/>*/
    public Guid IssuerToolID { get; init; }

    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="Lifetime"]/*'/>*/
    public TimeSpan? Lifetime { get; init; }

    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="ManifestHash"]/*'/>*/
    public String? ManifestHash { get; init; }

    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="Operations"]/*'/>*/
    public IEnumerable<Int32>? Operations { get; init; }

    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="Scopes"]/*'/>*/
    public IEnumerable<String>? Scopes { get; init; }

    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="Signer"]/*'/>*/
    public IAccessKeySigner? Signer { get; init; }

    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="Subject"]/*'/>*/
    public String Subject { get; init; } = String.Empty;

    /**<include file='AccessKeySecretCreateRequest.xml' path='AccessKeySecretCreateRequest/class[@name="AccessKeySecretCreateRequest"]/property[@name="ToolSchemaID"]/*'/>*/
    public String? ToolSchemaID { get; init; }
}