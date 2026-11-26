namespace KusDepot.Security.Assertions;

/**<include file='AccessKeyAssertionGeneratorContext.xml' path='AccessKeyAssertionGeneratorContext/struct[@name="AccessKeyAssertionGeneratorContext"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Assertions.AccessKeyAssertionGeneratorContext")]

public readonly struct AccessKeyAssertionGeneratorContext
{
    /**<include file='AccessKeyAssertionGeneratorContext.xml' path='AccessKeyAssertionGeneratorContext/struct[@name="AccessKeyAssertionGeneratorContext"]/property[@name="AccessPolicyContext"]/*'/>*/
    [JsonPropertyName("AccessPolicyContext")] [JsonRequired] [Id(0)]
    public AccessPolicyContext AccessPolicyContext { get; init; }

    /**<include file='AccessKeyAssertionGeneratorContext.xml' path='AccessKeyAssertionGeneratorContext/struct[@name="AccessKeyAssertionGeneratorContext"]/property[@name="IssueOptions"]/*'/>*/
    [JsonPropertyName("IssueOptions")] [JsonRequired] [Id(1)]
    public AccessKeyIssueOptions? IssueOptions { get; init; }

    /**<include file='AccessKeyAssertionGeneratorContext.xml' path='AccessKeyAssertionGeneratorContext/struct[@name="AccessKeyAssertionGeneratorContext"]/property[@name="Logger"]/*'/>*/
    [JsonIgnore] [field: NonSerialized]
    public ILogger? Logger { get; init; }

    /**<include file='AccessKeyAssertionGeneratorContext.xml' path='AccessKeyAssertionGeneratorContext/struct[@name="AccessKeyAssertionGeneratorContext"]/property[@name="ManifestIdentity"]/*'/>*/
    [JsonPropertyName("ManifestIdentity")] [JsonRequired] [Id(2)]
    public ToolManifestIdentity? ManifestIdentity { get; init; }

    /**<include file='AccessKeyAssertionGeneratorContext.xml' path='AccessKeyAssertionGeneratorContext/struct[@name="AccessKeyAssertionGeneratorContext"]/property[@name="Request"]/*'/>*/
    [JsonPropertyName("Request")] [JsonRequired] [Id(3)]
    public AccessRequest? Request { get; init; }

    /**<include file='AccessKeyAssertionGeneratorContext.xml' path='AccessKeyAssertionGeneratorContext/struct[@name="AccessKeyAssertionGeneratorContext"]/property[@name="RequestedKeyType"]/*'/>*/
    [JsonPropertyName("RequestedKeyType")] [JsonRequired] [Id(4)]
    public String RequestedKeyType { get; init; } = String.Empty;

    /**<include file='AccessKeyAssertionGeneratorContext.xml' path='AccessKeyAssertionGeneratorContext/struct[@name="AccessKeyAssertionGeneratorContext"]/property[@name="RequestContext"]/*'/>*/
    [JsonPropertyName("RequestContext")] [JsonRequired] [Id(5)]
    public AccessPolicyRequestContext? RequestContext { get; init; }

    /**<include file='AccessKeyAssertionGeneratorContext.xml' path='AccessKeyAssertionGeneratorContext/struct[@name="AccessKeyAssertionGeneratorContext"]/property[@name="Subject"]/*'/>*/
    [JsonPropertyName("Subject")] [JsonRequired] [Id(6)]
    public String Subject { get; init; } = String.Empty;

    /**<include file='AccessKeyAssertionGeneratorContext.xml' path='AccessKeyAssertionGeneratorContext/struct[@name="AccessKeyAssertionGeneratorContext"]/property[@name="Tool"]/*'/>*/
    [JsonPropertyName("Tool")] [JsonRequired] [Id(7)]
    public ITool? Tool { get; init; }

    /**<include file='AccessKeyAssertionGeneratorContext.xml' path='AccessKeyAssertionGeneratorContext/struct[@name="AccessKeyAssertionGeneratorContext"]/constructor[@name="Constructor"]/*'/>*/
    public AccessKeyAssertionGeneratorContext() {}
}