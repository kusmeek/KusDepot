namespace KusDepot;

/**<include file='DataProtectionInfo.xml' path='DataProtectionInfo/class[@name="DataProtectionInfo"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.DataProtectionInfo")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public sealed class DataProtectionInfo
{
    /**<include file='DataProtectionInfo.xml' path='DataProtectionInfo/class[@name="DataProtectionInfo"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public DataProtectionInfo() {}

    /**<include file='DataProtectionInfo.xml' path='DataProtectionInfo/class[@name="DataProtectionInfo"]/property[@name="Version"]/*'/>*/    
    [JsonPropertyName("Version")] [JsonRequired] [Id(0)]
    public Byte Version { get; init; } = 1;

    /**<include file='DataProtectionInfo.xml' path='DataProtectionInfo/class[@name="DataProtectionInfo"]/property[@name="ProtectedAt"]/*'/>*/
    [JsonPropertyName("ProtectedAt")] [JsonRequired] [Id(1)]
    public DateTimeOffset ProtectedAt { get; init; }

    /**<include file='DataProtectionInfo.xml' path='DataProtectionInfo/class[@name="DataProtectionInfo"]/property[@name="ProtectedByObjectId"]/*'/>*/
    [JsonPropertyName("ProtectedByObjectId")] [JsonRequired] [Id(2)]
    public Guid? ProtectedByObjectId { get; init; }

    /**<include file='DataProtectionInfo.xml' path='DataProtectionInfo/class[@name="DataProtectionInfo"]/property[@name="ProtectedByThumbprint"]/*'/>*/
    [JsonPropertyName("ProtectedByThumbprint")] [JsonRequired] [Id(3)]
    public String? ProtectedByThumbprint { get; init; }

    /**<include file='DataProtectionInfo.xml' path='DataProtectionInfo/class[@name="DataProtectionInfo"]/property[@name="Purpose"]/*'/>*/
    [JsonPropertyName("Purpose")] [JsonRequired] [Id(4)]
    public String? Purpose { get; init; }

    /**<include file='DataProtectionInfo.xml' path='DataProtectionInfo/class[@name="DataProtectionInfo"]/property[@name="Recipients"]/*'/>*/
    [JsonPropertyName("Recipients")] [JsonRequired] [Id(5)]
    public ImmutableArray<DataProtectionRecipientSummary> Recipients { get; init; } = [];

    /**<include file='DataProtectionInfo.xml' path='DataProtectionInfo/class[@name="DataProtectionInfo"]/property[@name="Assertions"]/*'/>*/
    [JsonPropertyName("Assertions")] [JsonRequired] [Id(6)]
    public ImmutableArray<DataProtectionAssertionSummary> Assertions { get; init; } = [];

    /**<include file='DataProtectionInfo.xml' path='DataProtectionInfo/class[@name="DataProtectionInfo"]/property[@name="HasMultipleRecipients"]/*'/>*/
    [JsonPropertyName("HasMultipleRecipients")] [JsonRequired] [Id(7)]
    public Boolean HasMultipleRecipients { get; init; }

    /**<include file='DataProtectionInfo.xml' path='DataProtectionInfo/class[@name="DataProtectionInfo"]/property[@name="ProtectionMode"]/*'/>*/
    [JsonPropertyName("ProtectionMode")] [JsonRequired] [Id(8)]
    public String? ProtectionMode { get; init; }
}