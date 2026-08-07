namespace KusDepot;

/**<include file='ToolManifestIdentity.xml' path='ToolManifestIdentity/class[@name="ToolManifestIdentity"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ToolManifestIdentity" , Namespace = "KusDepot")]
[GenerateSerializer] [Alias("KusDepot.ToolManifestIdentity")]
public sealed class ToolManifestIdentity
{
    /**<include file='ToolManifestIdentity.xml' path='ToolManifestIdentity/class[@name="ToolManifestIdentity"]/property[@name="AccessKeyRealmID"]/*'/>*/
    [JsonPropertyName("AccessKeyRealmID")] [JsonRequired]
    [DataMember(Name = "AccessKeyRealmID" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String AccessKeyRealmID { get; set; } = String.Empty;

    /**<include file='ToolManifestIdentity.xml' path='ToolManifestIdentity/class[@name="ToolManifestIdentity"]/property[@name="ManifestHash"]/*'/>*/
    [JsonPropertyName("ManifestHash")] [JsonRequired]
    [DataMember(Name = "ManifestHash" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String ManifestHash { get; set; } = String.Empty;

    /**<include file='ToolManifestIdentity.xml' path='ToolManifestIdentity/class[@name="ToolManifestIdentity"]/property[@name="ToolSchemaID"]/*'/>*/
    [JsonPropertyName("ToolSchemaID")] [JsonRequired]
    [DataMember(Name = "ToolSchemaID" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String ToolSchemaID { get; set; } = String.Empty;

    /**<include file='ToolManifestIdentity.xml' path='ToolManifestIdentity/class[@name="ToolManifestIdentity"]/method[@name="CreateManifest"]/*'/>*/
    public static ToolManifestIdentity Create(ToolManifest? manifest)
    {
        return new()
        {
            AccessKeyRealmID = manifest?.AccessKeyRealmID ?? manifest?.ToolSchemaID ?? String.Empty,
            ToolSchemaID = manifest?.ToolSchemaID ?? String.Empty,
            ManifestHash = manifest?.ManifestHash ?? String.Empty
        };
    }

    /**<include file='ToolManifestIdentity.xml' path='ToolManifestIdentity/class[@name="ToolManifestIdentity"]/method[@name="Create"]/*'/>*/
    public static ToolManifestIdentity Create(String? accesskeyrealmid , String? manifesthash , String? toolschemaid)
    {
        return new()
        {
            AccessKeyRealmID = accesskeyrealmid ?? String.Empty,
            ToolSchemaID = manifesthash ?? String.Empty,
            ManifestHash = toolschemaid ?? String.Empty
        };
    }

    /**<include file='ToolManifestIdentity.xml' path='ToolManifestIdentity/class[@name="ToolManifestIdentity"]/method[@name="Clone"]/*'/>*/
    public ToolManifestIdentity Clone()
    {
        return new()
        {
            AccessKeyRealmID = this.AccessKeyRealmID,
            ManifestHash = this.ManifestHash,
            ToolSchemaID = this.ToolSchemaID
        };
    }
}