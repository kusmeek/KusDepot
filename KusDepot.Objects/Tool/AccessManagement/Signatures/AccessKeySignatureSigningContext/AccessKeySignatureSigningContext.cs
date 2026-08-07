namespace KusDepot.Security.Signatures;

/**<include file='AccessKeySignatureSigningContext.xml' path='AccessKeySignatureSigningContext/struct[@name="AccessKeySignatureSigningContext"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Signatures.AccessKeySignatureSigningContext")]

public readonly record struct AccessKeySignatureSigningContext
{
    /**<include file='AccessKeySignatureSigningContext.xml' path='AccessKeySignatureSigningContext/struct[@name="AccessKeySignatureSigningContext"]/constructor[@name="Constructor"]/*'/>*/
    public AccessKeySignatureSigningContext() {}

    /**<include file='AccessKeySignatureSigningContext.xml' path='AccessKeySignatureSigningContext/struct[@name="AccessKeySignatureSigningContext"]/property[@name="AccessKeyRealmID"]/*'/>*/
    [JsonPropertyName("AccessKeyRealmID")] [JsonRequired] [Id(0)]
    public String AccessKeyRealmID { get; init; } = String.Empty;

    /**<include file='AccessKeySignatureSigningContext.xml' path='AccessKeySignatureSigningContext/struct[@name="AccessKeySignatureSigningContext"]/property[@name="Logger"]/*'/>*/
    [JsonIgnore] [field: NonSerialized]
    public ILogger? Logger { get; init; }

    /**<include file='AccessKeySignatureSigningContext.xml' path='AccessKeySignatureSigningContext/struct[@name="AccessKeySignatureSigningContext"]/property[@name="ManifestHash"]/*'/>*/
    [JsonPropertyName("ManifestHash")] [JsonRequired] [Id(1)]
    public String ManifestHash { get; init; } = String.Empty;

    /**<include file='AccessKeySignatureSigningContext.xml' path='AccessKeySignatureSigningContext/struct[@name="AccessKeySignatureSigningContext"]/property[@name="SignatureInfo"]/*'/>*/
    [JsonPropertyName("SignatureInfo")] [JsonRequired] [Id(2)]
    public AccessKeySignatureInfo SignatureInfo { get; init; }

    /**<include file='AccessKeySignatureSigningContext.xml' path='AccessKeySignatureSigningContext/struct[@name="AccessKeySignatureSigningContext"]/property[@name="SignedRegion"]/*'/>*/
    [JsonPropertyName("SignedRegion")] [JsonRequired] [Id(3)]
    public ReadOnlyMemory<Byte> SignedRegion { get; init; }

    /**<include file='AccessKeySignatureSigningContext.xml' path='AccessKeySignatureSigningContext/struct[@name="AccessKeySignatureSigningContext"]/property[@name="SignerAccessManagerID"]/*'/>*/
    [JsonPropertyName("SignerAccessManagerID")] [JsonRequired] [Id(4)]
    public Guid SignerAccessManagerID { get; init; }

    /**<include file='AccessKeySignatureSigningContext.xml' path='AccessKeySignatureSigningContext/struct[@name="AccessKeySignatureSigningContext"]/property[@name="SignerToolID"]/*'/>*/
    [JsonPropertyName("SignerToolID")] [JsonRequired] [Id(5)]
    public Guid SignerToolID { get; init; }

    /**<include file='AccessKeySignatureSigningContext.xml' path='AccessKeySignatureSigningContext/struct[@name="AccessKeySignatureSigningContext"]/property[@name="Subject"]/*'/>*/
    [JsonPropertyName("Subject")] [JsonRequired] [Id(6)]
    public String Subject { get; init; } = String.Empty;

    /**<include file='AccessKeySignatureSigningContext.xml' path='AccessKeySignatureSigningContext/struct[@name="AccessKeySignatureSigningContext"]/property[@name="ToolSchemaID"]/*'/>*/
    [JsonPropertyName("ToolSchemaID")] [JsonRequired] [Id(7)]
    public String ToolSchemaID { get; init; } = String.Empty;
}
