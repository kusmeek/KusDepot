namespace KusDepot.Security.Data;

/**<include file='DataProtectionAssertionSummary.xml' path='DataProtectionAssertionSummary/struct[@name="DataProtectionAssertionSummary"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[Alias("KusDepot.Security.Data.DataProtectionAssertionSummary")]

public readonly record struct DataProtectionAssertionSummary
{
    /**<include file='DataProtectionAssertionSummary.xml' path='DataProtectionAssertionSummary/struct[@name="DataProtectionAssertionSummary"]/constructor[@name="Constructor"]/*'/>*/
    public DataProtectionAssertionSummary() {}

    /**<include file='DataProtectionAssertionSummary.xml' path='DataProtectionAssertionSummary/struct[@name="DataProtectionAssertionSummary"]/property[@name="Field"]/*'/>*/
    [JsonPropertyName("Field")] [JsonRequired] [Id(0)]
    public String Field { get; init; } = String.Empty;

    /**<include file='DataProtectionAssertionSummary.xml' path='DataProtectionAssertionSummary/struct[@name="DataProtectionAssertionSummary"]/property[@name="FieldState"]/*'/>*/
    [JsonPropertyName("FieldState")] [JsonRequired] [Id(1)]
    public DataFieldState FieldState { get; init; }

    /**<include file='DataProtectionAssertionSummary.xml' path='DataProtectionAssertionSummary/struct[@name="DataProtectionAssertionSummary"]/property[@name="IssuerObjectId"]/*'/>*/
    [JsonPropertyName("IssuerObjectId")] [JsonRequired] [Id(2)]
    public Guid IssuerObjectId { get; init; }

    /**<include file='DataProtectionAssertionSummary.xml' path='DataProtectionAssertionSummary/struct[@name="DataProtectionAssertionSummary"]/property[@name="Thumbprint"]/*'/>*/
    [JsonPropertyName("Thumbprint")] [JsonRequired] [Id(3)]
    public String Thumbprint { get; init; } = String.Empty;

    /**<include file='DataProtectionAssertionSummary.xml' path='DataProtectionAssertionSummary/struct[@name="DataProtectionAssertionSummary"]/property[@name="CreatedAt"]/*'/>*/
    [JsonPropertyName("CreatedAt")] [JsonRequired] [Id(4)]
    public DateTimeOffset CreatedAt { get; init; }
}