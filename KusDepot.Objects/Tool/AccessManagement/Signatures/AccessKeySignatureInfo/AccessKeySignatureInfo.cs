namespace KusDepot.Security.Signatures;

/**<include file='AccessKeySignatureInfo.xml' path='AccessKeySignatureInfo/struct[@name="AccessKeySignatureInfo"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Signatures.AccessKeySignatureInfo")]

public readonly record struct AccessKeySignatureInfo
{
    /**<include file='AccessKeySignatureInfo.xml' path='AccessKeySignatureInfo/struct[@name="AccessKeySignatureInfo"]/constructor[@name="Constructor"]/*'/>*/
    public AccessKeySignatureInfo() {}

    /**<include file='AccessKeySignatureInfo.xml' path='AccessKeySignatureInfo/struct[@name="AccessKeySignatureInfo"]/property[@name="Algorithm"]/*'/>*/
    [JsonPropertyName("Algorithm")] [JsonRequired] [Id(0)]
    public AccessKeySignatureAlgorithm Algorithm { get; init; }

    /**<include file='AccessKeySignatureInfo.xml' path='AccessKeySignatureInfo/struct[@name="AccessKeySignatureInfo"]/property[@name="SignerKeyIDFormat"]/*'/>*/
    [JsonPropertyName("SignerKeyIDFormat")] [JsonRequired] [Id(1)]
    public AccessKeySignerKeyIdFormat SignerKeyIDFormat { get; init; }

    /**<include file='AccessKeySignatureInfo.xml' path='AccessKeySignatureInfo/struct[@name="AccessKeySignatureInfo"]/property[@name="SignerKeyID"]/*'/>*/
    [JsonPropertyName("SignerKeyID")] [JsonRequired] [Id(2)]
    public ImmutableArray<Byte> SignerKeyID { get; init; } = [];

    /**<include file='AccessKeySignatureInfo.xml' path='AccessKeySignatureInfo/struct[@name="AccessKeySignatureInfo"]/property[@name="SignatureLength"]/*'/>*/
    [JsonPropertyName("SignatureLength")] [JsonRequired] [Id(3)]
    public Int32 SignatureLength { get; init; }
}
