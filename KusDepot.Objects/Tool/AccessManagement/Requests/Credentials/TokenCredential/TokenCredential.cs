namespace KusDepot.Security.Requests;

/**<include file='TokenCredential.xml' path='TokenCredential/class[@name="TokenCredential"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.Requests.TokenCredential")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public record class TokenCredential : AccessCredential
{
    /**<include file='TokenCredential.xml' path='TokenCredential/class[@name="TokenCredential"]/property[@name="Token"]/*'/>*/
    [JsonPropertyName("Token")] [JsonRequired] [Id(0)]
    public String Token { get; init; } = String.Empty;
}