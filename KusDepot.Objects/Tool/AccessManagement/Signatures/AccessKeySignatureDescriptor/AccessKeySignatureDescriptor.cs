namespace KusDepot.Security.Signatures;

/**<include file='AccessKeySignatureDescriptor.xml' path='AccessKeySignatureDescriptor/struct[@name="AccessKeySignatureDescriptor"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Signatures.AccessKeySignatureDescriptor")]
public readonly record struct AccessKeySignatureDescriptor
{
    /**<include file='AccessKeySignatureDescriptor.xml' path='AccessKeySignatureDescriptor/struct[@name="AccessKeySignatureDescriptor"]/constructor[@name="Constructor"]/*'/>*/
    public AccessKeySignatureDescriptor() {}

    /**<include file='AccessKeySignatureDescriptor.xml' path='AccessKeySignatureDescriptor/struct[@name="AccessKeySignatureDescriptor"]/property[@name="Signed"]/*'/>*/
    [JsonPropertyName("Signed")] [JsonRequired] [Id(0)]
    public Boolean Signed { get; init; }

    /**<include file='AccessKeySignatureDescriptor.xml' path='AccessKeySignatureDescriptor/struct[@name="AccessKeySignatureDescriptor"]/property[@name="SignerToolID"]/*'/>*/
    [JsonPropertyName("SignerToolID")] [JsonRequired] [Id(1)]
    public Guid SignerToolID { get; init; }

    /**<include file='AccessKeySignatureDescriptor.xml' path='AccessKeySignatureDescriptor/struct[@name="AccessKeySignatureDescriptor"]/property[@name="SignerAccessManagerID"]/*'/>*/
    [JsonPropertyName("SignerAccessManagerID")] [JsonRequired] [Id(2)]
    public Guid SignerAccessManagerID { get; init; }

    /**<include file='AccessKeySignatureDescriptor.xml' path='AccessKeySignatureDescriptor/struct[@name="AccessKeySignatureDescriptor"]/property[@name="Algorithm"]/*'/>*/
    [JsonPropertyName("Algorithm")] [JsonRequired] [Id(3)]
    public AccessKeySignatureAlgorithm Algorithm { get; init; }

    /**<include file='AccessKeySignatureDescriptor.xml' path='AccessKeySignatureDescriptor/struct[@name="AccessKeySignatureDescriptor"]/property[@name="SignerKeyIDFormat"]/*'/>*/
    [JsonPropertyName("SignerKeyIDFormat")] [JsonRequired] [Id(4)]
    public AccessKeySignerKeyIdFormat SignerKeyIDFormat { get; init; }

    /**<include file='AccessKeySignatureDescriptor.xml' path='AccessKeySignatureDescriptor/struct[@name="AccessKeySignatureDescriptor"]/property[@name="SignerKeyID"]/*'/>*/
    [JsonPropertyName("SignerKeyID")] [JsonRequired] [Id(5)]
    public ImmutableArray<Byte> SignerKeyID { get; init; } = [];
}