namespace KusDepot.Security.Policy;

/**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Policy.AccessPolicyContext")]

public readonly struct AccessPolicyContext
{
    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="AccessKeyRealmID"]/*'/>*/
    [JsonPropertyName("AccessKeyRealmID")] [JsonRequired] [Id(0)]
    public String AccessKeyRealmID { get; init; } = String.Empty;

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="AssertionSummaries"]/*'/>*/
    [JsonPropertyName("AssertionSummaries")] [JsonRequired] [Id(1)]
    public ImmutableArray<AccessKeyAssertionSummary>? AssertionSummaries { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="Claims"]/*'/>*/
    [JsonPropertyName("Claims")] [JsonRequired] [Id(2)]
    public AccessKeyClaims? Claims { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="EvaluatedAt"]/*'/>*/
    [JsonPropertyName("EvaluatedAt")] [JsonRequired] [Id(3)]
    public DateTimeOffset EvaluatedAt { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="Expired"]/*'/>*/
    [JsonPropertyName("Expired")] [JsonRequired] [Id(4)]
    public Boolean Expired { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="IssueOptions"]/*'/>*/
    [JsonPropertyName("IssueOptions")] [JsonRequired] [Id(5)]
    public AccessKeyIssueOptions? IssueOptions { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="Key"]/*'/>*/
    [JsonPropertyName("Key")] [JsonRequired] [Id(6)]
    public AccessKey? Key { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="Logger"]/*'/>*/
    [JsonIgnore] [field: NonSerialized]
    public ILogger? Logger { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="ManifestHash"]/*'/>*/
    [JsonPropertyName("ManifestHash")] [JsonRequired] [Id(7)]
    public String ManifestHash { get; init; } = String.Empty;

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="ManifestMatched"]/*'/>*/
    [JsonPropertyName("ManifestMatched")] [JsonRequired] [Id(8)]
    public Boolean ManifestMatched { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="MembershipActive"]/*'/>*/
    [JsonPropertyName("MembershipActive")] [JsonRequired] [Id(9)]
    public Boolean MembershipActive { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="Mode"]/*'/>*/
    [JsonPropertyName("Mode")] [JsonRequired] [Id(10)]
    public AccessPolicyMode Mode { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="Operation"]/*'/>*/
    [JsonPropertyName("Operation")] [JsonRequired] [Id(11)]
    public Int32? Operation { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="OperationAllowed"]/*'/>*/
    [JsonPropertyName("OperationAllowed")] [JsonRequired] [Id(12)]
    public Boolean OperationAllowed { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="Operations"]/*'/>*/
    [JsonPropertyName("Operations")] [JsonRequired] [Id(13)]
    public ImmutableArray<Int32>? Operations { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="RequestContext"]/*'/>*/
    [JsonPropertyName("RequestContext")] [JsonRequired] [Id(14)]
    public AccessPolicyRequestContext? RequestContext { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="SignatureValidation"]/*'/>*/
    [JsonPropertyName("SignatureValidation")] [JsonRequired] [Id(15)]
    public SignatureValidation SignatureValidation { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="Subject"]/*'/>*/
    [JsonPropertyName("Subject")] [JsonRequired] [Id(16)]
    public String Subject { get; init; } = String.Empty;

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="Tool"]/*'/>*/
    [JsonPropertyName("Tool")] [JsonRequired] [Id(17)]
    public ITool? Tool { get; init; }

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/property[@name="ToolSchemaID"]/*'/>*/
    [JsonPropertyName("ToolSchemaID")] [JsonRequired] [Id(18)]
    public String ToolSchemaID { get; init; } = String.Empty;

    /**<include file='AccessPolicyContext.xml' path='AccessPolicyContext/struct[@name="AccessPolicyContext"]/constructor[@name="Constructor"]/*'/>*/
    public AccessPolicyContext() {}
}