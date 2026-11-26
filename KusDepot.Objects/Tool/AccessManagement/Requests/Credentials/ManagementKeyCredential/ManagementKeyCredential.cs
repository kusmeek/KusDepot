namespace KusDepot.Security.Requests;

/**<include file='ManagementKeyCredential.xml' path='ManagementKeyCredential/class[@name="ManagementKeyCredential"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.Requests.ManagementKeyCredential")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public record class ManagementKeyCredential : AccessCredential
{
    /**<include file='ManagementKeyCredential.xml' path='ManagementKeyCredential/class[@name="ManagementKeyCredential"]/property[@name="Key"]/*'/>*/
    [JsonPropertyName("Key")] [JsonRequired] [Id(0)]
    public String Key { get; init; } = String.Empty;
}