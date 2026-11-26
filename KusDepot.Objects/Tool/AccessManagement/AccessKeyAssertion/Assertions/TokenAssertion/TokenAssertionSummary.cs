namespace KusDepot.Security.Assertions;

/**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Assertions.TokenAssertionSummary")] [Immutable]

public sealed record class TokenAssertionSummary : AccessKeyAssertionSummary
{
    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public TokenAssertionSummary() { AssertionType = "token"; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="Audiences"]/*'/>*/
    [JsonPropertyName("Audiences")] [JsonRequired] [Id(0)]
    public ImmutableArray<String> Audiences { get; init; } = [];

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="AuthenticationContextClassReference"]/*'/>*/
    [JsonPropertyName("AuthenticationContextClassReference")] [JsonRequired] [Id(1)]
    public String? AuthenticationContextClassReference { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="AuthenticationMethods"]/*'/>*/
    [JsonPropertyName("AuthenticationMethods")] [JsonRequired] [Id(2)]
    public ImmutableArray<String> AuthenticationMethods { get; init; } = [];

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="AuthenticationTime"]/*'/>*/
    [JsonPropertyName("AuthenticationTime")] [JsonRequired] [Id(3)]
    public DateTimeOffset? AuthenticationTime { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="AuthorizedParty"]/*'/>*/
    [JsonPropertyName("AuthorizedParty")] [JsonRequired] [Id(4)]
    public String? AuthorizedParty { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="ClientID"]/*'/>*/
    [JsonPropertyName("ClientID")] [JsonRequired] [Id(5)]
    public String? ClientID { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="IssuedAt"]/*'/>*/
    [JsonPropertyName("IssuedAt")] [JsonRequired] [Id(6)]
    public DateTimeOffset? IssuedAt { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="Issuer"]/*'/>*/
    [JsonPropertyName("Issuer")] [JsonRequired] [Id(7)]
    public String? Issuer { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="JwtID"]/*'/>*/
    [JsonPropertyName("JwtID")] [JsonRequired] [Id(8)]
    public String? JwtID { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="NotAfter"]/*'/>*/
    [JsonPropertyName("NotAfter")] [JsonRequired] [Id(9)]
    public DateTimeOffset? NotAfter { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="NotBefore"]/*'/>*/
    [JsonPropertyName("NotBefore")] [JsonRequired] [Id(10)]
    public DateTimeOffset? NotBefore { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="ObjectID"]/*'/>*/
    [JsonPropertyName("ObjectID")] [JsonRequired] [Id(11)]
    public String? ObjectID { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="Scopes"]/*'/>*/
    [JsonPropertyName("Scopes")] [JsonRequired] [Id(12)]
    public ImmutableArray<String> Scopes { get; init; } = [];

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="SessionID"]/*'/>*/
    [JsonPropertyName("SessionID")] [JsonRequired] [Id(13)]
    public String? SessionID { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="Subject"]/*'/>*/
    [JsonPropertyName("Subject")] [JsonRequired] [Id(14)]
    public String? Subject { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/property[@name="TenantID"]/*'/>*/
    [JsonPropertyName("TenantID")] [JsonRequired] [Id(15)]
    public String? TenantID { get; init; }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/method[@name="Copy"]/*'/>*/
    public new TokenAssertionSummary Copy() => this with { };

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/method[@name="HasAudience"]/*'/>*/
    public Boolean HasAudience(String? audience)
    {
        return String.IsNullOrWhiteSpace(audience) is false && this.Audiences.Contains(audience,StringComparer.Ordinal);
    }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/method[@name="HasAuthenticationMethod"]/*'/>*/
    public Boolean HasAuthenticationMethod(String? authenticationmethod)
    {
        return String.IsNullOrWhiteSpace(authenticationmethod) is false && this.AuthenticationMethods.Contains(authenticationmethod,StringComparer.Ordinal);
    }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/method[@name="HasScope"]/*'/>*/
    public Boolean HasScope(String? scope)
    {
        return String.IsNullOrWhiteSpace(scope) is false && this.Scopes.Contains(scope,StringComparer.Ordinal);
    }

    /**<include file='TokenAssertionSummary.xml' path='TokenAssertionSummary/record[@name="TokenAssertionSummary"]/method[@name="IsExpiredAt"]/*'/>*/
    public Boolean IsExpiredAt(DateTimeOffset when)
    {
        return this.NotAfter is not null && when > this.NotAfter.Value;
    }
}