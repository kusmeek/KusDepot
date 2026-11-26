namespace KusDepot;

/**<include file='CommonIdentification.xml' path='CommonIdentification/record[@name="CommonIdentification"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.CommonIdentification")]

public sealed record class CommonIdentification
{
    /**<include file='CommonIdentification.xml' path='CommonIdentification/record[@name="CommonIdentification"]/property[@name="EnvelopeVersion"]/*'/>*/
    [JsonPropertyName("EnvelopeVersion")] [JsonRequired] [Id(0)]
    public Byte EnvelopeVersion { get; init; }

    /**<include file='CommonIdentification.xml' path='CommonIdentification/record[@name="CommonIdentification"]/property[@name="SignatureAlgorithm"]/*'/>*/
    [JsonPropertyName("SignatureAlgorithm")] [JsonRequired] [Id(1)]
    public Byte SignatureAlgorithm { get; init; }

    /**<include file='CommonIdentification.xml' path='CommonIdentification/record[@name="CommonIdentification"]/property[@name="Data"]/*'/>*/
    [JsonPropertyName("Data")] [JsonRequired] [Id(2)]
    public CommonIdentificationData Data { get; init; }

    /**<include file='CommonIdentification.xml' path='CommonIdentification/record[@name="CommonIdentification"]/property[@name="Signature"]/*'/>*/
    [JsonPropertyName("Signature")] [JsonRequired] [Id(3)]
    public Byte[] Signature { get; init; } = Array.Empty<Byte>();
}