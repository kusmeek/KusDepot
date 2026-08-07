namespace KusDepot.Security;

/**<include file='IssuedAccessKeyMaterial.xml' path='IssuedAccessKeyMaterial/class[@name="IssuedAccessKeyMaterial"]/main/*'/>*/
internal sealed class IssuedAccessKeyMaterial
{
    /**<include file='IssuedAccessKeyMaterial.xml' path='IssuedAccessKeyMaterial/class[@name="IssuedAccessKeyMaterial"]/property[@name="Assertions"]/*'/>*/
    public ImmutableArray<AccessKeyAssertion> Assertions { get; init; } = [];

    /**<include file='IssuedAccessKeyMaterial.xml' path='IssuedAccessKeyMaterial/class[@name="IssuedAccessKeyMaterial"]/property[@name="Ciphertext"]/*'/>*/
    public Byte[] Ciphertext { get; init; } = Array.Empty<Byte>();

    /**<include file='IssuedAccessKeyMaterial.xml' path='IssuedAccessKeyMaterial/class[@name="IssuedAccessKeyMaterial"]/property[@name="CacheKey"]/*'/>*/
    public MembershipCacheKey? CacheKey { get; init; }

    /**<include file='IssuedAccessKeyMaterial.xml' path='IssuedAccessKeyMaterial/class[@name="IssuedAccessKeyMaterial"]/property[@name="ManifestIdentity"]/*'/>*/
    public ToolManifestIdentity ManifestIdentity { get; init; } = null!;

    /**<include file='IssuedAccessKeyMaterial.xml' path='IssuedAccessKeyMaterial/class[@name="IssuedAccessKeyMaterial"]/property[@name="Membership"]/*'/>*/
    public MembershipToken? Membership { get; init; }

    /**<include file='IssuedAccessKeyMaterial.xml' path='IssuedAccessKeyMaterial/class[@name="IssuedAccessKeyMaterial"]/property[@name="Subject"]/*'/>*/
    public String Subject { get; init; } = String.Empty;

    /**<include file='IssuedAccessKeyMaterial.xml' path='IssuedAccessKeyMaterial/class[@name="IssuedAccessKeyMaterial"]/property[@name="Token"]/*'/>*/
    public AccessKeyToken Token { get; init; }
}