namespace KusDepot.Security.Requests;

/**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.Requests.AccessRequest")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonDerivedType(typeof(HostRequest),typeDiscriminator:nameof(HostRequest))]
[JsonDerivedType(typeof(ServiceRequest),typeDiscriminator:nameof(ServiceRequest))]
[JsonDerivedType(typeof(StandardRequest),typeDiscriminator:nameof(StandardRequest))]
[JsonDerivedType(typeof(ManagementRequest),typeDiscriminator:nameof(ManagementRequest))]

public abstract record class AccessRequest : IEquatable<AccessRequest>
{
    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/property[@name="Audiences"]/*'/>*/
    [JsonPropertyName("Audiences")] [JsonRequired] [Id(0)]
    public ImmutableArray<String>? Audiences { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/property[@name="Credential"]/*'/>*/
    [JsonPropertyName("Credential")] [JsonRequired] [Id(1)]
    public AccessCredential? Credential { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/property[@name="Lifetime"]/*'/>*/
    [JsonPropertyName("Lifetime")] [JsonRequired] [Id(2)]
    public TimeSpan? Lifetime { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/property[@name="Operations"]/*'/>*/
    [JsonPropertyName("Operations")] [JsonRequired] [Id(3)]
    public ImmutableArray<Int32>? Operations { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/property[@name="Properties"]/*'/>*/
    [JsonPropertyName("Properties")] [JsonRequired] [Id(4)]
    public ImmutableDictionary<String,String>? Properties { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/property[@name="RequestedAssertions"]/*'/>*/
    [JsonPropertyName("RequestedAssertions")] [JsonRequired] [Id(5)]
    public ImmutableArray<AccessAssertionRequest>? RequestedAssertions { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/property[@name="RequestedKeyType"]/*'/>*/
    [JsonPropertyName("RequestedKeyType")] [JsonRequired] [Id(6)]
    public String? RequestedKeyType { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/property[@name="Scopes"]/*'/>*/
    [JsonPropertyName("Scopes")] [JsonRequired] [Id(7)]
    public ImmutableArray<String>? Scopes { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/property[@name="Subject"]/*'/>*/
    [JsonPropertyName("Subject")] [JsonRequired] [Id(8)]
    public String? Subject { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="Equals"]/*'/>*/
    public virtual Boolean Equals(AccessRequest? other)
    {
        return ReferenceEquals(this,other);
    }

    ///<inheritdoc/>
    public override Int32 GetHashCode() => RuntimeHelpers.GetHashCode(this);

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="ParseAny"]/*'/>*/
    public static AccessRequest? Parse(String input , IFormatProvider? format = null)
    {
        try { return JsonUtility.Parse<AccessRequest>(input); }

        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="Parse"]/*'/>*/
    public static TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : AccessRequest
    {
        try { return JsonUtility.Parse<AccessRequest>(input) as TResult; }

        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    ///<inheritdoc/>
    public sealed override String ToString()
    {
        try { return JsonUtility.ToJsonString<AccessRequest>(this); }

        catch ( Exception _ ) { if(NoExceptions) { return String.Empty; } KusDepotLog.Error(_,ToStringFail); throw; }
    }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : AccessRequest
    {
        try { result = Parse<TResult>(input!,format); return result is not null; }

        catch { result = null; return false; }
    }
}