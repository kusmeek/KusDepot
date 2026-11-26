namespace KusDepot.Security.Assertions;

/**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Assertions.SamlAssertionSummary")] [Immutable]

public sealed record class SamlAssertionSummary : AccessKeyAssertionSummary
{
    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public SamlAssertionSummary() { AssertionType = "saml2"; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="Audiences"]/*'/>*/
    [JsonPropertyName("Audiences")] [JsonRequired] [Id(0)]
    public ImmutableArray<String> Audiences { get; init; } = [];

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="AssertionID"]/*'/>*/
    [JsonPropertyName("AssertionID")] [JsonRequired] [Id(1)]
    public String? AssertionID { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="AuthenticationContextClassReference"]/*'/>*/
    [JsonPropertyName("AuthenticationContextClassReference")] [JsonRequired] [Id(2)]
    public String? AuthenticationContextClassReference { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="AuthenticationTime"]/*'/>*/
    [JsonPropertyName("AuthenticationTime")] [JsonRequired] [Id(3)]
    public DateTimeOffset? AuthenticationTime { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="Destination"]/*'/>*/
    [JsonPropertyName("Destination")] [JsonRequired] [Id(4)]
    public String? Destination { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="InResponseTo"]/*'/>*/
    [JsonPropertyName("InResponseTo")] [JsonRequired] [Id(5)]
    public String? InResponseTo { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="Issuer"]/*'/>*/
    [JsonPropertyName("Issuer")] [JsonRequired] [Id(6)]
    public String? Issuer { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="NameID"]/*'/>*/
    [JsonPropertyName("NameID")] [JsonRequired] [Id(7)]
    public String? NameID { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="NameIDFormat"]/*'/>*/
    [JsonPropertyName("NameIDFormat")] [JsonRequired] [Id(8)]
    public String? NameIDFormat { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="NotAfter"]/*'/>*/
    [JsonPropertyName("NotAfter")] [JsonRequired] [Id(9)]
    public DateTimeOffset? NotAfter { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="NotBefore"]/*'/>*/
    [JsonPropertyName("NotBefore")] [JsonRequired] [Id(10)]
    public DateTimeOffset? NotBefore { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="Recipient"]/*'/>*/
    [JsonPropertyName("Recipient")] [JsonRequired] [Id(11)]
    public String? Recipient { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="SessionIndex"]/*'/>*/
    [JsonPropertyName("SessionIndex")] [JsonRequired] [Id(12)]
    public String? SessionIndex { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="SessionNotOnOrAfter"]/*'/>*/
    [JsonPropertyName("SessionNotOnOrAfter")] [JsonRequired] [Id(13)]
    public DateTimeOffset? SessionNotOnOrAfter { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/property[@name="SubjectConfirmationMethod"]/*'/>*/
    [JsonPropertyName("SubjectConfirmationMethod")] [JsonRequired] [Id(14)]
    public String? SubjectConfirmationMethod { get; init; }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/method[@name="Copy"]/*'/>*/
    public new SamlAssertionSummary Copy() => this with { };

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/method[@name="HasAudience"]/*'/>*/
    public Boolean HasAudience(String? audience)
    {
        return String.IsNullOrWhiteSpace(audience) is false && this.Audiences.Contains(audience,StringComparer.Ordinal);
    }

    /**<include file='SamlAssertionSummary.xml' path='SamlAssertionSummary/record[@name="SamlAssertionSummary"]/method[@name="IsExpiredAt"]/*'/>*/
    public Boolean IsExpiredAt(DateTimeOffset when)
    {
        return this.NotAfter is not null && when > this.NotAfter.Value;
    }
}