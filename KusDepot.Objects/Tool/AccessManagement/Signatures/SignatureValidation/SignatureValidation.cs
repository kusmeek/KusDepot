namespace KusDepot.Security.Signatures;

/**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Signatures.SignatureValidation")]

public readonly record struct SignatureValidation
{
    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/constructor[@name="Constructor"]/*'/>*/
    public SignatureValidation() {}

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="Signed"]/*'/>*/
    [JsonPropertyName("Signed")] [JsonRequired] [Id(0)]
    public Boolean Signed { get; init; }

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="VerificationAttempted"]/*'/>*/
    [JsonPropertyName("VerificationAttempted")] [JsonRequired] [Id(1)]
    public Boolean VerificationAttempted { get; init; }

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="SignatureValid"]/*'/>*/
    [JsonPropertyName("SignatureValid")] [JsonRequired] [Id(2)]
    public Boolean SignatureValid { get; init; }

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="SignerToolID"]/*'/>*/
    [JsonPropertyName("SignerToolID")] [JsonRequired] [Id(3)]
    public Guid SignerToolID { get; init; }

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="SignerAccessManagerID"]/*'/>*/
    [JsonPropertyName("SignerAccessManagerID")] [JsonRequired] [Id(4)]
    public Guid SignerAccessManagerID { get; init; }

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="Algorithm"]/*'/>*/
    [JsonPropertyName("Algorithm")] [JsonRequired] [Id(5)]
    public AccessKeySignatureAlgorithm Algorithm { get; init; }

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="SignerKeyIDFormat"]/*'/>*/
    [JsonPropertyName("SignerKeyIDFormat")] [JsonRequired] [Id(6)]
    public AccessKeySignerKeyIdFormat SignerKeyIDFormat { get; init; }

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="SignerKeyID"]/*'/>*/
    [JsonPropertyName("SignerKeyID")] [JsonRequired] [Id(7)]
    public ImmutableArray<Byte> SignerKeyID { get; init; } = [];

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="CertificateResolved"]/*'/>*/
    [JsonPropertyName("CertificateResolved")] [JsonRequired] [Id(8)]
    public Boolean CertificateResolved { get; init; }

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="CertificateThumbprint"]/*'/>*/
    [JsonPropertyName("CertificateThumbprint")] [Id(9)]
    public String? CertificateThumbprint { get; init; }

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="SignerTrusted"]/*'/>*/
    [JsonPropertyName("SignerTrusted")] [JsonRequired] [Id(10)]
    public Boolean SignerTrusted { get; init; }

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="ChainTrusted"]/*'/>*/
    [JsonPropertyName("ChainTrusted")] [JsonRequired] [Id(11)]
    public Boolean ChainTrusted { get; init; }

    /**<include file='SignatureValidation.xml' path='SignatureValidation/struct[@name="SignatureValidation"]/property[@name="FailureReason"]/*'/>*/
    [JsonPropertyName("FailureReason")] [Id(12)]
    public String? FailureReason { get; init; }
}