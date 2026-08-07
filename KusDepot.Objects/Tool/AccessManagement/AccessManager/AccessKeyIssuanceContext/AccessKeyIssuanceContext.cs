namespace KusDepot.Security;

/**<include file='AccessKeyIssuanceContext.xml' path='AccessKeyIssuanceContext/class[@name="AccessKeyIssuanceContext"]/main/*'/>*/
internal sealed record class AccessKeyIssuanceContext
{
    /**<include file='AccessKeyIssuanceContext.xml' path='AccessKeyIssuanceContext/class[@name="AccessKeyIssuanceContext"]/property[@name="Assertions"]/*'/>*/
    public ImmutableArray<AccessKeyAssertion> Assertions { get; init; } = [];

    /**<include file='AccessKeyIssuanceContext.xml' path='AccessKeyIssuanceContext/class[@name="AccessKeyIssuanceContext"]/property[@name="IssueOptions"]/*'/>*/
    public AccessKeyIssueOptions IssueOptions { get; init; } = null!;

    /**<include file='AccessKeyIssuanceContext.xml' path='AccessKeyIssuanceContext/class[@name="AccessKeyIssuanceContext"]/property[@name="IssuerAccessManagerID"]/*'/>*/
    public Guid IssuerAccessManagerID { get; init; }

    /**<include file='AccessKeyIssuanceContext.xml' path='AccessKeyIssuanceContext/class[@name="AccessKeyIssuanceContext"]/property[@name="IssuerToolID"]/*'/>*/
    public Guid IssuerToolID { get; init; }

    /**<include file='AccessKeyIssuanceContext.xml' path='AccessKeyIssuanceContext/class[@name="AccessKeyIssuanceContext"]/property[@name="Logger"]/*'/>*/
    public ILogger Logger { get; init; } = NullLogger.Instance;

    /**<include file='AccessKeyIssuanceContext.xml' path='AccessKeyIssuanceContext/class[@name="AccessKeyIssuanceContext"]/property[@name="ManifestIdentity"]/*'/>*/
    public ToolManifestIdentity ManifestIdentity { get; init; } = null!;

    /**<include file='AccessKeyIssuanceContext.xml' path='AccessKeyIssuanceContext/class[@name="AccessKeyIssuanceContext"]/property[@name="RequestContext"]/*'/>*/
    public AccessPolicyRequestContext? RequestContext { get; init; }

    /**<include file='AccessKeyIssuanceContext.xml' path='AccessKeyIssuanceContext/class[@name="AccessKeyIssuanceContext"]/property[@name="Subject"]/*'/>*/
    public String Subject { get; init; } = String.Empty;

    /**<include file='AccessKeyIssuanceContext.xml' path='AccessKeyIssuanceContext/class[@name="AccessKeyIssuanceContext"]/property[@name="Tool"]/*'/>*/
    public ITool? Tool { get; init; }
}