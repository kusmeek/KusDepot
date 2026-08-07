namespace KusDepot;

/**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/main/*'/>*/
[DataContract(Name = "ToolManifest" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.ToolManifest")] [Immutable]

public sealed record class ToolManifest : IEquatable<ToolManifest> , IParsable<ToolManifest>
{
    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/property[@name="ToolSchemaID"]/*'/>*/
    [JsonPropertyName("ToolSchemaID")] [JsonRequired]
    [DataMember(Name = "ToolSchemaID" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? ToolSchemaID { get; init; }

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/property[@name="AccessKeyRealmID"]/*'/>*/
    [JsonPropertyName("AccessKeyRealmID")] [JsonRequired]
    [DataMember(Name = "AccessKeyRealmID" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? AccessKeyRealmID { get; init; }

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/property[@name="ToolSchemaVersion"]/*'/>*/
    [JsonPropertyName("ToolSchemaVersion")] [JsonRequired]
    [DataMember(Name = "ToolSchemaVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? ToolSchemaVersion { get; init; }

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/property[@name="ManifestHash"]/*'/>*/
    [JsonPropertyName("ManifestHash")] [JsonRequired]
    [DataMember(Name = "ManifestHash" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public String? ManifestHash { get; set { field ??= value; } }

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/property[@name="ToolType"]/*'/>*/
    [JsonPropertyName("ToolType")] [JsonRequired]
    [DataMember(Name = "ToolType" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public String? ToolType { get; init; }

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/property[@name="Operations"]/*'/>*/
    [JsonPropertyName("Operations")] [JsonRequired]
    [DataMember(Name = "Operations" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    public ImmutableArray<ToolOperationDescriptor> Operations { get; init; } = [];

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/method[@name="Create"]/*'/>*/
    public static ToolManifest Create(ITool tool)
    {
        return Create(tool?.GetType().FullName,tool?.GetType().FullName,String.Empty,tool?.GetType().FullName,[]);
    }

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/method[@name="CreateSchema"]/*'/>*/
    public static ToolManifest Create(String? toolschemaid , String? accesskeyrealmid = null , String? toolschemaversion = null , String? tooltype = null , IEnumerable<ToolOperationDescriptor>? operations = null)
    {
        return new()
        {
            ToolSchemaID = toolschemaid,
            AccessKeyRealmID = accesskeyrealmid ?? toolschemaid,
            ToolSchemaVersion = toolschemaversion ?? String.Empty,
            ToolType = tooltype , Operations = operations?.ToImmutableArray() ?? []
        };
    }

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/method[@name="ComputeManifestHash"]/*'/>*/
    public ToolManifest ComputeManifestHash()
    {
        try
        {
            if(ManifestHash is not null) { return this; } StringBuilder builder = new();

            builder.Append(ToolSchemaID ?? String.Empty).Append(Environment.NewLine);

            builder.Append(ToolSchemaVersion ?? String.Empty).Append(Environment.NewLine);

            foreach(var operation in this.Operations.Where(_ => _ is not null).OrderBy(_ => _.Index).ThenBy(_ => _.MethodName,StringComparer.Ordinal))
            {
                builder.Append(operation.Index).Append('|').Append(operation.MethodName ?? String.Empty).Append(Environment.NewLine);
            }

            ManifestHash = Convert.ToHexString(SHA512.HashData(Encoding.UTF8.GetBytes(builder.ToString())));

            return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ComputeManifestHashFail); return this; }
    }

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(ToolManifest? other) { return ReferenceEquals(this,other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return RuntimeHelpers.GetHashCode(this); }

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/method[@name="IParsable{ToolManifest}.Parse"]/*'/>*/
    static ToolManifest IParsable<ToolManifest>.Parse(String input , IFormatProvider? format) { return Parse(input)!; }

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/method[@name="Parse"]/*'/>*/
    public static ToolManifest? Parse(String input)
    {
        try { return String.IsNullOrEmpty(input) ? null : JsonUtility.Parse<ToolManifest>(input); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); return null; }
    }
    ///<inheritdoc/>
    public override String ToString()
    {
        try { return JsonUtility.ToJsonString(this); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); return String.Empty; }
    }

    /**<include file='ToolManifest.xml' path='ToolManifest/record[@name="ToolManifest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(String? input , IFormatProvider? format , out ToolManifest result)
    {
        result = null!; if(input is null) { return false; }

        try { var _ = Parse(input); if(_ is not null) { result = _; return true; } return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,TryParseFail); return false; }
    }
}