namespace KusDepot;

/**<include file='ToolDescriptor.xml' path='ToolDescriptor/record[@name="ToolDescriptor"]/main/*'/>*/
[DataContract(Name = "ToolDescriptor" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.ToolDescriptor")] [Immutable]

public sealed record class ToolDescriptor : IEquatable<ToolDescriptor> , IParsable<ToolDescriptor>
{
    /**<include file='ToolDescriptor.xml' path='ToolDescriptor/record[@name="ToolDescriptor"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonRequired]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Guid? ID                   {get;init;}

    /**<include file='ToolDescriptor.xml' path='ToolDescriptor/record[@name="ToolDescriptor"]/property[@name="Type"]/*'/>*/
    [JsonPropertyName("Type")] [JsonRequired]
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? Type               {get;init;}

    /**<include file='ToolDescriptor.xml' path='ToolDescriptor/record[@name="ToolDescriptor"]/property[@name="Specifications"]/*'/>*/
    [JsonPropertyName("Specifications")] [JsonRequired]
    [DataMember(Name = "Specifications" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? Specifications     {get;init;}

    /**<include file='ToolDescriptor.xml' path='ToolDescriptor/record[@name="ToolDescriptor"]/property[@name="ExtendedData"]/*'/>*/
    [JsonPropertyName("ExtendedData")] [JsonRequired]
    [DataMember(Name = "ExtendedData" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public List<String>? ExtendedData {get;init;}

    ///<inheritdoc/>
    public override String ToString()
    {
        try { return JsonUtility.ToJsonString<ToolDescriptor>(this); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); return String.Empty; }
    }

    /**<include file='ToolDescriptor.xml' path='ToolDescriptor/record[@name="ToolDescriptor"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(ToolDescriptor? other) { return ReferenceEquals(this,other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return RuntimeHelpers.GetHashCode(this); }

    /**<include file='ToolDescriptor.xml' path='ToolDescriptor/record[@name="ToolDescriptor"]/method[@name="Create"]/*'/>*/
    public static ToolDescriptor Create(ITool tool) => new() { ID = tool?.GetID() , Type = tool?.GetType().FullName };

    /**<include file='ToolDescriptor.xml' path='ToolDescriptor/record[@name="ToolDescriptor"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(String? input , IFormatProvider? format , out ToolDescriptor result)
    {
        result = null!; if(input is null) { return false; }

        try { var _ = Parse(input); if(_ is not null) { result = _; return true; } return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,TryParseFail); return false; }
    }

    /**<include file='ToolDescriptor.xml' path='ToolDescriptor/record[@name="ToolDescriptor"]/method[@name="IParsable{ToolDescriptor}.Parse"]/*'/>*/
    static ToolDescriptor IParsable<ToolDescriptor>.Parse(String input , IFormatProvider? format) { return Parse(input)!; }

    /**<include file='ToolDescriptor.xml' path='ToolDescriptor/record[@name="ToolDescriptor"]/method[@name="Parse"]/*'/>*/
    public static ToolDescriptor? Parse(String input)
    {
        try { return String.IsNullOrEmpty(input) ? null : JsonUtility.Parse<ToolDescriptor>(input); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); return null; }
    }
}