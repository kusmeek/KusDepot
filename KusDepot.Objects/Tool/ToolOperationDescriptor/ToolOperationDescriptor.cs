namespace KusDepot;

/**<include file='ToolOperationDescriptor.xml' path='ToolOperationDescriptor/record[@name="ToolOperationDescriptor"]/main/*'/>*/
[DataContract(Name = "ToolOperationDescriptor" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.ToolOperationDescriptor")] [Immutable]

public sealed record class ToolOperationDescriptor : IEquatable<ToolOperationDescriptor> , IParsable<ToolOperationDescriptor>
{
    /**<include file='ToolOperationDescriptor.xml' path='ToolOperationDescriptor/record[@name="ToolOperationDescriptor"]/property[@name="Index"]/*'/>*/
    [JsonPropertyName("Index")] [JsonRequired]
    [DataMember(Name = "Index" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Int32 Index { get; init; }

    /**<include file='ToolOperationDescriptor.xml' path='ToolOperationDescriptor/record[@name="ToolOperationDescriptor"]/property[@name="MethodName"]/*'/>*/
    [JsonPropertyName("MethodName")] [JsonRequired]
    [DataMember(Name = "MethodName" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? MethodName { get; init; }

    /**<include file='ToolOperationDescriptor.xml' path='ToolOperationDescriptor/record[@name="ToolOperationDescriptor"]/property[@name="Description"]/*'/>*/
    [JsonPropertyName("Description")] [JsonRequired]
    [DataMember(Name = "Description" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? Description { get; init; }

    /**<include file='ToolOperationDescriptor.xml' path='ToolOperationDescriptor/record[@name="ToolOperationDescriptor"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(ToolOperationDescriptor? other) { return ReferenceEquals(this,other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return RuntimeHelpers.GetHashCode(this); }

    ///<inheritdoc/>
    public override String ToString()
    {
        try { return JsonUtility.ToJsonString(this); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); return String.Empty; }
    }

    /**<include file='ToolOperationDescriptor.xml' path='ToolOperationDescriptor/record[@name="ToolOperationDescriptor"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(String? input , IFormatProvider? format , out ToolOperationDescriptor result)
    {
        result = null!; if(input is null) { return false; }

        try { var _ = Parse(input); if(_ is not null) { result = _; return true; } return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,TryParseFail); return false; }
    }

    /**<include file='ToolOperationDescriptor.xml' path='ToolOperationDescriptor/record[@name="ToolOperationDescriptor"]/method[@name="IParsable{ToolOperationDescriptor}.Parse"]/*'/>*/
    static ToolOperationDescriptor IParsable<ToolOperationDescriptor>.Parse(String input , IFormatProvider? format) { return Parse(input)!; }

    /**<include file='ToolOperationDescriptor.xml' path='ToolOperationDescriptor/record[@name="ToolOperationDescriptor"]/method[@name="Parse"]/*'/>*/
    public static ToolOperationDescriptor? Parse(String input)
    {
        try { return String.IsNullOrEmpty(input) ? null : JsonUtility.Parse<ToolOperationDescriptor>(input); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); return null; }
    }
}
