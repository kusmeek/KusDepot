namespace KusDepot.Security.Assertions;

/**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/main/*'/>*/
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonDerivedType(typeof(TokenAssertion),typeDiscriminator:nameof(TokenAssertion))]
[GenerateSerializer] [Alias("KusDepot.Security.Assertions.AccessKeyAssertion")] [Immutable]

public abstract partial record class AccessKeyAssertion
{
    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/property[@name="AssertionType"]/*'/>*/
    [JsonPropertyName("AssertionType")] [JsonRequired] [Id(0)]
    public String AssertionType { get; init; } = String.Empty;

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/property[@name="Format"]/*'/>*/
    [JsonPropertyName("Format")] [JsonRequired] [Id(1)]
    public String Format { get; init; } = String.Empty;

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/property[@name="SerializationIdentifier"]/*'/>*/
    protected abstract String SerializationIdentifier { get; }

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/field[@name="Deserializers"]/*'/>*/
    private static readonly Dictionary<String,Func<ReadOnlyMemory<Byte>,AccessKeyAssertion?>> Deserializers = new(StringComparer.Ordinal);
}