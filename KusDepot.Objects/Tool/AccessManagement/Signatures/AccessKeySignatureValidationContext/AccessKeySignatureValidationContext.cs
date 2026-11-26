namespace KusDepot.Security.Signatures;

/**<include file='AccessKeySignatureValidationContext.xml' path='AccessKeySignatureValidationContext/struct[@name="AccessKeySignatureValidationContext"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.Security.Signatures.AccessKeySignatureValidationContext")]

public readonly record struct AccessKeySignatureValidationContext
{
    /**<include file='AccessKeySignatureValidationContext.xml' path='AccessKeySignatureValidationContext/struct[@name="AccessKeySignatureValidationContext"]/constructor[@name="Constructor"]/*'/>*/
    public AccessKeySignatureValidationContext() {}

    /**<include file='AccessKeySignatureValidationContext.xml' path='AccessKeySignatureValidationContext/struct[@name="AccessKeySignatureValidationContext"]/property[@name="AccessKeyRealmID"]/*'/>*/
    [JsonPropertyName("AccessKeyRealmID")] [JsonRequired] [Id(0)]
    public String AccessKeyRealmID { get; init; } = String.Empty;

    /**<include file='AccessKeySignatureValidationContext.xml' path='AccessKeySignatureValidationContext/struct[@name="AccessKeySignatureValidationContext"]/property[@name="Logger"]/*'/>*/
    [JsonIgnore] [field: NonSerialized]
    public ILogger? Logger { get; init; }

    /**<include file='AccessKeySignatureValidationContext.xml' path='AccessKeySignatureValidationContext/struct[@name="AccessKeySignatureValidationContext"]/property[@name="ManifestHash"]/*'/>*/
    [JsonPropertyName("ManifestHash")] [JsonRequired] [Id(1)]
    public String ManifestHash { get; init; } = String.Empty;

    /**<include file='AccessKeySignatureValidationContext.xml' path='AccessKeySignatureValidationContext/struct[@name="AccessKeySignatureValidationContext"]/property[@name="Signature"]/*'/>*/
    [JsonPropertyName("Signature")] [JsonRequired] [Id(2)]
    public AccessKeySignatureDescriptor Signature { get; init; }

    /**<include file='AccessKeySignatureValidationContext.xml' path='AccessKeySignatureValidationContext/struct[@name="AccessKeySignatureValidationContext"]/property[@name="SignatureBytes"]/*'/>*/
    [JsonPropertyName("SignatureBytes")] [JsonRequired] [Id(3)]
    public ReadOnlyMemory<Byte> SignatureBytes { get; init; }

    /**<include file='AccessKeySignatureValidationContext.xml' path='AccessKeySignatureValidationContext/struct[@name="AccessKeySignatureValidationContext"]/property[@name="SignedRegion"]/*'/>*/
    [JsonPropertyName("SignedRegion")] [JsonRequired] [Id(4)]
    public ReadOnlyMemory<Byte> SignedRegion { get; init; }

    /**<include file='AccessKeySignatureValidationContext.xml' path='AccessKeySignatureValidationContext/struct[@name="AccessKeySignatureValidationContext"]/property[@name="ToolSchemaID"]/*'/>*/
    [JsonPropertyName("ToolSchemaID")] [JsonRequired] [Id(5)]
    public String ToolSchemaID { get; init; } = String.Empty;
}