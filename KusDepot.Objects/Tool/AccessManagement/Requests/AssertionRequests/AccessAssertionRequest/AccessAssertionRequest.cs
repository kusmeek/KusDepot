namespace KusDepot.Security.Requests;

/**<include file='AccessAssertionRequest.xml' path='AccessAssertionRequest/class[@name="AccessAssertionRequest"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[Alias("KusDepot.Security.Requests.AccessAssertionRequest")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonDerivedType(typeof(SamlAssertionRequest),typeDiscriminator:nameof(SamlAssertionRequest))]
[JsonDerivedType(typeof(TokenAssertionRequest),typeDiscriminator:nameof(TokenAssertionRequest))]

public abstract record class AccessAssertionRequest
{
    /**<include file='AccessAssertionRequest.xml' path='AccessAssertionRequest/class[@name="AccessAssertionRequest"]/property[@name="AssertionType"]/*'/>*/
    [JsonPropertyName("AssertionType")] [JsonRequired] [Id(0)]
    public String AssertionType { get; init; } = String.Empty;

    /**<include file='AccessAssertionRequest.xml' path='AccessAssertionRequest/class[@name="AccessAssertionRequest"]/property[@name="Required"]/*'/>*/
    [JsonPropertyName("Required")] [JsonRequired] [Id(1)]
    public Boolean Required { get; init; } = true;
}