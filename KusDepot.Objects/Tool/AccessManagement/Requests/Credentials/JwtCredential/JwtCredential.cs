namespace KusDepot.Security.Requests;

/**<include file='JwtCredential.xml' path='JwtCredential/class[@name="JwtCredential"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.Requests.JwtCredential")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public record class JwtCredential : AccessCredential
{
    /**<include file='JwtCredential.xml' path='JwtCredential/class[@name="JwtCredential"]/property[@name="Token"]/*'/>*/
    [JsonPropertyName("Token")] [JsonRequired] [Id(0)]
    public String Token { get; init; } = String.Empty;
}