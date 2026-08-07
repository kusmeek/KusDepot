namespace KusDepot.Security;

/**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.AccessKeyIssueOptions")]

public sealed class AccessKeyIssueOptions
{
    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/property[@name="Audiences"]/*'/>*/
    [JsonPropertyName("Audiences")] [JsonRequired] [Id(0)]
    public ImmutableArray<String>? Audiences { get; set; }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/property[@name="Credential"]/*'/>*/
    [JsonPropertyName("Credential")] [JsonRequired] [Id(1)]
    public AccessCredential? Credential { get; set; }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/property[@name="Lifetime"]/*'/>*/
    [JsonPropertyName("Lifetime")] [JsonRequired] [Id(2)]
    public TimeSpan? Lifetime { get; set; }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/property[@name="Operations"]/*'/>*/
    [JsonPropertyName("Operations")] [JsonRequired] [Id(3)]
    public ImmutableArray<Int32>? Operations { get; set; }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/property[@name="Properties"]/*'/>*/
    [JsonPropertyName("Properties")] [JsonRequired] [Id(4)]
    public ImmutableDictionary<String,String>? Properties { get; set; }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/property[@name="RequestedAssertions"]/*'/>*/
    [JsonPropertyName("RequestedAssertions")] [JsonRequired] [Id(5)]
    public ImmutableArray<AccessAssertionRequest>? RequestedAssertions { get; set; }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/property[@name="RequestedKeyType"]/*'/>*/
    [JsonPropertyName("RequestedKeyType")] [JsonRequired] [Id(6)]
    public String? RequestedKeyType { get; set; }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/property[@name="Scopes"]/*'/>*/
    [JsonPropertyName("Scopes")] [JsonRequired] [Id(7)]
    public ImmutableArray<String>? Scopes { get; set; }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/property[@name="Subject"]/*'/>*/
    [JsonPropertyName("Subject")] [JsonRequired] [Id(8)]
    public String? Subject { get; set; }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="Create"]/*'/>*/
    public static AccessKeyIssueOptions Create(
        String? subject = null , IEnumerable<Int32>? operations = null , TimeSpan? lifetime = null , IEnumerable<String>? audiences = null , IEnumerable<String>? scopes = null,
        AccessCredential? credential = null , IEnumerable<KeyValuePair<String,String>>? properties = null , IEnumerable<AccessAssertionRequest>? requestedassertions = null,
        String? requestedkeytype = null)
    {
        return new()
        {
            Audiences = audiences?.ToImmutableArray(),
            Credential = credential,
            Lifetime = lifetime,
            Operations = operations?.ToImmutableArray(),
            Properties = properties?.ToImmutableDictionary(),
            RequestedAssertions = requestedassertions?.ToImmutableArray(),
            RequestedKeyType = requestedkeytype,
            Scopes = scopes?.ToImmutableArray(),
            Subject = subject
        };
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="AddAudience"]/*'/>*/
    public AccessKeyIssueOptions AddAudience(String? audience)
    {
        if(String.IsNullOrWhiteSpace(audience)) { return this; }

        ImmutableArray<String> values = this.Audiences ?? [];

        this.Audiences = values.Add(audience); return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="AddProperty"]/*'/>*/
    public AccessKeyIssueOptions AddProperty(String? key , String? value)
    {
        if(String.IsNullOrWhiteSpace(key)) { return this; }

        ImmutableDictionary<String,String> values = this.Properties ?? ImmutableDictionary<String,String>.Empty;

        this.Properties = values.SetItem(key,value ?? String.Empty); return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="AddRequestedAssertion"]/*'/>*/
    public AccessKeyIssueOptions AddRequestedAssertion(AccessAssertionRequest? requestedassertion)
    {
        if(requestedassertion is null) { return this; }

        ImmutableArray<AccessAssertionRequest> values = this.RequestedAssertions ?? [];

        this.RequestedAssertions = values.Add(requestedassertion); return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="AddOperation"]/*'/>*/
    public AccessKeyIssueOptions AddOperation(Int32 operation)
    {
        ImmutableArray<Int32> values = this.Operations ?? [];

        this.Operations = values.Add(operation); return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="AddScope"]/*'/>*/
    public AccessKeyIssueOptions AddScope(String? scope)
    {
        if(String.IsNullOrWhiteSpace(scope)) { return this; }

        ImmutableArray<String> values = this.Scopes ?? [];

        this.Scopes = values.Add(scope); return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="SetAudiences"]/*'/>*/
    public AccessKeyIssueOptions SetAudiences(IEnumerable<String>? audiences = null)
    {
        this.Audiences = audiences?.ToImmutableArray(); return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="SetCredential"]/*'/>*/
    public AccessKeyIssueOptions SetCredential(AccessCredential? credential = null)
    {
        this.Credential = credential; return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="SetLifetime"]/*'/>*/
    public AccessKeyIssueOptions SetLifetime(TimeSpan? lifetime = null)
    {
        this.Lifetime = lifetime; return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="SetOperations"]/*'/>*/
    public AccessKeyIssueOptions SetOperations(IEnumerable<Int32>? operations = null)
    {
        this.Operations = operations?.ToImmutableArray(); return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="SetProperties"]/*'/>*/
    public AccessKeyIssueOptions SetProperties(IEnumerable<KeyValuePair<String,String>>? properties = null)
    {
        this.Properties = properties?.ToImmutableDictionary(); return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="SetRequestedAssertions"]/*'/>*/
    public AccessKeyIssueOptions SetRequestedAssertions(IEnumerable<AccessAssertionRequest>? requestedassertions = null)
    {
        this.RequestedAssertions = requestedassertions?.ToImmutableArray(); return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="SetRequestedKeyType"]/*'/>*/
    public AccessKeyIssueOptions SetRequestedKeyType(String? requestedkeytype = null)
    {
        this.RequestedKeyType = String.IsNullOrWhiteSpace(requestedkeytype) ? null : requestedkeytype; return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="SetScopes"]/*'/>*/
    public AccessKeyIssueOptions SetScopes(IEnumerable<String>? scopes = null)
    {
        this.Scopes = scopes?.ToImmutableArray(); return this;
    }

    /**<include file='AccessKeyIssueOptions.xml' path='AccessKeyIssueOptions/class[@name="AccessKeyIssueOptions"]/method[@name="SetSubject"]/*'/>*/
    public AccessKeyIssueOptions SetSubject(String? subject = null)
    {
        this.Subject = String.IsNullOrWhiteSpace(subject) ? null : subject; return this;
    }
}