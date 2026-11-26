namespace KusDepot.Security;

/**<include file='DataFieldAssertion.xml' path='DataFieldAssertion/record[@name="DataFieldAssertion"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.DataFieldAssertion")]

public sealed record class DataFieldAssertion
{
    /**<include file='DataFieldAssertion.xml' path='DataFieldAssertion/record[@name="DataFieldAssertion"]/property[@name="EnvelopeVersion"]/*'/>*/
    [JsonPropertyName("EnvelopeVersion")] [JsonRequired] [Id(0)]
    public Byte EnvelopeVersion { get; init; }

    /**<include file='DataFieldAssertion.xml' path='DataFieldAssertion/record[@name="DataFieldAssertion"]/property[@name="SignatureAlgorithm"]/*'/>*/
    [JsonPropertyName("SignatureAlgorithm")] [JsonRequired] [Id(1)]
    public Byte SignatureAlgorithm { get; init; }

    /**<include file='DataFieldAssertion.xml' path='DataFieldAssertion/record[@name="DataFieldAssertion"]/property[@name="Data"]/*'/>*/
    [JsonPropertyName("Data")] [JsonRequired] [Id(2)]
    public DataFieldAssertionData Data { get; init; }

    /**<include file='DataFieldAssertion.xml' path='DataFieldAssertion/record[@name="DataFieldAssertion"]/property[@name="Signature"]/*'/>*/
    [JsonPropertyName("Signature")] [JsonRequired] [Id(3)]
    public Byte[] Signature { get; init; } = Array.Empty<Byte>();
}