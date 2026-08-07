namespace KusDepot.Security.Data;

/**<include file='DataProtectionRecipientSummary.xml' path='DataProtectionRecipientSummary/struct[@name="DataProtectionRecipientSummary"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.Data.DataProtectionRecipientSummary")]
public readonly record struct DataProtectionRecipientSummary
{
    /**<include file='DataProtectionRecipientSummary.xml' path='DataProtectionRecipientSummary/struct[@name="DataProtectionRecipientSummary"]/constructor[@name="Constructor"]/*'/>*/
    public DataProtectionRecipientSummary() {}

    /**<include file='DataProtectionRecipientSummary.xml' path='DataProtectionRecipientSummary/struct[@name="DataProtectionRecipientSummary"]/property[@name="ObjectId"]/*'/>*/
    [JsonPropertyName("ObjectId")] [JsonRequired] [Id(0)]
    public Guid? ObjectId { get; init; }

    /**<include file='DataProtectionRecipientSummary.xml' path='DataProtectionRecipientSummary/struct[@name="DataProtectionRecipientSummary"]/property[@name="Thumbprint"]/*'/>*/
    [JsonPropertyName("Thumbprint")] [JsonRequired] [Id(1)]
    public String? Thumbprint { get; init; }

    /**<include file='DataProtectionRecipientSummary.xml' path='DataProtectionRecipientSummary/struct[@name="DataProtectionRecipientSummary"]/property[@name="PublicKeyHash"]/*'/>*/
    [JsonPropertyName("PublicKeyHash")] [JsonRequired] [Id(2)]
    public ImmutableArray<Byte> PublicKeyHash { get; init; } = [];
}
