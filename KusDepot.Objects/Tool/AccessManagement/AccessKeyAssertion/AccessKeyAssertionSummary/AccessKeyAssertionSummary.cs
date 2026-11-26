namespace KusDepot.Security.Assertions;

/**<include file='AccessKeyAssertionSummary.xml' path='AccessKeyAssertionSummary/record[@name="AccessKeyAssertionSummary"]/main/*'/>*/
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonDerivedType(typeof(SamlAssertionSummary),typeDiscriminator:nameof(SamlAssertionSummary))]
[JsonDerivedType(typeof(TokenAssertionSummary),typeDiscriminator:nameof(TokenAssertionSummary))]
[GenerateSerializer] [Alias("KusDepot.Security.Assertions.AccessKeyAssertionSummary")] [Immutable]

public abstract record class AccessKeyAssertionSummary
{
    /**<include file='AccessKeyAssertionSummary.xml' path='AccessKeyAssertionSummary/record[@name="AccessKeyAssertionSummary"]/property[@name="AssertionType"]/*'/>*/
    [JsonPropertyName("AssertionType")] [JsonRequired] [Id(0)]
    public String AssertionType { get; init; } = String.Empty;

    /**<include file='AccessKeyAssertionSummary.xml' path='AccessKeyAssertionSummary/record[@name="AccessKeyAssertionSummary"]/property[@name="Format"]/*'/>*/
    [JsonPropertyName("Format")] [JsonRequired] [Id(1)]
    public String Format { get; init; } = String.Empty;

    /**<include file='AccessKeyAssertionSummary.xml' path='AccessKeyAssertionSummary/record[@name="AccessKeyAssertionSummary"]/method[@name="Copy"]/*'/>*/
    public AccessKeyAssertionSummary Copy() => this with { };
}