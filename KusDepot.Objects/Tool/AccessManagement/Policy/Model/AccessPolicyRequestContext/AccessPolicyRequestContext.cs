namespace KusDepot.Security.Policy;

/**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Policy.AccessPolicyRequestContext")]

public readonly struct AccessPolicyRequestContext
{
    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="Audiences"]/*'/>*/
    [JsonPropertyName("Audiences")] [JsonRequired] [Id(0)]
    public ImmutableArray<String>? Audiences { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="Credential"]/*'/>*/
    [JsonPropertyName("Credential")] [JsonRequired] [Id(1)]
    public AccessCredential? Credential { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="ExternalRequest"]/*'/>*/
    [JsonPropertyName("ExternalRequest")] [JsonRequired] [Id(2)]
    public Boolean ExternalRequest { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="HostMatched"]/*'/>*/
    [JsonPropertyName("HostMatched")] [JsonRequired] [Id(3)]
    public Boolean HostMatched { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="Lifetime"]/*'/>*/
    [JsonPropertyName("Lifetime")] [JsonRequired] [Id(4)]
    public TimeSpan? Lifetime { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="ManagementCredentialValid"]/*'/>*/
    [JsonPropertyName("ManagementCredentialValid")] [JsonRequired] [Id(5)]
    public Boolean ManagementCredentialValid { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="Operations"]/*'/>*/
    [JsonPropertyName("Operations")] [JsonRequired] [Id(6)]
    public ImmutableArray<Int32>? Operations { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="Properties"]/*'/>*/
    [JsonPropertyName("Properties")] [JsonRequired] [Id(7)]
    public ImmutableDictionary<String,String>? Properties { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="Request"]/*'/>*/
    [JsonPropertyName("Request")] [JsonRequired] [Id(8)]
    public AccessRequest? Request { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="RequestedAssertions"]/*'/>*/
    [JsonPropertyName("RequestedAssertions")] [JsonRequired] [Id(9)]
    public ImmutableArray<AccessAssertionRequest>? RequestedAssertions { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="RequestedKeyType"]/*'/>*/
    [JsonPropertyName("RequestedKeyType")] [JsonRequired] [Id(10)]
    public String RequestedKeyType { get; init; } = String.Empty;

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="RequestValidated"]/*'/>*/
    [JsonPropertyName("RequestValidated")] [JsonRequired] [Id(11)]
    public Boolean RequestValidated { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="Scopes"]/*'/>*/
    [JsonPropertyName("Scopes")] [JsonRequired] [Id(12)]
    public ImmutableArray<String>? Scopes { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="ServiceMatched"]/*'/>*/
    [JsonPropertyName("ServiceMatched")] [JsonRequired] [Id(13)]
    public Boolean ServiceMatched { get; init; }

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/property[@name="Subject"]/*'/>*/
    [JsonPropertyName("Subject")] [JsonRequired] [Id(14)]
    public String Subject { get; init; } = String.Empty;

    /**<include file='AccessPolicyRequestContext.xml' path='AccessPolicyRequestContext/struct[@name="AccessPolicyRequestContext"]/constructor[@name="Constructor"]/*'/>*/
    public AccessPolicyRequestContext() {}
}